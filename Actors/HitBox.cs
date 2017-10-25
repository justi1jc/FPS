/*
*     Author: James Justice
*     A HitBox allows projectiles and other damaging items to direct their damage.
*     To the parent Actor.
*/

using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour {
  public bool foot; // If true, this hitbox will check for landing.
  public string limb; // Name of associated limb or hardpoint.
  public Actor body;
  public Item item;
  
  /* Routes damage to connected entities. */
  public void ReceiveDamage(Damage dam){
    dam.limb = limb;
    if(body){ body.ReceiveDamage(dam); }
    if(item){ item.ReceiveDamage(dam); }
  }
}
