/*
*
*   A singleton whose purpose is to manage the session's data.
*   
*
*
*   Note: Axes and buttons must be added manually in the editor to
*   a specific project.
*
*
*/



﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
public class Session : MonoBehaviour {
  public static Session session;
  public static bool fileAccess = false; //True if files are currently accessed
  public string sessionName; //Name Used in save file.
  
  
  // Controller one linux values
  public static string C1 = "Joystick 1"; //Controller 1
  public static string XL = "XL"; // "X Axis" DeadZone: 0.2 Accuracy: 1
  public static string YL = "YL"; // "Y Axis" DeadZone: 0.2 Accuracy: 1
  public static string XR = "XR"; // "4rd Axis" DeadZone: 0.2 Accuracy: 1
  public static string YR = "YR"; // "5th Axis" DeadZone: 0.2 Accuracy: 1
  public static string RT = "RT"; // "6th Axis" DeadZone: 0.1 
  public static string LT = "LT"; // "3rd Axis" DeadZone: 0.1
  public static string DX = "DX";  // "7th Axis" for wired controllers
  public static string DY = "DY";  // "8th Axis"
  public static string RB = "joystick button 5"; // 5 Right bumper
  public static string LB = "joystick button 4"; // 4 Left bumper
  public static string A = "joystick button 0"; // 0
  public static string B = "joystick button 1"; // 1
  public static string X = "joystick button 2"; // 2
  public static string Y = "joystick button 3"; // 3
  public static string START = "joystick button 7"; // 7
  public static string SELECT = "joystick button 6"; // 6
  public static string DUB = "joystick button 13"; // 13  D-pad Up For wireless controllers
  public static string DDB = "joystick button 14"; // 14  D-pad down
  public static string DRB = "joystick button 11"; // 11  D-pad right
  public static string DLB = "joystick button 12"; // 12  D-pad left
  public static string RSC = "joystick button 10"; // 10  Right stick click
  public static string LSC = "joystick button 9";  // 9   left stick click
  
  // players
  Camera cam1;
  Camera cam2;
  List<Actor> players;
  List<Data> playerData;
  
  // World.
  const string MENU_BUILDING = "House";
  const string MENU_INTERIOR = "ActI";
  const string INIT_BUILDING = "House";
  const string INIT_INTERIOR = "ActI";
  HoloDeck[] decks;
  Vector3[] spawnPoints;
  Vector3[] spawnRots;
  public MapRecord map;
  bool interior = true; // True if interior is currently loaded.
  string buildingName = MENU_BUILDING;
  string interiorName = MENU_INTERIOR;
  int xCord, yCord; // Coordinates on overworld
  
  // Main menu UI
  bool mainMenu; // True when main menu is active.
  Camera sesCam;
  Menu sesMenu;
  
  void Awake(){
    if(Session.session){ Destroy(this); }
    else{ Session.session = this; }
    decks = new HoloDeck[1];
    players = new List<Actor>();
    playerData = new List<Data>();
    spawnPoints = new Vector3[1];
    spawnRots = new Vector3[1];
    CreateMenu();
    LoadMaster();
    CreateDeck();
  }
  
  
  void Update(){

  }
  
  public void RegisterPlayer(Actor actor, int player, Camera cam){
    if(player == 1){ cam1 = cam; }
    else if(player == 2){ cam2 = cam; }
    UpdateCameras();
    players.Add(actor);
  }
  
  public void UnregisterPlayer(int player){
    if(player == 1){ cam1 = null; }
    else if(player == 2){ cam2 = null; }
    UpdateCameras();
  }
  
  public void CreateGame(string sesName){
    sessionName = sesName;
    DestroyMenu();
    CreateLoadingScreen();
    decks[0].initialized = false;
    LoadInterior(INIT_BUILDING, INIT_INTERIOR);
    DestroyLoadingScreen();
    Spawn("Player1", spawnPoints[0]);
    
  }
  
  /*
    Loads a particular interior into a specified deck. If init is false, 
    the player will be placed at the specified door.
  */
  public void LoadInterior(
    string building, 
    string cellName, 
    int deck = 0, 
    int door = -1,
    bool init = false
  ){
    SavePlayers();
    decks[deck].LoadInterior(building, cellName, door, playerData, init);
    playerData = new List<Data>();
  }
  
  /* Packs the current cell and loads an exterior. */
  public void LoadExterior(){
    
  }
  
