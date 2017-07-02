/*
    HUDMenu
    Serves to display the player's heads-up display.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HUDMenu : Menu{
  public string message = "";
  
  public HUDMenu(MenuManager manager) : base(manager){}
  
  public override void Render(){
    Actor actor = manager.actor;
    if(actor == null){ return; }
    if(actor.Alive()){ RenderAlive(); }
    else{ RenderDead(); }
  }
  
  void RenderAlive(){
    Actor actor = manager.actor;
    if(actor == null || actor.stats == null){ return; }
    StatHandler stats = actor.stats;
    // Display Condition bars
    int ih = Height()/20;
    int iw = Width()/3;
    int x, y;
    string str;
    
    str = "Health: " + stats.health;
    Box(str, XOffset(), 17*ih, iw, ih);
    
    str = "Stamina: " + stats.stamina;
    Box(str, XOffset(), 18*ih, iw, ih);
    
    str = "Mana: " + stats.mana;
    Box(str, XOffset(), 19*ih, iw, ih);
    
    str = "X";
    Box(str, XOffset() + Width()/2, Height()/2, ih, ih);    
    
    // Display Item info
    str = actor.ItemInfo();
    Box(str, XOffset() + 2*iw, 18*ih, iw, 2*ih);
    
    // Display item in reach, if it exists.
    if(actor.itemInReach){
      Item inReach = manager.actor.itemInReach.GetComponent<Item>();
      x = XOffset() + iw;
      y =  19 * ih;
      if(inReach.displayName != ""){ 
        Box(inReach.displayName, x, y, iw, ih);
      }
    }
    else if(actor.actorInReach){
      str = actor.ActorInteractionText();
      x = XOffset() + iw;
      y = 19 * ih;
      Box(str, x, y, iw, ih); 
    }
  }
  
  void RenderDead(){
    int iw = Width()/2;
    int ih = Height()/3;
    Box(message, XOffset() + iw/2, Height()/2, iw, ih);
  }
  
  public override void UpdateFocus(){}
  
  public override void Input(int button){}
  
}
