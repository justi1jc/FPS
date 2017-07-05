/*
    MainMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MainMenu : Menu{
  int subMenu;
  string sesName;
  public MainMenu(MenuManager manager) : base(manager){
    subMenu = 0;
    sesName = "";
    syMin = 0;
    syMax = 2;
  }
  
  public override void Render(){
    int iw = Width()/6;
    int ih = Height()/4;
    int x = XOffset() + iw;
    string str;
    
    if(Button("Arena", x, 0, 4*iw, ih)){
      manager.Change("ARENALOBBY");
    }
    
    switch(subMenu){
      case 0:
        str = "New Game";
        if(Button(str, x, ih, 4*iw, ih, 0, 1 )){ 
          subMenu = 1;
          Sound(0);
        }
        break;
      case 1:
        sesName = TextField(sesName, x, ih, 3*iw, ih);
        if(Button("Start", x + 3*iw, ih, iw, ih)){
          Session.session.CreateGame(sesName);
          Sound(0);
        }
        break;
    }
    str = "Load Game";
    if(Button(str, x, 2*ih, 4*iw, ih, 0, 2 )){ 
      manager.Change("LOAD");
      Sound(0);
    }
    
    str = "Quit";
    if(Button(str, x, 3*ih, 4*iw, ih, 0, 3 )){ 
      Application.Quit();
      Sound(0);
    }
  }
  
  public override void UpdateFocus(){
    sx = 0;
    SecondaryBounds();
  }
  
  public override void Input(int button){
    if(button == A){ Sound(0); }
    switch(sy){
      case 0:
        if(subMenu == 0){ subMenu = 1; }
        else if(subMenu == 1){ Session.session.CreateGame(sesName); }
        break;
      case 1:
        manager.Change("LOAD");
        break;
      case 2:
        Application.Quit();
        break;
    }
  }
  
}
