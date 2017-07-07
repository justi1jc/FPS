/*
*   An AI for NPC Actors. 
*   An actor must be set up correctly(as defined in Actor.cs) in order to use
*   an AI.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class AI: MonoBehaviour{
  // AI profile
  public const int PASSIVE = 0; // flee when possible
  public const int HOSTILE = 1; // fight when possible
  public const int NEUTRAL = 2; // fight selectively
  
  // active tasks
  bool aiming;
  bool pursuing;
  bool moving;
  
  bool paused;
  Actor host;
  float pace = 0.01f;
  float sightDistance = 40f;
  float aimMargin = 1f;   // Acceptable deviation in aim.
  float turnSpeed = 0.01f; // Delay between turning
  public int profile;     // What sort of AI is this?
  List<Actor> sighted;
  Vector3 lastKnownPosition;
  
  /* Initial setup. */
  public void Begin(Actor _host){
    host = _host;
    paused = false;
    sighted = new List<Actor>();
    StartCoroutine(Think());
  }
  
  /* Cease activity. */
  public void Pause(){
    paused = true;
  }
  
  /* Resume activity. */
  public void Resume(){
    paused = false;
  }
  
  /* Switch for different */
  IEnumerator Think(){
    while(true){
      if(host.ragdoll){ yield return new WaitForSeconds(1f); }
      if(!paused){
        switch(profile){
          case PASSIVE:
          yield return StartCoroutine(ThinkPassive());
          break;
          case HOSTILE:
          yield return StartCoroutine(ThinkHostile());
          break;
          case NEUTRAL:
          break;
        }
      }
      yield return new WaitForSeconds(0f);
    }
  }
  
  /* Decision logic for passive AI */
  IEnumerator ThinkPassive(){
    yield return new WaitForSeconds(100f);
  }
  
  /* Decision logic for hostile AI */
  IEnumerator ThinkHostile(){
    if(paused){ yield break; }
    UpdateSighted();
    for(int i = 0; i < sighted.Count; i++){
      if(sighted[i]){
        lastKnownPosition = sighted[i].body.transform.position;
        yield return StartCoroutine(Fight(sighted[i]));
      }
    }
    yield return new WaitForSeconds(0f);
  }
  
  /* Gather sighted hostiles. */
  void UpdateSighted(){
    Actor actor = ScanForActor();
    sighted = new List<Actor>();
    if(actor){
      sighted.Add(actor);
    }
  }
  
  
  /* Engages enemy in combat. */
  IEnumerator Fight(Actor enemy){
    if(paused){ yield break; }
    if(!CanSee(enemy.body)){ 
      yield return StartCoroutine(SearchFor(enemy));
      if(!CanSee(enemy.body)){ yield break;}
    }
    while(!paused && CanSee(enemy.body) && enemy.Alive()){
      if(!pursuing){ StartCoroutine(Pursue(enemy)); }
      if(!aiming){ yield return StartCoroutine(AimAt(enemy)); }
      host.Use(0);
      yield return new WaitForSeconds(0.1f);
    }
    yield break;
  }
  
  /* Moves to last-known position and does a 360 sweep*/
  IEnumerator SearchFor(Actor target){
    
    yield break;
  }
  
  
  /* Aim at the target */
  IEnumerator AimAt(Actor target){
    if(paused){ yield break; }
    Transform head = host.head.transform;
    Transform thead = target.head.transform;
    Quaternion desired = Quaternion.LookRotation(head.position - thead.position);
    Quaternion actual = head.rotation;
    float angle = Quaternion.Angle(desired, actual);
    while(angle > aimMargin && !paused && CanSee(target.body)){
      Vector3 actualEulers = actual.eulerAngles;
      Vector3 desiredEulers = desired.eulerAngles;
      float x = actualEulers.x - desiredEulers.y;
      float y = actualEulers.y - desiredEulers.y;
      //if(Mathf.Abs(x) > 180){ x *= -1f; }
      if(Mathf.Abs(y) > 180){ y *= -1f; }
      host.Turn(new Vector3(x, y, 0f).normalized);
      yield return new WaitForSeconds(turnSpeed);
      if(!target){ yield break; }
      desired = Quaternion.LookRotation(head.position - thead.position);
      actual = head.rotation;
      
    }
    
    yield break;
  }
  
  
  /* Move directly at target. */
  IEnumerator Pursue(Actor target){
    if(!target){ yield break; }
    pursuing = true;
    Vector3 pos = host.body.transform.position;
    pos = new Vector3(pos.x, 0f, pos.z);
    Vector3 dest = new Vector3();
    if(target){ dest = target.body.transform.position; }
    dest = new Vector3(dest.x, 0f, dest.z); 
    while(Vector3.Distance(dest, pos) > 3f && CanSee(target.body) && !paused){
      if(!target){ yield break; }
      Vector3 move = dest - pos;
      host.AxisMove(move.x, move.z);
      yield return new WaitForSeconds(0.01f);
      pos = host.body.transform.position;
      pos = new Vector3(pos.x, 0f, pos.z);
      if(target){ dest = target.body.transform.position; }
      dest = new Vector3(dest.x, 0f, dest.z);
    }
    pursuing = false;
    yield break;
  }
  
  /* Moves in straight line to a destination. */
  IEnumerator MoveTo(Vector3 destination){
    moving = true;
    Vector3 dest = new Vector3(destination.x, 0f, destination.z);
    Vector3 pos = host.body.transform.position;
    pos = new Vector3(pos.x, 0f, pos.z);
    while(Vector3.Distance(pos, dest) > 2f && !paused){
      Vector3 move = dest - pos;
      host.AxisMove(move.x, move.z);
      yield return new WaitForSeconds(0.01f);
      pos = host.body.transform.position;
      pos = new Vector3(pos.x, 0f, pos.z);
    }
    moving = false;
    yield break;
  }
  
  /* Returns an Actor in sight, or null.  */
  Actor ScanForActor(){
    Vector3 center = host.body.transform.position;
    Vector3 halfExtents = new Vector3(20f, 20f, 20f);
    Vector3 direction = host.body.transform.forward;
    Quaternion orientation = host.body.transform.rotation;
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
      Actor actor = found[i].collider.gameObject.GetComponent<Actor>();
      if(actor && actor != host){
        int dist = (int)Vector3.Distance(
          host.body.transform.position,
          actor.body.transform.position
        );
        if(actor.head && CanSee(actor.body)){
          bool check = actor.stats != null 
            && !actor.stats.StatCheck("STEALTH", host.stats.perception);
          if(check){ return actor; }
        }
      }
    }
    return null;
  }
  
  /* Returns true if one of five raycasts reaches the target's collider */
  bool CanSee(GameObject target){
    if(!target){ return false; }
    Transform trans = host.head.transform;
    Vector3[] origins = {
      trans.position, // Center
      trans.position + trans.up,    // Up
      trans.position - trans.up,    // Down
      trans.position + trans.right, // Right
      trans.position - trans.right   // Left
    };
    for(int i = 0; i < origins.Length; i++){
      if(!target){ return false; }
      Vector3 origin = origins[i];
      Vector3 direction = target.transform.position - host.head.transform.position;
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
