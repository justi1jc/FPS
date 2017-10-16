/*
    A Melee weapon delivers damage upon contact for the duration of its swing.
    sounds[0] is striking sound
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Melee : Weapon{
  public bool stab; // Actor stabs with this item if true
  
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
    yield return new WaitForSeconds(damageStart);
    damageActive = true;
    yield return new WaitForSeconds(damageEnd);
    damageActive = false;
    ready = true;
  }
  
  void OnTriggerEnter(Collider col){ Strike(col); }
  void OnTriggerStay(Collider col){ Strike(col); }
  
  public override Data GetData(){
    Data dat = GetBaseData();
    AddWeaponData(dat);
    dat.itemType = Item.MELEE;
    return dat;
  }
}
