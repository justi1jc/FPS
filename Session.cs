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
  List<Data> playerData;
  Camera cam1;
  Camera cam2;
  
  // World.
  public List<HoloDeck> decks; // active HoloDecks
  public MapRecord map; // World map.
  
  // Main menu UI
  bool mainMenu; // True when main menu is active.
  HoloCell menuCell;
  Camera sesCam;
  Menu sesMenu;
  
  void Awake(){
    if(Session.session){ Destroy(this); }
    else{ Session.session = this; }
    decks = new List<HoloDeck>();
    CreateMenu();
  }
  
  /* Updates cameras and associates player with appropriate HoloDeck. */
  public void RegisterPlayer(Actor actor, int player, Camera cam){
    if(player == 1){ cam1 = cam; }
    else if(player == 2){ cam2 = cam; }
    UpdateCameras();
    for(int i = 0; i < decks.Count; i++){
      if(decks[i].RegisterPlayer(actor)){ break; }
    }
  }
  
  /* Updates the cameras according to remaining players. */
  public void UnregisterPlayer(int player){
    if(player == 1){ cam1 = null; }
    else if(player == 2){ cam2 = null; }
    UpdateCameras();
  }
  
  /* Create a new game.
     Warning: This is hardcoded in a project-specific fashion.
  */
  public void CreateGame(string sesName){
    sessionName = sesName;
    if(mainMenu){ DestroyMenu(); }
    CreateLoadingScreen();
    map = Cartographer.GetMaster();
    map = Cartographer.Generate(map, 3, 3);
    HoloDeck deck = CreateDeck();
    Cell initCell = GetStartingCell();
    if(initCell == null){ print("Init cell null"); return; }
    deck.LoadInterior(initCell, 0, false);
    deck.AddPlayer("player1");
    DestroyLoadingScreen();
  }
  
  /* Returns the interior cell the player aught to start in at the beginning of
     the game.
     WARNING: This is hardcoded and must be updated to match the master file.
  */
  public Cell GetStartingCell(){
    string INIT_BUILDING = "House";
    string INIT_ROOM = "ActI";
    for(int i = 0; i < map.interiors.Count; i++){
      bool bmatch = map.interiors[i].building == INIT_BUILDING;
      bool rmatch = map.interiors[i].displayName == INIT_ROOM;
      if(bmatch && rmatch){
        return map.interiors[i];
      }
    }
    return null;
  }
  
  /*
    Loads a particular interior into a specified deck. If init is false, 
    the player will be placed at the specified door.
  */
  public void LoadInterior(
    string building, 
    string cellName, 
    int x, int y,
    int deck = 0, 
    int door = -1,
    bool saveFirst = true
  ){
    decks[deck].LoadInterior(building, cellName, door, x, y, saveFirst);
  }
  
  /* Packs the current cell and loads an exterior. */
  public void LoadExterior(
    int x, int y,
    int deck = 0,
    int door = -1,
    bool saveFirst = true
  ){
    decks[deck].LoadExterior(door, x, y, saveFirst);
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
  
  /* Creates UI for main menu and renders menu cell in the background. 
     Warning: Is hardcoded with project-specific variables.
  */
  public void CreateMenu(){
    string MENU_BUILDING = "House";
    string MENU_INTERIOR = "ActI";
    mainMenu = true;
    GameObject go = new GameObject();
    go.transform.position = transform.position + new Vector3(10f, 10f, 0f);
    go.transform.LookAt(transform);
    sesCam = go.AddComponent(typeof(Camera)) as Camera;
    sesMenu = go.AddComponent(typeof(Menu)) as Menu;
    sesMenu.Change(Menu.MAIN);
    menuCell = new HoloCell(transform.position, -1);
    Cell c = GetMasterInterior(MENU_BUILDING, MENU_INTERIOR);
    if(c == null){ print("Couldn't find menu cell"); return; }
    menuCell.LoadData(c);
    
  }
  
  /* Grabs specified interior from loaded master file. */
  public Cell GetMasterInterior(string building, string name){
    for(int i = 0; i < map.buildings.Count; i++){
      if(map.buildings[i][0].building == building){
        for(int j = 0; j < map.buildings[i].Length; j++){
          if(map.buildings[i][j].displayName == name){
            return map.buildings[i][j];
          }
        }
      }
    }
    return null;
  }
  
  /* Destroys Camera and Menu attached to gameObject */
  public void DestroyMenu(){
    Camera cam = sesCam;
    sesCam = null;
    Destroy(cam.gameObject);
    mainMenu = false;
    menuCell.Clear();
    menuCell = null;
  }
  
  /* Clears all HoloDecks and then removes them. */
  public void ClearDecks(){
    for(int i = 0; i < decks.Count; i++){
      decks[i].ClearContents();
      Destroy(decks[i]);
    }
    decks = new List<HoloDeck>();
  }
  
  /* Initializes a new HoloDeck*/
  public HoloDeck CreateDeck(){
    HoloDeck ret = gameObject.AddComponent<HoloDeck>();
    decks.Add(ret);
    ret.Start();
    return ret;
  }
  
  /* Gathers player data from all decks. */
  public List<Data> GetPlayers(){
    List<Data> ret = new List<Data>();
    for(int i = 0; i < decks.Count; i++){ ret.AddRange(decks[i].GetPlayers()); }
    return ret;
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
    if(record == null){ print("Null game record"); return; }
    LoadData(record);
    if(playerData.Count == 0){ print("There are no players."); return; }
    HoloDeck hd = CreateDeck();
    Data player = playerData[0];
    Cell c = player.lastPos;
    if(c.interior){
      hd.LoadInterior(c.building, c.displayName, -1, c.x, c.y, false);
    }
    else{
      hd.LoadExterior(-1, c.x, c.y, false);
    }
    hd.AddPlayer(player);
    playerData.Remove(player);
  }
  
  /* Returns a GameRecord containing this Session's data. */
  GameRecord GetData(){
    GameRecord record = new GameRecord();
    record.sessionName = sessionName;
    for(int i = 0; i < decks.Count; i++){
      if(decks[i].interior){ decks[i].SaveInterior(); }
      else{ decks[i].SaveExterior(); }
    }
    record.map = map;
    record.players = GetPlayers();
    return record;
  }
  
  /* Loads the contents of a GameRecord */
  public void LoadData(GameRecord dat){
    sessionName = dat.sessionName;
    map = dat.map;
    playerData = dat.players;
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
  
  /* Returns a requested interior or null.
     TODO: Cache map to reduce overhead.
     TODO: Make sure requested cell is not already active.
  */
  public Cell GetInterior(string building, string name, int x, int y){
    for(int i = 0; i < map.interiors.Count; i++){
      bool bmatch = building == map.interiors[i].building;
      bool nmatch = name == map.interiors[i].displayName;
      bool xmatch = x == map.interiors[i].x;
      bool ymatch = y == map.interiors[i].y;
      if(bmatch && nmatch && xmatch && ymatch){ return map.interiors[i]; }
    }
    return null;
  }
  
  /* Updates a specified interior. */
  public void SetInterior(string building, string name, int x, int y, Cell c){
    for(int i = 0; i < map.interiors.Count; i++){
      bool bmatch = building == map.interiors[i].building;
      bool nmatch = name == map.interiors[i].displayName;
      bool xmatch = x == map.interiors[i].x;
      bool ymatch = y == map.interiors[i].y;
      if(bmatch && nmatch && xmatch && ymatch){
        map.interiors[i] = c;
        return; 
      }
    }
  }
  
  /* Returns a specififed exterior or null. 
     TODO: Cache map to reduce overhead.
     TODO: Make sure requested cell is not already active.
  */
  public Cell GetExterior(int x, int y){
    for(int i = 0; i < map.exteriors.Count; i++){
      bool xmatch = map.exteriors[i].x == x;
      bool ymatch = map.exteriors[i].y == y;
      if(xmatch && ymatch){ return map.exteriors[i]; }
    }
    return null;
  }
  
  /* Updates a specified exterior. */
  public void SetExterior(int x, int y, Cell c){
    for(int i = 0; i < map.exteriors.Count; i++){
      bool xmatch = map.exteriors[i].x == x;
      bool ymatch = map.exteriors[i].y == y;
      if(xmatch && ymatch){
        map.exteriors[i] = c;
        return; 
      }
    }
  }
}
