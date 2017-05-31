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
  public const int NONE      = -1; // Do nothing
  public const int HUD       =  0; // in-game HUD
  public const int MAIN      =  1; // main menu
  public const int INVENTORY =  2; // inventory
  public const int OPTIONS   =  3; // pause menu
  public const int SPEECH    =  4; // speech menu
  public const int TRADE     =  5; // trading menu
  public const int QUEST     =  6; // quest menu
  public const int ABILITY   =  7; // abilities menu
  public const int STATS     =  8; // rpg stats menu
  public const int LOOT      =  9; // looking in containers
  public const int LOAD      = 10; // Load the game
  
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
  
  
  public List<string> notifications;
  public float notificationTimer = 6f;
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
  public List<Data> contents; // Contents of a container/npc inventory.
  public int subMenu = 0; // To account for menus within a menu.
  public string sesName = ""; // Name for new game.
  public List<GameRecord> files;
  
  public void Awake(){
    notifications = new List<string>();
  }
  
  /* Changes the active menu and sets up variables for it. */
  public void Change(int menu){
    if(!actor){ activeMenu = NONE; }
    if(menu == ABILITY){ AbilitySetup(); }
    if(menu <= LOAD && menu >= NONE){
      activeMenu = menu;
      px = py = sx = sy = 0;
      UpdateFocus();
      if(menu == MAIN){ subMenu = 0; }
      if(menu == TRADE && actor && actor.interlocutor){
        selling = new List<Data>(actor.inventory);
        buying = new List<Data>(actor.interlocutor.inventory);
        sold = new List<Data>();
        bought = new List<Data>();
        balance = 0;
      }
      if(menu == LOAD){
        py = -1;
        files = Session.session.LoadFiles();
      }
      if(menu == HUD && actor){ actor.SetMenuOpen(false); }
      else if(actor){actor.SetMenuOpen(true); }
    }
  }
  
  public void Notify(string message){
    print("Notified:" + message);
    notifications.Add(message);
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
  
  /* Convenience method to render button and return if it's been clicked. */
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
  
  string TextField(string text, int posx, int posy, int scalex, int scaley){
    return  GUI.TextField(new Rect(posx, posy, scalex, scaley), text, 25);
  }
  
  
  void OnGUI(){
      RenderNotifications();
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
      case LOOT:
        RenderLoot();
        break;
      case LOAD:
        RenderLoad();
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
  
  
  void RenderNotifications(){
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
  
  void RenderHUD(){
    // Display Condition bars
    int cbsx = 3;  // condition bar width scale
    int cbsy = 20; // condition bar height scale
    int ch = Height()/15; // Condition height
    int cw = Width()/3;   // Condition width
    int x, y;
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
      x = XOffset() + Width() - (2*Width()/cbsx);
      y = (9 * Height()/cbsy);
      if(inReach.displayName != ""){ 
        Box(inReach.displayName, x, y, Width()/cbsx, Height()/cbsy);
      }
    }
    else if(actor.actorInReach){
        str = actor.ActorInteractionText();
        x = XOffset() + Width() - (2 * Width()/cbsx);
        y = 9 * Height()/cbsy;
        Box(str, x, y, Width()/cbsx, Height()/cbsy); 
      }
    }
  
  
  /* This menu renders in the absence of an Actor. */
  void RenderMain(){
    int iw = Width()/6;
    int ih = Height()/4;
    int x = XOffset() + iw;
    string str;
    
    switch(subMenu){
      case 0:
        str = "New Game";
        if(Button(str, x, ih, 4*iw, ih, 0, 1 )){ 
          subMenu = 1;
        }
        break;
      case 1:
        sesName = TextField(sesName, x, ih, 3*iw, ih);
        if(Button("Start", x + 3*iw, ih, iw, ih)){
          Session.session.CreateGame(sesName);
        }
        break;
    }
    str = "Load Game";
    if(Button(str, x, 2*ih, 4*iw, ih, 0, 2 )){ 
      Change(LOAD);
    }
    
    str = "Quit";
    if(Button(str, x, 3*ih, 4*iw, ih, 0, 3 )){ 
      Application.Quit();
    }
    
  }
  
  
  void RenderInventory(){
    Box("", XOffset(), 0, Width(), Height()); //Draw Background
    GUI.color = Color.green;
    List<Data> inv = actor.inventory;
    int iw = Width()/4;
    int ih = Height()/20;
    string str = "";
    
    str = "Currency: " + actor.currency; 
    Box(str, XOffset(), 0, iw, 2*ih);
    
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() +iw, Height()/2, Width()-iw, ih * inv.Count),
      new Vector2(0, ih * sy),
      new Rect(0, 0, 200, 200)
    );
    
    for(int i = 0; i < inv.Count; i++){
      GUI.color = Color.green; 
      Data item = inv[i];
      string selected = "";
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
        Change(QUEST);
    }
    if(sx == -1){ GUI.color = Color.green; }
  }
  
  
  void RenderOptions(){
    string str;
    int iw = Width()/6;
    int ih = Height()/6;
    int x = XOffset() + iw;
    
    str = "Resume";
    if(Button(str, x, 0, 2*iw, ih, 0, 0)){
      Change(HUD);
    }
    
    str = "Load";
    if(Button(str, x, ih, 2*iw, ih, 0, 1)){
      Session.session.LoadFiles();
      Change(LOAD);
    }
    
    str = "Settings";
    if(Button(str, x, 2*ih, 2*iw, ih, 0, 2)){
      print("Settings");
    }
    
    str = "Save";
    if(Button(str, x, 3*ih, 2*iw, ih, 0, 3)){
      Session.session.SaveGame(Session.session.sessionName);
      
    }
    
    str = "Save and Quit.";
    if(Button(str, x, 4*ih, 2*iw, ih, 0, 4)){
      Session.session.SaveGame(Session.session.sessionName);
      Application.Quit();
    }
    
    str = "Quit.";
    if(Button(str, x, 5*ih, 2*iw, ih, 0, 5)){
      Application.Quit();
    }
  }

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

    str = actor.displayName + ": " + actor.currency;
    x = XOffset() + iw;
    y = (Height()/2) - (2*ih);
    Box(str, x, y, iw, 2*ih);

    str = actor.interlocutor.displayName + ": " + actor.interlocutor.currency;
    x = XOffset() + 2* iw;
    y = (Height()/2) - (2*ih);
    Box(str, x, y, iw, 2*ih);

    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() + iw, Height()/2, iw, Height()/2),
      scrollPosition,
      new Rect(0, 0, 200, 200)
    );
    GUI.color = Color.green;

    for(int i = 0; i < selling.Count; i++){
      y = i*ih;
      str = selling[i].displayName;
      if(selling[i].stack > 1){ str += "(" + selling[i].stack + ")"; }
      if(Button(str, 0, y, iw, ih, 0, i)){
        Sell(i);
      }
    }
    
    GUI.EndScrollView();
    
    scrollPositionB = GUI.BeginScrollView(
      new Rect(XOffset() + 2*iw, Height()/2, iw, Height()/2),
      scrollPositionB,
      new Rect(0, 0, 200, 200)
    );

    for(int i = 0; i < buying.Count; i++){
      y = i*ih;
      str = buying[i].displayName;
      if(buying[i].stack > 1){ str += "(" + buying[i].stack + ")"; }
      if(Button(str, 0, y, iw, ih, 0, i)){
        Buy(i);
      }
    }
    
    
    GUI.EndScrollView();
    
    
    if(balance < 0){
      str = "" + (-1*balance);
      str = str + "=> ";
    }
    else if(balance > 0){
      str = "<=" + balance;
    }
    else{
      str = "" + balance;
    }
    x = XOffset() + iw + iw/2;
    y = Height()/4;
    Box(str, x, y, iw, ih);
    
    if((balance > 0 || -balance <= actor.currency) && (bought.Count > 0 || sold.Count > 0)){
      str = "Complete trade";
      x = XOffset();
      y = Height()/4;
      if(Button(str, x, y, iw, ih, -1)){
        FinalizeTrade();
      }
    }
    
    str = "Talk";
    x = XOffset() + Width() - iw;
    y = Height()/2;
    if(Button(str, x, y, iw, ih, 1)){
      Change(SPEECH);
    }
        
  }
  
    /* Buy an item from npc. 
    Add it to bought if player does not own it.
  */
  void Buy( int i){
    Data item = buying[i];
    balance -= item.baseValue;
    selling.Add(item);
    buying.Remove(item);
    sold.Remove(item);
    if(actor.inventory.IndexOf(item) == -1){
      bought.Add(item);
    }
  }

  /* Sell an item to npc. */
  void Sell(int i){
    Data item = selling[i];
    balance += item.baseValue;
    buying.Add(item);
    selling.Remove(item);
    bought.Remove(item);
    if(actor.interlocutor.inventory.IndexOf(item) == -1){
      sold.Add(item);
    }
  }

    /* Distribute items that were bought and sold. */
  void FinalizeTrade(){
    for(int i = 0; i < bought.Count; i++){
      Data item = bought[i];
      actor.interlocutor.inventory.Remove(item);
      actor.inventory.Add(item);
    }
    for(int i = 0; i < sold.Count; i++){
      Data item = sold[i];
      actor.inventory.Remove(item);
      actor.interlocutor.inventory.Add(item);
    }
    if(balance > actor.interlocutor.currency){
      balance = actor.interlocutor.currency;
    }
    actor.interlocutor.currency -= balance;
    actor.currency += balance;
    
    balance = 0;
    sold = new List<Data>();
    bought = new List<Data>();
    selling = new List<Data>(actor.inventory);
    buying = new List<Data>(actor.interlocutor.inventory);
  }


  void RenderQuest(){
    Box("", XOffset(), 0, Width(), Height()); // Background
    int iw = Width()/4;
    int ih = Height()/20;
    int y = 0;
    string str = "";
    str = "Inventory";
    if(Button(str, Width() - iw, Height()/2, iw, ih, 1, 0)){ Change(INVENTORY); }
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() +iw, Height()/2, iw, Height()),
      scrollPosition,
      new Rect(0, 0, iw, 200)
    );
    
    List<Quest> quests = Session.session.quests;
    for(int i = 0; i < quests.Count; i++){ 
      y = ih * i;
      if(Button(quests[i].Name(), 0, y, iw, ih, 0, i)){
        print(quests[i].Name());
      }
      
    }
    GUI.EndScrollView();
    if(sy < 0 || sy >= quests.Count){ return; }
    str = "";
    string[] obj = quests[sy].Objectives();
    for(int i = 0; i < obj.Length; i++){
      str += obj[i] + "\n";
      Box(str, XOffset() + 2*iw, Height()/2 + i*y, iw, ih);
    }
    
  }
  
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
  
  
  void RenderLoot(){
    Box("", XOffset(), 0, Width(), Height());

    GUI.color = Color.green;
    
    int iw = Width()/4;
    int ih = Height()/20;
    int y = 0;
    string str = "";
    
    str = "Currency: " + actor.currency; 
    Box(str, XOffset(), 0, iw, 2*ih);
    
    scrollPosition = GUI.BeginScrollView(
      new Rect(XOffset() +iw, Height()/2, iw, Height()),
      scrollPosition,
      new Rect(0, 0, iw, 200)
    );
    
    List<Data> inv = actor.inventory;
    List<Data> invB = contents;
    
    for(int i = 0; i < inv.Count; i++){ 
      Data item = inv[i];
      string selected ="";
      if(i == actor.primaryIndex){ selected += "Right Hand "; }
      if(i == actor.secondaryIndex){ selected += "Left Hand "; }
      string name = item.displayName;
      string info = " " + item.stack + "/" + item.stackSize;
      if(i == sy && sx == 0){ GUI.color = Color.yellow; }
      str = selected + name + info;
      y = ih * i;
      if(Button(str, 0, y, iw, ih, 0, i)){
        contents.Add(item);
        inv.Remove(item);
      }
    }
    GUI.EndScrollView();
    
    scrollPositionB = GUI.BeginScrollView(
      new Rect(XOffset() + 2*iw, Height()/2, iw, Height()),
      scrollPositionB,
      new Rect(0, 0, iw, 200)
    );    
    for(int i = 0; i < invB.Count; i++){ 
      Data item = invB[i];
      string selected ="";
      if(i == actor.primaryIndex){ selected += "Right Hand "; }
      if(i == actor.secondaryIndex){ selected += "Left Hand "; }
      string name = item.displayName;
      string info = " " + item.stack + "/" + item.stackSize;
      if(i == sy && sx == 0){ GUI.color = Color.yellow; }
      str = selected + name + info;
      y = ih * i;
      if(Button(str, 0, y, iw, ih, 0, i)){
        actor.StoreItem(item);
        invB.Remove(item);
      }
    }
    GUI.EndScrollView();
  }
  
  
  void RenderLoad(){
    string str;
    int iw = Width()/6;
    int ih = Height()/5;
    int x, y;
    x = XOffset() + iw;
    if(files != null){
      
      Box("Load Menu", x, 0, iw, ih);
      scrollPositionB = GUI.BeginScrollView(
        new Rect(XOffset() + iw, ih, iw, 2*ih),
        scrollPositionB,
        new Rect(0, 0, iw, ih * files.Count)
      );    
      
      
      for(int i = 0; i < files.Count; i++){
        y = i * ih / 2;
        str = files[i].sessionName;
        if(Button(str, 0, y, iw, ih/2)){ py = i; }
      }
      
      GUI.EndScrollView();
    }
    int dest = actor ? OPTIONS : MAIN;
    if(Button("Back", x, 3*ih, iw, ih)){ Change(dest); }
  
    if(py > -1 && py < files.Count){
      str = files[py].sessionName;
      x = XOffset() + 3 * iw; 
      Box(str,x, 0, iw, ih/2);
      
      str = "Important fact about this file";
      y = ih/2;
      Box(str, x, y, iw, ih/2);
      
      if(files[py].players.Count > 0){
        str = files[py].players[0].displayName;
        y = ih;
        Box(str, x, y, iw, ih/2);
      }
      str = "Load";
      y = ih + ih/2;
      if(Button(str, x, y, iw, ih/2)){ 
        Session.session.LoadGame(files[py].sessionName); 
      }
      str = "Delete";
      y = 2*ih;
      if(Button(str, x, y, iw, ih/2)){ 
        Session.session.DeleteFile(files[py].sessionName);
        files = Session.session.LoadFiles();
      }
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
      case LOOT:
        LootFocus();
        break;
      case LOAD:
        LoadFocus();
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
  
  void LootFocus(){
    syMax = 0;
    syMin = 0;
    sxMax = 1;
    sxMin = 0;
    if(sx == 0){ syMax = actor.inventory.Count -1; }
    else if(sx == 1){ syMax = contents.Count -1; }
    SecondaryBounds();
  }
  
  void LoadFocus(){
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
      case LOOT:
        LootInput(button);
        break;
      case LOAD:
        LoadInput(button);
        break;
    }
  }
  
  void DefaultExit(int button){
    if(button == B || button == Y){
      Change(HUD);
      actor.SetMenuOpen(false);
    }
  }
  
  void MainInput(int button){}
  
  void InventoryInput(int button){
    DefaultExit(button);
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
  
  void OptionsInput(int button){
    DefaultExit(button);
  }
  
  void SpeechInput(int button){
    DefaultExit(button);
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
    DefaultExit(button);
  }
  
  void QuestInput(int button){
    DefaultExit(button);
  }
  
  void AbilityInput(int button){
    DefaultExit(button);
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
    DefaultExit(button);
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
  
  void LootInput(int button){
    DefaultExit(button);
  }
  
  void LoadInput(int button){
    if(button == B || button == Y){ Change(OPTIONS); }
  }
}
