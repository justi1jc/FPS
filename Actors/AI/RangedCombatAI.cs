/*
    Hostile AI searches for enemies if manager.target is null. Once a target
    is selected, it will equip a weapon or ability and change control to 
    a corresponding 
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RangedCombatAI : AI{
  bool firing = true;
  bool positioning = true;
  Ranged weapon;
  Actor combatant;
  public RangedCombatAI(Actor actor, AIManager manager) : base(actor, manager){}
  
  public override IEnumerator Begin(){
    yield return new WaitForSeconds(0f);
    weapon = (Ranged)actor.arms.Peek(EquipSlot.RIGHT);
    if(weapon == null){
      manager.Change("HOSTILE");
      yield break;
    }
    if(manager.target != null){ combatant = manager.target.GetComponent<Actor>(); }
    if(combatant != null && combatant.Alive()){
      actor.StartCoroutine(FireAt());
      actor.StartCoroutine(Positioning());
    }
    while(combatant != null && combatant.Alive()){
      yield return new WaitForSeconds(1f);
    }
    firing = positioning = false;
    actor.StopCoroutine("Pursue");
    actor.StopCoroutine("AimAt");
    actor.StopCoroutine("MoveTo");
    if(actor.defaultAI != ""){ manager.Change(actor.defaultAI); }
    else{ manager.Change("IDLE"); }
  }
  
  
  public IEnumerator Positioning(){
    while(combatant != null && positioning){
      Vector3 pos = actor.transform.position;
      Vector3 cpos = combatant.transform.position;
      float dist = Vector3.Distance(pos, cpos);
      if(dist > 10f){
        Vector3 dir = (cpos - pos).normalized;
        dir *= (dist - 10f);
        yield return actor.StartCoroutine( MoveTo(pos + dir));
      }
      yield return new WaitForSeconds(0.1f);
    }
    yield return null;
  }
  
  /* Aims and fires at the combatant */
  public IEnumerator FireAt(){
    firing = true;
    bool reloaded = false;
    while(weapon != null && firing && combatant != null){
      if(weapon.ammo > 0){ reloaded = false; }
      yield return actor.StartCoroutine(AimAt(combatant));
      actor.Use(0);
      yield return new WaitForSeconds(0.1f);
      if(!reloaded && weapon.ammo == 0){
        actor.Use(2);
        yield return new WaitForSeconds(1f);
        reloaded = true;
      }
      else if(reloaded && weapon.ammo == 0){
        firing = false;
        manager.Change("HOSTILE");
      }
      else{ reloaded = false; }
    }
    yield return new WaitForSeconds(0f);
  }
  
}
