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
  
  bool paused;
  Actor host;
  float sightDistance = 20f;
  float aimMargin = 1f; // Acceptable deviation in aim.
  float turnSpeed = 0.1f; // Delay between turning
  public int profile; // What sort of AI is this?
  List<Actor> sighted;
  List<Vector3> lastKnownPos;
  
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
  
  IEnumerator Think(){
    while(true){
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
      yield return new WaitForSeconds(1f);
    }
  }
  
  /* Decision logic for passive AI */
  IEnumerator ThinkPassive(){
    yield return new WaitForSeconds(100f);
  }
  
  /* Decision logic for hostile AI */
  IEnumerator ThinkHostile(){
    if(paused){ yield break; }
    UpdateEnemies();
    for(int i = 0; i < sighted.Count; i++){
      if(sighted[i]){
        yield return StartCoroutine(Fight(sighted[i]));
      }
    }
    yield return new WaitForSeconds(0f);
  }
  
  /* Gather sighted hostiles. */
  void UpdateEnemies(){
    Actor enemy = ScanForActor();
    sighted = new List<Actor>();
    lastKnownPos = new List<Vector3>();
    if(enemy){
      sighted.Add(enemy);
      lastKnownPos.Add(enemy.body.transform.position);
    }
  }
  
  
  /* Engages enemy in combat. */
  IEnumerator Fight(Actor enemy){
    if(paused){ yield break;}
    if(!CanSee(enemy.body)){ StartCoroutine(SearchFor(enemy)); yield break; }
    yield return StartCoroutine(AimAt(enemy));
    print("Aimed at " + enemy);
    yield return new WaitForSeconds(0f);
  }
  
  IEnumerator SearchFor(Actor target){
    print("Searching for " + target);
    yield return new WaitForSeconds(0f);
  }
  
  
  /* Aim at the target */
  IEnumerator AimAt(Actor target){
    Vector3 relRot = (target.body.transform.position - host.head.transform.position).normalized;
    Quaternion realRot = Quaternion.LookRotation(relRot, host.head.transform.up);
    Quaternion desiredRot = host.head.transform.rotation;
    float angle = Quaternion.Angle(realRot, desiredRot);
    while(angle > aimMargin){
      Vector3 realEulers = realRot.eulerAngles;
      Vector3 desiredEulers = desiredRot.eulerAngles;
      Vector3 turnRot = new Vector3(
        realEulers.x - desiredEulers.x,
        realEulers.y - desiredEulers.y,
        0f
      );
      
      host.Turn(turnRot.normalized);
      
      yield return new WaitForSeconds(turnSpeed);
      relRot = (target.body.transform.position - host.head.transform.position).normalized;
      realRot = Quaternion.LookRotation(relRot, host.head.transform.up);
      desiredRot = host.head.transform.rotation;
      angle = Quaternion.Angle(realRot, desiredRot);
    }
    yield break;
  }
  
  /* Moves in straight line to a destination. */
  IEnumerator MoveTo(Vector3 destination){
    yield return new WaitForSeconds(0f);
  }
  
  /* Returns an Actor in sight, or null.  */
  Actor ScanForActor(){
    Vector3 center = host.body.transform.position;
    Vector3 halfExtents = new Vector3(10f, 10f, 1f);
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
          bool check = !actor.StealthCheck(host.perception, dist);
          if(check){ return actor; }
        }
      }
    }
    return null;
  }
  
  /* Returns true if one of five raycasts reaches the target's collider */
  bool CanSee(GameObject target){
    Transform trans = host.head.transform;
    Vector3[] origins = {
      trans.position, // Center
      trans.position + trans.up,    // Up
      trans.position - trans.up,    // Down
      trans.position + trans.right, // Right
      trans.position - trans.right   // Left
    };
    for(int i = 0; i < origins.Length; i++){
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
