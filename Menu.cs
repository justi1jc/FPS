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
  
  public int activeMenu = NONE;
  int px, py; // Primary focus (ie which table or is selected.)
  int pxMax, pyMax, pxMin, pyMin; // Primary focus boundaries.
  int sx, sy; // secondary focus (ie which item in a table is selected.)
  int sxMax, syMax, syMan, syMin; // Secondary focus boundaries.  

  public void Change(int menu){
    if(menu <= QUEST && menu >= NONE){
      activeMenu = menu;
      px = py = sx = sy = 0;
      UpdateFocus();
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
  
  void RenderHUD(){ print("Rendering HUD");}
  void RenderMain(){}
  void RenderInventory(){ print("Rendering Inventory");}
  void RenderOptions(){}
  void RenderSpeech(){}
  void RenderTrade(){}
  void RenderQuest(){}
  
  void MainFocus(){}
  void HUDFocus(){}
  void InventoryFocus(){}
  void OptionsFocus(){}
  void SpeechFocus(){}
  void TradeFocus(){}
  void QuestFocus(){}
  
}
