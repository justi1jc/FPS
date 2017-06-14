/*
    Base class for the menus and HUD found in the game.
*/﻿

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Menu{
  // Button constants
  public const int UP    = 0;
  public const int DOWN  = 1;
  public const int LEFT  = 2;
  public const int RIGHT = 3;
  public const int A     = 4;
  public const int B     = 5;
  public const int X     = 6;
  public const int Y     = 7;
  public const int RT    = 8;
  public const int LT    = 9;
  
  public MenuManager manager;
  public List<string> notifications = new List<string>(); // Notifications that must be displayed.
  public bool split; // True if screen is split.
  public bool right; // True if this screen is on the right side of the sceen.
  public float notificationTimer = 6f; // Duration of each notification's display.
  public int px, py; // Primary focus (ie which table or is selected.)
  public int pxMax, pyMax, pxMin, pyMin; // Primary focus boundaries.
  public int sx, sy; // secondary focus (ie which item in a table is selected.)
  public int sxMax, syMax, sxMin, syMin; // Secondary focus boundaries.
  public Vector2 scrollPosition = Vector2.zero;  // Primary scroll position
  public Vector3 scrollPositionB = Vector2.zero; // Secondary scroll position
  
  
  public Menu(MenuManager manager){
    this.manager = manager;
    this.split = this.manager.split;
    this.right = this.manager.right;
  }
  
  /* Queue a notification. */
  public void Notify(string message){
    notifications.Add(message);
  }

  /* Height of the screen. */
  public int Height(){
    return Screen.height;
  }

  /* Width of the screen. */
  public int Width(){
    int x = Screen.width;
    if(split){ x /= 2; }
    return x;
  }
  
  /* Width Offset for splitscreen.  */
  public int XOffset(){
    if(right){ return Width(); }
    return 0;
  }
  
  /* Convenience method to render box. */
  public void Box(string text, int posx, int posy, int scalex, int scaley){
    GUI.color = Color.green;
    GUI.Box(new Rect(posx, posy, scalex, scaley), text);
  }
  
  /* Convenience method to render button and return if it's been clicked. */
  public bool Button(
    string text,
    int posx,
    int posy,
    int scalex,
    int scaley,
    int x = -10000,
    int y = -10000
  ){
    GUI.color = Color.green; 
    if( (x == -10000 || sx == x ) && (y == -10000 || sy == y) ){
      GUI.color = Color.yellow;
    }
    bool click = GUI.Button(new Rect(posx, posy, scalex, scaley), text);
    if( (x == -10000 || sx == x ) && (y == -10000 || sy == y) ){
      GUI.color = Color.green;
    }
    return click;
  }
  
  /* Convenience method for TextField. */
  public string TextField(string text, int posx, int posy, int scalex, int scaley){
    return  GUI.TextField(new Rect(posx, posy, scalex, scaley), text, 25);
  }

  /* Menu render logic goes here. */
  public virtual void Render(){}
  
  /* Renders all active notifications. */
  public void RenderNotifications(){
    if(notifications.Count == 0){ return; }
    notificationTimer -= Time.deltaTime;
    if(notificationTimer < 0f){
      notifications.Remove(notifications[0]);
      notificationTimer = 6f;
      return;
    }
    string str = notifications[0];
    int x = Width()/2;
    int y = Height()/6;
    Box(str, XOffset()+x, y, x, y);
  }
  
  /* Logic for focus movement goes here */
  public virtual void UpdateFocus(){}
  
  /* Keeps secondary focus within bounds/. */
  public void SecondaryBounds(){
    if(sy > syMax){ sy = syMax; }
    if(sy < syMin){ sy = syMin; }
    if(sx > sxMax){ sx = sxMax; }
    if(sx < sxMin){ sx = sxMin; }
  }
  
  /* Handles directional input and defers other inputs to Input() */
  public void Press(int button){
    switch(button){
      case UP:
        sy--;
        UpdateFocus();
        break;
      case DOWN:
        sy++;
        UpdateFocus();
        break;
      case RIGHT:
        sx++;
        UpdateFocus();
        break;
      case LEFT:
        sx--;
        UpdateFocus();
        break;
    }
    if(button <= LT && button >= A){ Input(button); }
  }
  
  /* Performing actions according to focus goes here. */
  public virtual void Input(int button){}
  
  /* Convenience method for default exit conditions. */
  public void DefaultExit(int button){
    if(button == B || button == Y){
      manager.Change("HUD");
      manager.actor.SetMenuOpen(false);
    }
  }
  
  
}
