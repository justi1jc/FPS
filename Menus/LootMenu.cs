/*
    LootMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LootMenu : Menu{
  List<Data> inv, invB;
  public LootMenu(MenuManager manager) : base(manager){
    if(manager.actor != null){ inv = manager.actor.inventory; }
    else{ inv = null; }
    invB = manager.contents;
    syMax = 0;
    syMin = 0;
    sxMax = 1;
    sxMin = 0;
  }
  
  public override void Render(){
    Box("", XOffset(), 0, Width(), Height()); // Background
    int iw = Width()/4;
    int ih = Height()/20;
    int y = 0;
    string str = "";
    
    // Render player inventory
    if(inv == null || invB == null || manager.actor == null){ return; }
    Actor actor = manager.actor;
    str = "Currency: " + actor.currency; 
    
    Box(str, XOffset(), 0, iw, 2*ih);
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() +iw, Height()/2, iw, Height()),
      scrollPosition,
      new Rect(0, 0, iw, 200)
    );
    for(int i = 0; i < inv.Count; i++){ 
      Data item = inv[i];
      string selected ="";
      if(i == manager.actor.primaryIndex){ selected += "Right Hand "; }
      else if(i == manager.actor.secondaryIndex){ selected += "Left Hand "; }
      string name = item.displayName;
      string info = " " + item.stack + "/" + item.stackSize;
      str = selected + name + info;
      y = ih * i;
      if(Button(str, 0, y, iw, ih, 0, i)){
        invB.Add(item);
        inv.Remove(item);
      }
    }
    GUI.EndScrollView();
    
    // Render other inventory
    scrollPositionB = GUI.BeginScrollView(
      new Rect(XOffset() + 2*iw, Height()/2, iw, Height()),
      scrollPositionB,
      new Rect(0, 0, iw, 200)
    );    
    for(int i = 0; i < invB.Count; i++){ 
      Data item = invB[i];
      string selected ="";
      string name = item.displayName;
      string info = " " + item.stack + "/" + item.stackSize;
      str = selected + name + info;
      y = ih * i;
      if(Button(str, 0, y, iw, ih, 1, i)){
        actor.StoreItem(item);
        invB.Remove(item);
      }
    }
    GUI.EndScrollView();
  }
  
  public override void UpdateFocus(){
    if(sx == 0){ syMax = inv.Count -1; }
    else if(sx == 1){ syMax = invB.Count -1; }
    SecondaryBounds();
  }
  
  public override void Input(int button){
    DefaultExit(button);
    if(sy < 0){ return; }
    if(manager.actor == null){ return; }
    if(inv == null || invB == null){ return; }
    if(sx == 0 && sy > inv.Count){ return; }
    if(sx == 1 && sy > invB.Count){ return; }
    Data item;
    if(sx == 0 && button == A){
      item = inv[sy];
      invB.Add(item);
      inv.Remove(item);
    }
    if(sx == 1 && button == A){
      item = invB[sy];
      manager.actor.StoreItem(item);
      invB.Remove(item);
    }
  }
  
}
