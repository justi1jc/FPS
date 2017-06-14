/*
    AbilityMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AbilityMenu : Menu{
  
  public AbilityMenu(MenuManager manager){
    this.manager = manager;
    this.split = this.manager.split;
    this.right = this.manager.right;
  }
  
  public override void Render(){}
  
  public override void UpdateFocus(){}
  
  public override void Input(int button){}
  
}
