/*
    Sentry AI just waits for an enemy to come into sight, then fights
    them by switching to MeleeAI or RangedAI.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class SentryAI : AI{
  public SentryAI(Actor actor, AIManager manager) : base(actor, manager){
    destinationState = AI.States.Sentry;
  }
  
  public override void Update(){
    if(!EnemiesFound()){ return; }
    if(EquipRangedWeapon()){ Transition(States.Ranged); }
    else{
      EquipMeleeWeapon();
      Transition(States.Melee);
    }
  }
}
