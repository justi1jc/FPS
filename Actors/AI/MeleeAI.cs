/*
    MeleeAI fights enemies up-close with a melee weapon by moving directly at
    them.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MeleeAI : AI{
  public MeleeAI(Actor actor, AIManager manager) : base(actor, manager){}
  
  public override void Update(){
    try{
      if(!TargetFound()){ Transition(AI.States.Sentry); }
      bool inRange = MoveToward(target.body.transform.position);
      bool aimed = AimAt(target.head.transform, 10f);
      if(inRange && aimed){ actor.Use(Item.Inputs.A_Down); }
    }catch(System.Exception e){}
  }
}
