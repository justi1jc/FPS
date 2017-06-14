/*
    StatsMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StatsMenu : Menu{
  
  public StatsMenu(MenuManager manager) : base(manager){
  }
  
  public override void Render(){
    Box("", XOffset(), 0, Width(), Height()); // Background
    int ah = Height()/14; // Attribute height
    int aw = Width()/6;  // Attribute width
    int aOff = (int)(3.5 * ah); // y Offset for attributes
    int sh = Height()/6; // Skill height
    int sOff = 2 * aw + XOffset();// x offset for skills
    
    
     
    string str = "";
    
    // Render navigation buttons.
    str = "Abilities";
    if(Button(str, XOffset(), Height()/4, aw, Height()/2, -1 )){
      manager.Change("ABILITY");
    }
    str = "Quests";
    if(Button(str, XOffset()+5*aw, Height()/4, aw, Height()/2, 1)){
      manager.Change("QUEST");
    }
    
    Actor actor = manager.actor;
    if(actor == null){ return; }
    
    // Render attribute column
    str = "Intelligence: " + actor.intelligence;
    Box(str,XOffset()+aw, aOff+0, aw, ah);
    
    str = "Charisma: " + actor.charisma;
    Box(str,XOffset()+aw, aOff+ah, aw, ah);
    
    str = "Endurance: " + actor.endurance;
    Box(str,XOffset()+aw, aOff+2*ah, aw, ah);
    
    str = "Perception: " + actor.perception;
    Box(str,XOffset()+aw, aOff+3*ah, aw, ah);
    
    str = "Agility: " + actor.agility;
    Box(str,XOffset()+aw, aOff+4*ah, aw, ah);
    
    str = "Willpower: " + actor.willpower;
    Box(str,XOffset()+aw, aOff+5*ah, aw, ah);
    
    str = "Strength: " + actor.strength;
    Box(str,XOffset()+aw, aOff+6*ah, aw, ah);
    
    str = "Level: " + actor.level;
    Box(str, XOffset()+aw, 0, aw, ah);
    
    str = "XP: " + actor.xp;
    Box(str, XOffset()+aw, ah, aw, ah);
    
    str = "Next level: " + actor.nextLevel;
    Box(str, XOffset()+aw, 2*ah, aw, ah);
    
    // Render Skills
    str = "Remaining Skill Points: " + actor.skillPoints;
    Box(str, sOff, 0, aw, sh);
    
    str = "Ranged: " + actor.ranged;
    if(Button(str, sOff, sh, aw, sh, 0, 0)){
      if(actor.skillPoints > 0 && actor.ranged < 100){
        actor.skillPoints--; actor.ranged++;
      }
    }
    str = "Melee: " + actor.melee;
    if(Button(str, sOff, 2*sh, aw, sh, 0, 1)){
      if(actor.skillPoints > 0 && actor.melee < 100){
        actor.skillPoints--; actor.melee++;
      }
    }
    str = "Unarmed: " + actor.unarmed;
    if(Button(str, sOff, 3*sh, aw, sh, 0, 2)){
      if(actor.skillPoints > 0 && actor.unarmed < 100){
        actor.skillPoints--; actor.unarmed++;
      }
    }
    str = "Magic: " + actor.magic;
    if(Button(str, sOff, 4*sh, aw, sh, 0, 3)){
      if(actor.skillPoints > 0 && actor.magic < 100){
        actor.skillPoints--; actor.magic++;
      }
    }
    str = "Stealth: " + actor.stealth;
    if(Button(str, sOff, 5*sh, aw, sh, 0, 4)){
      if(actor.skillPoints > 0 && actor.stealth < 100){
        actor.skillPoints--; actor.stealth++;
      }
    }
    
  }
  
  public override void UpdateFocus(){
    syMax = 4;
    syMin = 0;
    sxMax = 1;
    sxMin = -1;
    if(sx != 0){sy = 0; }
    SecondaryBounds();
  }
  
  public override void Input(int button){
    DefaultExit(button);
    if(manager.actor == null){ return; }
    Actor actor = manager.actor;
    if(sx == -1 && button == A){ manager.Change("ABILITY"); }
    else if(sx == 0 && button == A && actor.skillPoints > 0){
      switch(sy){
        case 0:
          if(actor.ranged < 100){
            actor.skillPoints--; actor.ranged++;
          }
          break;
        case 1:
          if(actor.melee < 100){
            actor.skillPoints--; actor.melee++;
          }
          break;
        case 2:
          if(actor.unarmed < 100){
            actor.skillPoints--; actor.unarmed++;
          }
          break;
        case 3:
          if(actor.magic < 100){
            actor.skillPoints--; actor.magic++;
          }
          break;
        case 4:
          if(actor.stealth < 100){
            actor.skillPoints--; actor.stealth++;
          }
          break;
      }
    }
    else if(sx == 1 && button == A){ manager.Change("QUEST"); }
  }
  
}
