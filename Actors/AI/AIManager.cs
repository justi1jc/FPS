/*
    The AIManager is a wrapper for the ai scripts. 
*/


using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


[System.Serializable]
public class AIManager{
  public bool paused = false;
  AI ai;
  public Actor actor;
  [System.NonSerialized]public GameObject target;
  [System.NonSerialized]public List<Actor> sighted;
  
  public AIManager(Actor actor, string initial = "IDLE"){
    this.actor = actor;
    sighted = new List<Actor>();
    Change(initial);
    MonoBehaviour.print("constructor");
  }
  
  /* Informs active script of receiving damage from an Actor */
  public void ReceiveDamage(int damage, Actor damager){
    if(damage > 0 && ai != null){ ai.ReceiveDamage(damage, damager); }
  }
  
  public void Pause(){ paused = true; }
  public void Resume(){ paused = false; }
  
  /* Swaps the active ai script */
  public void Change(string behaviour){
    MonoBehaviour.print("Changing to" + behaviour);
    switch(behaviour.ToUpper()){
      case "PASSIVE": // Inert AI
        ai = new AI(actor, this);
        break;
      case "IDLE": // Stands around until attacked.
        ai = (AI)new IdleAI(actor, this);
        break;
      case "HoSTILE":
        ai = (AI)new HostileAI(actor, this);
        break;
      case "MELEECOMBAT":
        break;
      case "RANGEDCOMBAT":
        break;
    }
  }
}
