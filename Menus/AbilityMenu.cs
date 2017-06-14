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
    if(manager.actor == null){ return; }
    for(int i = 0; i < manager.actor.abilities.Length; i++){
      if(manager.actor.abilities[i]){
        selections.Add(i);
      }
    }
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
    
    for(int i = 0; i < selections.Count; i++){
      int ability = selections[i];
      string selected = "";
      if(i == manager.actor.rightAbility){ selected += "Right Hand "; }
      if(i == manager.actor.leftAbility){ selected += "Left Hand "; }
      if(Button(selected, 0, ih * i, iw, ih, 0, i)){
        manager.actor.EquipAbility(ability);
      }
      if(Button("EquipLeft", iw, ih * i, iw, ih, 0, i)){
        manager.actor.EquipAbilitySecondary(ability);
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
    switch(sx){
      case -1:
        if(button == A){ manager.Change("INVENTORY"); }
        break;
      case 0:
        if(button == A || button == RT){ manager.actor.EquipAbility(selections[sy]); }
        if(button == LT){ manager.actor.EquipAbilitySecondary(selections[sy]); }
        break;
      case 1:
        if(button == A){ manager.Change("STATS"); }
        break;
    }
  }
  
}
