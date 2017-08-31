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
    switch(subMenu){
      case 0: RenderMain(); break;
      case 1: RenderCredits(); break;
    }
  }
  
  public void RenderMain(){
    int iw = Width()/6;
    int ih = Height()/8;
    int x = XOffset() + iw;
    string str;
    
    if(Button("Arena", x, 0, iw, ih)){
      manager.Change("ARENALOBBY");
      Sound(0);
    }
    
    str = "Credits";
    if(Button(str, x, 6*ih, iw, ih)){
      subMenu = 1;
      Sound(0);
    }
    
    str = "Quit";
    if(Button(str, x, 7*ih, iw, ih)){ 
      Application.Quit();
      Sound(0);
    }
  }
  
  public void RenderCredits(){
    int iw = Width();
    int ih = Height()/10;
    string str;
    string attr = " Kevin MacLeod (incompetech.com)\n";
    attr += "Licensed under Creative Commons: By Attribution 3.0 License\n";
    attr += "http://creativecommons.org/licenses/by/3.0/\n";
    
    str = "Programming, textures, modeling, animation: Blukat Studios\n\n\n\n\n"; 
    str += "Music:\n\n";
    str += "\"Burnt Spirit\"" + attr;
    str += "\"Crossing the Chasm\"" + attr; 
    str += "\"Curse of the Scarab\"" + attr;
    str += "\"Five Armies\"" + attr;
    str += "\"Heroic Age\"" + attr;
    str += "\"Power Respored\"" + attr;
    str += "\"Prelude and Action\"" + attr;
    str += "\"Unholy Knight\"" + attr;
    
    Box(str, 0, 0, iw, 9*ih, Color.black);
    
    str = "Back";
    if(Button(str, 0, 9*ih, iw, ih)){
      subMenu = 0;
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
