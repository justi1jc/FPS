/*
    The AIManager is a wrapper for the ai scripts, allowing each ai script
    to be swapped out automatically with the transition of each state. 
*/


using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Diagnostics;


public class AIManager{
  public string currentBehaviour;
  public bool paused = true;
  public int updateDelay = 1000; // Milisecond update delay.
  private AI ai;
  public Stopwatch watch;
  public Actor actor;
  public List<GameObject> enemies, objectives;
  
  public AI.States currentState = AI.States.None; // Active state
  public AI.States nextState = AI.States.None; // Override default transitions.
  
  /**
    * Default constructor.
    * @param {Actor} actor - The actor to control.
    */
  public AIManager(Actor actor){
    this.actor = actor;
    this.enemies = new List<GameObject>();
    this.objectives = new List<GameObject>();
    this.watch = Stopwatch.StartNew();
  }
  
  /**
    * Should pass on an event to the active AI.
    * @param {AIEvent} evt - the event to pass to the ai.
    */
  public void HandleEvent(AIEvent evt){
    if(ai != null){ ai.HandleEvent(evt); }
  }
  
  /**
    * Transitions from this state to a new one.
    * @param {States} state - state to transition to.
    * @param {States} next - new value for nextState
    */
  public void Transition(AI.States current, AI.States next = AI.States.None){
    currentState = current;
    nextState = next;
    switch(current){
      case AI.States.None: this.ai = new AI(actor, this); break;
      case AI.States.Sentry: break;
      case AI.States.Search: break;
      case AI.States.Melee: break;
      case AI.States.Ranged: break;
      case AI.States.Guard: break;
      case AI.States.Advance: break;
    }
  }
  
  /**
    * Updates if not paused and ai is available.
    */
  public void Tick(){
    if(!paused && ai != null){ ai.Update(); }
  }
  
  /**
    * Pauses the manager, preventing it from updating on each Tick().
    */
  public void Pause(){ 
    paused = true; 
  }
  
  /**
    * Resumes the updating on each Tick().
    */
  public void Start(AI.States state = AI.States.None){
    MonoBehaviour.print("Starting");
    Transition(state);
    paused = false;
  }
  
}
