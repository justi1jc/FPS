/*
    ArenaLobbyMenu allows the player to configure the arena match before
    playing it.
    
    NOTE: This menu references scenes in a unity project. You will have to
    change some of the arguments if you use a different environment. 
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ArenaLobbyMenu : Menu{
  public ArenaLobbyMenu(MenuManager manager) : base(manager){}
  
  public override void Render(){
    int ih = Height()/10;
    int iw = Width()/5; 
    Box("Arena Lobby", 2*iw, ih, iw, ih);
    string str = "Players: " + Session.session.playerCount;
    if(Button(str, 2*iw, 2*ih, iw, ih)){ TogglePlayers(); }
    if(Button("Start", Width()-iw, Height()-ih, iw, ih)){ 
      manager.Change("NONE");
      Sound(0);
      SceneManager.LoadScene("Arena_Empty");
    }
    if(Button("Back", 0, Height()-ih, iw, ih)){ 
      manager.Change("MAIN"); 
      Sound(0);
    }
  }
  
  void TogglePlayers(){
    int players = Session.session.playerCount;
    if(players == 1){ players = 2; }
    else{ players = 1; }
    Session.session.playerCount = players;
  }
  public override void UpdateFocus(){}
  
  public override void Input(int button){}
}
