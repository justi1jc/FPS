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
  
  
}
