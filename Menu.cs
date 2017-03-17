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
  int sx, sy; // secondary focus (ie which item in a table is selected.)
  int sxMax, syMax, sxMin, syMin; // Secondary focus boundaries.  
  List<int> selections = null; // What selections are available.
  public Vector2 scrollPosition = Vector2.zero;
  
  public void Change(int menu){
    if(!actor){ activeMenu = NONE; }
    if(menu == ABILITY){ AbilitySetup(); }
    if(menu <= ABILITY && menu >= NONE){
      activeMenu = menu;
      px = py = sx = sy = 0;
      UpdateFocus();
      if(menu == HUD && actor){ actor.SetMenuOpen(false); }
      else if(actor){actor.SetMenuOpen(true); }
    }
  }
  
  /* Populates selection with available abilities */
  void AbilitySetup(){
    selections = new List<int>();
    for(int i = 0; i < actor.abilities.Length; i++){
      if(actor.abilities[i]){
        selections.Add(i);
      }
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
    if(actor.itemInReach){
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
    else if(actor.actorInReach){ 
        GUI.Box(
          new Rect(
                XOffset() + Width() - (2* Width()/cbsx),
                (9 * Height()/cbsy),
                Width()/cbsx,
                Height()/cbsy
              ),
          actor.ActorInteractionText()
        );
      }
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
      new Rect(XOffset() +iw, Height()/2, Width()-iw, Height()),
      scrollPosition,
      new Rect(0, 0, 200, 200)
    );
    
    List<Data> inv = actor.inventory;
    
    for(int i = 0; i < inv.Count; i++){
      GUI.color = Color.green; 
      Data item = inv[i];
      string selected ="";
      if(i == actor.primaryIndex){ selected += "Right Hand "; }
      if(i == actor.secondaryIndex){ selected += "Left Hand "; }
      string name = item.displayName;
      string info = " " + item.stack + "/" + item.stackSize;
      if(i == sy && sx == 0){ GUI.color = Color.yellow; }
      if(GUI.Button(
        new Rect(0, ih * i, iw + iw/2, ih),
        selected + name + info
      )){
        actor.Equip(i);
      }
      if(GUI.Button(
        new Rect(iw + iw/2, ih * i, iw/2, ih),
        "DROP"
      )){ actor.DiscardItem(i); }
      if(i == sy && sx == 0){ GUI.color = Color.green; }
    }
    GUI.EndScrollView();
    
    if(sx == 1){ GUI.color = Color.yellow; }
    if(GUI.Button(
        new Rect(XOffset() + Width() - iw, Height()/2, iw, ih),
        "Abilities"
      )){
        Change(ABILITY);
    }
    if(sx == 1){ GUI.color = Color.green; }
    
    if(sx == -1){ GUI.color = Color.yellow; }
    if(GUI.Button(
        new Rect(XOffset(), Height()/2, iw, ih),
        "Quests"
      )){
        print("Quests not implemented.");
    }
    if(sx == -1){ GUI.color = Color.green; }
  }
  
  
  void RenderOptions(){}

  /* Render the dialogue screen. */
  void RenderSpeech(){
    if(!actor){ return; }
    if(!actor.interlocutor){print("Interlocutor missing"); return;}
    GUI.skin.button.wordWrap = true; // Make sure text wraps in buttons.
    GUI.skin.box.wordWrap = true; // Make sure text wraps in boxes.
    GUI.Box(
      new Rect(XOffset(), 0, Width(), Height()),
      ""
    );
    SpeechTree st = actor.interlocutor.speechTree;
    if(st == null){ print("Interlocutor lacks speech tree"); return; }
    GUI.color = Color.green;
    int iw = Width()/3;
    int ih = Height()/15;
    int mid = Height()/2;
    string text = "";
    
    // Render Prompt
    GUI.Box(
      new Rect(iw, 0, iw, 7*ih),
        st.ActiveNode().prompt
    );
    
    // Render Option 0
    if(!st.ActiveNode().hidden[0]){
      if(sy==0 && sx == 0){ GUI.color = Color.yellow; }
      text = st.ActiveNode().options[0];
      if(GUI.Button(
        new Rect(iw, ih*7, iw, 2*ih),
          text
      )){
        st.SelectOption(0);
      }
      if(sy==0 && sx == 0){ GUI.color = Color.green; }
    }
    
    // Render Option 1
    if(!st.ActiveNode().hidden[1]){
      if(sy==1 && sx == 0){ GUI.color = Color.yellow; }
      text = st.ActiveNode().options[1];
      if(GUI.Button(
        new Rect(iw, ih*9, iw, 2*ih),
          text
      )){
        st.SelectOption(1);
      }
      if(sy==1 && sx == 0){ GUI.color = Color.green; }
    }
    
    // Render Option 2
    if(!st.ActiveNode().hidden[2]){
      if(sy==2 && sx == 0){ GUI.color = Color.yellow; }
      text = st.ActiveNode().options[2];
      if(GUI.Button(
        new Rect(iw, ih*11, iw, 2*ih),
          text
      )){
        st.SelectOption(2);
      }
      if(sy==2 && sx == 0){ GUI.color = Color.green; }
    }
    
    // Render Option 3
    if(!st.ActiveNode().hidden[3]){
      if(sy==3 && sx == 0){ GUI.color = Color.yellow; }
      text = st.ActiveNode().options[3];
      if(GUI.Button(
        new Rect(iw, ih*13, iw, 2*ih),
          text
      )){
        st.SelectOption(3);
      }
      if(sy==3 && sx == 0){ GUI.color = Color.green; }
    }
  }
  
  
  void RenderTrade(){}
  void RenderQuest(){}
  void RenderAbility(){
    GUI.Box(
      new Rect(XOffset(), 0, Width(), Height()),
      ""
    );
    
    int iw = Width()/4;
    int ih = Height()/20;
    
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() + iw, Height()/2, Width()-iw, Height()),
      scrollPosition,
      new Rect(0, 0, 200, 200)
    );
    
    List<Data> inv = actor.inventory;
    
    for(int i = 0; i < selections.Count; i++){
      GUI.color = Color.green; 
      int ability = selections[i];
      string selected = "";
      if(i == actor.rightAbility){ selected += "Right Hand "; }
      if(i == actor.leftAbility){ selected += "Left Hand "; }
      string name = actor.AbilityInfo(ability);
      if(i == sy && sx == 0){ GUI.color = Color.yellow; }
      if(GUI.Button(
        new Rect(0, ih * i, iw, ih),
        selected + name
      )){
        actor.EquipAbility(ability);
      }
      if(GUI.Button(
        new Rect(iw, ih * i, iw, ih),
          "EquipLeft"
      )){
        actor.EquipAbilitySecondary(ability);
      }
      if(i == sy && sx == 0){ GUI.color = Color.green; }
    }
    GUI.EndScrollView();
    
    if(sx == 1){ GUI.color = Color.yellow; }
    if(GUI.Button(
        new Rect(XOffset() + Width() - iw, Height()/2, iw, ih),
        "Quests"
      )){
        print("Quests not implemented");
    }
    if(sx == 1){ GUI.color = Color.green; }
    
    if(sx == -1){ GUI.color = Color.yellow; }
    if(GUI.Button(
        new Rect(XOffset(), Height()/2, iw, ih),
        "Inventory"
      )){
        Change(INVENTORY);
    }
    if(sx == -1){ GUI.color = Color.green; }
  }

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
    if(sx > sxMax){ sx = sxMax; }
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
  void AbilityFocus(){
    if(selections != null){ syMax = selections.Count - 1; }
    else{ syMax = 0; }
    syMin = 0;
    sxMax = 1;
    sxMin = -1;
    if(sx != 0){ sy = 0; }
    SecondaryBounds();
  }
 
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
      if(button == A){ Change(ABILITY); return; }
    }
    
  }
  void OptionsInput(int button){}
  
  void SpeechInput(int button){
    if(button == B || button == Y){ // Exit menu
      Change(HUD);
      actor.SetMenuOpen(false);
      return;
    }
  }
  
  void TradeInput(int button){}
  void QuestInput(int button){}
  void AbilityInput(int button){
    if(button == B || button == Y){ // Exit menu
      Change(HUD);
      actor.SetMenuOpen(false);
      return;
    }
    if(sx == -1){
      if(button == A){ Change(INVENTORY); return; }
    }
    if(sx == 0){
      if(button == A || button == RT){ actor.EquipAbility(selections[sy]); return; }
      if(button == LT){ actor.EquipAbilitySecondary(selections[sy]); return;}
    }
    if(sx == 1){
      if(button == A){ print("Quests not implemented"); return; }
    }
  }
}
