/*
    The MenuHandler acts as an interface that manages the active menu and
    changes it according to the need. This class is cognizent of all menu
    subclasses, and must be updated when adding new menus. It is responsible
    for storing all variables such as actor references or contents that are
    needed in order to display different menus properly.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MenuManager: MonoBehaviour{
  public Actor actor; // Actor connected to this manager.
  public Menu active; // Active menu
  public bool split; // True if screen is split.
  public bool right; // True if this screen is on the right side of the sceen.
  public AudioClip[] sounds;
  public Arena arena;
  
  /* Subclass variables */
  public Inventory contents;
  
  
  /* Send input to menu.. */
  public void Press(int button){ active.Press(button); }
  
  public void Awake(){
    InitSounds();
    
  }
  
  public void InitSounds(){
    sounds = new AudioClip[1];
    sounds[0] = (AudioClip)Resources.Load("Sounds/SFX/menubing1") as AudioClip;
  }
  
  /* Called once per frame. */
  void OnGUI(){
    if(active != null){
      active.RenderNotifications();
      active.Render();
      active.RenderCursor();
    }
  }
  
  /* Send notification to menu. */
  public void Notify(string message){ active.Notify(message); }
  
  /* Change the active menu */
  public void Change(string selection){
    switch(selection.ToUpper()){
      case "NONE":
        active = new Menu(this);
        break;
      case "HUD":
        active = (Menu)(new HUDMenu(this));
        break;
      case "MAIN":
        active = (Menu)(new MainMenu(this));
        break;
      case "INVENTORY":
        active = (Menu)(new InventoryMenu(this));
        break;
      case "OPTIONS":
        active = (Menu)(new OptionsMenu(this));
        break;
      case "SETTINGS":
        active = (Menu)(new SettingsMenu(this));
        break;
      case "SPEECH":
        active = (Menu)(new SpeechMenu(this));
        break;
      case "TRADE":
        active = (Menu)(new TradeMenu(this));
        break;
      case "QUEST":
        active = (Menu)(new QuestMenu(this));
        break;
      case "ABILITY":
        active = (Menu)(new AbilityMenu(this));
        break;
      case "STATS":
        active = (Menu)(new StatsMenu(this));
        break;
      case "LOOT":
        active = (Menu)(new LootMenu(this));
        break;
      case "LOAD":
        active = (Menu)(new LoadMenu(this));
        break;
      case "ARENALOBBY":
        active = (Menu)(new ArenaLobbyMenu(this));
        break;
      case "ARENAHUD":
        active = (Menu)(new ArenaHUDMenu(this));
        break;
      case "EDITKITMENU":
        active = (Menu)(new EditKitMenu(this));
        break;
    }
  }
  
  public void Sound(int i){
    if(i < 0 || sounds == null || i >= sounds.Length || sounds[i] == null){ 
      MonoBehaviour.print("Invalid sound:" + i); 
      return; 
    }
    float vol = PlayerPrefs.HasKey("masterVolume") ? PlayerPrefs.GetFloat("masterVolume") : 1f; 
    AudioSource.PlayClipAtPoint(sounds[i], transform.position, vol);
  }
}
