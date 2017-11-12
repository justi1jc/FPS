/*
    AI is the base class for other ai scripts and contains convenience methods.
    Each AI script represents a state in a state machine.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MeleeAI : AI{
  public MeleeAI(Actor actor, AIManager manager) : base(actor, manager){
    MonoBehaviour.print("MeleeCombat started");
  }
  public override void Update(){
    try{
      if(!TargetFound()){ Transition(AI.States.Sentry); }
      bool inRange = MoveToward(target.body.transform.position);
      bool aimed = AimAt(target.head.transform, 10f);
      if(inRange && aimed){ actor.Use(Item.Inputs.A_Down); }
    }catch(System.Exception e){}
  }
}
