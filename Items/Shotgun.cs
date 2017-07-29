using UnityEngine;
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
    for(int i = 0; i < pellets; i++){ FireProjectile(spread); }
  }
  
  
  
  /* Adds ammo to weapon individually. */
  public override void LoadAmmo(){
    if(holder == null){ return; }
    int available = holder.RequestAmmo(ammunition, 1);
    if(available > 0){ ammo = ammo + available; return; }
  }
  
  
}
