/*
    Container is Decor that contains an inventory of other items.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Container : Decor{
  List<Data> contents = null;
  
  public void Start(){
    if(contents == null){ contents = new List<Data>(); }
  }
  
  public override void Interact(Actor a, int mode = -1, string message = ""){
    if(a != null && a.menu != null){
      a.menu.contents = contents;
      a.menu.Change(Menu.LOOT);
      return;
    }
  }
  
  public override Data GetData(){
    Data dat = GetBaseData();
    for(int j = 0; j < contents.Count; j++){
      dat.inventory.inv.Add(contents[j]);
    }
    return dat;
  }
  
  public override void LoadData(Data dat){
    LoadBaseData(dat);
    if(dat.inventory != null){ contents = new List<Data>(dat.inventory.inv); }
    else{ contents = new List<Data>(); }
  }
  
}
