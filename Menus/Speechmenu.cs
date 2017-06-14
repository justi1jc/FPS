/*
    SpeechMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpeechMenu : Menu{
  
  public SpeechMenu(MenuManager manager){
    this.manager = manager;
    this.split = this.manager.split;
    this.right = this.manager.right;
  }
  
  public override void Render(){}
  
  public override void UpdateFocus(){}
  
  public override void Input(int button){}
  
}
