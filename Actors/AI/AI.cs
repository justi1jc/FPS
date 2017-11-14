/*
    AI is the base class for other ai scripts and contains convenience methods.
    Each AI script represents a state in a state machine.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AI{
  public enum States{
    None, // No behavior
    Sentry, // Stands and scans fo enemies to fight.
    Search, // Actively seeks out enemies to fight.
    Melee, // Fight enemies simply with melee weapon.
    Ranged, // Fight enemies simply with ranged weapon.
    Guard, // Fight enemies whilst keeping priximity to first objective.
    Advance, // Move to desired location, fighting along the way.
    Retreat // Distance self from enemies.
  };
  public States destinationState = States.None; // Redirects the next state's transition.
  public const float visualRange = 50f;
  public Actor actor;
  public Actor target; // The actor being focused on.
  public AIManager manager;
  
  public AI(Actor actor, AIManager manager){
    this.actor = actor;
    this.manager = manager;
  }
  
  /**
    * Transitions states, allowing AIManager.nextState to override default
    * destination.
    * @param {States} current - default destination.
    * @param {States} next - value for AIManager.nextState.
    */
  protected void Transition(States current){
    if(manager.nextState == States.None){ 
      manager.Transition(current, destinationState); 
    }
    else{ manager.Transition(manager.nextState); }
  }
  
  /**
    * Performs one iteration of this AI's work.
    * This method should be called by AIManager.cs
    */
  public virtual void Update(){ MonoBehaviour.print("Base AI"); }
  
  /**
    * Handle an event. Should be called from AIManager.
    * @param {AIEvent} evt - The event that has occurred.
    */
  public virtual void HandleEvent(AIEvent evt){}
  
  /**
    * Returns true if enemies have been found. If not, it scans for enemies.
    */
  public bool EnemiesFound(){
    if(manager.enemies.Count > 0){ return true; }
    List<Actor> sighted = ScanForActors();
    List<GameObject> ret = new List<GameObject>();
    foreach(Actor a in sighted){
      if(actor.stats.Enemy(a)){ ret.Add(a.gameObject); }
    }
    if(ret.Count > 0){ manager.enemies = ret; }
    return false;
  }
  
  /**
    * Returns true if target is found, or can be found from the list of enemies.
    */
  public bool TargetFound(){
    try{
      if(target != null && target.Alive()){ return true; }
      manager.PruneEnemies();
      if(manager.enemies.Count == 0){ return false; }
      float minDist = 10000000f;
      Actor minActor = null;
      Vector3 aPos = actor.transform.position;
      foreach(GameObject go in manager.enemies){
        if(go != null){
          Actor a = go.GetComponent<Actor>();
          if(a != null){
            float dist = Vector3.Distance(aPos, go.transform.position);
            if(dist < minDist){
              minDist = dist;
              minActor = a;
            }
          }
        }
      }
      if(minActor != null){
        target = minActor;
        return true;
      }
    }catch(System.Exception e){}
    return false;
  }
  
  /**
    * Returns a list of all actors in sight.
    */
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
        if(a != null){
          StatHandler.Stats stealth = StatHandler.Stats.Stealth;
          int per = actor.stats.perception;
          bool check = a.stats != null && !a.stats.StatCheck(stealth, per);
          if(check){ ret.Add(a); }
          else{ MonoBehaviour.print("Check failed"); }
        }
      }
    }
    return ret;
  }
  
  /**
    * Checks if actor is already wielding a ranged weapon.
    * Attempts to equip actor with ranged weapon if not. 
    * Returns true if actor winds up equipped with a ranged weapon.
    */
  public bool EquipRangedWeapon(){
    if(actor.arms.WieldingRanged()){ return true; }
    for(int i = 0; i < actor.inventory.slots; i++){
      Data dat = actor.inventory.Peek(i);
      if(dat != null && (Item.Types)dat.itemType == Item.Types.Ranged){
        actor.Equip(i);
        return true;
      }
    }
    return false;
  }
  
  /**
    * Equips actor with a melee weapon, or just unarmed ability.
    * Returns true upon equipping a melee weapon.
    */
  public bool EquipMeleeWeapon(){
    if(actor.arms.WieldingMelee()){ return true; }
    for(int i = 0; i < actor.inventory.slots; i++){
      Data dat = actor.inventory.Peek(i);
      if(dat != null && (Item.Types)dat.itemType == Item.Types.Melee){
        actor.Equip(i);
        return true;
      }
    }
    actor.arms.EquipAbility(Item.GetItem("Abilities/Unarmed"));
    return false;
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
      float maxDistance = visualRange;
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
  
  /**
    * Moves towards a target
    */
  public bool MoveToward(Vector3 destination, float dist = 3f){
    Vector3 dest = new Vector3(destination.x, 0f, destination.z);
    Vector3 pos = actor.body.transform.position;
    pos = new Vector3(pos.x, 0f, pos.z);
    if(Vector3.Distance(pos, dest) <= dist){
      if(actor.walking){ 
        actor.walking = false;
        actor.SetAnimBool("walking", false);
      }
      return true;
    }
    if(!actor.walking){
      actor.walking = true;
      actor.SetAnimBool("walking", true);
    }
    Vector3 move = dest - pos;
    actor.AxisMove(move.x, move.z);
    return false;
  }
  
  
  
  /**
    * Returns true if currently aiming at specified transform wthin margin.
    */
  public bool AimAt(Transform trans, float aimMargin = 1f){
    Transform head = actor.head.transform;
    Quaternion desired = Quaternion.LookRotation(trans.position - head.position);
    Vector3 hrot = head.rotation.eulerAngles;
    Quaternion actual = Quaternion.Euler(hrot.x, hrot.y, 0f);
    float angle = Quaternion.Angle(desired, actual);
    Vector3 actualEulers = actual.eulerAngles;
    Vector3 desiredEulers = desired.eulerAngles;
    float x = desiredEulers.x - actualEulers.x;
    float y = desiredEulers.y - actualEulers.y;
    if(Mathf.Abs(x) > 180){ x *= -1f; }
    if(Mathf.Abs(y) > 180){ y *= -1f; }
    if(x < 0 && actor.headRoty < -59f ||
       x > 0 && actor.headRoty > 59f){
    }
    Vector3 turn = new Vector3(x, y, 0f).normalized;
    float turnRate = 3f;
    turn *= (turnRate < angle ? turnRate : angle);
    actor.Turn(turn);
    if(angle <= aimMargin){ return true; }
    return false;
  } 
}
