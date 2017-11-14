/*
    AI fights enemies from a distance using a ranged weapon.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RangedAI : AI{
  public RangedAI(Actor actor, AIManager manager) : base(actor, manager){}
  
  public override void Update(){
    try{
      if(!TargetFound()){ Transition(AI.States.Sentry); }
      bool inRange = MoveToward(target.body.transform.position);
      bool aimed = AimAt(target.head.transform, 10f);
      if(inRange && aimed){ 
        actor.Use(Item.Inputs.A_Down);
        actor.Use(Item.Inputs.A_Up); // This will cease automatic fire.
      }
    }catch(System.Exception e){}
  }
}
