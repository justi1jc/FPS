/*
    Weapon is base for specific weapons.
    The main purpose is to provide a one-line comparison to check if 
    an Item is a weapon.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : Item{  
  public override string GetInfo(){
    return displayName + " " + damage + " dmg"; 
  }
  
  protected void AddWeaponData(Data dat){
    dat.ints.Add(damage);
  }
  
  /* Returns the damage of a weapon's data, or 0 upon failure. */
  public static int Damage(Data dat){
    if(dat == null || dat.ints.Count < 2){
      return dat.ints[1];
    }
    return 0;
  }
}
