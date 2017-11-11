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
  private float x, y;
  private bool moved; // True if xDir or yDir are set.
  public ActorInputHandler(Actor actor, string device){
    this.actor = actor;
    this.devMan = new DeviceManager(device);
  }
  
  /* Collects inputs and calls appropriate method to handle them. */
  public void Update(){
    x = y = 0.0f;
    moved = false;
    foreach(InputEvent action in devMan.GetInputs()){ 
      if(menuOpen){ HandleMenuAction(action); }
      else{ HandleActorAction(action); }
    }
    if(moved){ actor.StickMove(x, y); }
  }
  
  public void SetMenuOpen(bool open){
    menuOpen = open;
  }
  
  /* Acts upon action in the context of controlling an open menu. */
  private void HandleMenuAction(InputEvent action){
    if( actor == null || actor.GetMenu() == null || action == null){ return; }
    if(action.IsButton()){ HandleMenuButton(action); }
    else{ HandleMenuAxis(action); }
  }
  
  /* Handles button input in menu. */
  public void HandleMenuButton(InputEvent evt){
    InputEvent.Actions action = evt.action;
    float dt = evt.downTime;
    MenuManager menu = actor.GetMenu();
    
    if(action == InputEvent.Actions.Down || action == InputEvent.Actions.Held){
      switch(evt.button){
        case InputEvent.Buttons.W: menu.Press(Menu.Buttons.Up); break;
        case InputEvent.Buttons.Up: menu.Press(Menu.Buttons.Up); break;
        case InputEvent.Buttons.DUp: menu.Press(Menu.Buttons.Up); break;
        
        case InputEvent.Buttons.S: menu.Press(Menu.Buttons.Down); break;
        case InputEvent.Buttons.Down: menu.Press(Menu.Buttons.Down); break;
        case InputEvent.Buttons.DDown: menu.Press(Menu.Buttons.Down); break;
        
        case InputEvent.Buttons.A: menu.Press(Menu.Buttons.Left); break;
        case InputEvent.Buttons.Left: menu.Press(Menu.Buttons.Left); break;
        case InputEvent.Buttons.DLeft: menu.Press(Menu.Buttons.Left); break;
        
        case InputEvent.Buttons.D: menu.Press(Menu.Buttons.Right); break;
        case InputEvent.Buttons.Right: menu.Press(Menu.Buttons.Right); break;
        case InputEvent.Buttons.DRight: menu.Press(Menu.Buttons.Right); break;
      }
    }
    if(action == InputEvent.Actions.Down){
      switch(evt.button){
        case InputEvent.Buttons.E: menu.Press(Menu.Buttons.A); break;
        case InputEvent.Buttons.Enter: menu.Press(Menu.Buttons.A); break;
        case InputEvent.Buttons.C_A: menu.Press(Menu.Buttons.A); break;
        
        case InputEvent.Buttons.Backspace: menu.Press(Menu.Buttons.B); break;
        case InputEvent.Buttons.Tab: menu.Press(Menu.Buttons.B); break;
        case InputEvent.Buttons.C_B: menu.Press(Menu.Buttons.B); break;
        
        case InputEvent.Buttons.R: menu.Press(Menu.Buttons.X); break;
        case InputEvent.Buttons.C_X: menu.Press(Menu.Buttons.X); break;
        
        case InputEvent.Buttons.Escape: menu.Press(Menu.Buttons.Start); break;
        case InputEvent.Buttons.Start: menu.Press(Menu.Buttons.Start); break;
      }
    }
  }
  
  /* Handles axis input in menu. */
  public void HandleMenuAxis(InputEvent action){
    if(action.axis != InputEvent.Axes.LStick){ return; }
    float x = action.x;
    float y = action.y;
    if(y > 0){ actor.menu.Press(Menu.Buttons.Up); }
    else if( y < 0){ actor.menu.Press(Menu.Buttons.Down); }
    if(x < 0){ actor.menu.Press(Menu.Buttons.Left); }
    else if(x > 0){ actor.menu.Press(Menu.Buttons.Right); }
  }
  
  /* Acts upon action in the context of controlling actor. */
  private void HandleActorAction(InputEvent action){
    if(actor == null || action == null){ return; }
    if(action.IsButton()){ HandleActorButton(action); }
    else{ HandleActorAxis(action); }
    if(x == 0 && y == 0 && actor.walking){
      actor.StickMove(0f, 0f);
    }
  }
  
  /* Handles button input for Actor. */
  public void HandleActorButton(InputEvent evt){
    InputEvent.Actions action = evt.action;
    InputEvent.Buttons btn = evt.button;
    float dt = evt.downTime;
    if(action == InputEvent.Actions.Held || action == InputEvent.Actions.Down){
      switch(btn){
        case InputEvent.Buttons.W:
          y = 1.0f;
          moved = true; 
          break;
        case InputEvent.Buttons.S: 
          y = -1.0f;
          moved = true; 
          break;
        case InputEvent.Buttons.D: 
          x = 1;
          moved = true; 
          break;
        case InputEvent.Buttons.A: 
          x = -1;
          moved = true;
          break;
      }
      
    }
    else if(action == InputEvent.Actions.Up){
      if(
        btn == InputEvent.Buttons.W ||
        btn == InputEvent.Buttons.A || 
        btn == InputEvent.Buttons.S ||
        btn == InputEvent.Buttons.D
      ){ moved = true; }
    }
    
    if(action == InputEvent.Actions.Down){
      if(!actor.arms.Single()){
        switch(btn){
          case InputEvent.Buttons.RT: actor.Use(Item.Inputs.B_Down); return; break;
          case InputEvent.Buttons.LT: actor.Use(Item.Inputs.A_Down); return; break;
        }
      }
      switch(btn){
        case InputEvent.Buttons.M_0: actor.Use(Item.Inputs.A_Down); break;
        case InputEvent.Buttons.RT: actor.Use(Item.Inputs.A_Down); break;
        
        case InputEvent.Buttons.M_1: actor.Use(Item.Inputs.B_Down); break;
        case InputEvent.Buttons.LT: actor.Use(Item.Inputs.B_Down); break;
        
        case InputEvent.Buttons.L_Shift: actor.SetSprinting(true); break;
        case InputEvent.Buttons.LB: actor.SetSprinting(true); break;
        
        case InputEvent.Buttons.Q: actor.Drop(); break;
        case InputEvent.Buttons.RB: actor.Drop(); break;
        
        case InputEvent.Buttons.F: actor.Use(Item.Inputs.D_Down); break;
        case InputEvent.Buttons.C_B: actor.Use(Item.Inputs.D_Down); break;
        
        case InputEvent.Buttons.Escape: actor.ChangeMenu("OPTIONS"); break;
        case InputEvent.Buttons.Start: actor.ChangeMenu("OPTIONS"); break;
        
        case InputEvent.Buttons.Tab: actor.ChangeMenu("INVENTORY"); break;
        case InputEvent.Buttons.C_Y: actor.ChangeMenu("INVENTORY"); break;
        
        case InputEvent.Buttons.R: actor.Use(Item.Inputs.C_Down); break;
        case InputEvent.Buttons.E: actor.Interact(); break;
        case InputEvent.Buttons.C_X: 
          if(actor.actorInReach != null || actor.itemInReach != null){
            actor.Interact();
          }
          else{ actor.Use(Item.Inputs.C_Down); }
          break;
        case InputEvent.Buttons.Space: actor.Jump(); break;
        case InputEvent.Buttons.C_A: actor.Jump(); break;
        
        case InputEvent.Buttons.L_Ctrl: actor.ToggleCrouch(); break;
        case InputEvent.Buttons.LStick: actor.ToggleCrouch(); break;
      }
    }
    else if(action == InputEvent.Actions.Up){
      if(!actor.arms.Single()){
        switch(btn){
          case InputEvent.Buttons.RT: actor.Use(Item.Inputs.B_Up); return; break;
          case InputEvent.Buttons.LT: actor.Use(Item.Inputs.A_Up); return; break;
        }
      }
      switch(btn){
        case InputEvent.Buttons.M_0: actor.Use(Item.Inputs.A_Up); break;
        case InputEvent.Buttons.RT: actor.Use(Item.Inputs.A_Up); break;
        
        case InputEvent.Buttons.M_1: actor.Use(Item.Inputs.B_Up); break;
        case InputEvent.Buttons.LT: actor.Use(Item.Inputs.B_Up); break;
        
        case InputEvent.Buttons.L_Shift: actor.SetSprinting(false); break;
        case InputEvent.Buttons.LB: actor.SetSprinting(false); break;
        
        case InputEvent.Buttons.L_Ctrl: actor.ToggleCrouch(); break;
      }
    }
  }
  
  /* Handles axis input for Actor. */
  public void HandleActorAxis(InputEvent evt){
    if(evt.axis == InputEvent.Axes.Mouse || evt.axis == InputEvent.Axes.LStick){
      actor.Turn(new Vector3(evt.x, evt.y, 0f));
    }
    else if(evt.axis == InputEvent.Axes.LStick){ 
      x = evt.x;
      y = evt.y;
      moved = true;
    }
  }
}
