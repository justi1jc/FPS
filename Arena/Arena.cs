/*
    Arena is the base for all Arena gamemodes. An Arena instance should exist
    in any scene used for the arena and contain map-specific data such as 
    spawnpoints. When started, this gamemode  
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class Arena : MonoBehaviour{
  public List<int> scores;
  public List<StatHandler.Factions> factions; 
  public List<string> names;
  public List<Actor> players;
  protected int time;// Remaining round time in seconds.
  protected int bots;// Number of bots in arena.
  protected int startingTime; // Total round duration.
  protected bool respawns; // True if players respawn.
  protected bool teams; // True if teams are enabled.
  protected bool p1red, p2red; // True if respective player is on red team.
  protected string kit; // Default kit for players.
  protected MenuManager menu;
  protected GameModes gameMode; // Active gamemode selected from lobby.
  public AudioClip[] gameModeClips; // Audio for gamemodes
  public Transform[] soloSpawns, redSpawns, blueSpawns;
  public bool spawnWeapons; // If set to true, spawners will spawn weapons. 
  
  // GameMode  constants
  public enum GameModes{ None, Deathmatch };
  
  public virtual void Start(){
    Arena arena = null;
    switch(ActiveGameMode()){
      case GameModes.Deathmatch: 
        DeathmatchArena dmarena = gameObject.AddComponent<DeathmatchArena>();
        arena = (Arena)dmarena;
      break;
    }
    if(arena == null){ return; }
    menu = gameObject.AddComponent<MenuManager>();
    menu.Change("ARENAHUD");
    arena.CopyData(this);
    arena.LoadDataFromSession();
    arena.Begin();
  }
  
  /* STUB: Handles a SessionEvent according to the game mode. */
  public virtual void HandleEvent(SessionEvent evt){}
  
  /* STUB: Starts the selected gamemode. */
  public virtual void Begin(){}
  
  /* STUB: Loads settings contained within Session.session.arenaData
     Note that this data should be encoded specifically for the current
     gamemode.
  */
  public virtual void LoadDataFromSession(){}
  
  /* STUB: Main update loop. */
  protected virtual IEnumerator UpdateRoutine(){ 
    yield return new WaitForSeconds(1f); 
  }
  
  /* STUB: Returns a string to display on the HUD */
  protected virtual string Message(){ return ""; }
  
  /* Copies over data from one instance to another */
  public void CopyData(Arena ar){
    menu = ar.menu;
    menu.arena = (Arena)this;
    soloSpawns = ar.soloSpawns;
    redSpawns = ar.redSpawns;
    blueSpawns = ar.blueSpawns;
    gameModeClips = ar.gameModeClips;
  }

  /* Plays the AudioClip for this game mode. */
  protected void GameModeAnnouncement(){
    if(gameModeClips == null || gameModeClips.Length < (int)gameMode || gameMode == GameModes.None){ 
      print("Invalid clip.");
      return; 
    }
    float vol = PlayerPrefs.HasKey("masterVolume") ? PlayerPrefs.GetFloat("masterVolume") : 1f;
    AudioClip clip = gameModeClips[(int)gameMode];
    Vector3 pos = FindPlayerOne();
    AudioSource.PlayClipAtPoint(clip, pos, vol);
  }
  
  /* Returns the position of player1 */
  protected Vector3 FindPlayerOne(){
    foreach(Actor a in players){
      if(a != null && a.playerNumber == 1){
        return a.gameObject.transform.position;
      }
    }
    return new Vector3();
  }
  

  /* Updates hud with current time and objectives */
  protected void UpdateHUD(){
    if(menu == null || menu.active == null){ 
      print("NoHUD");
      return;
    }
    ArenaHUDMenu HUD = (ArenaHUDMenu)menu.active;
    if(HUD == null){ print("Menu null"); return; }
    HUD.message = Message();
    if(!respawns){
      HUD.message += ("\nPlayers remaining: \n" + players.Count);
    }
    time--;
  }
  
  /* Displays respawn timer and respawns the player at its completion. */
  protected IEnumerator RespawnPlayer(Actor player){
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
    }
    yield return new WaitForSeconds(0f);
  }
  
  /* Returns a random spawnpoint according to the faction, or null. */
  public Transform GetSpawnTransform(StatHandler.Factions faction = StatHandler.Factions.Feral){
    switch(faction){
      case StatHandler.Factions.Feral: 
        return soloSpawns[Random.Range(0, soloSpawns.Length)]; 
        break;
      case StatHandler.Factions.RedTeam: 
        return redSpawns[Random.Range(0, redSpawns.Length)];
        break;
      case StatHandler.Factions.BlueTeam: 
        return blueSpawns[Random.Range(0, blueSpawns.Length)];
        break;
    }
    return null;
  }
  
  /* Records the points for a kill individually or for the team according to
     the teams setting. Friendly kills subtract points.
  */
  protected void RecordKill(SessionEvent evt){
    if(evt.args == null || evt.args.Length < 3){ return; }
    if(evt.args[0] == null || evt.args[1] == null){ return; }
    if(evt.args[0].ints.Count < 3 || evt.args[1].ints.Count < 3){ return; }
    int killerId = evt.args[1].ints[1]; // Killer's id from ActorDeathData
    int faction = evt.args[0].ints[2]; // Victim's faction from ActorDeathData
    if(killerId != -1){
      if(teams && (int)factions[killerId] == faction){ 
        scores[killerId]--;
      }
      else if(teams){ scores[killerId]++; }
      else{ scores[killerId]++; }
    }
  }
  
  /* Displays the respawn timer for a dead player. */
  protected IEnumerator RespawnTimer(Actor player, HUDMenu playerHUD){
    int respawnTimer = 5;
    for(int i = respawnTimer; i > 0; i--){
      if(playerHUD != null){ playerHUD.message = "Respawn in \n" + i; }  
      yield return new WaitForSeconds(1f);
    }
  }
  
  protected bool OneTeamLeft(){
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
  protected void InitPlayers(){
    scores = new List<int>();
    names = new List<string>();
    factions = new List<StatHandler.Factions>();
    players = new List<Actor>();
    for(int i = 0; i < bots; i++){
      StatHandler.Factions faction = StatHandler.Factions.Feral;
      if(teams){
        faction = (i<(bots/2)) ? StatHandler.Factions.RedTeam : StatHandler.Factions.BlueTeam; 
        factions.Add(faction);
      }
      SpawnPlayer("Enemy", i, faction);
      scores.Add(0);
      names.Add("Bot " + (i+1));
    }
    if(teams){ 
      factions.Add(p1red ? StatHandler.Factions.RedTeam : StatHandler.Factions.BlueTeam);
    }
    SpawnPlayer(
      "player1", 
      bots, 
      p1red ? StatHandler.Factions.RedTeam : StatHandler.Factions.BlueTeam
    );
    scores.Add(0);
    names.Add("Player1");
    
    
    if(Session.session.playerCount > 1){ 
      if(teams){ 
        factions.Add(p2red ? StatHandler.Factions.RedTeam : StatHandler.Factions.BlueTeam);
      }
      SpawnPlayer(
        "player2",
        bots + 1,
        p2red ? StatHandler.Factions.RedTeam : StatHandler.Factions.BlueTeam
      );
      scores.Add(0);
      names.Add("Player2");
    }
  }

  /* Spawns a player from a prefab at a random spawnpoint. */
  protected void SpawnPlayer(
    string prefabName, 
    int id,
    StatHandler.Factions faction, 
    Transform trans = null 
  ){
    GameObject pref = (GameObject)Resources.Load(
      "Prefabs/"+ prefabName,
      typeof(GameObject)
    );
    if(trans == null){ trans = GetSpawnTransform(faction); } 
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
          if(factions[id] == StatHandler.Factions.RedTeam){
            Kit.ApplyToClothes("Equipment/Shirt", ref actor, true, 1f, 0f, 0f, 1f);
          }
          else{
            Kit.ApplyToClothes("Equipment/Shirt", ref actor, true, 0f, 0f, 1f, 1f);
          }
          
        }
        else{
          actor.stats.faction = StatHandler.Factions.Feral;
        }
        Kit.ApplyToClothes("Equipment/Pants", ref actor, true, 0f, 0f, 0f, 1f);
      }
    }
  }
  
  /* Returns the name of a gamemode based on its constant */
  public static string GameModeName(GameModes mode){
    switch(mode){
      case GameModes.None: return "None"; break;
      case GameModes.Deathmatch: return "Deathmatch"; break;
    }
    return "";
  }
  
  /* Returns the gamemode from Session's arenaData, or NONE. */
  protected GameModes ActiveGameMode(){
    if(!Session.Active() || Session.session.arenaData == null){ 
      return GameModes.None; 
    }
    Data dat = Session.session.arenaData;
    if(dat.ints == null || dat.ints.Count < 1){ return GameModes.None; }
    return (GameModes)dat.ints[0];
  }
  
  /* Returns remaining time in minutes and seconds. */
  protected string Time(){
    int minutes = time/60;
    int seconds = time%60;
    string secs = "";
    if(seconds < 10){ secs += "0"; }
    secs += seconds;
    return minutes + ":" + secs;
  }

}
