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
    if(Button("Start", Width()-iw, Height()-ih, iw, ih)){ 
      manager.Change("NONE");
      SceneManager.LoadScene("Arena_Empty");
    }
    if(Button("Back", 0, Height()-ih, iw, ih)){ 
      manager.Change("MAIN");
    }
  }
  
  public override void UpdateFocus(){}
  
  public override void Input(int button){}
}
