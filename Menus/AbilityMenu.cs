/*
    AbilityMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AbilityMenu : Menu{
  public List<int> selections;
  
  public AbilityMenu(MenuManager manager) : base(manager){
    syMin = 0;
    sxMin = -1;
    syMax = 1;
    
    selections = new List<int>();
    if(manager.actor == null || manager.actor.stats != null){ return; }
    selections = manager.actor.stats.abilities;
  }
  
  public override void Render(){
    Box("", XOffset(), 0, Width(), Height()); // Background
    int iw = Width()/4;
    int ih = Height()/20;
    
    // Display navigation buttons.
    if(Button("Stats", XOffset() + Width() - iw, Height()/2, iw, ih, 1, 0)){
      manager.Change("STATS");
    }
    if(Button("Inventory", XOffset(), Height()/2, iw, ih, -1, 0)){
      manager.Change("INVENTORY");
    }
    
    // Display ability buttons.
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() + iw, Height()/2, Width()-iw, Height()),
      scrollPosition,
      new Rect(0, 0, 200, 200)
    );
    if(manager.actor == null || manager.actor.arms == null){ return; }
    Actor actor = manager.actor;
    selections = manager.actor.stats.abilities;
    for(int i = 0; i < selections.Count; i++){
      int ability = selections[i];
      string selected = "";
      if(ability == actor.arms.offHandAbility){ selected += "Right "; }
      if(ability == actor.arms.handAbility){ selected += "Left "; }
      selected += actor.arms.AbilityInfo(i);
      if(Button(selected, 0, ih * i, iw, ih, 0, i)){
        actor.arms.EquipAbility(ability, false);
      }
      if(Button("Equip Left", iw, ih * i, iw, ih, 0, i)){
        actor.arms.EquipAbility(ability, true);
      }
    }
    GUI.EndScrollView();
  }
  
  public override void UpdateFocus(){
    if(selections != null){ syMax = selections.Count - 1; }
    else{ syMax = 0; }
    if(sx != 0){ sy = 0; }
    SecondaryBounds();
  }
  
  public override void Input(int button){
    DefaultExit(button);
    if(manager.actor == null || manager.actor.arms == null){ return; }
    Actor actor = manager.actor;
    switch(sx){
      case -1:
        if(button == A){ manager.Change("INVENTORY"); }
        break;
      case 0:
        if(button == A || button == RT){ 
          actor.arms.EquipAbility(selections[sy], true); 
        }
        if(button == LT){ 
          actor.arms.EquipAbility(selections[sy], false); 
        }
        break;
      case 1:
        if(button == A){ manager.Change("STATS"); }
        break;
    }
  }
  
}
