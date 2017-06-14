/*
    MainMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MainMenu : Menu{
  
  public MainMenu(MenuManager manager){
    this.manager = manager;
    this.split = this.manager.split;
    this.right = this.manager.right;
  }
  
  public override void Render(){}
  
  public override void UpdateFocus(){}
  
  public override void Input(){}
  
}
