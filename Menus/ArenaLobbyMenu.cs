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
  private int mapIndex; // Index of currently selected map.
  private bool respawns = true; // True if players will respawn.
  private bool spawnWeapons = false; // True if weaponSpawners will be active.
  private int bots = 15; // Number of bots;
  private string kit = "NONE";
  private int kitId = 0;
  private bool teams = false;
  private bool p1red = false; // True if player1 is on the red team.
  private bool p2red = false; // True if player2 is on the red team.
  private List<Kit> kits;
  private int gameMode = (int)Arena.GameModes.Deathmatch;
  private List<ArenaMap> maps;
  
  
  public ArenaLobbyMenu(MenuManager manager) : base(manager){
    duration = 10;
    mapIndex = 0;
    kits = new List<Kit>();
    for(int i = 0; i < 6; i++){
      Kit k = Kit.LoadKit(i);
      if(k != null){ kits.Add(k); }
    }
    kits.AddRange(Session.session.GetKits());
    maps = MapsByMode((Arena.GameModes)gameMode);
    UpdateGameMode();
    PrevKit();
  }

  public override void Render(){
    int ih = Height()/10;
    int iw = Width()/5;
    int x, y, h, w;
    Box("Arena Lobby", 2*iw, 0, iw, ih);
    string str = "Players: " + Session.session.playerCount;
    if(Button(str, 0, ih, iw, ih)){ 
      TogglePlayers(); 
      Sound(0); 
    }
    str = "GameMode:" + Arena.GameModeName((Arena.GameModes)gameMode);
    if(Button(str, 0, 2*ih, iw, ih)){ NextGameMode(); }
    RenderMap(iw, ih);
    if(Button("Start", Width()-iw, Height()-ih, iw, ih)){ StartArena(); }
    if(Button("Back", 0, Height()-ih, iw, ih)){ 
      manager.Change("MAIN");
      Sound(0);
    }
    switch((Arena.GameModes)gameMode){
      case Arena.GameModes.Deathmatch: RenderDeathmatch(); break;
    }
    
  }
  
  /* Renders config options relevant to deathmatch mode. */
  public void RenderDeathmatch(){
    int ih = Height()/10;
    int iw = Width()/5;
    int x, y, w, h;
    string str;
    
    
    str = "Teams: " + (teams ? "Yes" : "No");
    if(Button(str, 0, 3*ih, iw, ih )){ teams = !teams; }
    
    str = "Duration:" + duration;
    Box(str, 0, 5*ih, iw, ih/2);
    x = 0;
    y = 5*ih + ih/2;
    w = iw;
    h = ih/2;
    duration = (int)GUI.HorizontalSlider(new Rect(x, y, w, h), duration, 1, 60);
    
    str = "Bots: " + bots;
    Box(str, 0, 6*ih, iw, ih/2);
    x = 0;
    y = 6*ih + ih/2;
    w = iw;
    h = ih/2;
    
    bots = (int)GUI.HorizontalSlider(new Rect(x,y,w,h), bots, 0, 128);
    
    str = "Kit: " + kit;
    if(Button(str, 0, 7*ih, iw, ih)){ NextKit(); }
    
    str = "Customize kits";
    if(Button(str, 0, 8*ih, iw, ih)){ manager.Change("EDITKITMENU"); }
    
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
  
  
  /* Returns a list of maps compatible with the given gamemode */
  public List<ArenaMap> MapsByMode(Arena.GameModes gameMode){
    List<ArenaMap> allMaps = ArenaMap.GetMaps();
    List<ArenaMap> ret = new List<ArenaMap>();
    foreach(ArenaMap map in allMaps){
      if(map.CompatibleMode((Arena.GameModes)gameMode)){ ret.Add(map); }
    }
    return ret;
  }
  
  /* Renders the selected map's buttons, boxes, and thumbnail. */
  public void RenderMap(int iw, int ih){
    if(mapIndex < 0 || mapIndex >= maps.Count || maps[mapIndex] == null){
      return;
    }
    ArenaMap map = maps[mapIndex];
    string str = "Map:" + map.name;
    if(Button(str, 0, 4*ih, iw, ih)){ NextMap(); Sound(0);}
    if(map.thumbnail != null){ Box(map.thumbnail, iw, 2*ih, 4*iw, 7*ih); }
    else{ MonoBehaviour.print( map.name + " has null thumbnail"); }
  }
  
  /* Cycles through valid maps. */
  private void NextMap(){
    mapIndex++;
    if(mapIndex >= maps.Count){ mapIndex = 0; }
  }
  
  
  /* Cycles through available kits. */
  private void NextKit(){
    kitId++;
    if(kitId >= kits.Count){ kitId = 0; }
    if(kits.Count == 0){ kit = "NONE"; }
    else{ kit = kits[kitId].name; }
  }
  
  /* Cycles through available kits. */
  private void PrevKit(){
    kitId--;
    if(kitId < 0){ kitId = 0; }
    if(kits.Count == 0){ kit = "NONE"; }
    else{ kit = kits[kitId].name; }
  }
  
  /* Cycles through gamemodes. */
  private void NextGameMode(){
    gameMode++;
    if(gameMode > (int)Arena.GameModes.Deathmatch){
      gameMode = (int)Arena.GameModes.Deathmatch; 
    }
    UpdateGameMode();
    maps = MapsByMode((Arena.GameModes)gameMode);
    mapIndex = 0;
  }
  
  /* Configures games according to gamemode. */
  private void UpdateGameMode(){
  }
  
  /* Set arena options to session and begin arena mode */
  public void StartArena(){
    Session.session.DestroyMenu();
    Data dat = null;
    switch((Arena.GameModes)gameMode){
      case Arena.GameModes.Deathmatch: dat = GetDeathmatchData(); break;
    }
    
    Session.session.arenaData = dat;
    manager.Change("NONE");
    Sound(0);
    SceneManager.LoadScene(maps[mapIndex].name);
  }
  
  /* Returns data formatted for the deathmatch gamemode. */
  private Data GetDeathmatchData(){
    Data ret = new Data();
    ret.ints.Add(gameMode);
    ret.ints.Add(duration);
    ret.ints.Add(bots);
    ret.strings.Add(kit);
    ret.bools.Add(respawns);
    ret.bools.Add(teams);
    ret.bools.Add(p1red);
    ret.bools.Add(p2red);
    ret.bools.Add(spawnWeapons);
    return ret;
  }
  
  void TogglePlayers(){
    int players = Session.session.playerCount;
    if(players == 1){ players = 2; }
    else{ players = 1; }
    Session.session.playerCount = players;
  }
  public override void UpdateFocus(){}
  
  public override void Input(Buttons button){}
}
