/*
    A Melee weapon delivers damage upon contact for the duration of its swing.
    sounds[0] is striking sound
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Melee : Weapon{
  public float damageStart; // Begin of effective swing
  public float damageEnd;   // End of effective swing
  public bool damageActive; // True if damage can be given
  public float knockBack;   // Magnitude of force exerted on target
  public bool stab; // Actor stabs with this item if true
  
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
      if(holder != null){
        if(holder.GetRoot(col.gameObject.transform) == holder.transform.transform){
          return;
        }
      }
      HitBox hb = col.gameObject.GetComponent<HitBox>();
      if(hb){
        StartCoroutine(CoolDown(cooldown));
        hb.ReceiveDamage(damage, gameObject);
        damageActive = false;
        if(hb.body != null){
          Rigidbody hbrb = hb.body.gameObject.GetComponent<Rigidbody>();
          Vector3 forward = transform.position - hb.body.transform.position;
          forward = new Vector3(forward.x, 0f, forward.y);
          if(hbrb){ hbrb.AddForce(forward * knockBack); }
        }
      }
      Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
      if(rb){ rb.AddForce(transform.forward * knockBack); Sound(0); }
      Item item = col.gameObject.GetComponent<Item>();
      if(item != null && !item.ability){
        item.holder.arms.Drop(item);
      }
    }
  }
  
  public override Data GetData(){
    Data dat = GetBaseData();
    dat.itemType = Item.MELEE;
    return dat;
  }
}
