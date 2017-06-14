/*
    HUDMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HUDMenu : Menu{
  
  public HUDMenu(MenuManager manager){
    this.manager = manager;
    this.split = this.manager.split;
    this.right = this.manager.right;
  }
  
  public override void Render(){}
  
  public override void UpdateFocus(){}
  
  public override void Input(int button){}
  
}
