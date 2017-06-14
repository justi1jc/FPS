/*
    The MenuHandler acts as an interface that manages the active menu and
    changes it according to the need. This class is cognizent of all menu
    subclasses, and must be updated when adding new menus.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MenuManager: MonoBehaviour{
  public Actor a; // Actor connected to this manager.
  public Menu active; // Active menu
  public bool split; // True if screen is split.
  public bool right; // True if this screen is on the right side of the sceen.
  
  
  /* Send input to menu.. */
  public void Press(int button){ menu.press(button); }
  
  /* Called once per frame. */
  void OnGUI(){
    active.RenderNotifications();
    active.Render();
  }
  
  /* Send notification to menu. */
  public void Notify(string message){ menu.Notify(message); }
  
  /* Change the active menu */
  public void Change(string selection){
    switch(selection.ToUpper()){
      case "NONE":
        active = new Menu();
        break;
      case "HUD":
        break;
      case "MAIN":
        break;
      case "INVENTORY":
        break;
      case "OPTIONS":
        break;
      case "SPEECH":
        break;
      case "TRADE":
        break;
      case "QUEST":
        break;
      case "ABILITY":
        break;
      case "STATS":
        break;
      case "LOOT":
        break;
      case "LOAD":
        break;
    }
  
  }
}
