/*
*   Menu controller for Players. This should be attached to an Actor's head.
*   
*
*
*/


﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
  int sxMax, syMax, sxMin, syMin; // Secondary focus boundaries.  
  public Vector2 scrollPosition = Vector2.zero;
  
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
    // Display Condition bars
    int cbsx = 3;  // condition bar width scale
    int cbsy = 10; // condition bar height scale
    GUI.Box( 
      new Rect(XOffset(), (9 * Height()/cbsy), Width()/cbsx, Height()/cbsy),
      ("HP: " + actor.health)
    );
    
    // Display Item info
    GUI.Box(
      new Rect(
            XOffset() + Width() - Width()/cbsx,
            (9 * Height()/cbsy),
            Width()/cbsx,
            Height()/cbsy
          ),
      actor.ItemInfo()
    );
    
    // Display item in reach, if it exists.
    if(!actor.itemInReach){ return; }
    Item inReach = actor.itemInReach.GetComponent<Item>(); 
    GUI.Box(
      new Rect(
            XOffset() + Width() - (2* Width()/cbsx),
            (9 * Height()/cbsy),
            Width()/cbsx,
            Height()/cbsy
          ),
      inReach.displayName
    );
  }
  
  
  void RenderMain(){}
  
  
  void RenderInventory(){
    //Draw Background
    GUI.Box(
      new Rect(XOffset(), 0, Width(), Height()),
      ""
    );
    
    //Display Items
    int iw = Width()/2;
    int ih = Height()/20;
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset(), Height()/2, Width(), Height()), scrollPosition, new Rect(0, 0, 200, 200)
    );
    
    List<Data> inv = actor.inventory;
    
    for(int i = 0; i < inv.Count; i++){
      Data item = inv[i];
      string selected = i==sy ? ">" : "";
      string name = item.displayName;
      string info = " " + item.stack + "/" + item.stackSize;
      if(GUI.Button(
        new Rect(0, ih * i, iw, ih),
        selected + name + info
      )){
        actor.Equip(i);
      }
      if(GUI.Button(
        new Rect(iw, ih * i, iw, ih),
        "DROP"
      )){ actor.DiscardItem(i); }
    }
    GUI.EndScrollView();
    
    
  }
  
  
  void RenderOptions(){}
  void RenderSpeech(){}
  void RenderTrade(){}
  void RenderQuest(){}

  /* Call appropriate menu's focus update handler. */
  void UpdateFocus(){
    switch(activeMenu){
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
  
  void InventoryFocus(){
    syMax = actor.inventory.Count-1;
    syMin = 0;
    if(sy > syMax){ sy = syMax; }
    if(sy < syMin){ sy = 0; }
    sxMax = 0;
    sxMin = 0;
    if(sx > syMax){ sx = sxMax; }
    if(sx < syMin){ sx = sxMin; }
  }
  void OptionsFocus(){}
  void SpeechFocus(){}
  void TradeFocus(){}
  void QuestFocus(){}
 
  /* Receives button press from Actor. */
  public void Press( int button){
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
    if(button <= Y && button >= A){ MenuInput(button); }
  }
 
  /* Call appropriate menu's input handler. */
  void MenuInput(int button){
    switch(activeMenu){
      case MAIN:
        MainInput(button);
        break;
      case INVENTORY:
        InventoryInput(button);
        break;
      case OPTIONS:
        OptionsInput(button);
        break;
      case SPEECH:
        SpeechInput(button);
        break;
      case TRADE:
        TradeInput(button);
        break;
      case QUEST:
        QuestInput(button);
        break;
    }
  }
  
  void MainInput(int button){}
  void InventoryInput(int button){
    if(button == B || button == Y){ // Exit menu
      Change(HUD);
      actor.SetMenuOpen(false);
      return;
    }
    if(button == A){ actor.Equip(sy); return; }
    if(button == X){ actor.DiscardItem(sy); return; }
    
  }
  void OptionsInput(int button){}
  void SpeechInput(int button){}
  void TradeInput(int button){}
  void QuestInput(int button){}
}