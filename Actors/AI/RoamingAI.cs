/*
  Roaming AI will move about the map randomly, waiting for an enemy
  to come into sight. Once an enemy is sighted, the roaming AI will
  transition to the HostileAI.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RoamingAI : AI{
  
  public RoamingAI(Actor actor, AIManager manager) : base(actor, manager){
    actor.StartCoroutine(FindEnemy());
  }

  public IEnumerator FindEnemy(){
    yield return new WaitForSeconds(0f);;
    
  }
  
  public void ConstructMap(){}

}
