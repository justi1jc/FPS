/*
    Idle AI will just stand there and retaliate to attacks.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class IdleAI : AI{
  public IdleAI(Actor actor, AIManager manager) : base(actor, manager){}
  
  public override void ReceiveDamage(int damage, Actor damager){
    manager.target = damager.gameObject;
    manager.Change("HOSTILE");
  }
  
}
