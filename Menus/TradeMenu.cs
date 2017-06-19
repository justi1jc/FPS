/*
    TradeMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TradeMenu : Menu{
  int balance;
  List<Data> buying, selling, bought, sold;
   
  public TradeMenu(MenuManager manager) : base(manager){
    syMin = 0;
    sxMin = 0;
    sxMax = 2;
    if(manager.actor == null || manager.actor.interlocutor == null){
      selling = buying = sold = bought = null;
      balance = 0;
      return;
    }
    
    selling = new List<Data>(manager.actor.inventory.inv);
    buying = new List<Data>(manager.actor.interlocutor.inventory.inv);
    ClearNulls(selling);
    ClearNulls(buying);
    sold = new List<Data>();
    bought = new List<Data>();
    balance = 0;
  }
  
  public void ClearNulls(List<Data> dats){
    for(int i = 0; i < dats.Count; i++){
      if(dats[i] == null){
        dats.Remove(dats[i]);
        i--;
      }
    }
  }
  
  public override void Render(){
    int x, y; // To set up positioning before Box/Button call.
    string str; // To set up Box/Button call
    Box("", XOffset(), 0, Width(), Height());
    int iw = Width()/4;
    int ih = Height()/20;
    if(manager.actor == null || manager.actor.interlocutor == null){ return; }
    Actor actor = manager.actor;
    x = XOffset() + iw;
    y = (Height()/2) - (2*ih);
    

    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() + iw, Height()/2, iw, Height()/2),
      scrollPosition,
      new Rect(0, 0, 200, 200)
    );
    GUI.color = Color.green;

    for(int i = 0; i < selling.Count; i++){
      y = i*ih;
      str = selling[i] != null ? selling[i].displayName : "";
      if(selling[i] != null && selling[i].stack > 1){ str += "(" + selling[i].stack + ")"; }
      if(Button(str, 0, y, iw, ih, 0, i)){
        Sell(i);
      }
    }
    
    GUI.EndScrollView();
    
    scrollPositionB = GUI.BeginScrollView(
      new Rect(XOffset() + 2*iw, Height()/2, iw, Height()/2),
      scrollPositionB,
      new Rect(0, 0, 200, 200)
    );

    for(int i = 0; i < buying.Count; i++){
      y = i*ih;
      str = buying[i] != null ? buying[i].displayName : "Null";
      if(buying[i] != null && buying[i].stack > 1){ str += "(" + buying[i].stack + ")"; }
      if(Button(str, 0, y, iw, ih, 0, i)){
        Buy(i);
      }
    }
    GUI.EndScrollView();
    
    
    if(balance < 0){
      str = "" + (-1*balance);
      str = str + "=> ";
    }
    else if(balance > 0){
      str = "<=" + balance;
    }
    else{
      str = "" + balance;
    }
    x = XOffset() + iw + iw/2;
    y = Height()/4;
    Box(str, x, y, iw, ih);
    
    if((balance >= 0) && (bought.Count > 0 || sold.Count > 0)){
      str = "Complete trade";
      x = XOffset();
      y = Height()/4;
      if(Button(str, x, y, iw, ih, -1)){
        FinalizeTrade();
      }
    }
    
    str = "Talk";
    x = XOffset() + Width() - iw;
    y = Height()/2;
    if(Button(str, x, y, iw, ih, 1)){
      manager.Change("SPEECH");
    }
  }
  
  void Buy( int i){
    if(manager.actor == null){ return; }
    if(manager.actor.interlocutor == null){ return; }
    Actor actor = manager.actor;
    Data item = buying[i];
    balance -= item.baseValue;
    selling.Add(item);
    buying.Remove(item);
    sold.Remove(item);
    if(actor.inventory.IndexOf(item) == -1){
      bought.Add(item);
    }
  }

  /* Sell an item to npc. */
  void Sell(int i){
    if(manager.actor == null){ return; }
    if(manager.actor.interlocutor == null){ return; }
    Actor actor = manager.actor;
    Data item = selling[i];
    balance += item.baseValue;
    buying.Add(item);
    selling.Remove(item);
    bought.Remove(item);
    if(actor.interlocutor.inventory.IndexOf(item) == -1){
      sold.Add(item);
    }
  }

    /* Distribute items that were bought and sold. */
  void FinalizeTrade(){
    if(manager.actor == null){ return; }
    if(manager.actor.interlocutor == null){ return; }
    Actor actor = manager.actor;
    Actor interlocutor = manager.actor.interlocutor;
    Inventory inv = actor.inventory;
    Inventory invB = interlocutor.inventory;
    for(int i = 0; i < bought.Count; i++){
      Data item = bought[i];
      int remainder = inv.Store(invB.Retrieve(invB.IndexOf(item)));
      if(remainder > 0){ 
        item = new Data(item);
        item.stack = remainder;
        inv.DiscardItem(item, manager.transform.position);
      }
    }
    for(int i = 0; i < sold.Count; i++){
      Data item = sold[i];
      int remainder = invB.Store(inv.Retrieve(inv.IndexOf(item)));
      if(remainder > 0){ 
        item = new Data(item);
        item.stack = remainder;
        invB.DiscardItem(item, manager.transform.position); 
      }
    }
    balance = 0;
    sold = new List<Data>();
    bought = new List<Data>();
    selling = new List<Data>(actor.inventory.inv);
    buying = new List<Data>(actor.interlocutor.inventory.inv);
    ClearNulls(selling);
    ClearNulls(buying);
  }
  
  public override void UpdateFocus(){
    if(buying == null || selling == null){ return; }
    if(sx == 0){ syMax = selling.Count -1; }
    else if(sx == 1){ syMax = buying.Count -1; }
    else if(sx == 2){ syMax = 0;}
    SecondaryBounds();
  }
  
  public override void Input(int button){
    DefaultExit(button);
    switch(sx){
      case 0:
        if(sy < 0 || sy > syMax){ return; }
        Sell(sy);
        break;
      case 1:
        if(sy < 0 || sy > syMax){ return; }
        Buy(sy);
        break;
      case 2:
        manager.Change("SPEECH");
        break;
    }
  }
  
  /* Stores item in targeted inventory and drops overflow. */
  public void Store(Inventory target, Data dat){
    dat.stack = target.Store(dat);
    if(dat.stack > 0){ target.DiscardItem(dat, manager.transform.position); }
  }
  
}
