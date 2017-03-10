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
  public const int HUD       = 0; // in-game HUD
  public const int MAIN      = 1; // main menu
  public const int INVENTORY = 2; // inventory
  public const int OPTIONS   = 3; // pause menu
  public const int SPEECH    = 4; // speech menu
  public const int TRADE     = 5; // trading menu
  public const int QUEST     = 6; // quest menu
  public const int ABILITY   = 7; // abilities menu
  
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
  
  public Actor actor;
  public int activeMenu = NONE;
  public bool split;
  public bool right;
  int px, py; // Primary focus (ie which table or is selected.)
  int pxMax, pyMax, pxMin, pyMin; // Primary focus boundaries.
  public int sx, sy; // secondary focus (ie which item in a table is selected.)
  int sxMax, syMax, sxMin, syMin; // Secondary focus boundaries.  
  public Vector2 scrollPosition = Vector2.zero;
  
  public void Change(int menu){
    if(!actor){ activeMenu = NONE; }
    if(menu <= ABILITY && menu >= NONE){
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
      case ABILITY:
        RenderAbility();
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
    
    int iw = Width()/4;
    int ih = Height()/20;
    
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() +iw, Height()/2, Width()-iw, Height()), scrollPosition, new Rect(0, 0, 200, 200)
    );
    
    List<Data> inv = actor.inventory;
    
    for(int i = 0; i < inv.Count; i++){
      GUI.color = Color.blue; 
      Data item = inv[i];
      string selected ="";
      if(i == actor.primaryIndex){ selected += "Right Hand "; }
      if(i == actor.secondaryIndex){ selected += "Left Hand "; }
      string name = item.displayName;
      string info = " " + item.stack + "/" + item.stackSize;
      if(i == sy && sx == 0){ GUI.color = Color.yellow; }
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
      if(i == sy && sx == 0){ GUI.color = Color.blue; }
    }
    GUI.EndScrollView();
    
    if(sx == 1){ GUI.color = Color.yellow; }
    if(GUI.Button(
        new Rect(Width()-iw, Height()/2, iw, ih),
        "Abilities"
      )){
        Change(ABILITY);
    }
    if(sx == 1){ GUI.color = Color.blue; }
    
    if(sx == -1){ GUI.color = Color.yellow; }
    if(GUI.Button(
        new Rect(XOffset(), Height()/2, iw, ih),
        "Quests"
      )){
        print("Quests not implemented.");
    }
    if(sx == -1){ GUI.color = Color.blue; }
  }
  
  
  void RenderOptions(){}
  void RenderSpeech(){}
  void RenderTrade(){}
  void RenderQuest(){}
  void RenderAbility(){}

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
      case ABILITY:
        AbilityFocus();
        break;
    }
  }
  
  void SecondaryBounds(){
    if(sy > syMax){ sy = syMax; }
    if(sy < syMin){ sy = syMin; }
    if(sx > sxMax){ print(sx); sx = sxMax; }
    if(sx < sxMin){ sx = sxMin; }
  }
  void MainFocus(){}
  
  void InventoryFocus(){
    syMax = actor.inventory.Count-1;
    syMin = 0;
    sxMax = 1;
    sxMin = -1;
    if(sx != 0){ sy = 0; }
    SecondaryBounds();
  }
  void OptionsFocus(){}
  void SpeechFocus(){}
  void TradeFocus(){}
  void QuestFocus(){}
  void AbilityFocus(){}
 
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
    if(button <= LT && button >= A){ MenuInput(button); }
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
      case ABILITY:
        AbilityInput(button);
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
    if(sx == -1){
      if(button == A){ print("Quests not implemented"); return; }
    }
    if(sx == 0){ // Hovering over inventory list.
      if(button == A && sy < actor.inventory.Count){ actor.Equip(sy); return; }
      if(button == X && sy < actor.inventory.Count){ actor.DiscardItem(sy); return; }
      if(button == RT && sy < actor.inventory.Count){ actor.Equip(sy); return; }
      if(button == LT && sy < actor.inventory.Count){ actor.EquipSecondary(sy); return; }
    }
    if(sx == 1){
      if(button == A){ print("Abiltiies not implemented."); }//Change(ABILITY); return; }
    }
    
  }
  void OptionsInput(int button){}
  void SpeechInput(int button){}
  void TradeInput(int button){}
  void QuestInput(int button){}
  void AbilityInput(int button){}
}
