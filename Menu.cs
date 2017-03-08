/*
*   Menu controller for Players. This should be attached to an Actor's head.
*   
*
*
*/


﻿using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour{

  // Menu constants
  public const int NONE      =-1; // Do nothing
  public const int HUD       = 0; // just HUD
  public const int MAIN      = 1; // main menu
  public const int INVENTORY = 2; // inventory/abilities 
  public const int OPTIONS   = 3; // pause menu
  public const int SPEECH    = 4; // speech menu
  public const int TRADE     = 5; // trading menu
  public const int QUEST     = 6; // quest menu
  
  // Button constants
  public const int UP    = 0;
  public const int DOWN  = 1;
  public const int LEFT  = 2;
  public const int RIGHT = 3;
  public const int A     = 4;
  public const int B     = 5;
  public const int X     = 6;
  public const int Y     = 7;
  
  
  public Actor actor;
  public int activeMenu = NONE;
  public bool split;
  public bool right;
  int px, py; // Primary focus (ie which table or is selected.)
  int pxMax, pyMax, pxMin, pyMin; // Primary focus boundaries.
  int sx, sy; // secondary focus (ie which item in a table is selected.)
  int sxMax, syMax, syMan, syMin; // Secondary focus boundaries.  

  /* Receives button press from Actor. */
  public void Press( int button){
    switch(button){
      case UP:
        sy++;
        UpdateFocus();
        break;
      case DOWN:
        sy--;
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
    if(button <= Y && button >= A){ MenuInput(button); }
  }

  public void Change(int menu){
    if(!actor){ activeMenu = NONE; }
    if(menu <= QUEST && menu >= NONE){
      activeMenu = menu;
      px = py = sx = sy = 0;
      UpdateFocus();
      if(menu == HUD && actor){ actor.SetMenuOpen(false); }
    }
  }
  
  void OnGUI(){
    switch(activeMenu){
      case HUD:
        RenderHUD();
        break;
      case MAIN:
        RenderMain();
        break;
      case INVENTORY:
        RenderInventory();
        break;
      case OPTIONS:
        RenderOptions();
        break;
      case SPEECH:
        RenderSpeech();
        break;
      case TRADE:
        RenderTrade();
        break;
      case QUEST:
        RenderQuest();
        break;
    }
  }
  int Height(){
    return Screen.height;
  }
  int Width(){
    int x = Screen.width;
    if(split){ x /= 2; }
    return x;
  }
  /* Width Offset for splitscreen.  */
  int XOffset(){
    if(right){ return Width(); }
    return 0;
  }
  
  void RenderHUD(){
    if(!actor){ return; } // The HUD needs actor info to display.
    
    //Display Condition bars
    int cbsx = 3;  // condition bar width scale
    int cbsy = 10; // condition bar height scale
    GUI.Box( 
      new Rect(XOffset(), (9 * Height()/cbsy), Width()/cbsx, Height()/cbsy),
      ("HP: " + actor.health)
    );
    
    //Display Item info
    GUI.Box(
      new Rect(
            XOffset() + Width() - Width()/cbsx,
            (9 * Height()/cbsy),
            Width()/cbsx,
            Height()/cbsy
          ),
      actor.ItemInfo()
    );
  }
  void RenderMain(){}
  void RenderInventory(){ print("Rendering Inventory");}
  void RenderOptions(){}
  void RenderSpeech(){}
  void RenderTrade(){}
  void RenderQuest(){}

  /* Call appropriate menu's focus handler. */
  void UpdateFocus(){
    switch(activeMenu){
      case HUD:
        HUDFocus();
        break;
      case MAIN:
        MainFocus();
        break;
      case INVENTORY:
        InventoryFocus();
        break;
      case OPTIONS:
        OptionsFocus();
        break;
      case SPEECH:
        SpeechFocus();
        break;
      case TRADE:
        TradeFocus();
        break;
      case QUEST:
        QuestFocus();
        break;
    }
  }
  
  void MainFocus(){}
  void HUDFocus(){}
  void InventoryFocus(){}
  void OptionsFocus(){}
  void SpeechFocus(){}
  void TradeFocus(){}
  void QuestFocus(){}
 
  /* Call appropriate menu's input handler. */
  void MenuInput(int button){
  }
  
  void MainInput(int button){}
  void InventoryInput(int button){}
  void OptionsInput(int button){}
  void SpeechInput(int button){}
  void TradeInput(int button){}
  void QuestInput(int button){}
}
