/*
    A Melee weapon delivers damage upon contact for the duration of its swing.
    sounds[0] is striking sound
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Melee : Weapon{
  public bool stab; // Actor stabs with this item if true
  private int meleeDamage;
  
  public void Start(){
    ready = true;
  }
  
  public override void Use(int action){
    if(action == A_DOWN && ready){ StartCoroutine(Swing()); }
  }
  
  /* Swings melee weapon. */
  IEnumerator Swing(){
    ready = false;
    damageActive = false;
    if(holder != null){
      int stam = holder.stats.DrainCondition(StatHandler.STAMINA, 25);
      meleeDamage = (damage * stam)/25;
    }
    else{ meleeDamage = damage; }
    yield return new WaitForSeconds(damageStart);
    damageActive = true;
    yield return new WaitForSeconds(damageEnd);
    damageActive = false;
    ready = true;
  }
  
  void OnTriggerEnter(Collider col){ Strike(col, meleeDamage); }
  void OnTriggerStay(Collider col){ Strike(col, meleeDamage); }
  
  public override Data GetData(){
    Data dat = GetBaseData();
    AddWeaponData(dat);
    dat.itemType = Item.MELEE;
    return dat;
  }
  
  /* Accounts for damage. */
  public override string GetUseInfo(int row = 0){
    string ret = "";
    switch(row){
      case 0: ret = "Damage: " + damage; break;
    }
    return ret;
  }
}
