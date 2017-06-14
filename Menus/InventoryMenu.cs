/*
    InventoryMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InventoryMenu : Menu{
  List<Data> inv;
  public InventoryMenu(MenuManager manager) : base(manager){
    if(manager.actor != null){
      inv = manager.actor.inventory;
    }
    else{ inv = null; }
    syMin = 0;
    sxMax = 1;
    sxMin = -1;
  }
  
  public override void Render(){
    Box("", XOffset(), 0, Width(), Height()); //Draw Background
    List<Data> inv = manager.actor.inventory;
    int iw = Width()/4;
    int ih = Height()/20;
    string str = "";
    
    // Display Navigation buttons
    if(Button("Abilities", XOffset() + Width() - iw, Height()/2, iw, ih, 1, 0)){
      manager.Change("ABILITY");
    }
    if(Button("Quests", XOffset(), Height()/2, iw, ih, -1, 0)){
      manager.Change("QUEST");
    }
    
    
    // Display Inventory
    Actor actor = manager.actor;
    if(actor == null || inv == null){ return; }
    str = "Currency: " + actor.currency; 
    Box(str, XOffset(), 0, iw, 2*ih);
    
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() +iw, Height()/2, Width()-iw, ih * inv.Count),
      new Vector2(0, ih * sy),
      new Rect(0, 0, 200, 200)
    );
    
    for(int i = 0; i < inv.Count; i++){ 
      Data item = inv[i];
      string selected = "";
      if(i == actor.primaryIndex){ selected += "Right Hand "; }
      if(i == actor.secondaryIndex){ selected += "Left Hand "; }
      string name = item.displayName;
      string info = " " + item.stack + "/" + item.stackSize;
      if(i == sy && sx == 0){ GUI.color = Color.yellow; }
      str = selected + name + info;
      if(Button(str, XOffset(), ih * i, iw + iw/2, ih, 0, i)){ 
        actor.Equip(i); 
      }
      if(Button("DROP", XOffset() + iw + iw/2, ih * i, iw/2, ih)){ 
        actor.DiscardItem(i); 
      }
    }
    GUI.EndScrollView();
  }
  
  public override void UpdateFocus(){
    if(inv != null){ syMax = inv.Count-1; }
    if(sx != 0){ sy = 0; }
    SecondaryBounds();
  }
  
  public override void Input(int button){
    DefaultExit(button);
    switch(sx){
      case -1:
        if(button == A){ manager.Change("QUEST"); }
        break;
      case 0:
        if(inv == null){ return; }
        if(sy < inv.Count){
          if(manager.actor == null){ return; }
          Actor actor = manager.actor;
          if(button == A){ actor.Equip(sy); }
          else if(button == X){ actor.DiscardItem(sy); }
          else if(button == RT){ actor.Equip(sy); }
          else if(button == LT){ actor.EquipSecondary(sy); }
        }
        break;
      case 1:
        if(button == A){ manager.Change("ABILITY"); }
        break;
    }
  }
  
}
