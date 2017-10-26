/*
    Arena handles the arena gameplay.
    Players are spawned and a timer is set.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class Arena : MonoBehaviour{
  public List<int> scores;
  public List<int> factions; 
  public List<string> names;
  public List<Actor> players;
  int time;// Remaining round time in seconds.
  int bots;// Number of bots in arena.
  int startingTime; // Total round duration.
  bool respawns; // True if players respawn.
  bool teams; // True if teams are enabled.
  bool p1red, p2red; // True if respective player is on red team.
  string kit; // Default kit for players.
  MenuManager menu;
  int gameMode; // Active gamemode selected from lobby.
  public AudioClip[] gameModeClips; // Audio for gamemodes
  public Transform[] soloSpawns, redSpawns, blueSpawns;
  public bool spawnWeapons; // If set to true, spawners will spawn weapons. 
  
  // GameMode  constants
  public const int NONE = -1;
  public const int DEATHMATCH = 0;
  public const int TEAMDEATHMATCH = 1;
  public const int ELIMINATION = 2;
  public const int TEAMELIMINATION = 3;
  
  public void Start(){
    if(Session.Active()){ 
      Session.session.arena = this;
      Session.session.gameMode = Session.ARENA;
    }
    StartCoroutine(StartGame());
  }
  
  /* This co-routine exists to work around an animation bug caused 
     by Initializing() while loading the scene without a delay. */
  public IEnumerator StartGame(){
    yield return new WaitForSeconds(0.05f);
    Initialize();
  }

  /* Begin a new round. */
  public void Initialize(){
    Session.session.world = new World();
    Data dat = Session.session.arenaData;
    menu = gameObject.AddComponent<MenuManager>();
    menu.Change("ARENAHUD");
    menu.arena = this;
    time = startingTime = dat != null ? 60 * dat.ints[0] : 600;
    bots = dat != null ? dat.ints[1] : 0;
    gameMode = dat != null ? dat.ints[2] : NONE;
    if(dat != null){
      respawns = dat.bools[0];
      teams = dat.bools[1];
      p1red = dat.bools[2];
      p2red = dat.bools[3];
      spawnWeapons = dat.bools[4];
      kit = dat.strings[0];
      
    }
    InitPlayers();
    GameModeAnnouncement();
    StartCoroutine(UpdateRoutine());
  }

  IEnumerator UpdateRoutine(){
    while(time > 0){
      UpdateHUD();
      HandleKills();
      yield return new WaitForSeconds(1f);
    }
  }
  
  /* Plays the AudioClip for this game mode. */
  private void GameModeAnnouncement(){
    if(gameModeClips == null || gameModeClips.Length < gameMode || gameMode == NONE){ 
      print("Invalid clip.");
      return; 
    }
    float vol = PlayerPrefs.HasKey("masterVolume") ? PlayerPrefs.GetFloat("masterVolume") : 1f;
    AudioClip clip = gameModeClips[gameMode];
    Vector3 pos = FindPlayerOne();
    AudioSource.PlayClipAtPoint(clip, pos, vol);
  }
  
  /* Returns the position of player1 */
  private Vector3 FindPlayerOne(){
    foreach(Actor a in players){
      if(a != null && a.playerNumber == 1){
        return a.gameObject.transform.position;
      }
    }
    return new Vector3();
  }
  

  /* Updates hud with current time and objectives */
  void UpdateHUD(){
    ArenaHUDMenu HUD = (ArenaHUDMenu)menu.active;
    if(HUD == null){ print("Menu null"); return; }
    HUD.message = "Arena deathmatch\n" + Time();
    if(!respawns){
      HUD.message += ("\nPlayers remaining: \n" + players.Count);
    }
    time--;
    if(time < 1){ EndGame(); }
  }
  
  /* Stop gameplay and display end game message. */
  void EndGame(){
    ArenaHUDMenu HUD = (ArenaHUDMenu)menu.active;
    HUD.subMenu = 1;
    for(int i = 0; i < players.Count; i++){
      players[i].SetMenuOpen(true);
      Destroy(players[i].gameObject);
    }
  }

  void HandleKills(){
    for(int i = 0; i < players.Count; i++){
      Actor actor = players[i];
      if(!actor.Alive()){
        players.Remove(actor);
        i--;
        StartCoroutine(RespawnPlayer(actor));
      }
    }
  }

  /* Displays respawn timer and respawns the player at its completion. */
  IEnumerator RespawnPlayer(Actor player){
    List<Item> items = player.DiscardAllItems();
    if(player.droppedLoot != null){ items.AddRange(player.droppedLoot); }
    foreach(Item item in items){
      if(item != null && item.gameObject != null){
        Despawner d = item.gameObject.AddComponent(typeof(Despawner)) as Despawner;
        d.Despawn(60f);
      }
    }
    HUDMenu playerHUD = null;
    if(player.playerNumber > 0 && player.playerNumber < 5){
      MenuManager playerMenu = player.menu;
      playerMenu.Change("HUD");
      if(playerMenu){ playerHUD = (HUDMenu)playerMenu.active; }
    }
    if(respawns){
      yield return RespawnTimer(player, playerHUD);
      Data dat = player.GetData();
      int id = player.id;
      Destroy(player.gameObject);
      if(time > 0){ SpawnPlayer(dat.prefabName, id, player.stats.faction); }
    }
    else{ 
      if(playerHUD != null){ playerHUD.message = "You are dead."; }
      players.Remove(player);
      player.SetMenuOpen(true);
      if(players.Count <= 1){ EndGame(); }
      else if(!respawns && OneTeamLeft()){ EndGame(); }
    }
    yield return new WaitForSeconds(0f);
  }
  
  public Transform GetSpawnTransform(int faction = StatHandler.NEUTRAL){
    switch(faction){
      case StatHandler.NEUTRAL: 
        return soloSpawns[Random.Range(0, soloSpawns.Length)]; 
        break;
      case StatHandler.REDTEAM: 
        return redSpawns[Random.Range(0, redSpawns.Length)];
        break;
      case StatHandler.BLUETEAM: 
        return blueSpawns[Random.Range(0, blueSpawns.Length)];
        break;
    }
    
    return null;
  }
  
  /* Handles a SessionEvent according to the game mode. */
  public void HandleEvent(SessionEvent evt){
    switch(evt.code){
      case SessionEvent.DEATH:
        if(
          gameMode == DEATHMATCH || gameMode == TEAMDEATHMATCH ||
          gameMode == ELIMINATION || gameMode == TEAMELIMINATION
        ){ RecordKill(evt); }
        break;
    }
  }
  
  
  /* Records the points for a kill individually or for the team according to
     the teams setting. Friendly kills subtract points.
  */
  private void RecordKill(SessionEvent evt){
    if(evt.args == null || evt.args.Length < 3){ return; }
    if(evt.args[0] == null || evt.args[1] == null){ return; }
    if(evt.args[0].ints.Count < 3 || evt.args[1].ints.Count < 3){ return; }
    int killerId = evt.args[1].ints[1]; // Killer's id from ActorDeathData
    int faction = evt.args[0].ints[2]; // Victim's faction from ActorDeathData
    if(killerId != -1){
      if(teams && factions[killerId] == faction){ 
        scores[killerId]--;
      }
      else if(teams){ scores[killerId]++; }
      else{ scores[killerId]++; }
    }
  }
  
  /* Displays the respawn timer for a dead player. */
  private IEnumerator RespawnTimer(Actor player, HUDMenu playerHUD){
    int respawnTimer = 5;
    for(int i = respawnTimer; i > 0; i--){
      if(playerHUD != null){ playerHUD.message = "Respawn in \n" + i; }  
      yield return new WaitForSeconds(1f);
    }
  }
  
  bool OneTeamLeft(){
    if(!teams){ return false; }
    int reds = 0;
    int blues = 0;
    foreach(Actor player in players){
      if(player.stats.faction == StatHandler.Faction("BLUE") && player.Alive()){ blues++; }
      else if(player.Alive()){ reds++; }
    }
    return ((reds == 0) || (blues == 0));
  }

  /* Add players to map according to starting spot. */
  void InitPlayers(){
    scores = new List<int>();
    names = new List<string>();
    factions = new List<int>();
    players = new List<Actor>();
    for(int i = 0; i < bots; i++){
      int faction = StatHandler.NEUTRAL;
      if(teams){
        faction = (i<(bots/2)) ? StatHandler.REDTEAM : StatHandler.BLUETEAM; 
        factions.Add(faction);
      }
      SpawnPlayer("Enemy", i, faction);
      scores.Add(0);
      names.Add("Bot " + (i+1));
    }
    if(teams){ 
      factions.Add(p1red ? StatHandler.REDTEAM : StatHandler.BLUETEAM);
    }
    SpawnPlayer("player1", bots, p1red ? StatHandler.REDTEAM : StatHandler.BLUETEAM);
    scores.Add(0);
    names.Add("Player1");
    
    
    if(Session.session.playerCount > 1){ 
      if(teams){ 
        factions.Add(p2red ? StatHandler.REDTEAM : StatHandler.BLUETEAM);
      }
      SpawnPlayer("player2", bots + 1, p2red ? StatHandler.REDTEAM : StatHandler.BLUETEAM);
      scores.Add(0);
      names.Add("Player2");
    }
  }

  /* Spawns a player from a prefab at a random spawnpoint. */
  void SpawnPlayer(string prefabName, int id, int faction ){
    GameObject pref = (GameObject)Resources.Load(
      "Prefabs/"+ prefabName,
      typeof(GameObject)
    );
    Transform trans = GetSpawnTransform(faction);
    if(trans == null){
      print("Invalid transform for faction " + faction);
      return;
    }
    Vector3 pos = trans.position;
    Quaternion rot = trans.rotation; 
    GameObject go = (GameObject)GameObject.Instantiate(
      pref,
      pos,
      rot
    );
    Actor actor = go.GetComponent<Actor>();
    if(actor != null){
      actor.stats.faction = faction;
      players.Add(actor);
      if(kit != "NONE"){
        Kit k = Session.session.GetKit(kit);
        if(k != null){ k.ApplyKit(ref actor); }
      }
      if(id != -1){
        actor.id = id;
        if(teams){
          actor.stats.faction = factions[id];
          if(factions[id] == StatHandler.REDTEAM){
            Kit.ApplyToClothes("Equipment/Shirt", ref actor, true, 1f, 0f, 0f, 1f);
          }
          else{
            Kit.ApplyToClothes("Equipment/Shirt", ref actor, true, 0f, 0f, 1f, 1f);
          }
          
        }
        else{
          actor.stats.faction = StatHandler.NEUTRAL;
        }
        Kit.ApplyToClothes("Equipment/Pants", ref actor, true, 0f, 0f, 0f, 1f);
      }
    }
  }
  
  /* Returns the name of a gamemode based on its constant */
  public static string GameModeName(int mode){
    switch(mode){
      case NONE: return "None"; break;
      case DEATHMATCH: return "Deathmatch"; break;
      case TEAMDEATHMATCH: return "Team Deathmatch"; break;
      case ELIMINATION: return "Elimination"; break;
      case TEAMELIMINATION: return "Team Elimination"; break;
    }
    return "";
  }
  
  /* Returns remaining time in minutes and seconds. */
  string Time(){
    int minutes = time/60;
    int seconds = time%60;
    string secs = "";
    if(seconds < 10){ secs += "0"; }
    secs += seconds;
    return minutes + ":" + secs;
  }

}
