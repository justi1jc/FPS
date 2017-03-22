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
  public const int STATS     = 8; // rpg stats menu
  
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
  List<int> selections = null; // What selections are available.
  public Vector2 scrollPosition = Vector2.zero; // primary Scroll position
  public Vector2 scrollPositionB = Vector2.zero; // Secondayr scroll position
  List<Data> selling, buying; // Items sold to NPC, bought from NPC.
  List<Data> sold, bought;    // Items changing hands in trade.
  int balance;            // Trade balance.
  
  
  public void Change(int menu){
    if(!actor){ activeMenu = NONE; }
    if(menu == ABILITY){ AbilitySetup(); }
    if(menu <= STATS && menu >= NONE){
      activeMenu = menu;
      px = py = sx = sy = 0;
      UpdateFocus();
      if(menu == TRADE && actor && actor.interlocutor){
        selling = new List<Data>(actor.inventory);
        buying = new List<Data>(actor.interlocutor.inventory);
        sold = new List<Data>();
        bought = new List<Data>();
        print("Selling count:"+ selling.Count);
        balance = 0;
      }
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
  
  void Box(string text, int posx, int posy, int scalex, int scaley){
    GUI.color = Color.green;
    GUI.Box(new Rect(posx, posy, scalex, scaley), text);
  }
  
  // Convenience method to render button and return if it's been clicked.
  bool Button(
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
      case STATS:
        RenderStats();
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
    int cbsy = 20; // condition bar height scale
    int ch = Height()/15; // Condition height
    int cw = Width()/3;   // Condition width
    
    string str;
    
    str = "Health: " + actor.health;
    Box(str, XOffset(), 12 * ch, cw, ch);
    
    str = "Stamina: " + actor.stamina;
    Box(str, XOffset(), 13 * ch, cw, ch);
    
    str = "Mana: " + actor.mana;
    Box(str, XOffset(), 14 * ch, cw, ch);
    
    
    // Display Item info
    str = actor.ItemInfo();
    Box(str, XOffset() + 2*cw, 14*ch, cw, ch);
    
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
    GUI.color = Color.green;
    
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
    int iw = Width()/6;
    int ih = Height()/15;
    int mid = Height()/2;
    string text = "";
    
    // Render Prompt
    GUI.Box(
      new Rect(XOffset()+(2*iw), 0, 4*iw, 7*ih),
        st.ActiveNode().prompt
    );
    
    // Render Option 0
    if(!st.ActiveNode().hidden[0]){
      if(sy==0 && sx == 0){ GUI.color = Color.yellow; }
      text = st.ActiveNode().options[0];
      if(GUI.Button(
        new Rect(XOffset()+(2*iw), ih*7, 4*iw, 2*ih),
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
        new Rect(XOffset()+(2*iw), ih*9, 4*iw, 2*ih),
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
        new Rect(XOffset()+(2*iw), ih*11, 4*iw, 2*ih),
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
        new Rect(XOffset()+(2*iw), ih*13, 4*iw, 2*ih),
          text
      )){
        st.SelectOption(3);
      }
      if(sy==3 && sx == 0){ GUI.color = Color.green; }
    }
    
    if(Button("Trade", XOffset(), Height()/2, iw, ih)){
      Change(TRADE);
    }
  }
  
  
  void RenderTrade(){
    int x, y; // To set up positioning before Box/Button call.
    string str; // To set up Box/Button call
    Box("", XOffset(), 0, Width(), Height());
    int iw = Width()/4;
    int ih = Height()/20;
    
    scrollPosition = GUI.BeginScrollView(
      new Rect(iw, Height()/2, iw, Height()/2),
      scrollPosition,
      new Rect(0, 0, 200, 200)
    );
    GUI.color = Color.green;

    for(int i = 0; i < selling.Count; i++){
      y = i*ih;
      str = selling[i].displayName;
      if(selling[i].stack > 1){ str += "(" + selling[i].stack + ")"; }
      if(Button(str, 0, y, iw, ih, 0, i)){
        Data item = selling[i];
        selling.Remove(item);
        sold.Add(item);
        buying.Add(item);
        balance -= 10;
      }
    }
    
    GUI.EndScrollView();
    
    scrollPositionB = GUI.BeginScrollView(
      new Rect(2*iw, Height()/2, iw, Height()/2),
      scrollPositionB,
      new Rect(0, 0, 200, 200)
    );

    for(int i = 0; i < buying.Count; i++){
      y = i*ih;
      str = buying[i].displayName;
      if(buying[i].stack > 1){ str += "(" + buying[i].stack + ")"; }
      if(Button(str, 0, y, iw, ih, 0, i)){
        Data item = buying[i];
        buying.Remove(item);
        selling.Add(item);
        balance += 10;
      }
    }
    
    
    GUI.EndScrollView();
    
    
    if(balance < 0){
      str = "" + (-1*balance);
      str = "<= " + str;
    }
    else if(balance > 0){
      str = "" + balance + "=>";
    }
    else{
      str = "" + balance;
    }
    x = iw + iw/2;
    y = Height()/4;
    Box(str, x, y, iw, ih);
    
    str = "Talk";
    x = XOffset() + Width() - iw;
    y = Height()/2;
    if(Button(str, x, y, iw, ih, 1)){
      Change(SPEECH);
    }
        
  }
  
  
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
        "Stats"
      )){
        Change(STATS);
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
  
  /* Render menu for viewing/editing stats. */
  void RenderStats(){
    int ah = Height()/14; // Attribute height
    int aw = Width()/6;  // Attribute width
    int aOff = (int)(3.5 * ah); // y Offset for attributes
    int sh = Height()/6; // Skill height
    int sOff = 2 * aw + XOffset();// x offset for skills
    
    // Render background
    GUI.Box(
      new Rect(XOffset(), 0, Width(), Height()),
      ""
    );
    GUI.color = Color.green; // Set color to green as default.
    
    
    // Render attribute column 
    string str = "";
    
    str = "Intelligence: " + actor.intelligence;
    Box(str,XOffset()+aw, aOff+0, aw, ah);
    
    str = "Charisma: " + actor.charisma;
    Box(str,XOffset()+aw, aOff+ah, aw, ah);
    
    str = "Endurance: " + actor.endurance;
    Box(str,XOffset()+aw, aOff+2*ah, aw, ah);
    
    str = "Perception: " + actor.perception;
    Box(str,XOffset()+aw, aOff+3*ah, aw, ah);
    
    str = "Agility: " + actor.agility;
    Box(str,XOffset()+aw, aOff+4*ah, aw, ah);
    
    str = "Willpower: " + actor.willpower;
    Box(str,XOffset()+aw, aOff+5*ah, aw, ah);
    
    str = "Strength: " + actor.strength;
    Box(str,XOffset()+aw, aOff+6*ah, aw, ah);
    
    str = "Level: " + actor.level;
    Box(str, XOffset()+aw, 0, aw, ah);
    
    str = "XP: " + actor.xp;
    Box(str, XOffset()+aw, ah, aw, ah);
    
    str = "Next level: " + actor.nextLevel;
    Box(str, XOffset()+aw, 2*ah, aw, ah);
    
    
    // Render Skills
    str = "Remaining Skill Points: " + actor.skillPoints;
    Box(str, sOff, 0, aw, sh);
    
    str = "Ranged: " + actor.ranged;
    if(Button(str, sOff, sh, aw, sh, 0, 0)){
      if(actor.skillPoints > 0 && actor.ranged < 100){
        actor.skillPoints--; actor.ranged++;
      }
    }
    str = "Melee: " + actor.melee;
    if(Button(str, sOff, 2*sh, aw, sh, 0, 1)){
      if(actor.skillPoints > 0 && actor.melee < 100){
        actor.skillPoints--; actor.melee++;
      }
    }
    str = "Unarmed: " + actor.unarmed;
    if(Button(str, sOff, 3*sh, aw, sh, 0, 2)){
      if(actor.skillPoints > 0 && actor.unarmed < 100){
        actor.skillPoints--; actor.unarmed++;
      }
    }
    str = "Magic: " + actor.magic;
    if(Button(str, sOff, 4*sh, aw, sh, 0, 3)){
      if(actor.skillPoints > 0 && actor.magic < 100){
        actor.skillPoints--; actor.magic++;
      }
    }
    str = "Stealth: " + actor.stealth;
    if(Button(str, sOff, 5*sh, aw, sh, 0, 4)){
      if(actor.skillPoints > 0 && actor.stealth < 100){
        actor.skillPoints--; actor.stealth++;
      }
    }
    
    // Render Ability navigation button.
    str = "Abilities";
    if(Button(str, XOffset(), Height()/4, aw, Height()/2, -1 )){
      Change(ABILITY);
    }
       
    // Render Quest navigation button.
    str = "Quests";
    if(Button(str, XOffset()+5*aw, Height()/4, aw, Height()/2, 1)){
      print("Quests not implemented.");
    }
    
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
      case STATS:
        StatsFocus();
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
  
  void SpeechFocus(){
    syMax = 3;
    syMin = 0;
    sxMax = 0;
    sxMin = 0;
    SecondaryBounds();
    SpeechTree st = actor.interlocutor.speechTree;
    if(st.ActiveNode().hidden[sy]){ sy--; }
  }
  
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
  void StatsFocus(){
    syMax = 4;
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
      case STATS:
        StatsInput(button);
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
    SpeechTree st = actor.interlocutor.speechTree;
    if(button == A){
      if(st.ActiveNode().hidden[sy]){ return; }
      switch(sy){
        case 0:
          st.SelectOption(0);
          break;
        case 1:
          st.SelectOption(1);
          break;
        case 2:
          st.SelectOption(2);
          break;
        case 3:
          st.SelectOption(3);
          break;
      }
      sy = 0;
    }
  }
  
  void TradeInput(int button){
    if(button == B || button == Y){ // Exit menu
      Change(HUD);
      actor.SetMenuOpen(false);
      return;
    }
  }
  
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
      if(button == A){ Change(STATS); return; }
    }
  }
  
  void StatsInput(int button){
    if(button == B || button == Y){ // Exit menu
      Change(HUD);
      actor.SetMenuOpen(false);
      return;
    }
    
    if(sx == -1){ if(button == A){ Change(ABILITY); return; } }
    if(sx == 0 && button == A && actor.skillPoints > 0){
      switch(sy){
        case 0:
          if(actor.ranged < 100){
            actor.skillPoints--; actor.ranged++;
          }
          break;
        case 1:
          if(actor.melee < 100){
            actor.skillPoints--; actor.melee++;
          }
          break;
        case 2:
          if(actor.unarmed < 100){
            actor.skillPoints--; actor.unarmed++;
          }
          break;
        case 3:
          if(actor.magic < 100){
            actor.skillPoints--; actor.magic++;
          }
          break;
        case 4:
          if(actor.stealth < 100){
            actor.skillPoints--; actor.stealth++;
          }
          break;
      }
    }
    if(sx == 1){ if(button == A){ print("Quests not implented."); return; } }
    
  }
}
