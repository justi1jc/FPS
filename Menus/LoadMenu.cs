/*
    LoadMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LoadMenu : Menu{
  
  List<GameRecord> files;
  public LoadMenu(MenuManager manager) : base(manager){
    py = -1;
    files = Session.session.LoadFiles();
  }
  
  public override void Render(){
    string str;
    int iw = Width()/6;
    int ih = Height()/5;
    int x, y;
    x = XOffset() + iw;
    
    Box("Load Menu", x, 0, iw, ih);
    string dest = manager.actor != null ? "OPTIONS" : "MAIN";
    int back = files != null ? files.Count : 0;
    if(Button("Back", x, 3*ih, iw, ih, 0, back)){ manager.Change(dest); Sound(0);}
    
    // Render file buttons
    if(files == null){ return; }
    scrollPositionB = GUI.BeginScrollView(
      new Rect(XOffset() + iw, ih, iw, 2*ih),
      scrollPositionB,
      new Rect(0, 0, iw, ih * files.Count)
    );
    for(int i = 0; i < files.Count; i++){
      y = i * ih / 2;
      str = files[i].sessionName;
      if(Button(str, 0, y, iw, ih/2, 0, i)){ py = i; Sound(0); }
    }
    GUI.EndScrollView();
    
    if(py > -1 && py < files.Count){
      str = files[py].sessionName;
      x = XOffset() + 3 * iw; 
      Box(str,x, 0, iw, ih/2);
      
      str = "Load";
      y = ih;
      if(Button(str, x, y, iw, ih/2, 1, 0)){ 
        Session.session.LoadGame(files[py].sessionName);
        Sound(0);
      }
      str = "Delete";
      y = ih + ih/2;
      if(Button(str, x, y, iw, ih/2, 1, 1)){ 
        Session.session.DeleteFile(files[py].sessionName);
        files.Remove(files[py]);
        Sound(0);
      }
    }
  }
  
  public override void UpdateFocus(){
    if(sx < 0 || py < 0){ sx = 0; }
    if(sx > 1){ sx = 1; }
    if(sy < 0){ sy = 0; }
    if(sx == 0){  
      syMax = files != null ? files.Count : 0;
      if(sy > syMax){ sy = syMax; }
    }
    if(sx == 1){
      if(sy > 1){ sy = 1; }
    }
  }
  
  public override void Input(Buttons button){
    string dest = manager.actor != null ? "OPTIONS" : "MAIN";
    if(button == Buttons.A){ Sound(0); }
    if(button == Buttons.B || button == Buttons.Y){ manager.Change(dest); }
    if(sx == 0 && button == Buttons.A){
      if(sy < 0 || files == null){ return; }
      if(sy < files.Count){ py = sy; }
      if(sy == files.Count){ manager.Change(dest); }
    }
    if(sx == 1 && button == Buttons.A){
      switch(sy){
        case 0:
          Session.session.LoadGame(files[py].sessionName); 
          break;
        case 1:
          Session.session.DeleteFile(files[py].sessionName);
          files.Remove(files[py]);
          break;
      }
    }
  }
  
}
