/*
    Arena handles the arena gameplay.
    Players are spawned and a timer is set.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class Arena : MonoBehaviour{
  public List<Transform> spawnPoints; //Stores direction and position to spawn players in.
  public List<Actor> players;
  int time;// Remaining round time in seconds.
  int startingTime; // Total round duration.
  MenuManager menu;
  
  public void Start(){
    Initialize();
  }
  
  /* Begin a new round. */
  public void Initialize(){
    Data dat = Session.session.arenaData;
    menu = gameObject.AddComponent<MenuManager>();
    menu.Change("ARENAHUD");
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
    time--;
    if(time < 1){ 
      HUD.subMenu = 1;
      for(int i = 0; i < players.Count; i++){
        players[i].SetMenuOpen(true);
        Destroy(players[i].gameObject);
      }
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
    int respawnTimer = 5;
    HUDMenu playerHUD = null;
    if(player.playerNumber > 0 && player.playerNumber < 5){
      MenuManager playerMenu = player.menu;
      if(playerMenu){ playerHUD = (HUDMenu)playerMenu.active; }
    }
    for(int i = respawnTimer; i > 0; i--){
      if(playerHUD != null){ playerHUD.message = "Respawn in \n" + i; }  
      yield return new WaitForSeconds(1f);
    }
    Data dat = player.GetData();
    Destroy(player.gameObject);
    SpawnPlayer(dat.prefabName);
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
    players = new List<Actor>();
    SpawnPlayer("player1");
    if(Session.session.playerCount > 1){ SpawnPlayer("player2"); }
    int bots = 15;
    for(int i = 0; i < bots; i++){ SpawnPlayer("Enemy"); }
  }
  
  /* Spawns a player from a prefab at a random spawnpoint. */
  void SpawnPlayer(string prefabName){
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
