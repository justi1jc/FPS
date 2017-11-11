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
      RenderItemRow(i, iw, ih);
    }
    GUI.EndScrollView();
  }
  
  public void RenderItemRow(int i, int iw, int ih){
    Inventory inv = manager.actor.inventory;
    Data item = inv.Peek(i);
    EquipSlot arms = manager.actor.arms;
    string statusText = "";
    Inventory.Statuses status = inv.GetStatus(i);
    switch(status){
      case Inventory.Statuses.Primary: statusText = "[Right]"; break;
      case Inventory.Statuses.Secondary: statusText = "[Left]"; break;
      case Inventory.Statuses.Head: statusText = "[Head]"; break;
      case Inventory.Statuses.Torso: statusText = "[Torso]"; break;
      case Inventory.Statuses.Legs: statusText = "[Legs]"; break;
    }
    string name = item == null ? "EMPTY" : item.displayName;
    string info = item == null ? "" : " " + item.stack + "/" + item.stackSize;
    string str = statusText + name + info;
    if(Button(str, 0, ih * i, iw, ih, 0, i)){ 
      if(status == Inventory.Statuses.Stored){ manager.actor.Equip(i); }
      else if(status != Inventory.Statuses.Empty){ UnequipByIndex(i); }
      Sound(0); 
    }
    if(status == Inventory.Statuses.Stored && arms.DualEquipAvailable(item)){
      if(Button("Dual Wield", iw, ih * i, iw/2, ih, 1, i)){ 
        manager.actor.DualEquip(i);
        Sound(0);
      }
    }
    if(Button("Drop", iw + iw/2, ih * i, iw/2, ih, 2, i)){ 
      manager.actor.DiscardItem(i);
      Sound(0);
    }
  }
  
  void RenderArms(int iw, int ih){
    int x, y;
    string str;
    if(arms.Peek(EquipSlot.LEFT) != null){
      str = "Left" + arms.Peek(EquipSlot.LEFT).GetInfo();
      y = (Height()/2) - (2*ih);
      x = XOffset() + iw;
      if(Button(str, x, y, iw + iw/2, ih, 0, -2)){ 
        arms.Store(EquipSlot.LEFT);
        Sound(0);
      }
    }
    if(arms.Peek(EquipSlot.RIGHT) != null){
      str = "Right" + arms.Peek(EquipSlot.RIGHT).GetInfo();
      y = (Height()/2) -ih;
      x = XOffset() + iw;
      if(Button(str, x, y, iw + iw/2, ih, 0, -1)){
        arms.Store(EquipSlot.RIGHT);
        Sound(0);
      }
    }
  }
  
  /* Unequips item found at this index. */
  void UnequipByIndex(int index){
    Inventory inv = manager.actor.inventory;
    Inventory.Statuses status = inv.GetStatus(index);
    PaperDoll doll = manager.actor.doll;
    switch(status){
      case Inventory.Statuses.Primary: 
        manager.actor.arms.Store(EquipSlot.RIGHT);
        break;
      case Inventory.Statuses.Secondary:
        manager.actor.arms.Store(EquipSlot.LEFT);
        break;
      case Inventory.Statuses.Head: doll.Store(PaperDoll.Slots.Head); break;
      case Inventory.Statuses.Torso: doll.Store(PaperDoll.Slots.Torso); break;
      case Inventory.Statuses.Legs: doll.Store(PaperDoll.Slots.Legs); break;
    }
  }
  
  /* Renders the actor's PaperDoll slots. */
  void RenderEquipment(int x, int y){
    if(manager.actor.doll == null){ return; }
    int iw = Width()/4;
    int ih = Height()/20;
    string str;
    PaperDoll doll = manager.actor.doll;
    Data eq = doll.Peek(PaperDoll.Slots.Head);
    str = "Head : " + (eq != null ? eq.displayName : "" );
    if(Button(str, x, y, iw, ih, 0, -5 )){ doll.Store(PaperDoll.Slots.Head); }
    eq = manager.actor.doll.Peek(PaperDoll.Slots.Torso);
    str = "Torso : " + (eq != null ? eq.displayName : "" );
    if(Button(str, x, y + ih, iw, ih, 0, -4)){ doll.Store(PaperDoll.Slots.Torso); }
    eq = manager.actor.doll.Peek(PaperDoll.Slots.Legs);
    str = "Legs : " + (eq != null ? eq.displayName : "" );
    if(Button(str, x, y + 2*ih, iw, ih, 0, -3)){ doll.Store(PaperDoll.Slots.Legs); }
  }

  /* Deligates to actor */
  void Equip(int slot, bool primary){
    Inventory.Statuses status = inv.GetStatus(slot);
    PaperDoll doll = manager.actor.doll;
    Data dat = null;
    if(status == Inventory.Statuses.Empty){ return; }
    else if(status == Inventory.Statuses.Stored){ manager.actor.Equip(slot); }
    else{
      switch(status){
        case Inventory.Statuses.Primary: arms.Store(EquipSlot.RIGHT); break;
        case Inventory.Statuses.Secondary: arms.Store(EquipSlot.LEFT); break;
        case Inventory.Statuses.Head:
          dat = doll.Peek(PaperDoll.Slots.Head);
          if(dat != null){ doll.Store(PaperDoll.Slots.Head);}
          break;
        case Inventory.Statuses.Torso:
          dat = doll.Peek(PaperDoll.Slots.Head);
          if(dat != null){doll.Store(PaperDoll.Slots.Torso); }
          break;
        case Inventory.Statuses.Legs:
          dat = doll.Peek(PaperDoll.Slots.Head);
          if(dat != null){ doll.Store(PaperDoll.Slots.Legs); }
          break;
      }
    }
  }
  
  public override void UpdateFocus(){
    if(inv != null){ syMax = inv.slots-1; }
    if(sx < 0){ sx = 0; }
    else if(sx > 2){ sx = 2; }
    SecondaryBounds();
  }
  
  public override void Input(Buttons button){
    DefaultExit(button);
    
  }
  
  /*  Controller actions for an item. */
  public void DefaultItemInput(Buttons button){
    if(button == Buttons.X){ manager.actor.DiscardItem(sy); }
  }

}
