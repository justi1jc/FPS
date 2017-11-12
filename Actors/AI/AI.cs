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
  
  public Actor actor;
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
  private void Transition(States current, States next){
    if(manager.nextState != States.None){ manager.Transition(current, next); }
    else{ manager.Transition(manager.nextState); }
  }
  
  /**
    * Performs one iteration of this AI's work.
    * This method should be called by AIManager.cs
    */
  public virtual void Update(){ MonoBehaviour.print("Update"); }
  
  /**
    * Handle an event. Should be called from AIManager.
    * @param {AIEvent} evt - The event that has occurred.
    */
  public virtual void HandleEvent(AIEvent evt){}
}
