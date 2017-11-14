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
  public Modes mode = Modes.None; // Current mode.
  public enum Modes{ None, Arena, Adventure }; // Modes of play
  
  // Arena variables
  public int playerCount = 1;
  private List<Kit> kits;
  public Arena arena;
  int currentID = 0;
  
  // player variables
  List<Data> playerData;
  Camera cam1;
  Camera cam2;
  
  // Main menu UI
  Camera sesCam;
  public MenuManager sesMenu;
  public JukeBox jukeBox;

  /**
    * Destroys self if Session.session already exists, otherwise
    * Initializes main menu.
    */
  void Awake(){
    DontDestroyOnLoad(gameObject);
    if(Session.session != null){ Destroy(this); }
    else{ Session.session = this; }
    if(jukeBox == null){ jukeBox = new JukeBox(this); }
    CreateMenu();
  }
  
  
  /**
    * Updates cameras.
    * @param {Actor} actor - The actor to be registered.
    * @param {int} player - The player number.
    * @param {Camera} cam - The camera associated with the player. 
    */
  public void RegisterPlayer(Actor actor, int player, Camera cam){
    if(player == 1){ cam1 = cam; }
    else if(player == 2){ cam2 = cam; }
    UpdateCameras();
  }
  
  /** 
    * Updates the cameras according to remaining players.
    * @param {int} player - The player number.
    */
  public void UnregisterPlayer(int player){
    if(player == 1){ cam1 = null; }
    else if(player == 2){ cam2 = null; }
    UpdateCameras();
  }
  
  /**
    * Returns true if the session instance exists.
    * @return {bool} true if session instance exists.
    */
  public static bool Active(){
    return Session.session != null;
  }

  /** 
    * Cached access to Kit.GetKits() to reduce file parsing.
    * Reload clears the cache when set to true.
    * @param {bool} reload - Whether to reload kits from their files.
    * @return {List<Kit>} the list of available kits.
    */
  public List<Kit> GetKits(bool reload = false){
    if(kits == null || reload){ 
      kits = Kit.GetKits();
      for(int i = 0; i < 6; i++){ kits.Add(Kit.LoadKit(i)); }
    }
    return new List<Kit>(kits);
  }
  
  /**
    * Returns a specific kit by name, or null.
    * @param {string} kitName - the desired kit's name.
    * @return {Kit} - the desired kit.
    */
  public Kit GetKit(string kitName){
    if(kits == null){ kits = Kit.GetKits(); }
    foreach(Kit kit in kits){
      if(kit.name == kitName){ return kit; }
    }
    print(kitName + " not found.");
    return null;
  }
  
  
  /** 
    * Sets up each player's Menu 
    */
  void UpdateCameras(){
    bool split = playerCount == 2;
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
  
  /** 
    * Creates UI for main menu and renders menu cell in the background. 
    * Warning: Is hardcoded with project-specific variables.
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
  
  /**
    * Destroys Camera and Menu attached to gameObject 
    */
  public void DestroyMenu(){
    if(jukeBox != null){ jukeBox.Stop(); }
    Camera cam = sesCam;
    sesCam = null;
    if(cam != null){ Destroy(cam.gameObject); }
  }
  
  /** 
    * Route a SessionEvent to its appropriate destination.
    * @param {SessionEvent} evt - the event to route. 
    */
  public void ReceiveEvent(SessionEvent evt){
    if(evt.destination == SessionEvent.Destinations.Session){ 
      HandleEvent(evt); 
    }
    else if(evt.destination == SessionEvent.Destinations.Arena && arena != null){
      arena.HandleEvent(evt);
    }
    else{
      if(mode == Modes.Arena && arena != null){ arena.HandleEvent(evt); }
    }
  }
  
  /**
    * Handle a SessionEvent directed toward the session.
    * @param {SesssionEvent} evt - 
    */
  public void HandleEvent(SessionEvent evt){
    print(evt.message);
  }
  
}
