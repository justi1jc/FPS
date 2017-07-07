/*
    QuestMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class QuestMenu : Menu{
  
  public QuestMenu(MenuManager manager) : base(manager){
    sxMin = 0;
    sxMax = 1;
    syMin = 0;
  }
  
  public override void Render(){
    Box("", XOffset(), 0, Width(), Height()); // Background
    int iw = Width()/4;
    int ih = Height()/20;
    int y = 0;
    string str = "";
    
    str = "Inventory";
    if(Button(str, Width() - iw, Height()/2, iw, ih, 1, 0)){ manager.Change("INVENTORY"); Sound(0);}
    
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() +iw, Height()/2, iw, Height()),
      scrollPosition,
      new Rect(0, 0, iw, 200)
    );
    
    List<Quest> quests = Session.session.quests;
    for(int i = 0; i < quests.Count; i++){ 
      y = ih * i;
      if(Button(quests[i].Name(), 0, y, iw, ih, 0, i)){
        MonoBehaviour.print(quests[i].Name());
      }
      
    }
    GUI.EndScrollView();
    if(sy < 0 || sy >= quests.Count){ return; }
    str = "";
    string[] obj = quests[sy].Objectives();
    for(int i = 0; i < obj.Length; i++){
      str += obj[i] + "\n";
      Box(str, XOffset() + 2*iw, Height()/2 + i*y, iw, ih);
    }
  }
  
  public override void UpdateFocus(){
    if(sx == 0){
      syMax = Session.session.quests.Count -1;
    }
    else{ sy = 0;}
    SecondaryBounds();
  }
  
  public override void Input(int button){
    DefaultExit(button);
    if(button == A){ Sound(0); }
    switch(sx){
      case 0:
        if(sy < 0 || sy > Session.session.quests.Count -1){ return; }
        MonoBehaviour.print(Session.session.quests[sy].Name());
        break;
      case 1:
        manager.Change("INVENTORY");
        break;
    }
  }
  
}
