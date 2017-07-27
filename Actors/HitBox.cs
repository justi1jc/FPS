/*
*     Author: James Justice
*     A HitBox allows projectiles and other damaging items to direct their damage.
*     To the parent Actor.
*/

using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour {
  public bool foot; // If true, this hitbox will check for landing.
  public Actor body;
  public float multiplier = 1.0f; // Multiplies damage coming through.
  public void ReceiveDamage(int damage, GameObject weapon){
    if(!body){ return; }
    int finalDamage = (int)((float)damage * multiplier);
    body.ReceiveDamage(finalDamage, weapon);
  }
}
