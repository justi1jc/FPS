/*
    Food loads 
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scenery : Item{
  public int healing; // Health gained by consumption.
  bool ready = true;
  public float cooldown; // Delay between uses
  
  public override void Use(int action){
    if(action == 0){ Consume(); }
  }
  
  /* Heals holder. */
  public void Consume(){
    if(!ready || holder == null){ return; }
    holder.ReceiveDamage(-healing, gameObject);
    this.stack--;
    if(stack < 1){
      holder.Drop();
      Destroy(this.gameObject);
    }
    StartCoroutine(CoolDown(cooldown));
  }
}
