/*
    The purpose of the ActorInputManager is to abstract input-handling out of
    the Actor class and force loose coupling between an Actor and its 
    controller.
*/


using System;
using System.Collections.Generic;
using UnityEngine;

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
  
  public void SetMenuOpen(bool open){
    menuOpen = open;
  }
  
  /* Acts upon action in the context of controlling an open menu. */
  private void HandleMenuAction(string[] action){
    if(
      actor == null || actor.GetMenu() == null || action == null ||
      action.Length == 0
    ){ 
      return;
    }
    MenuManager menu = actor.GetMenu();
    string btn = action[0];
    float x, y;
    
    if(action.Length > 1 && (action[1] == "DOWN" || action[1] == "HELD")){
      if(btn == "K_W" || btn == "K_UP" || btn == "DUP"){ 
        menu.Press(Menu.UP);
      }
      if(btn == "K_S" || btn == "K_DOWN" || btn == "DDOWN"){
        menu.Press(Menu.DOWN);
      }
      if(btn == "K_A" || btn == "K_LEFT" || btn == "DLEFT"){
        menu.Press(Menu.LEFT);
      }
      if(btn == "K_D" || btn == "K_RIGHT" || btn == "DRIGHT"){
        menu.Press(Menu.RIGHT);
      }
    }
    
    if(action.Length > 1 && action[1] == "DOWN"){
      if(btn == "K_ENTER" || btn == "K_E" || btn == "A"){ menu.Press(Menu.A); }
      if(btn == "B" || btn == "K_BACKSPACE" || btn == "K_TAB"){
        menu.Press(Menu.B);
      }
      if(btn == "K_R" || btn == "X"){
        menu.Press(Menu.X);
      }
      if(btn == "START" || btn == "K_ESCAPE"){ menu.Press(Menu.START); }
    }
    
    if(action.Length > 3 && action[0] == "LEFTSTICK"){
      x = float.Parse(action[2],
        System.Globalization.CultureInfo.InvariantCulture);
      y = float.Parse(action[3],
        System.Globalization.CultureInfo.InvariantCulture);
      if(y > 0){ actor.menu.Press(Menu.UP); }
      else if(y < 0){ actor.menu.Press(Menu.DOWN); }
      if(x < 0){ actor.menu.Press(Menu.LEFT); }
      else if(x > 0){ actor.menu.Press(Menu.RIGHT); }
    }
  }
  
  /* Acts upon action in the context of controlling actor. */
  private void HandleActorAction(string[] action){
    if(actor == null || action == null || action.Length == 0){ return; }
    string btn = action[0];
    float x, y;
    
    if(action.Length > 1 && (action[1] == "DOWN" || action[1] == "HELD")){
      x = y = 0f;
      if(btn == "K_W"){ y = 1f; }
      else if(btn == "K_S"){ y = -1f; }
      if(btn == "K_D"){ x = 1f; }
      else if(btn == "K_A"){ x = -1f; }
      actor.StickMove(x, y);
    }
    
    if(action.Length > 1 && action[1] == "DOWN"){
      if(btn == "M0" || btn == "RT"){ actor.Use(0); }
      if(btn == "M1" || btn == "LT"){ actor.Use(1); }
      if(btn == "K_SHIFT" || btn == "LB"){ actor.SetSprinting(true); }
      if(btn == "K_Q" || btn == "RB"){ actor.Drop(); }
      if(btn == "K_F" || btn == "B"){ actor.Use(7); }
      if((btn == "K_ESCAPE" || btn == "START")){ actor.ChangeMenu("OPTIONS"); }
      if((btn == "Y"  || btn == "K_TAB")){ actor.ChangeMenu("INVENTORY"); }
      if(btn == "X"){
        actor.Interact(0);
        actor.Use(2);
      }
      if(btn == "K_SPACEBAR" || btn == "A"){ actor.Jump(); }
      if(btn == "LSTICKCLICK" || btn == "K_CTRL"){ actor.ToggleCrouch(); }
      switch(btn){
        case "K_R": actor.Use(2); break;
        case "K_E": actor.Interact(0); break;
      }
    }
    else if(action.Length > 1 && action[1] == "HELD"){
      if(btn == "M0" || btn == "RT"){ actor.Use(3); }
      if(btn == "M1" || btn == "LT"){ actor.Use(4); }
    }
    else if(action.Length > 1 && action[1] == "UP"){
      if(btn == "M0" || btn == "RT"){ actor.Use(5); }
      if(btn == "M1" || btn == "LT"){ actor.Use(6); }
      if(btn == "K_SHIFT" || btn == "LB"){ actor.SetSprinting(false); }
    }
    
    if(action.Length > 3 && action[1] == "AXIS"){
      x = float.Parse(action[2],
          System.Globalization.CultureInfo.InvariantCulture);
      y = float.Parse(action[3],
        System.Globalization.CultureInfo.InvariantCulture);
      if(action[0] == "LEFTSTICK"){
        actor.StickMove(x, y);
      }
      else if(action[0] == "RIGHTSTICK" || action[0] == "MOUSE"){
        actor.Turn(new Vector3(x, y, 0f));
      }
    }
  }
}
