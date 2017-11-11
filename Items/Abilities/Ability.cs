/*
    An Ability is an item that lacks a physical form. As such, it should not be
    dropped nor stored in an Inventory.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability : Item{
  
  /* Destroys this item. */
  public override void Drop(){}
  
  /* Returns the data for this  */
  public void AddAbilityData(ref Data dat){
    dat.itemType = (int)Item.Types.Ability;  
  }
}
