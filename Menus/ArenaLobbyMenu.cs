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
    dat.bools.Add(respawns);
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
