/*
    Shotguns fire a multitude of projectiles that spread.
    sounds[0] = firing sound
    sounds[1] = reloading sound
    sounds[2] = dry fire
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Shotgun : Ranged{
  public int pellets; // How many projectiles are fired with each shot.
  public float spread;
  
  /* Fires multiple projectiles at once.  */
  public override void Fire(){
    if(ammo < 1 || !ready){ return; }
    ammo--;
    Sound(0);
    Vector3 off = holder != null ? holder.stats.AccuracyPenalty() : new Vector3();
    for(int i = 0; i < pellets; i++){
      FireProjectile(spread, off.x, off.y, off.z);
    }
    if(holder != null){ holder.Recoil(recoil); }
  }
  
  /* Adds ammo to weapon individually. */
  public override void LoadAmmo(){
    if(holder == null){ return; }
    if(ammo >= maxAmmo){ return; }
    int available = holder.RequestAmmo(ammunition, 1);
    if(available > 0){ ammo = ammo + available; return; }
  }
  
  /* Updates damage to use pellets */
  public override string GetUseInfo(int row = 0){
    string ret = "";
    switch(row){
      case 0: 
        ret = "Damage: " + damage + "X" + pellets;
        ret += "(" + (damage * pellets) + ")";
        break;
      case 1: ret = "Capacity: " + maxAmmo; break;
      case 2: ret = "Rate of fire: " + (60f/cooldown) + " /minute"; break;
      case 3: ret = "Spread: " + spread; break;
      case 4: 
        ret = "Full reload time: " + (reloadDelay*maxAmmo) + " seconds";
        break;
    }
    return ret;
  }
}
