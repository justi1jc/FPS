/*
    Weapon is base for specific weapons.
    The main purpose is to provide a one-line comparison to check if 
    an Item is a weapon.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : Item{
  public int damage;     // Damage this weapon does.
  public float cooldown; // Time between uses.
  public bool chargeable; // Weather or not this weapon can be charged.
  public int charge;     // Current charge of weapon
  public int chargeMax;  // Max charge a weapon can have before swinging.
  public int effectiveDamage;     // Damage as the result of charging.
  public bool executeOnCharge; // If true, weapon will trigger upon full charge.
  
  public override string GetInfo(){
    return displayName + " " + damage + " dmg"; 
  }
}
