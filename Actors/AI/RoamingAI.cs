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
  bool wandering = true;
  public RoamingAI(Actor actor, AIManager manager) : base(actor, manager){
    actor.StartCoroutine(Begin());
    
  }
  
  public override IEnumerator Begin(){
    actor.StartCoroutine(Wander());
    yield return actor.StartCoroutine(FindEnemy());
    
    yield break;
  }
  
  public IEnumerator Wander(){
    while(wandering){
      Vector3 dest = FindDest();
      MonoBehaviour.print("Moving to " + dest);
      yield return actor.StartCoroutine(MoveTo(dest));
    }
    yield return new WaitForSeconds(0f);
  }
  
  /* Returns a random position that the AI can walk to in a straight line. */
  Vector3 FindDest(){
    Vector3 dir = new Vector3();
    switch(UnityEngine.Random.Range(0, 4)){
      case 0:
        dir = actor.transform.right;
        break;
      case 1:
        dir = -actor.transform.right;
        break;
      case 2:
        dir = actor.transform.forward;
        break;
      case 3:
        dir = -actor.transform.forward;
        break;
    }
    float dist = Random.Range(5f, 50f);
    
    Vector3 origin = actor.transform.position;
    while(dist > 4.9f){
      if(Physics.Raycast(origin, dir, dist)){
        dist -= 5f;
      }
      else{
        return origin + (dir*dist);
      }
    }
    return origin;
  }

}
