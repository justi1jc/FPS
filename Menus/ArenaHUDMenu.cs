/*
    ArenaHUDMenu displays notifications related to the Arena gamemode at hand,
    and will display the scores of players at the end of a match.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class ArenaHUDMenu : Menu{
  public int subMenu = 0;
  public string message = "Arena";
  
  public ArenaHUDMenu(MenuManager manager) : base(manager){}
  
  public override void Render(){
    if(subMenu == 0){ RenderHUD(); }
    else{ RenderEndGame(); }
  }
  
  public void RenderHUD(){
    int ih = Height()/10;
    int iw = Width()/5;
    Box(message, (Width()-iw)/2, 0, iw, 2*ih);
  }
  
  public void RenderEndGame(){
    int ih = Height()/20;
    int iw = Width()/5;
    string str = ""; 
    Box("End game report.", 2*iw, ih, iw, 2*ih);
    if(Button("Replay", Width()-iw, Height()-ih, iw, ih)){ 
      SceneManager.LoadScene("Arena_Empty");
    }
    if(Button("Main Menu", 0, Height()-ih, iw, 2*ih)){ 
      Session.session.sesMenu.Change("MAIN");
      SceneManager.LoadScene("Main");
    }
    for(int i = 0; i < manager.arena.scores.Count; i++){
      str = manager.arena.names[i] + " " + manager.arena.scores[i];
      Box(str, iw, ih + (ih * i), iw, ih);
    }
  }
  
  public override void UpdateFocus(){}
  
  public override void Input(int button){}
}
