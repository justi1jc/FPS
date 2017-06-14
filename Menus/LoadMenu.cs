/*
    LoadMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LoadMenu : Menu{
  
  public LoadMenu(MenuManager manager){
    this.manager = manager;
    this.split = this.manager.split;
    this.right = this.manager.right;
  }
  
  public override void Render(){}
  
  public override void UpdateFocus(){}
  
  public override void Input(int button){}
  
}
