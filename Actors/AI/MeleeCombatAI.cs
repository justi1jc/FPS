/*
    
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MeleeCombatAI : AI {
  public Actor combatant;
  public bool follow = true;
  public bool attack = true;
  public bool trained = true;
  
  public MeleeCombatAI(Actor actor, AIManager manager) : base(actor, manager){}
  
  public override IEnumerator Begin(){
    yield return new WaitForSeconds(0f);
    if(manager.target != null){ combatant = manager.target.GetComponent<Actor>(); }
    if(combatant != null){
      actor.StartCoroutine(Follow());
      actor.StartCoroutine(Attack());
      actor.StartCoroutine(Train());
    }
    while(combatant != null && combatant.Alive()){ yield return new WaitForSeconds(1f); }
    follow = attack = trained = false;
    if(actor.defaultAI != ""){ manager.Change(actor.defaultAI); }
    else{ manager.Change("IDLE"); }
  }
  
  /* Pursue continuously. */
  public IEnumerator Follow(){
    while(follow && combatant != null){
      yield return actor.StartCoroutine( Pursue(combatant, 0f));
      yield return new WaitForSeconds(0.1f);
    }
  }
  
  /* Aims continuously. */
  public IEnumerator Train(){
    while(trained && combatant != null){
      yield return actor.StartCoroutine(AimAt(combatant));
      yield return new WaitForSeconds(0.1f);
    }
    yield return new WaitForSeconds(0f);
  }
  
  public IEnumerator Attack(){
    while(attack && combatant != null){
      Vector3 pos = actor.transform.position;
      Vector3 cpos = combatant.transform.position;
      float dist = Vector3.Distance(pos, cpos);
      if(dist < 3f){
        actor.Use(0);
        yield return new WaitForSeconds(0.4f);
      }
      yield return new WaitForSeconds(0.1f);
    }
    yield return new WaitForSeconds(0f);
  }
  
  
}
