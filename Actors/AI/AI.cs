/*
    AI is the base class for other ai scripts and contains convenience methods.
    Each script represents a state of mind or task.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AI{
  public Actor actor;
  public AIManager manager;
  float pace = 0.01f;
  float sightDistance = 40f;
  float aimMargin = 1f;   // Acceptable deviation in aim.
  float turnSpeed = 0.01f; // Delay between turning
  public bool pursuing;
  public bool moving;
  
  public AI(Actor actor, AIManager manager){
    this.actor = actor;
    this.manager = manager;
    actor.StartCoroutine(Begin());
  }
  
  public virtual IEnumerator Begin(){ yield return null;}
  
  
  public virtual void ReceiveDamage(int damage, Actor damager){}
  
  /* Move directly at target. */
  public IEnumerator Pursue(Actor target){
    if(!target){ yield break; }
    pursuing = true;
    Vector3 pos = actor.body.transform.position;
    pos = new Vector3(pos.x, 0f, pos.z);
    Vector3 dest = new Vector3();
    if(target){ dest = target.body.transform.position; }
    dest = new Vector3(dest.x, 0f, dest.z); 
    while(Vector3.Distance(dest, pos) > 3f && CanSee(target.body) && !manager.paused){
      if(!target){ yield break; }
      Vector3 move = dest - pos;
      actor.AxisMove(move.x, move.z);
      yield return new WaitForSeconds(0.01f);
      pos = actor.body.transform.position;
      pos = new Vector3(pos.x, 0f, pos.z);
      if(target){ dest = target.body.transform.position; }
      dest = new Vector3(dest.x, 0f, dest.z);
    }
    pursuing = false;
    yield break;
  }
  
  /* Moves in straight line to a destination. */
  public IEnumerator MoveTo(Vector3 destination){
    moving = true;
    Vector3 dest = new Vector3(destination.x, 0f, destination.z);
    Vector3 pos = actor.body.transform.position;
    pos = new Vector3(pos.x, 0f, pos.z);
    while(Vector3.Distance(pos, dest) > 2f && !manager.paused){
      Vector3 move = dest - pos;
      actor.AxisMove(move.x, move.z);
      yield return new WaitForSeconds(0.01f);
      pos = actor.body.transform.position;
      pos = new Vector3(pos.x, 0f, pos.z);
    }
    moving = false;
    yield break;
  }
  
  /* Returns a list of visible actors sight.  */
  public List<Actor> ScanForActors(){
    List<Actor> ret = new List<Actor>();
    Vector3 center = actor.body.transform.position;
    Vector3 halfExtents = new Vector3(20f, 20f, 20f);
    Vector3 direction = actor.body.transform.forward;
    Quaternion orientation = actor.body.transform.rotation;
    float distance = sightDistance;
    int layerMask = ~(1 << 8);
    RaycastHit[] found = Physics.BoxCastAll(
      center,
      halfExtents,
      direction,
      orientation,
      distance,
      layerMask,
      QueryTriggerInteraction.Ignore
    );
    for(int i = 0; i < found.Length; i++){
      Actor a = found[i].collider.gameObject.GetComponent<Actor>();
      if(a && a != actor && ret.IndexOf(a) == -1){
        int dist = (int)Vector3.Distance(
          actor.body.transform.position,
          a.body.transform.position
        );
        if(a.head && CanSee(actor.body)){
          bool check = a.stats != null 
            && !a.stats.StatCheck("STEALTH", actor.stats.perception);
          if(check){ ret.Add(a); }
        }
      }
    }
    return ret;
  }
  
  
  /* Aim at the target */
  public IEnumerator AimAt(Actor target){
    if(manager.paused){ yield break; }
    Transform head = actor.head.transform;
    Transform thead = target.head.transform;
    Quaternion desired = Quaternion.LookRotation(head.position - thead.position);
    Quaternion actual = head.rotation;
    float angle = Quaternion.Angle(desired, actual);
    while(angle > aimMargin && !manager.paused && CanSee(target.body)){
      Vector3 actualEulers = actual.eulerAngles;
      Vector3 desiredEulers = desired.eulerAngles;
      float x = actualEulers.x - desiredEulers.y;
      float y = actualEulers.y - desiredEulers.y;
      //if(Mathf.Abs(x) > 180){ x *= -1f; }
      if(Mathf.Abs(y) > 180){ y *= -1f; }
      actor.Turn(new Vector3(x, y, 0f).normalized);
      yield return new WaitForSeconds(turnSpeed);
      if(!target){ yield break; }
      desired = Quaternion.LookRotation(head.position - thead.position);
      actual = head.rotation;
      
    }
    
    yield break;
  }
  
  public void Update(){
    
  }
  
  /* Returns true if one of five raycasts reaches the target's collider */
  public bool CanSee(GameObject target){
    if(!target){ return false; }
    Transform trans = manager.actor.head.transform;
    Vector3[] origins = {
      trans.position,               // Center
      trans.position + trans.up,    // Up
      trans.position - trans.up,    // Down
      trans.position + trans.right, // Right
      trans.position - trans.right   // Left
    };
    for(int i = 0; i < origins.Length; i++){
      if(!target){ return false; }
      Vector3 origin = origins[i];
      Vector3 direction = target.transform.position - actor.head.transform.position;
      RaycastHit hit;
      float maxDistance = sightDistance;
      int layerMask = ~(1 << 8);
      if(Physics.Raycast(
          origin,
          direction,
          out hit,
          maxDistance,
          layerMask,
          QueryTriggerInteraction.Ignore
        )){
        if(hit.collider.gameObject == target){ return true; }
      }
    }
    return false;
  }
}
