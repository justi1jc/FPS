/*
    HUDMenu
    Serves to display the player's heads-up display.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HUDMenu : Menu{
  public string message = "";
  public Texture innerReticle, outerReticle;
  
  public HUDMenu(MenuManager manager) : base(manager){}
  
  public override void Render(){
    innerReticle = Resources.Load("Textures/reticle_1") as Texture;
    outerReticle = Resources.Load("Textures/reticle_2") as Texture;
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
    string str = "";
    
    float current = (float)stats.health;
    float max = (float)stats.healthMax;
    x = XOffset();
    y = 17*ih;
    str = "" + stats.health + "/" + stats.healthMax;
    ProgressBar(current, max, (float)x, (float)y, Color.red, str);
    
    current = (float)stats.stamina;
    max = (float)stats.staminaMax; 
    y = 18*ih;
    str = "" + stats.stamina + "/" + stats.staminaMax;
    ProgressBar(current, max, (float)x, (float)y, Color.green, str);
    
    current = (float)stats.mana; 
    max = (float)stats.manaMax;
    y = 19*ih;
    str = "" + stats.mana + "/" + stats.manaMax;
    ProgressBar(current, max, (float)x, (float)y, Color.blue, str);
    
    RenderReticle();
    
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
  
  /* Stub to prevent HUD from rendering cursor. */
  public override void RenderCursor(){}
  
  /* Render graphic reticle in center of screen. */
  public void RenderReticle(){
    GUI.backgroundColor = new Color(0f, 0f, 0f, 0f);
    float size = 100f;
    float x = (Width() - size)/2f;
    float y = (Height() - size)/2f;
    GUI.Box(new Rect(x, y, size, size), innerReticle);
    int accuracy = manager.actor.stats.AccuracyScore();
    float scale = 1.5f - ((float)accuracy)/100f;
    size *= scale;
    x = (Width() - size)/2f;
    y = (Height() - size)/2f;
    GUI.Box(new Rect(x, y, size, size), outerReticle);
    GUI.backgroundColor = new Color(1f, 1f, 1f, 1f);
  }
  
  void RenderDead(){
    int iw = Width()/2;
    int ih = Height()/3;
    Box(message, XOffset() + iw/2, Height()/2, iw, ih);
  }
  
  public override void UpdateFocus(){}
  
  public override void Input(int button){
    if(button == START){ 
      manager.Change("OPTIONS");
      if(manager.actor != null){ manager.actor.SetMenuOpen(true); }
    }
  
  }
  
}
