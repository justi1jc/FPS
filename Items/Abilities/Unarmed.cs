/*
    Unarmed enables an Actor to punch with both hands.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unarmed : Ability{
  public float damageStart, damageEnd, knockBack, cooldown;
  public int damage;
  bool damageActive;
  public void Awake(){
    oneHanded = false;
    ready = true;
  }
  
  public override void Use(int use){
    if(use == Item.A_DOWN && ready){ RightPunch(); }
  }
  
  /* Triggers Actor's Right punch animation */
  private void RightPunch(){
    StartCoroutine(Swing());
    if(holder != null){ holder.SetAnimTrigger("rightPunch"); }
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
    if(col.gameObject == null){ MonoBehaviour.print("Gameobject missing"); return; }
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
      if(item != null && !(item is Ability) && item.holder!= null){
        item.holder.arms.Drop(item);
      }
    }
  }
  
  public override Data GetData(){
    Data dat = GetBaseData();
    AddAbilityData(ref dat);
    return dat;
  }
  
}
