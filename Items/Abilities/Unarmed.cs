/*
    Unarmed enables an Actor to punch with both hands.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unarmed : Ability{
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
  
  
  
  public override Data GetData(){
    Data dat = GetBaseData();
    AddAbilityData(ref dat);
    return dat;
  }
  
}
