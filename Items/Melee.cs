/*
    A Melee weapon delivers damage upon contact for the duration of its swing.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Melee : Weapon{
  public float damageStart; // Begin of effective swing
  public float damageEnd;   // End of effective swing
  public bool damageActive; // True if damage can be given
  public float knockBack;   // Magnitude of force exerted on target
  
  public override void Use(int action){
    if(action == 0 && ready){ 
      chargeable = true;
      ChargeSwing();
    }
  }
  
  /* Charges up a swing. */
  void ChargeSwing(){
    if(chargeable && charge < chargeMax){
      charge++;
      effectiveDamage = (damage * charge) / chargeMax;
    }
    if(chargeable && executeOnCharge && (charge >= chargeMax) && ready){
      charge = 0;
      chargeable = false;
      StartCoroutine(Swing());
    }
  }
  
  /* Swings melee weapon. */
  IEnumerator Swing(){
    ready = false;
    damageActive = false;
    yield return new WaitForSeconds(damageStart);
    damageActive = true;
    transform.position += transform.forward;
    yield return new WaitForSeconds(damageEnd);
    damageActive = false;
    ready = true;
    transform.position -= transform.forward;
  }
  
  void OnTriggerEnter(Collider col){ Strike(col); }
  void OnTriggerStay(Collider col){ Strike(col); }
  
  /* Exert force and damage onto target.  */
  void Strike(Collider col){
    if(damageActive){
      HitBox hb = col.gameObject.GetComponent<HitBox>();
      if(hb){
        StartCoroutine(CoolDown(cooldown));
        hb.ReceiveDamage(effectiveDamage, gameObject);
        chargeable = false;
        effectiveDamage = 0;
      }
      Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
      if(rb){ rb.AddForce(transform.forward * knockBack); }
    }
  }
}
