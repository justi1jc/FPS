/*
    InventoryMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InventoryMenu : Menu{
  Inventory inv;
  EquipSlot arms;
  public InventoryMenu(MenuManager manager) : base(manager){
    if(manager.actor != null && manager.actor.inventory != null){
      inv = manager.actor.inventory;
      arms = manager.actor.arms;
    }
    else{ 
      inv = null; 
      arms = null;
    }
    syMin = -1;
    sxMax = 3;
    sxMin = -1;
  }
  
  public override void Render(){
    Box("", XOffset(), 0, Width(), Height()); //Draw Background
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
    if(actor == null || inv == null || arms == null){ return; }
    
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() +iw, Height()/2, Width()-iw, ih * inv.slots),
      new Vector2(0, ih * sy),
      new Rect(0, 0, 200, 200)
    );
    
    if(arms.handItem != null){
      str = "Left" + arms.handItem.GetInfo();
      if(Button(str, XOffset(), -2*ih, iw + iw/2, ih, 0, -2)){ UnEquip(true); }
    }
    if(arms.offHandItem != null){
      str = "Right" + arms.offHandItem.GetInfo();
      if(Button(str, XOffset(), -1*ih, iw + iw/2, ih, 0, -1)){ UnEquip(false); }
    }
    for(int i = 0; i < inv.inv.Count; i++){
      Data item = inv.Peek(i);
      string name = item == null ? "EMPTY" : item.displayName;
      string info = item == null ? "" : " " + item.stack + "/" + item.stackSize;
      str = name + info;
      if(Button(str, XOffset(), ih * i, iw + iw/2, ih, 0, i)){ 
        Equip(inv.Retrieve(i), true); 
      }
      if(Button("OffHand", XOffset() + iw + iw/2, ih * i, iw/2, ih, i, 1)){ 
        Equip(inv.Retrieve(i), false);
      }
      if(Button("Drop", XOffset() + (2*iw), ih * i, iw/2, ih, i, 2)){ 
        actor.DiscardItem(i); 
      }
    }
    GUI.EndScrollView();
  }
  
  void Equip(Data dat, bool right){
    List<Data> discarded;
    discarded = arms.Equip(dat);
    for(int i = 0; i < discarded.Count; i++){
      discarded[i].stack = inv.Store(discarded[i]);
      if(discarded[i].stack > 0){ manager.actor.DiscardItem(discarded[i]); }
    }
  }
  
  public override void UpdateFocus(){
    if(inv != null){ syMax = inv.slots-1; }
    SecondaryBounds();
  }
  
  public override void Input(int button){
    DefaultExit(button);
    if(manager.actor == null || inv == null || arms == null){ return; }
    Actor actor = manager.actor;
    
    switch(sx){
      case -1:
        if(button == A){ manager.Change("QUEST"); }
        break;
      case 0:
        if(sy == -2){ UnEquip(true); }
        else if(sy == -1){ UnEquip(false); }
        else if(sy < inv.slots){
          if(button == A){ Equip(inv.Retrieve(sy), false); }
          DefaultItemInput(button);
        }
        break;
      case 1:
        if(sy < inv.slots){
          if(button == A){ Equip(inv.Retrieve(sy), true); }
          DefaultItemInput(button);
        }
        break;
      case 2:
        if(sy < inv.slots){
          if(button == A){ actor.DiscardItem(sy); }
          DefaultItemInput(button);
        }
        break;
      case 3:
        if(button == A){ manager.Change("ABILITY"); }
        break;
    }
  }
  
  /*  Controller actions for an item. */
  public void DefaultItemInput(int button){
    if(button == X){ manager.actor.DiscardItem(sy); }
    else if(button == RT){ Equip(inv.Retrieve(sy), false); }
    else if(button == LT){ Equip(inv.Retrieve(sy), true); }
  }
  
  /* Return to inventory or drop. */
  public void UnEquip(bool primary){
    Data displaced = arms.Remove(primary);
    displaced.stack = inv.Store(displaced);
    if(displaced.stack > 0){ manager.actor.DiscardItem(displaced); }
  }
}
