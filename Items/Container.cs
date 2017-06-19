/*
    Container is Decor that contains an inventory of other items.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Container : Decor{
  Inventory contents = null;
  
  public void Wake(){
    if(contents == null){ contents = new Inventory(); }
  }
  
  public override void Interact(Actor a, int mode = -1, string message = ""){
    if(a != null && a.menu != null){
      a.menu.contents = contents;
      a.menu.Change("LOOT");
      return;
    }
  }
  
  public override Data GetData(){
    Data dat = GetBaseData();
    dat.inventoryRecord = contents.GetData();
    return dat;
  }
  
  public override void LoadData(Data dat){
    LoadBaseData(dat);
    if(contents == null){ contents = new Inventory(); }
    if(dat.inventoryRecord != null){ contents.LoadData(dat.inventoryRecord); }
  }
  
}
