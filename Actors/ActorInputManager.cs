/*
    The purpose of the ActorInputManager is to abstract input-handling out of
    the Actor class and force loose coupling between an Actor and its 
    controller.
*/


using System;
using System.Collections.Generic;

public class ActorInputHandler{
  private Actor actor;
  private DeviceManager devMan;
  private bool menuOpen = false;
  
  public ActorInputHandler(Actor actor, string device){
    this.actor = actor;
    this.devMan = new DeviceManager(device);
  }
  
  /* Collects inputs and calls appropriate method to handle them. */
  public void Update(){
    foreach(string[] action in devMan.GetInputs()){ 
      if(menuOpen){ HandleMenuAction(action); }
      else{ HandleActorAction(action); } 
    }
  }
  
  /* Acts upon action in context of controlling an open menu. */
  private void HandleMenuAction(string[] action){}
  
  /* Acts upon action in context of controlling actor. */
  private void HandleActorAction(string[] action){}
}
