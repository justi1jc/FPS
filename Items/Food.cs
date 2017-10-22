/*
    Food is an item that imparts health onto its user.
    sounds[0] = eating sound
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Food : Item{
  public int healing; // Health gained by consumption.
  
  public override void Use(int action){
    if(action == A_DOWN){ Consume(); }
  }
  
  /* Heals holder. */
  public void Consume(){
    if(!ready || holder == null){ return; }
    holder.ReceiveDamage(new Damage(-healing, gameObject));
    this.stack--;
    Sound(0);
    if(stack < 1){
      holder.Drop();
      Destroy(this.gameObject);
    }
    StartCoroutine(CoolDown(cooldown));
  }
}
