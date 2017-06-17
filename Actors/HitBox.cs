/*
*     Author: James Justice
*     A HitBox allows projectiles and other damaging items to direct their damage.
*     To the parent Actor.
*/

using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour {
  public Actor body;
  public void ReceiveDamage(int damage, GameObject weapon){
    if(!body){ return; }
    body.ReceiveDamage(damage, weapon);
  }
}
