/*
    A singleton whose purpose is to manage the session's data and 
    route calls to their appropriate destination..
   
    Note: Axes and buttons must be added manually in the editor to
    a specific project.
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
  private readonly object syncLock = new object(); // Mutex lock
  public string sessionName; //Name Used in save file.
  public Data arenaData = null;
  public Modes mode = Modes.None;
  
  // Modes of play.
  public enum Modes{ None, Arena, Adventure };
  
  // Arena
  public int playerCount = 1;
  private List<Kit> kits;
  public Arena arena;
  
  // Adventure
  public World world;
  public List<HoloDeck> decks; // active HoloDecks
  int currentID = 0;
  
  // players
  List<Data> playerData;
  Camera cam1;
  Camera cam2;

  
  // Main menu UI
  Camera sesCam;
  public MenuManager sesMenu;
  public JukeBox jukeBox;

  void Awake(){
    DontDestroyOnLoad(gameObject);
    if(Session.session != null){ Destroy(this); }
    else{ Session.session = this; }
    decks = new List<HoloDeck>();
    if(jukeBox == null){ jukeBox = new JukeBox(this); }
    CreateMenu();
  }
  
  /* Updates cameras and associates player with appropriate HoloDeck. */
  public void RegisterPlayer(Actor actor, int player, Camera cam){
    if(player == 1){ cam1 = cam; }
    else if(player == 2){ cam2 = cam; }
    UpdateCameras();
  }
  
  /* Updates the cameras according to remaining players. */
  public void UnregisterPlayer(int player){
    if(player == 1){ cam1 = null; }
    else if(player == 2){ cam2 = null; }
    UpdateCameras();
  }
  
  /* Returns true if the session instance exists. */
  public static bool Active(){
    return Session.session != null;
  }
  
  /* Create a new game.
     Warning: This is hardcoded in a project-specific fashion.
  */
  public void CreateAdventure(){
    if(sesMenu != null){ DestroyMenu(); }
    world = new World();
    world.CreateAdventure();
  }

  /* Cached access to Kit.GetKits() to reduce file parsing.
     Reload clears the cache when set to true.
  */
  public List<Kit> GetKits(bool reload = false){
    if(kits == null || reload){ 
      kits = Kit.GetKits();
      for(int i = 0; i < 6; i++){ kits.Add(Kit.LoadKit(i)); }
    }
    return new List<Kit>(kits);
  }
  
  /* Returns a specific kit by name, or null. */
  public Kit GetKit(string kitName){
    if(kits == null){ kits = Kit.GetKits(); }
    foreach(Kit kit in kits){
      if(kit.name == kitName){ return kit; }
    }
    print(kitName + " not found.");
    return null;
  }
  
  /* Load contents from a specific file. */
  public void LoadGame(string fileName){
    print("method stub");
    if(sesMenu != null){ DestroyMenu();}
    if(world != null){ world.Clear(); }
    world = new World();
    world.LoadGame(fileName);
  }
  
  
  /* Sets up each player's Menu */
  void UpdateCameras(){
    bool split = playerCount == 2;
    //print("PlayerCount:" + playerCount);
    if(split){
      if(cam1 != null){ cam1.rect = new Rect(0f, 0f, 0.5f, 1f); }
      if(cam2 != null){ cam2.rect = new Rect(0.5f, 0, 0.5f, 1f); }
      MenuManager menu = null;
      if(cam1 != null){ 
        menu = cam1.gameObject.GetComponent<MenuManager>();
        if(menu){ menu.split = true; menu.right = false; }
      }
      if(cam2 != null){ 
        menu = cam2.gameObject.GetComponent<MenuManager>(); 
        if(menu){ menu.split = true; menu.right = true; }
      }
    }
    else if(cam1){
      cam1.rect = new Rect(0f, 0f, 1f, 1f);
      MenuManager menu = cam1.gameObject.GetComponent<MenuManager>();
      if(menu){ menu.split = false; menu.right = false; }
    }
    else if(cam2){
      cam2.rect = new Rect(0f, 0f, 1f, 1f);
      MenuManager menu = cam2.gameObject.GetComponent<MenuManager>();
      if(menu){ menu.split = false; menu.right = false; }
    }   
  }
  
  /* Creates UI for main menu and renders menu cell in the background. 
     Warning: Is hardcoded with project-specific variables.
  */
  public void CreateMenu(){
    Cursor.visible = false;
    string MENU_BUILDING = "House";
    string MENU_INTERIOR = "Entrance";
    GameObject go = new GameObject();
    sesCam = go.AddComponent(typeof(Camera)) as Camera;
    sesMenu = go.AddComponent(typeof(MenuManager)) as MenuManager;
    go.AddComponent<AudioListener>();
    sesMenu.Change("MAIN");
    jukeBox.Play("Menu");
  }
  
  /* Destroys Camera and Menu attached to gameObject */
  public void DestroyMenu(){
    if(jukeBox != null){ jukeBox.Stop(); }
    Camera cam = sesCam;
    sesCam = null;
    if(cam != null){ Destroy(cam.gameObject); }
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
    ret.id = decks.IndexOf(ret);
    return ret;
  }
  
  /* Returns the deck this position is in, or null */
  public HoloCell GetCell(Vector3 pos){
    for(int i = 0; i < decks.Count; i++){
      HoloCell hc = decks[i].ContainingCell(pos);
      if(hc != null){ return hc; }
    }
    return null;
  } 
  
  /* Gathers player data from all decks. */
  public List<Data> GetPlayerData(){
    List<Data> ret = new List<Data>();
    for(int i = 0; i < decks.Count; i++){ ret.AddRange(decks[i].GetPlayers()); }
    return ret;
  }
  
  /* Returns a GameRecord containing this Session's data. */
  GameRecord GetData(){
    if(world != null){ return world.GetData(); }
    return null;
  }
  
  /* Loads the contents of a GameRecord */
  public void LoadData(GameRecord dat){
    sessionName = dat.sessionName;
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
  
  /* Deletes a specified save. */
  public void DeleteFile(string fileName){
    if(fileAccess){ return; }
    fileAccess = true;
    BinaryFormatter bf = new BinaryFormatter();
    string path = Application.persistentDataPath + "/" + fileName + ".save";
    if(!File.Exists(path)){ fileAccess = false; return; }
    File.Delete(path);
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
  
  /* Returns all active actors in this session. */
  public List<Actor> GetActors(){
    List<Actor> ret = new List<Actor>();
    for(int i = 0; i < decks.Count; i++){
      ret.AddRange(decks[i].GetActors());
    }
    return ret;
  }
  
  /* Returns all active players in this session. */
  public List<Actor> GetPlayers(){
    List<Actor> ret = new List<Actor>();
    for(int i = 0; i < decks.Count; i++){
      ret.AddRange(decks[i].players);
    }
    return ret;
  }
  
  /* Sends a notification to every player's HUD, 
     or a specific player's HUD */
  public void Notify(string message, Actor player = null){
    List<Actor> players = GetPlayers();
    for(int i = 0; i < players.Count; i++){ 
      if(player == null || player == players[i]){ players[i].Notify(message); }
    }
  }
  
  /* Route a SessionEvent to its appropriate destination. */
  public void ReceiveEvent(SessionEvent evt){
    if(evt.destination == SessionEvent.Destinations.Session){ 
      HandleEvent(evt); 
    }
    else if(evt.destination == SessionEvent.Destinations.Arena && arena != null){
      arena.HandleEvent(evt);
    }
    else if(evt.destination == SessionEvent.Destinations.World && world != null){
      // STUB awaiting World.cs implementation.
    }
    else{
      if(mode == Modes.Arena && arena != null){ arena.HandleEvent(evt); }
      if(mode == Modes.Adventure && world != null){
        // STUB awaiting World.cs implementation
      }
    }
  }
  
  /* Handle a SessionEvent directed toward the session. */
  public void HandleEvent(SessionEvent evt){
    print(evt.message);
  }
  
}
