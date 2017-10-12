/*
    AbilityMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AbilityMenu : Menu{  
  public AbilityMenu(MenuManager manager) : base(manager){
    if(manager.actor == null || manager.actor.stats != null){ return; }
  }
  
  public override void Render(){
    Box("", XOffset(), 0, Width(), Height()); // Background
    int iw = Width()/4;
    int ih = Height()/20;
    
    // Display navigation buttons.
    if(Button("Stats", XOffset() + Width() - iw, Height()/2, iw, ih, 1, 0)){
      manager.Change("STATS");Sound(0);
    }
    if(Button("Inventory", XOffset(), Height()/2, iw, ih, -1, 0)){
      manager.Change("INVENTORY"); Sound(0);
    }
    
  }
  
}
