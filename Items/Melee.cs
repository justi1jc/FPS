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
  
  public void Start(){
    ready = true;
  }
  
  public override void Use(int action){
    if(action == 0 && ready){ StartCoroutine(Swing()); }
  }
  
  /* Swings melee weapon. */
  IEnumerator Swing(){
    ready = false;
    damageActive = false;
    yield return new WaitForSeconds(damageStart);
    damageActive = true;
    yield return new WaitForSeconds(damageEnd);
    damageActive = false;
    ready = true;
  }
  
  void OnTriggerEnter(Collider col){ Strike(col); }
  void OnTriggerStay(Collider col){ Strike(col); }
  
  /* Exert force and damage onto target.  */
  void Strike(Collider col){
    if(damageActive){
      HitBox hb = col.gameObject.GetComponent<HitBox>();
      if(hb){
        StartCoroutine(CoolDown(cooldown));
        hb.ReceiveDamage(damage, gameObject);
      }
      Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
      if(rb){ rb.AddForce(transform.forward * knockBack); }
    }
  }
}
