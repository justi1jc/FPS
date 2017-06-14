/*
    OptionsMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class OptionsMenu : Menu{
  
  public OptionsMenu(MenuManager manager) : base(manager){
    syMin = 0;
    syMax = 5;
  }
  
  public override void Render(){
    string str;
    int iw = Width()/6;
    int ih = Height()/6;
    int x = XOffset() + iw;
    str = "Resume";
    if(Button(str, x, 0, 2*iw, ih, 0, 0)){
      manager.Change("HUD");
    }
    
    str = "Load";
    if(Button(str, x, ih, 2*iw, ih, 0, 1)){
      Session.session.LoadFiles();
      manager.Change("LOAD");
    }
    
    str = "Settings";
    if(Button(str, x, 2*ih, 2*iw, ih, 0, 2)){
      MonoBehaviour.print("Settings");
    }
    
    str = "Save";
    if(Button(str, x, 3*ih, 2*iw, ih, 0, 3)){
      Session.session.SaveGame(Session.session.sessionName);
    }
    
    str = "Save and Quit.";
    if(Button(str, x, 4*ih, 2*iw, ih, 0, 4)){
      Session.session.SaveGame(Session.session.sessionName);
      Application.Quit();
    }
    
    str = "Quit.";
    if(Button(str, x, 5*ih, 2*iw, ih, 0, 5)){
      Application.Quit();
    }
  }
  
  public override void UpdateFocus(){
    sx = 0;
    SecondaryBounds();
  }
  
  public override void Input(int button){
    DefaultExit(button);
    if(button == A){
      switch(sy){
        case 0:
          manager.Change("HUD");
          break;
        case 1:
          Session.session.LoadFiles();
          manager.Change("LOAD");
          break;
        case 2:
          MonoBehaviour.print("Settings");
          break;
        case 3:
          Session.session.SaveGame(Session.session.sessionName);
          break;
        case 4:
          Session.session.SaveGame(Session.session.sessionName);
          Application.Quit();
          break;
        case 5:
          Application.Quit();
          break;
      }
    }
  }
  
}
