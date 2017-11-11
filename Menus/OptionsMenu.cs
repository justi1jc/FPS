/*
    OptionsMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class OptionsMenu : Menu{
  int submenu = 0;
  public OptionsMenu(MenuManager manager) : base(manager){
    syMin = 0;
    syMax = 5;
  }
  
  public override void Render(){
    if(submenu == 0){ RenderMainOptions();}
    else if(submenu == 1){ RenderQuitOptions(); } 
  }
  
  public void RenderMainOptions(){
    string str;
    int iw = Width()/6;
    int ih = Height()/6;
    int x = XOffset() + iw;
    str = "Resume";
    if(Button(str, x, 0, 2*iw, ih, 0, 0)){
      manager.Change("HUD");
      if(manager.actor != null){ manager.actor.SetMenuOpen(false);}
    }
    
    str = "Quit.";
    if(Button(str, x, ih, 2*iw, ih, 0, 1)){
      submenu = 1;
      Sound(0);
    }
    
    str = "Settings";
    if(Button(str, x, 2*ih, 2*iw, ih, 0, 2)){
      manager.Change("SETTINGS");
      Sound(0);
    }
    
    if(Session.session != null && Session.session.mode == Session.Modes.None){
      str = "Load";
      if(Button(str, x, 3*ih, 2*iw, ih, 0, 3)){
        Session.session.LoadFiles();
        manager.Change("LOAD");
        Sound(0);
      }
      str = "Save";
      if(Button(str, x, 4*ih, 2*iw, ih, 0, 4)){
        Session.session.world.SaveGame();
        Sound(0);
      }
    }
    
  }
  
  public void RenderQuitOptions(){
    string str;
    int iw = Width()/6;
    int ih = Height()/6;
    int x = XOffset() + iw;
    str = "";
    
    str = "Back";
    if(Button(str, x, 0, 2*iw, ih, 0, 0)){
      submenu = 0;
      Sound(0);
    }
    
    str = "Main Menu";
    if(Button(str, x, ih, 2*iw, ih, 0, 1)){
      SceneManager.LoadScene("Main");
    }
    
    str = "Exit Game";
    if(Button(str, x, 2*ih, 2*iw, ih, 0, 2)){
      Application.Quit();
      Sound(0);
    }
    
    
    if(Session.session != null && Session.session.mode == Session.Modes.None){
      str = "Save and Quit.";
      if(Button(str, x, 3*ih, 2*iw, ih, 0, 3)){
        Session.session.world.SaveGame();
        Application.Quit();
        Sound(0);
      }
    }
    
  }
  
  public override void UpdateFocus(){
    sx = 0;
    SecondaryBounds();
  }
  
  public override void Input(Buttons button){
    DefaultExit(button);
    if(submenu == 0){ MainOptionsInput(button); }
    else if(submenu == 1){ QuitOptionsInput(button); }
  }
  
  public void MainOptionsInput(Buttons button){
    if(button == Buttons.A){
      Sound(0);
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
          Session.session.world.SaveGame();
          break;
        case 4:
          Session.session.world.SaveGame();
          Application.Quit();
          break;
        case 5:
          Application.Quit();
          break;
      }
    }
  }
  
  public void QuitOptionsInput(Buttons button){
    if(button == Buttons.A){
      Sound(0);
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
          Session.session.world.SaveGame();
          break;
        case 4:
          Session.session.world.SaveGame();
          Application.Quit();
          break;
        case 5:
          Application.Quit();
          break;
      }
    }
  }
  
}
