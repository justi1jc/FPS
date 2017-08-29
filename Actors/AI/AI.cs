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
  float turnSpeed = 0.0005f; // Delay between turning
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
  public IEnumerator Pursue(Actor target, float dist = 3f){
    if(!target){ yield break; }
    pursuing = true;
    Vector3 pos = actor.body.transform.position;
    pos = new Vector3(pos.x, 0f, pos.z);
    Vector3 dest = new Vector3();
    if(target){ dest = target.body.transform.position; }
    dest = new Vector3(dest.x, 0f, dest.z);
    manager.actor.SetAnimBool("walking", true);
    while(target != null && Vector3.Distance(dest, pos) > dist && CanSee(target.body) && !manager.paused){
      Vector3 move = dest - pos;
      actor.AxisMove(move.x, move.z);
      yield return new WaitForSeconds(0.01f);
      pos = actor.body.transform.position;
      pos = new Vector3(pos.x, 0f, pos.z);
      if(target){ dest = target.body.transform.position; }
      dest = new Vector3(dest.x, 0f, dest.z);
    }
    manager.actor.SetAnimBool("walking", false);
    pursuing = false;
    yield break;
  }
  
  /* Moves in straight line to a destination. */
  public IEnumerator MoveTo(Vector3 destination){
    moving = true;
    Vector3 dest = new Vector3(destination.x, 0f, destination.z);
    Vector3 pos = actor.body.transform.position;
    pos = new Vector3(pos.x, 0f, pos.z);
    manager.actor.SetAnimBool("walking", true);
    while(Vector3.Distance(pos, dest) > 2f && !manager.paused){
      Vector3 move = dest - pos;
      actor.AxisMove(move.x, move.z);
      yield return new WaitForSeconds(0.01f);
      pos = actor.body.transform.position;
      pos = new Vector3(pos.x, 0f, pos.z);
    }
    manager.actor.SetAnimBool("walking", false);
    moving = false;
    yield break;
  }
  
  /* Returns a list of visible actors sight.  */
  public List<Actor> ScanForActors(){
    List<Actor> ret = new List<Actor>();
    Vector3 center = actor.body.transform.position;
    Vector3 halfExtents = new Vector3(100f, 20f, 100f);
    Vector3 direction = actor.body.transform.forward;
    Quaternion orientation = actor.body.transform.rotation;
    float distance = 0.1f;
    int layerMask = LayerMask.GetMask("Person");
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
        if(a){
          bool check = a.stats != null 
            && !a.stats.StatCheck("STEALTH", actor.stats.perception);
          if(check){ ret.Add(a); }
          else{ MonoBehaviour.print("Check failed"); }
        }
      }
    }
    return ret;
  }
  
  
  /* Aim at the target */
  public IEnumerator AimAt(Actor target, float aimMargin = 1f){
    if(manager.paused){ yield break; }
    Transform head = actor.head.transform;
    Transform thead = target.head.transform;
    Quaternion desired = Quaternion.LookRotation(thead.position - head.position);
    Vector3 hrot = head.rotation.eulerAngles;
    Quaternion actual = Quaternion.Euler(hrot.x, hrot.y, 0f);
    float angle = Quaternion.Angle(desired, actual);
    while(angle > aimMargin && !manager.paused){
      Vector3 actualEulers = actual.eulerAngles;
      Vector3 desiredEulers = desired.eulerAngles;
      float x = desiredEulers.x - actualEulers.x;
      float y = desiredEulers.y - actualEulers.y;
      if(Mathf.Abs(x) > 180){ x *= -1f; }
      if(Mathf.Abs(y) > 180){ y *= -1f; }
      if(x < 0 && actor.headRoty < -59f ||
         x > 0 && actor.headRoty > 59f){
         //x = 0f;
      }
      actor.Turn(new Vector3(x, y, 0f).normalized);
      yield return new WaitForSeconds(turnSpeed);
      if(!target){ yield break; }
      desired = Quaternion.LookRotation(thead.position - head.position);
      hrot = head.rotation.eulerAngles;
      actual = Quaternion.Euler(hrot.x, hrot.y, 0f);
      angle = Quaternion.Angle(desired, actual);
    }
    yield break;
  }
  
  /* Returns true if one of five raycasts reaches the target's collider */
  public bool CanSee(GameObject target, Vector3 pos = new Vector3()){
    if(!target){ return false; }
    Transform trans = manager.actor.head.transform;
    if(pos == new Vector3()){
      pos = manager.actor.head.transform.position;
    }
    Vector3[] origins = {
      pos,               // Center
      pos + trans.up,    // Up
      pos - trans.up,    // Down
      pos + trans.right, // Right
      pos - trans.right  // Left
    };
    for(int i = 0; i < origins.Length; i++){
      if(!target){ return false; }
      Vector3 origin = origins[i];
      Vector3 direction = target.transform.position - actor.head.transform.position;
      RaycastHit hit;
      float maxDistance = sightDistance;
      int layerMask = LayerMask.GetMask("Person");
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
