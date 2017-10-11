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
    
    // Display Inventory
    Actor actor = manager.actor;
    if(actor == null || inv == null || arms == null){ return; }
    
    RenderEquipment(XOffset() + iw, (Height()/2)-(5*ih));
    RenderArms(iw, ih);
    
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() +iw, Height()/2, Width()-iw, ih * inv.slots),
      new Vector2(0, ih * sy),
      new Rect(0, 0, 200, 200)
    );
    for(int i = 0; i < inv.slots; i++){
      Data item = inv.Peek(i);
      string status = "";
      switch(inv.GetStatus(i)){
        case Inventory.PRIMARY: status = "[Left]"; break;
        case Inventory.SECONDARY: status = "[Right]"; break;
        case Inventory.HEAD: status = "[Head]"; break;
        case Inventory.TORSO: status = "[Torso]"; break;
        case Inventory.LEGS: status = "[Legs]"; break;
      }
      string name = item == null ? "EMPTY" : item.displayName;
      string info = item == null ? "" : " " + item.stack + "/" + item.stackSize;
      str = status + name + info;
      if(Button(str, 0, ih * i, iw, ih, 0, i)){ 
        Equip(i, true);
        Sound(0); 
      }
      if(Button("OffHand", iw, ih * i, iw/2, ih, 1, i)){ 
        Equip(i, false);
        Sound(0);
      }
      if(Button("Drop", iw + iw/2, ih * i, iw/2, ih, 2, i)){ 
        actor.DiscardItem(i);
        Sound(0);
      }
    }
    GUI.EndScrollView();
  }
  
  void RenderArms(int iw, int ih){
    int x, y;
    string str;
    if(arms.Peek(EquipSlot.LEFT) != null){
      str = "Left" + arms.Peek(EquipSlot.LEFT).GetInfo();
      y = (Height()/2) - (2*ih);
      x = XOffset() + iw;
      if(Button(str, x, y, iw + iw/2, ih, 0, -2)){ 
        UnEquip(EquipSlot.LEFT);
        Sound(0);
      }
    }
    if(arms.Peek(EquipSlot.RIGHT) != null){
      str = "Right" + arms.Peek(EquipSlot.RIGHT).GetInfo();
      y = (Height()/2) -ih;
      x = XOffset() + iw;
      if(Button(str, x, y, iw + iw/2, ih, 0, -1)){
        UnEquip(EquipSlot.RIGHT);
        Sound(0);
      }
    }
  }
  
  void RenderEquipment(int x, int y){
    if(manager.actor.doll == null){ return; }
    int iw = Width()/4;
    int ih = Height()/20;
    string str;
    Data eq = manager.actor.doll.Peek("HEAD");
    str = "Head : " + (eq != null ? eq.displayName : "" );
    if(Button(str, x, y, iw, ih, 0, -5 )){ StoreEquipment("HEAD"); }
    eq = manager.actor.doll.Peek("TORSO");
    str = "Torso : " + (eq != null ? eq.displayName : "" );
    if(Button(str, x, y + ih, iw, ih, 0, -4)){ StoreEquipment("TORSO"); }
    eq = manager.actor.doll.Peek("LEGS");
    str = "Legs : " + (eq != null ? eq.displayName : "" );
    if(Button(str, x, y + 2*ih, iw, ih, 0, -3)){ StoreEquipment("LEGS"); }
  }

  /* Stores a piece of equipment back in its slot in the inventory. */
  void StoreEquipment(string slot){
    Data dat = manager.actor.doll.Retrieve(slot);
    if(dat == null){ return; }
    int status = -1;
    switch(slot){
      case "HEAD": status = Inventory.HEAD; break;
      case "TORSO": status = Inventory.TORSO; break;
      case "LEGS": status = Inventory.LEGS; break;
    }
    inv.StoreEquipped(dat, status);
  }

  /* Deligates to actor */
  void Equip(int slot, bool primary){
    int status = inv.GetStatus(slot);
    if(status == Inventory.EMPTY){ return; }
    else if(status == Inventory.STORED){ manager.actor.Equip(slot); }
    else{
      switch(status){
        case Inventory.PRIMARY: UnEquip(EquipSlot.RIGHT); break;
        case Inventory.SECONDARY: UnEquip(EquipSlot.LEFT); break;
        case Inventory.HEAD: StoreEquipment("HEAD"); break;
        case Inventory.TORSO: StoreEquipment("TORSO"); break;
        case Inventory.LEGS: StoreEquipment("LEGS"); break;
      }
    }
  }
  
  public override void UpdateFocus(){
    if(inv != null){ syMax = inv.slots-1; }
    if(sx < 0){ sx = 0; }
    else if(sx > 2){ sx = 2; }
    SecondaryBounds();
  }
  
  public override void Input(int button){
    DefaultExit(button);
    if(manager.actor == null || inv == null || arms == null){ return; }
    Actor actor = manager.actor;
    if(button == A){ Sound(0); }
    switch(sx){
      case 0:
        if(sy == -5){ StoreEquipment("HEAD"); }
        if(sy == -4){ StoreEquipment("TORSO"); }
        if(sy == -3){ StoreEquipment("LEGS"); }
        if(sy == -2){ UnEquip(EquipSlot.RIGHT); }
        else if(sy == -1){ UnEquip(EquipSlot.LEFT); }
        else if(sy < inv.slots){
          if(button == A){ Equip(sy, false); }
          DefaultItemInput(button);
        }
        break;
      case 1:
        if(sy < inv.slots){
          if(button == A){ Equip(sy, true); }
          DefaultItemInput(button);
        }
        break;
      case 2:
        if(sy < inv.slots){
          if(button == A){ actor.DiscardItem(sy); }
          DefaultItemInput(button);
        }
        break;
    }
  }
  
  /*  Controller actions for an item. */
  public void DefaultItemInput(int button){
    if(button == X){ manager.actor.DiscardItem(sy); }
    else if(button == RT){ Equip(sy, false); }
    else if(button == LT){ Equip(sy, true); }
  }
  
  /* Return to inventory or drop. */
  public void UnEquip(int hand){
    Data displaced = arms.Remove(hand);
    if(displaced == null){ return; }
    int remainder = inv.Store(new Data(displaced));
    if(remainder > 0){
      displaced.stack = remainder;
      manager.actor.DiscardItem(displaced); 
    }
  }
}
