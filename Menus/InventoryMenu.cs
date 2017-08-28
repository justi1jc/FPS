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
    syMin = -5;
    sxMax = 3;
    sxMin = -1;
  }
  
  public override void Render(){
    Box("", XOffset(), 0, Width(), Height()); //Draw Background
    int iw = Width()/4;
    int ih = Height()/20;
    int x, y;
    string str = "";
    
    // Display Navigation buttons
    if(Button("Abilities", XOffset() + Width() - iw, Height()/2, iw, ih, 3, 0)){
      manager.Change("ABILITY");
      Sound(0);
    }
    if(Button("Quests", XOffset(), Height()/2, iw, ih, -1, 0)){
      manager.Change("QUEST"); 
      Sound(0);
    }
    
    
    // Display Inventory
    Actor actor = manager.actor;
    if(actor == null || inv == null || arms == null){ return; }
    
    
    if(arms.handItem != null){
      str = "Left" + arms.handItem.GetInfo();
      y = (Height()/2) - (2*ih);
      x = XOffset() + iw;
      if(Button(str, x, y, iw + iw/2, ih, 0, -2)){ UnEquip(true); Sound(0);}
    }
    if(arms.offHandItem != null){
      str = "Right" + arms.offHandItem.GetInfo();
      y = (Height()/2) -ih;
      x = XOffset() + iw;
      if(Button(str, x, y, iw + iw/2, ih, 0, -1)){ UnEquip(false); Sound(0);}
    }
    
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() +iw, Height()/2, Width()-iw, ih * inv.slots),
      new Vector2(0, ih * sy),
      new Rect(0, 0, 200, 200)
    );
    for(int i = 0; i < inv.slots; i++){
      Data item = inv.Peek(i);
      string name = item == null ? "EMPTY" : item.displayName;
      string info = item == null ? "" : " " + item.stack + "/" + item.stackSize;
      str = name + info;
      if(Button(str, 0, ih * i, iw, ih, 0, i)){ 
        Equip(inv.Retrieve(i), true);
        Sound(0); 
      }
      if(Button("OffHand", iw, ih * i, iw/2, ih, 1, i)){ 
        Equip(inv.Retrieve(i), false);
        Sound(0);
      }
      if(Button("Drop", iw + iw/2, ih * i, iw/2, ih, 2, i)){ 
        actor.DiscardItem(i);
        Sound(0);
      }
    }
    GUI.EndScrollView();
    RenderEquipment(XOffset() + 3*iw, 0);
  }
  
  
  void RenderEquipment(int x, int y){
    if(manager.actor.doll == null){ return; }
    int iw = Width()/4;
    int ih = Height()/20;
    string str;
    Data eq = manager.actor.doll.Peek("HEAD");
    str = "Head : " + (eq != null ? eq.displayName : "" );
    if(Button(str, x, y, iw, ih )){ StoreEquip("HEAD"); }
    eq = manager.actor.doll.Peek("TORSO");
    str = "Torso : " + (eq != null ? eq.displayName : "" );
    if(Button(str, x, y + ih, iw, ih )){ StoreEquip("TORSO"); }
    eq = manager.actor.doll.Peek("LEGS");
    str = "Legs : " + (eq != null ? eq.displayName : "" );
    if(Button(str, x, y + 2*ih, iw, ih )){ StoreEquip("LEGS"); }
  }

  void StoreEquip(string slot){
    Data dat = manager.actor.doll.Retrieve(slot);
    if(dat == null){ return; }
    manager.actor.StoreItem(dat);
  }
  void Equip(Data dat, bool primary){
    manager.actor.Equip(dat, primary);
  }
  
  public override void UpdateFocus(){
    if(inv != null){ syMax = inv.slots-1; }
    if(sy > 0){
      if(sx < 0){ sx = 0; }
      else if(sx > 2){ sx = 2; }
    }
    SecondaryBounds();
  }
  
  public override void Input(int button){
    DefaultExit(button);
    if(manager.actor == null || inv == null || arms == null){ return; }
    Actor actor = manager.actor;
    if(button == A){ Sound(0); }
    switch(sx){
      case -1:
        if(button == A){ manager.Change("QUEST"); }
        break;
      case 0:
        if(sy == -5){ StoreEquipment("HEAD"); }
        if(sy == -4){ StoreEquipment("TORSO"); }
        if(sy == -3){ StoreEquipment("LEGS"); }
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
    int remainder = inv.Store(new Data(displaced));
    if(remainder > 0){
      displaced.stack = remainder;
      manager.actor.DiscardItem(displaced); 
    }
  }
  
  public void StoreEquipment(string slot = "NONE"){
    Data displaced = manager.actor.doll.Retrieve(slot);
    if(displaced == null){ return; }
    int remainder = inv.Store(new Data(displaced));
    if(remainder > 0){
      displaced.stack = remainder;
      manager.actor.DiscardItem(displaced);
    }
  }
}
