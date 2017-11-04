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
  public void HandleMenuButton(InputEvent action){
    int btn = action.button;
    int pt = action.pressType;
    float dt = action.downTime;
    MenuManager menu = actor.GetMenu();
    
    if(pt == InputEvent.DOWN || pt == InputEvent.HELD){
      switch(btn){
        case InputEvent.K_W: menu.Press(Menu.UP); break;
        case InputEvent.K_UP: menu.Press(Menu.UP); break;
        case InputEvent.X360_DUP: menu.Press(Menu.UP); break;
        
        case InputEvent.K_S: menu.Press(Menu.DOWN); break;
        case InputEvent.K_DOWN: menu.Press(Menu.DOWN); break;
        case InputEvent.X360_DDOWN: menu.Press(Menu.DOWN); break;
        
        case InputEvent.K_A: menu.Press(Menu.LEFT); break;
        case InputEvent.K_LEFT: menu.Press(Menu.LEFT); break;
        case InputEvent.X360_DLEFT: menu.Press(Menu.LEFT); break;
        
        case InputEvent.K_D: menu.Press(Menu.RIGHT); break;
        case InputEvent.K_RIGHT: menu.Press(Menu.RIGHT); break;
        case InputEvent.X360_DRIGHT: menu.Press(Menu.RIGHT); break;
      }
    }
    if(pt == InputEvent.DOWN){
      switch(btn){
        case InputEvent.K_E: menu.Press(Menu.A); break;
        case InputEvent.K_ENTER: menu.Press(Menu.A); break;
        case InputEvent.X360_A: menu.Press(Menu.A); break;
        
        case InputEvent.K_BACKSPACE: menu.Press(Menu.B); break;
        case InputEvent.K_TAB: menu.Press(Menu.B); break;
        case InputEvent.X360_B: menu.Press(Menu.B); break;
        
        case InputEvent.K_R: menu.Press(Menu.X); break;
        case InputEvent.X360_X: menu.Press(Menu.X); break;
        
        case InputEvent.K_ESC: menu.Press(Menu.START); break;
        case InputEvent.X360_START: menu.Press(Menu.START); break;
      }
    }
  }
  
  /* Handles axis input in menu. */
  public void HandleMenuAxis(InputEvent action){
    if(action.axis != InputEvent.X360_LEFTSTICK){ return; }
    float x = action.x;
    float y = action.y;
    if(y > 0){ actor.menu.Press(Menu.UP); }
    else if( y < 0){ actor.menu.Press(Menu.DOWN); }
    if(x < 0){ actor.menu.Press(Menu.LEFT); }
    else if(x > 0){ actor.menu.Press(Menu.RIGHT); }
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
  public void HandleActorButton(InputEvent action){
    int btn = action.button;
    int pt = action.pressType;
    float dt = action.downTime;
    if(pt == InputEvent.HELD || pt == InputEvent.DOWN){
      switch(btn){
        case InputEvent.K_W:
          y = 1.0f;
          moved = true; 
          break;
        case InputEvent.K_S: 
          y = -1.0f;
          moved = true; 
          break;
        case InputEvent.K_D: 
          x = 1;
          moved = true; 
          break;
        case InputEvent.K_A: 
          x = -1;
          moved = true;
          break;
      }
      
    }
    else if(pt == InputEvent.UP){
      if(
        btn == InputEvent.K_W ||
        btn == InputEvent.K_A || 
        btn == InputEvent.K_S ||
        btn == InputEvent.K_D
      ){ moved = true; }
    }
    
    if(pt == InputEvent.DOWN){
      if(!actor.arms.Single()){
        switch(btn){
          case InputEvent.X360_RT: actor.Use(Item.B_DOWN); return; break;
          case InputEvent.X360_LT: actor.Use(Item.A_DOWN); return; break;
        }
      }
      switch(btn){
        case InputEvent.MOUSE_0: actor.Use(Item.A_DOWN); break;
        case InputEvent.X360_RT: actor.Use(Item.A_DOWN); break;
        
        case InputEvent.MOUSE_1: actor.Use(Item.B_DOWN); break;
        case InputEvent.X360_LT: actor.Use(Item.B_DOWN); break;
        
        case InputEvent.K_L_SHIFT: actor.SetSprinting(true); break;
        case InputEvent.X360_LB: actor.SetSprinting(true); break;
        
        case InputEvent.K_Q: actor.Drop(); break;
        case InputEvent.X360_RB: actor.Drop(); break;
        
        case InputEvent.K_F: actor.Use(Item.D_DOWN); break;
        case InputEvent.X360_B: actor.Use(Item.D_DOWN); break;
        
        case InputEvent.K_ESC: actor.ChangeMenu("OPTIONS"); break;
        case InputEvent.X360_START: actor.ChangeMenu("OPTIONS"); break;
        
        case InputEvent.K_TAB: actor.ChangeMenu("INVENTORY"); break;
        case InputEvent.X360_Y: actor.ChangeMenu("INVENTORY"); break;
        
        case InputEvent.K_R: actor.Use(Item.C_DOWN); break;
        case InputEvent.K_E: actor.Interact(); break;
        case InputEvent.X360_X: 
          if(actor.actorInReach != null || actor.itemInReach != null){
            actor.Interact();
          }
          else{ actor.Use(Item.C_DOWN); }
          break;
        case InputEvent.K_SPACE: actor.Jump(); break;
        case InputEvent.X360_A: actor.Jump(); break;
        
        case InputEvent.K_L_CTRL: actor.ToggleCrouch(); break;
        case InputEvent.X360_LSTICKCLICK: actor.ToggleCrouch(); break;
      }
    }
    else if(pt == InputEvent.HELD){
      switch(btn){
      }
    }
    else if(pt == InputEvent.UP){
      if(!actor.arms.Single()){
        switch(btn){
          case InputEvent.X360_RT: actor.Use(Item.B_UP); return; break;
          case InputEvent.X360_LT: actor.Use(Item.A_UP); return; break;
        }
      }
      switch(btn){
        case InputEvent.MOUSE_0: actor.Use(Item.A_UP); break;
        case InputEvent.X360_RT: actor.Use(Item.A_UP); break;
        
        case InputEvent.MOUSE_1: actor.Use(Item.B_UP); break;
        case InputEvent.X360_LT: actor.Use(Item.B_UP); break;
        
        case InputEvent.K_L_SHIFT: actor.SetSprinting(false); break;
        case InputEvent.X360_LB: actor.SetSprinting(false); break;
        
        case InputEvent.K_L_CTRL: actor.ToggleCrouch(); break;
      }
    }
  }
  
  /* Handles axis input for Actor. */
  public void HandleActorAxis(InputEvent action){
    int axis = action.axis;
    float xDir = action.x;
    float yDir = action.y;
    
    if(axis == InputEvent.MOUSE || axis == InputEvent.X360_RIGHTSTICK){
      actor.Turn(new Vector3(xDir, yDir, 0f));
    }
    else if(axis == InputEvent.X360_LEFTSTICK){ 
      x = xDir;
      y = yDir;
      moved = true;
    }
  }
}