  /* Create camera and menu to display loading screen. */
  public void CreateLoadingScreen(){
  }
  
  /* Remove camera and menu. */
  public void DestroyLoadingScreen(){
  }
  
  /* Sets up each player's Menu */
  void UpdateCameras(){
    bool split = cam1 && cam2;
    if(split){
      cam1.rect = new Rect(0f, 0f, 0.5f, 1f);
      cam2.rect = new Rect(0.5f, 0, 0.5f, 1f);
      Menu menu = cam1.gameObject.GetComponent<Menu>();
      if(menu){ menu.split = true; menu.right = false; }
      menu = cam2.gameObject.GetComponent<Menu>();
      if(menu){ menu.split = true; menu.right = true; }
    }
    else if(cam1){
      cam1.rect = new Rect(0f, 0f, 1f, 1f);
      Menu menu = cam1.gameObject.GetComponent<Menu>();
      if(menu){ menu.split = false; menu.right = false; }
    }
    else if(cam2){
      cam2.rect = new Rect(0f, 0f, 1f, 1f);
      Menu menu = cam2.gameObject.GetComponent<Menu>();
      if(menu){ menu.split = false; menu.right = false; }
    }   
  }
  
  /* Instantiates gameObject of a specific prefab  
     as close to the desired location as possible.
     will spawn gameObject directly on top of map if
     there's no space large enough for the movecheck. */
  public GameObject Spawn(string prefab, Vector3 pos){
    GameObject go = null;
    GameObject pref = (GameObject)Resources.Load(
      prefab,
      typeof(GameObject)
    );
    if(!pref){ print("Prefab null:" + prefab); return go; }
    Vector3[] candidates = GroundedColumn(pos, pref.transform.localScale);
    int min = 0;
    float minDist, dist;
    for(int i = 0; i < candidates.Length; i++){
      minDist = Vector3.Distance(candidates[min], pos);
      dist = Vector3.Distance(candidates[i], pos);
      if(minDist > dist){
        min = i;
      }
    }
    go = (GameObject)GameObject.Instantiate(
      pref,
      candidates[min],
      Quaternion.identity
    );
    return go;
  }
  
  /* Adds Camera and Menu to gameObject, sets main menu. */
  public void CreateMenu(){
    mainMenu = true;
    GameObject go = new GameObject();
    go.transform.position = transform.position + new Vector3(10f, 10f, 0f);
    go.transform.LookAt(transform);
    sesCam = go.AddComponent(typeof(Camera)) as Camera;
    sesMenu = go.AddComponent(typeof(Menu)) as Menu;
    sesMenu.Change(Menu.MAIN);
  }
  
  /* Destroys Camera and Menu attached to gameObject */
  public void DestroyMenu(){
    Camera cam = sesCam;
    sesCam = null;
    Destroy(cam.gameObject);
    mainMenu = false;
  }
  
  public void CreateDeck(){
    decks[0] = gameObject.AddComponent(typeof(HoloDeck)) as HoloDeck;
    decks[0].interior = interior;
    decks[0].initialized = false;
    decks[0].LoadInterior(buildingName, interiorName, -1, playerData, true);
  }
  
  public void LoadMaster(){
    CellSaver saver = gameObject.AddComponent(typeof(CellSaver)) as CellSaver;
    saver.LoadMaster();
    map = saver.map;
    Destroy(saver);
  }
  
  /* Returns an array of viable positions consisting of empty space directly
     above colliders, This is like surveying how many stories a building
     has to it. */
  Vector3[] GroundedColumn(Vector3 pos, Vector3 scale,
                           float max=100f, float min=-100f){
    Vector3 origin = pos;
    List<Vector3> grounded = new List<Vector3>();
    pos = new Vector3( pos.x, max, pos.z);
    Vector3 last = pos;
    bool lastPlace = true;
    while(pos.y > min){
      bool check = PlacementCheck(pos, scale);
      if(!check && lastPlace){ grounded.Add(last + Vector3.up); }
      last = pos;
      pos = new Vector3(pos.x, pos.y-scale.y, pos.z);
      lastPlace = check;
    }
    if(grounded.Count == 0){ grounded.Add(origin); } // Don't return an empty array.
    return grounded.ToArray();
  }
  
