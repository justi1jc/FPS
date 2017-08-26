/*
    ArenaLobbyMenu allows the player to configure the arena match before
    playing it.
    
    NOTE: This menu references scenes in a unity project. You will have to
    change some of the arguments if you use a different environment. 
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ArenaLobbyMenu : Menu{
  private int duration; // Duration in minutes.
  private string map;   // map currently selected.
  private int mapIndex; // Index of currently selected map.
  private bool respawns = true; // True if players will respawn.
  private int bots = 15; // Number of bots;
  private string kit = "RANDOM";
  private int kitId = 0;
  private bool teams = false;
  private bool p1red = false; // True if player1 is on the red team.
  private bool p2red = false; // True if player2 is on the red team.
  
  public ArenaLobbyMenu(MenuManager manager) : base(manager){
    duration = 10;
    map = "Arena_Empty";
    mapIndex = 0;
  }

  public override void Render(){
    int ih = Height()/10;
    int iw = Width()/5;
    Box("Arena Lobby", 2*iw, ih, iw, ih);
    string str = "Players: " + Session.session.playerCount;
    if(Button(str, 0, 2*ih, iw, ih)){ TogglePlayers(); Sound(0); }
    str = "Map:" + map;
    if(Button(str, 0, 3*ih, iw, ih)){ NextMap(); Sound(0);}
    str = "Duration:" + duration;
    if(Button(str, 0, 4*ih, iw, ih)){ NextDuration(); }
    str = "Respawns:" + (respawns ? "Yes" : "No");
    if(Button(str, 0, 5*ih, iw, ih)){ respawns = !respawns; }
    if(Button("Start", Width()-iw, Height()-ih, iw, ih)){ StartArena(); }
    if(Button("Back", 0, Height()-ih, iw, ih)){ 
      manager.Change("MAIN"); 
      Sound(0);
    }
    str = "Bots: " + bots;
    Box(str, 0, 6*ih, iw, ih/2);
    if(Button("-", 0, 6*ih + (ih/2), iw/2, ih/2) && bots > 0){ bots--; }
    if(Button("+", iw/2, 6*ih + (ih/2), iw/2, ih/2) && bots < 32){ bots++; }
    
    str = "Kit: " + kit;
    if(Button(str, 0, 7*ih, iw, ih)){ NextKit(); }
    
    str = "Teams: " + (teams ? "Yes" : "No");
    if(Button(str, 0, 8*ih, iw, ih)){ teams = !teams; } 
    
    int players = Session.session.playerCount;
    if(players > 0){
      str = "Player 1";
      Box(str, 4*iw, 0, iw, ih/2);
      if(teams){
        str = p1red ? "Red team" : "Blue team";
        if(Button(str, 4*iw, ih/2, iw, ih/2)){ p1red = !p1red; }
      }
    }
    if(players > 1){
      str = "Player 2";
      Box(str, 4*iw, ih, iw, ih/2);
      if(teams){
        str = p2red ? "Red team" : "Blue team";
        if(Button(str, 4*iw, ih + (ih/2), iw, ih/2)){ p2red = !p2red; }
      }
    }
    
  }
  
  /* Cycles through available kits. */
  void NextKit(){
    kitId++;
    if(kitId > 5){ kitId = 0; }
    switch(kitId){
      case 0: kit = "RANDOM"; break;
      case 1: kit = "GUNRUNNER"; break;
      case 2: kit = "RIFLEMAN"; break;
      case 3: kit = "SHOTGUNNER"; break;
      case 4: kit = "SWORDSMAN"; break;
      case 5: kit = "ASSASSIN"; break;
    }
  }
  
  /* Cycles through valid durations. */
  void NextDuration(){
    duration += 5;
    if(duration > 20){ duration = 5; }
  }
  
  /* Cycles through valid maps. */
  void NextMap(){
    mapIndex++;
    if(mapIndex > 1){ mapIndex = 0; }
    UpdateMap();
  }

  /* Changes map text based on map index. */
  void UpdateMap(){
    switch(mapIndex){
      case 0:
        map = "Arena_Empty";
        break;
      case 1:
        map = "Arena_Urban";
        break;
    }
  }
  
  /* Set arena options to session and begin arena mode */
  public void StartArena(){
    Data dat = new Data();
    dat.ints.Add(duration);
    dat.ints.Add(bots);
    dat.bools.Add(respawns);
    dat.strings.Add(kit);
    dat.bools.Add(teams);
    dat.bools.Add(p1red);
    dat.bools.Add(p2red);
    Session.session.arenaData = dat;
    
    manager.Change("NONE");
    Sound(0);
    SceneManager.LoadScene(map);
  }
  
  void TogglePlayers(){
    int players = Session.session.playerCount;
    if(players == 1){ players = 2; }
    else{ players = 1; }
    Session.session.playerCount = players;
  }
  public override void UpdateFocus(){}
  
  public override void Input(int button){}
}
