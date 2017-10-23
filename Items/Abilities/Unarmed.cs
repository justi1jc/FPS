/*
    Unarmed enables an Actor to punch with both hands.
    sounds[0] is the striking sound.
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
    damage = holder.stats.DrainCondition(StatHandler.STAMINA, 25);
    damage += holder.stats.GetStat(StatHandler.STRENGTH);
    yield return new WaitForSeconds(damageStart);
    damageActive = true;
    yield return new WaitForSeconds(damageEnd);
    damageActive = false;
    ready = true;
  }
  
  void OnTriggerEnter(Collider col){ Strike(col, damage); }
  void OnTriggerStay(Collider col){ Strike(col, damage); }
  
  
  
  public override Data GetData(){
    Data dat = GetBaseData();
    AddAbilityData(ref dat);
    return dat;
  }
  
}
