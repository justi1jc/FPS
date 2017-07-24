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
  public List<string> names;
  public List<Transform> spawnPoints; //Stores direction and position to spawn players in.
  public List<Actor> players;
  int time;// Remaining round time in seconds.
  int startingTime; // Total round duration.
  bool respawns; // True if players respawn.
  MenuManager menu;

  public void Start(){
    Initialize();
  }

  /* Begin a new round. */
  public void Initialize(){
    Data dat = Session.session.arenaData;
    menu = gameObject.AddComponent<MenuManager>();
    menu.Change("ARENAHUD");
    menu.arena = this;
    time = startingTime = dat != null ? 60 * dat.ints[0] : 600;
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
      scores[player.killerId]++;
    }
    int respawnTimer = 5;
    HUDMenu playerHUD = null;
    if(player.playerNumber > 0 && player.playerNumber < 5){
      MenuManager playerMenu = player.menu;
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
    }
    yield return new WaitForSeconds(0f);
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
    players = new List<Actor>();
    int bots = 15;
    for(int i = 0; i < bots; i++){ 
      SpawnPlayer("Enemy", i);
      scores.Add(0);
      names.Add("Bot " + (i+1));
    }
    SpawnPlayer("player1", bots);
    scores.Add(0);
    names.Add("Player1");
    
    if(Session.session.playerCount > 1){ 
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
      LootTable.Kit("ASSAULT", ref actor);
      if(id != -1){ actor.id = id; }
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
