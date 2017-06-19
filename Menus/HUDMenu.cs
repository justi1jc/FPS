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
    int cbsx = 3;  // condition bar width scale
    int cbsy = 20; // condition bar height scale
    int ch = Height()/15; // Condition height
    int cw = Width()/3;   // Condition width
    int x, y;
    string str;
    
    str = "Health: " + stats.health;
    Box(str, XOffset(), 12 * ch, cw, ch);
    
    str = "Stamina: " + stats.stamina;
    Box(str, XOffset(), 13 * ch, cw, ch);
    
    str = "Mana: " + stats.mana;
    Box(str, XOffset(), 14 * ch, cw, ch);
    
    
    // Display Item info
    str = actor.ItemInfo();
    Box(str, XOffset() + 2*cw, 14*ch, cw, ch);
    
    // Display item in reach, if it exists.
    if(actor.itemInReach){
      Item inReach = manager.actor.itemInReach.GetComponent<Item>();
      x = XOffset() + Width() - (2*Width()/cbsx);
      y = (9 * Height()/cbsy);
      if(inReach.displayName != ""){ 
        Box(inReach.displayName, x, y, Width()/cbsx, Height()/cbsy);
      }
    }
    else if(actor.actorInReach){
      str = actor.ActorInteractionText();
      x = XOffset() + Width() - (2 * Width()/cbsx);
      y = 9 * Height()/cbsy;
      Box(str, x, y, Width()/cbsx, Height()/cbsy); 
    }
  }
  
  void RenderDead(){
    int iw = Width()/2;
    int ih = Height()/3;
    Box(message, iw/2, Height()/2, iw, ih);
  }
  
  public override void UpdateFocus(){}
  
  public override void Input(int button){}
  
}