  /* Performs a boxcast at a certain point and scale. Returns true on collison. */
  bool PlacementCheck(Vector3 pos, Vector3 scale){
    Vector3 direction = Vector3.up;
    float distance = scale.y;
    Vector3 center = pos;
    Vector3 halfExtents = scale / 2;
    Quaternion orientation = Quaternion.identity;
    int layerMask = ~(1 << 8);
    RaycastHit hit;
    bool check = !Physics.BoxCast(
      center,
      halfExtents,
      direction,
      out hit,
      orientation,
      distance,
      layerMask,
      QueryTriggerInteraction.Ignore
    );
    return check;
  }
  
  /* Overwrite specific file with current session's game data. */
  public void SaveGame(string fileName){
    if(fileAccess){ return; }
    fileAccess = true;
    BinaryFormatter bf = new BinaryFormatter();
    string path = Application.persistentDataPath + "/" + fileName + ".save"; 
    using(FileStream file = File.Create(path)){
      GameRecord record = GetData();
      bf.Serialize(file, record);
      file.Close();
    }
    fileAccess = false;
  }
  
  /* Load contents from a specific file. */
  public void LoadGame(string fileName){
    GameRecord record = LoadFile(fileName);
    if(record == null){ return; }
    ClearData();
    LoadData(record);
    if(interior){ LoadInterior(buildingName, interiorName, 0, -1, true); }
  }
  
  /* Returns a GameRecord containing this Session's data. */
  GameRecord GetData(){
    GameRecord record = new GameRecord();
    record.sessionName = sessionName;
    for(int i = 0; i < decks.Length; i++){
      if(decks[i].interior){ decks[i].SaveInterior(); }
      else{ print("Exteriors not implemented"); }
    }
    record.interior = interior;
    record.map = map;
    record.currentBuilding = decks[0].deck.building;
    record.currentInterior = decks[0].deck.displayName;
    record.x = xCord;
    record.y = yCord;
    record.players = playerData;
    
    for(int i = 0; i < players.Count; i++){
      if(players[i] != null){ record.players.Add(players[i].GetData()); }
    }
    for(int i = 0; i < record.players.Count; i++){
      print("Saved NPC: " + record.players[i].displayName);
    }
    return record;
  }
  
  public void LoadData(GameRecord dat){
    sessionName = dat.sessionName;
    map = dat.map;
    buildingName = dat.currentBuilding;
    interiorName = dat.currentInterior;
    playerData = dat.players;
  }
  
  /* Clears all Cells and players. */
  public void ClearData(){
    if(mainMenu){ DestroyMenu(); }
    for(int i = 0; i < decks.Length; i++){
      decks[i].ClearInterior();
    }
    for(int i = 0; i < players.Count; i++){
      if(players[i] != null){ Destroy(players[i].gameObject); }
    }
    players = new List<Actor>();
  }
  
  /* Returns a GameRecord containing data from a specified file, or null.*/
  GameRecord LoadFile(string fileName){
    
    if(fileAccess){ return null; }
    fileAccess = true;
    BinaryFormatter bf = new BinaryFormatter();
    string path = Application.persistentDataPath + "/" + fileName + ".save";
    if(!File.Exists(path)){ fileAccess = false; return null; }
    using(FileStream file = File.Open(path, FileMode.Open)){
      GameRecord record = (GameRecord)bf.Deserialize(file);
      file.Close();
      fileAccess = false;
      return record;
    }
  }
  
  /* Returns an array of every valid GameRecord in the directory.*/
  public List<GameRecord> LoadFiles(){
    if(fileAccess){ return new List<GameRecord>(); }
    fileAccess = true;
    List<GameRecord> records = new List<GameRecord>();
    string path = Application.persistentDataPath + "/";
    DirectoryInfo dir = new DirectoryInfo(path);
    FileInfo[] info = dir.GetFiles("*.save");
    BinaryFormatter bf = new BinaryFormatter();
    for(int i = 0; i < info.Length; i++){
      using(FileStream file = File.Open(info[i].FullName, FileMode.Open)){
        GameRecord record = (GameRecord)bf.Deserialize(file);
        records.Add(record);
      }
    }
    fileAccess = false;
    return records;
  }
  
  /* Collects the data of all registered players. */
  public void SavePlayers(){
    for(int i = 0; i < players.Count; i++){
      if(players[i] != null){
        playerData.Add(players[i].GetData());
      }
    }
  }
}
