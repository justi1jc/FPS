/*
    LootMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LootMenu : Menu{
  Inventory inv, invB;
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
    if(actor == null){ return; }
    
    Box(str, XOffset(), 0, iw, 2*ih);
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() +iw, Height()/2, iw, Height()),
      scrollPosition,
      new Rect(0, 0, iw, 200)
    );
    for(int i = 0; i < inv.slots; i++){
      Data item = inv.Peek(i);
      str = item != null ? item.displayName + item.stack + "/" + item.stackSize : "EMPTY";
      y = ih * i;
      if(Button(str, 0, y, iw, ih, 0, i)){
        Store(invB, inv.Retrieve(i));
        Sound(0);
      }
    }
    GUI.EndScrollView();
    
    // Render other inventory
    scrollPositionB = GUI.BeginScrollView(
      new Rect(XOffset() + 2*iw, Height()/2, iw, Height()),
      scrollPositionB,
      new Rect(0, 0, iw, 200)
    );    
    for(int i = 0; i < invB.slots; i++){ 
      Data item = invB.Peek(i);
      str = item != null ? item.displayName + item.stack + "/" + item.stackSize : "EMPTY";
      y = ih * i;
      if(Button(str, 0, y, iw, ih, 1, i)){ Store(inv, invB.Retrieve(i)); Sound(0);}
    }
    GUI.EndScrollView();
  }
  
  public override void UpdateFocus(){
    if(sx == 0){ syMax = inv.slots -1; }
    else if(sx == 1){ syMax = invB.slots -1; }
    SecondaryBounds();
  }

  public override void Input(int button){
    DefaultExit(button);
    if(button == A){ Sound(0); }
    if(button == X){ StoreAll(invB, inv); }
    if(sy < 0){ return; }
    if(manager.actor == null){ return; }
    if(inv == null || invB == null){ return; }
    if(sx == 0 && sy > inv.slots){ return; }
    if(sx == 1 && sy > invB.slots){ return; }
    Data item;
    if(sx == 0 && button == A){
      item = inv.Retrieve(sy);
      Store(invB, item);
    }
    if(sx == 1 && button == A){
      item = invB.Retrieve(sy);
      Store(inv, item);
    }
  }

  public void StoreAll(Inventory giver, Inventory receiver){
    for(int i = 0; i < giver.slots; i++){
      Store(receiver, giver.Retrieve(i));
    }
  }

  /* Stores item in targeted inventory and drops overflow. */
  public void Store(Inventory target, Data dat){
    if(dat == null){ return; }
    int remainder = target.Store(new Data(dat));
    dat.stack = remainder;
    if(remainder > 0){ target.DiscardItem(dat, manager.transform.position); }
  }
}
