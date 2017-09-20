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
  public List<Transform> spawnPoints; //Stores direction and position to spawn players in.
  public List<Actor> players;
  int time;// Remaining round time in seconds.
  int bots;// Number of bots in arena.
  int startingTime; // Total round duration.
  bool respawns; // True if players respawn.
  bool teams; // True if teams are enabled.
  bool p1red, p2red; // True if respective player is on red team.
  string kit; // Default kit for players.
  MenuManager menu;

  public void Start(){
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
    if(dat != null){
      respawns = dat.bools[0];
      teams = dat.bools[1];
      p1red = dat.bools[2];
      p2red = dat.bools[3];
      kit = dat.strings[0];
      
    }
    PopulateSpawnPoints();
    InitPlayers();
    StartCoroutine(UpdateRoutine());
  }

  IEnumerator UpdateRoutine(){
    while(time > 0){
      UpdateHUD();
      HandleKills();
      yield return new WaitForSeconds(1f);
    }
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
    if(player.killerId != -1){
      if(teams && factions[player.killerId] == player.stats.faction){ 
        scores[player.killerId]--;
      }
      else if( teams){ scores[player.killerId]++; }
      scores[player.killerId]++;
    }
    player.DiscardAllItems();
    int respawnTimer = 5;
    HUDMenu playerHUD = null;
    if(player.playerNumber > 0 && player.playerNumber < 5){
      MenuManager playerMenu = player.menu;
      playerMenu.Change("HUD");
      if(playerMenu){ playerHUD = (HUDMenu)playerMenu.active; }
    }
    if(respawns){
      for(int i = respawnTimer; i > 0; i--){
        if(playerHUD != null){ playerHUD.message = "Respawn in \n" + i; }  
        yield return new WaitForSeconds(1f);
      }
      Data dat = player.GetData();
      int id = player.id;
      Destroy(player.gameObject);
      SpawnPlayer(dat.prefabName, id);
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
  
  bool OneTeamLeft(){
    int reds = 0;
    int blues = 0;
    foreach(Actor player in players){
      if(player.stats.faction == StatHandler.Faction("BLUE") && player.Alive()){ blues++; }
      else if(player.Alive()){ reds++; }
    }
    return ((reds == 0) || (blues == 0));
  }
  
  /* Populate spawnpoints from child transforms. */
  void PopulateSpawnPoints(){
    spawnPoints = new List<Transform>();
    foreach(Transform t in transform){
      spawnPoints.Add(t);
    }
  }

  /* Add players to map according to starting spot. */
  void InitPlayers(){
    scores = new List<int>();
    names = new List<string>();
    factions = new List<int>();
    players = new List<Actor>();
    for(int i = 0; i < bots; i++){ 
      if(teams){
        int faction = (i<(bots/2)) ? 1 : 2;
        factions.Add(faction);
      }
      SpawnPlayer("Enemy", i);
      scores.Add(0);
      names.Add("Bot " + (i+1));
    }
    if(teams){ factions.Add(p1red ? 1 : 2); }
    SpawnPlayer("player1", bots);
    scores.Add(0);
    names.Add("Player1");
    
    
    if(Session.session.playerCount > 1){ 
      if(teams){ factions.Add(p2red ? 1 : 2); }
      SpawnPlayer("player2", bots + 1);
      scores.Add(0);
      names.Add("Player2");
    }
  }

  /* Spawns a player from a prefab at a random spawnpoint. */
  void SpawnPlayer(string prefabName, int id = -1){
    Transform trans = spawnPoints[Random.Range(0, spawnPoints.Count)]; 
    Vector3 pos = trans.position;
    Quaternion rot = trans.rotation; 
    GameObject pref = (GameObject)Resources.Load(
      "Prefabs/"+ prefabName,
      typeof(GameObject)
    );
    GameObject go = (GameObject)GameObject.Instantiate(
      pref,
      pos,
      rot
    );
    Actor actor = go.GetComponent<Actor>();
    if(actor != null){
      players.Add(actor);
      if(kit != ""){ LootTable.Kit(kit, ref actor); }
      if(id != -1){
        actor.id = id;
        if(teams){
          actor.stats.faction = factions[id];
          string shirt = (factions[id] == 1) ? "RED" : "BLUE";
          LootTable.Kit(shirt, ref actor);
        }
        else{
          LootTable.Kit("PANTS", ref actor);
          actor.stats.faction = 1;
        }
      }
    }
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
