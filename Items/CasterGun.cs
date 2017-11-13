/*
  A caster gun utilizes an internal reserve of mana to cast destruction spells.
  When equipped, a CasterGun will refill its reserve by draining mana from its
  user.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CasterGun : Ranged{
  private int manaReserve; // Mana that has not yet been converted into ammo.
  public int conversionCost; // Mana per ammo cost.
  public float conversionDelay; // How long to wait between recharge
  
  
  /* Disabled reload, since weapon recharges. */
  public override IEnumerator Reload(){ yield return new WaitForSeconds(0f); }
  
  /* Bonds this castergun to actor and begins recharge */
  public override void Hold(Actor a){
    ammunition = "NONE";
    if(held){ return; }
    held = true;
    holder = a;
    
    Rigidbody rb = transform.GetComponent<Rigidbody>();
    if(rb != null){
      rb.isKinematic = true;
      rb.useGravity = false;
    }
    transform.localPosition = heldPos;
    Quaternion lr = Quaternion.Euler(heldRot.x, heldRot.y, heldRot.z);
    transform.localRotation = lr;
    Collider c = transform.GetComponent<Collider>();
    c.isTrigger = true;
    StartCoroutine(Recharge());
  }
  
  /* Drains holder's mana and converts it to ammo. */
  private IEnumerator Recharge(){
    while(holder != null){
      if(ammo < maxAmmo){
        manaReserve += holder.stats.DrainCondition(StatHandler.Stats.Mana, conversionCost);
        if(manaReserve >= conversionCost){
          manaReserve -= conversionCost;
          ammo++;
        }
      }
      yield return new WaitForSeconds(conversionDelay);
    }
    yield return new WaitForSeconds(0f);
  }
  
  /* Accounts for recharging. */
  public override string GetUseInfo(int row = 0){
    string ret = "";
    switch(row){
      case 0: ret = "Damage: " + damage; break;
      case 1: ret = "Capacity: " + maxAmmo; break;
      case 2: ret = "Rate of fire: " + (60f/cooldown) + "/minute"; break;
      case 3: ret = "Mane per shot: " + conversionCost; break;
      case 4: ret = "Charges per minute: " + (60f/conversionDelay); break;
    }
    return ret;
  }

}
