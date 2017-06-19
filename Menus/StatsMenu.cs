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
    if(actor == null || actor.stats == null){ return; }
    StatHandler stats = actor.stats;
    
    // Render attribute column
    str = "Intelligence: " + stats.intelligence;
    Box(str,XOffset()+aw, aOff+0, aw, ah);
    
    str = "Charisma: " + stats.charisma;
    Box(str,XOffset()+aw, aOff+ah, aw, ah);
    
    str = "Endurance: " + stats.endurance;
    Box(str,XOffset()+aw, aOff+2*ah, aw, ah);
    
    str = "Perception: " + stats.perception;
    Box(str,XOffset()+aw, aOff+3*ah, aw, ah);
    
    str = "Agility: " + stats.agility;
    Box(str,XOffset()+aw, aOff+4*ah, aw, ah);
    
    str = "Willpower: " + stats.willpower;
    Box(str,XOffset()+aw, aOff+5*ah, aw, ah);
    
    str = "Strength: " + stats.strength;
    Box(str,XOffset()+aw, aOff+6*ah, aw, ah);
    
    str = "Level: " + stats.level;
    Box(str, XOffset()+aw, 0, aw, ah);
    
    str = "XP: " + stats.xp;
    Box(str, XOffset()+aw, ah, aw, ah);
    
    str = "Next level: " + stats.nextLevel;
    Box(str, XOffset()+aw, 2*ah, aw, ah);
    
    // Render Skills
    str = "Remaining Skill Points: " + stats.skillPoints;
    Box(str, sOff, 0, aw, sh);
    
    str = "Ranged: " + stats.ranged;
    if(Button(str, sOff, sh, aw, sh, 0, 0)){
      if(stats.skillPoints > 0 && stats.ranged < 100){
        stats.skillPoints--; stats.ranged++;
      }
    }
    str = "Melee: " + stats.melee;
    if(Button(str, sOff, 2*sh, aw, sh, 0, 1)){
      if(stats.skillPoints > 0 && stats.melee < 100){
        stats.skillPoints--; stats.melee++;
      }
    }
    str = "Unarmed: " + stats.unarmed;
    if(Button(str, sOff, 3*sh, aw, sh, 0, 2)){
      if(stats.skillPoints > 0 && stats.unarmed < 100){
        stats.skillPoints--; stats.unarmed++;
      }
    }
    str = "Magic: " + stats.magic;
    if(Button(str, sOff, 4*sh, aw, sh, 0, 3)){
      if(stats.skillPoints > 0 && stats.magic < 100){
        stats.skillPoints--; stats.magic++;
      }
    }
    str = "Stealth: " + stats.stealth;
    if(Button(str, sOff, 5*sh, aw, sh, 0, 4)){
      if(stats.skillPoints > 0 && stats.stealth < 100){
        stats.skillPoints--; stats.stealth++;
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
    if(manager.actor == null || manager.actor.stats == null){ return; }
    Actor actor = manager.actor;
    StatHandler stats = actor.stats;
    if(sx == -1 && button == A){ manager.Change("ABILITY"); }
    else if(sx == 0 && button == A && stats.skillPoints > 0){
      switch(sy){
        case 0:
          if(stats.ranged < 100){
            stats.skillPoints--; stats.ranged++;
          }
          break;
        case 1:
          if(stats.melee < 100){
            stats.skillPoints--; stats.melee++;
          }
          break;
        case 2:
          if(stats.unarmed < 100){
            stats.skillPoints--; stats.unarmed++;
          }
          break;
        case 3:
          if(stats.magic < 100){
            stats.skillPoints--; stats.magic++;
          }
          break;
        case 4:
          if(stats.stealth < 100){
            stats.skillPoints--; stats.stealth++;
          }
          break;
      }
    }
    else if(sx == 1 && button == A){ manager.Change("QUEST"); }
  }
  
}
