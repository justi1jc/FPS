/*
    SearchAI wanders randomly in search of enemies to fight.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Diagnostics;

public class SearchAI : AI{
  Vector3 dest;
  Stopwatch watch;
  const int TIMEOUT = 5000; 
  public SearchAI(Actor actor, AIManager manager) : base(actor, manager){
    destinationState = AI.States.Search;
    dest = NextDest();
    watch = Stopwatch.StartNew();
  }
  
  
  /**
    * Moves to a random destination, changing that destination if it
    * reached or the walking times out.
    * If enemies are sighted, will equip the actor and fight using
    * RangedAI or MeleeAI.
    */
  public override void Update(){
    if(MoveToward(dest) || watch.ElapsedMilliseconds > TIMEOUT){ 
      dest = NextDest();
      watch = Stopwatch.StartNew(); 
    }
    if(EnemiesFound()){
      if(EquipRangedWeapon()){ Transition(States.Ranged); }
      else{
        EquipMeleeWeapon();
        Transition(States.Melee);
      }
    }
  }
  
  /**
    * Returns a random position to move to.
    * @return {Vector3} - the new destination to move towards.
    */
  private Vector3 NextDest(){
    int range = 5;
    int x = Random.Range(-range, -range);
    int z = Random.Range(-range, range);
    Vector3 pos = actor.transform.position;
    Vector3 delta = new Vector3(x, 0, z);
    return pos + delta;
  }

}

