/*
    HoloDeck is responsible for loading cells from the session and updating
    the existing cells in the session. 
    
    Interior rendering consists of loading the contents of a single cell.
    
    Exterior rendering consists of loading a cell and a number of layers of
    cells surrounding it.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HoloDeck : MonoBehaviour{ 
  public bool interior; // True if an interior is currently loaded.
  public Cell deck; // Active cell
  public int id; // Id for multiple holodecks in a session.
  public Vector3 spawnRot, spawnPos;
  public List<Actor> players;
  public List<Data> playerData;
  List<HoloCell> cells;
  public HoloCell focalCell = null; // HoloCell that players will be placed into.
  
  /* Initialize */
  public void Awake(){
    id = Session.session.decks.IndexOf(this);
    players = new List<Actor>();
    playerData = new List<Data>();
    cells = new List<HoloCell>();
  }
  
  /* Requests an interior cell from the Session and loads it. */
  public void LoadInterior(
    string building, 
    string cellName,
    int door,
    int x, int y,
    bool saveFirst
  ){
    print("Loading interior " + cellName);
    Cell c = Session.session.GetInterior(building, cellName, x, y);
    if(c != null){ LoadInterior(c, door, saveFirst); }
    else{ print("Could not find " + building + ":" +  name + " at " + x + "," + y); }
  }
  
  /* Loads a given interior cell. */
  public void LoadInterior(Cell c, int door, bool saveFirst){
    print("Door id is" + door);
    SavePlayers();
    if(saveFirst && interior){ SaveInterior(); }
    else if(saveFirst){ SaveExterior(); }
    ClearContents();
    cells.Add(new HoloCell(transform.position, this));
    focalCell = cells[0];
    focalCell.LoadData(c, door);
    LoadPlayers();
  }
  
  /* Updates interior in Session's data with current content. */
  public void SaveInterior(){
    Cell c = focalCell.GetData();
    Session.session.SetInterior(c.building, c.displayName, c.x, c.y, c);
  }
  
  /* Recenters the HoloDeck on specified exterior and loads relevant cells.*/
  public void LoadExterior(
    int door, // If -1, ignores doors.
    int x, int y,
    bool saveFirst
  ){
    //TODO
  }
  
  /* Loads one particular exterior cell. */
  public void LoadExterior(){
    //TODO
  }
  
  /* Clears the contents of all HoloCells and deletes them. */
  public void ClearContents(){
    for(int i = 0; i < cells.Count; i++){ cells[i].Clear(); }
    cells = new List<HoloCell>();
    focalCell = null;
  }
  
  /* Updates all active exterior cells to Session's map. */
  public void SaveExterior(){
    for(int i = 0; i < cells.Count; i++){
      Cell c = cells[i].GetData();
      Session.session.SetExterior(c.x, c.y, c);
    }
  }
  
  /* Updates specified exterior, if it is active. */
  public void SaveExterior(int x, int y){
    for(int i = 0; i < cells.Count; i++){
      bool xmatch = x == cells[i].cell.x;
      bool ymatch = y == cells[i].cell.y;
      if(xmatch && ymatch){ 
        Cell c = cells[i].GetData();
        Session.session.SetExterior(x, y, c);
      }
    }
  }
  
  /* Returns a list of any exteriors within or adjacent to given coordinates. */
  public List<Cell> FindExteriors(int x, int y){
    MapRecord map = Session.session.map;
    List<Cell> cells = new List<Cell>();
    for(int i = 0; i < map.exteriors.Count; i++){
      Cell c = map.exteriors[i];
      if(Adjacent(c.x, c.y, x, y)){
        cells.Add(c);
      }
    }
    return cells;
  }
  
  /* Returns true if two xy pairs are next to one another. */
  public bool Adjacent(int x1, int y1, int x2, int y2){
    int xdiff = x1 - x2;
    int ydiff = y1 - y2;
    if(xdiff < 0){ xdiff *= -1; }
    if(ydiff < 0){ ydiff *= -1; }
    if(xdiff < 2 && ydiff < 2){ return true; }
    return false;
  }
  
  /* Associates this actor with this HoloDeck and returns true if player
     is in this deck. */
  public bool RegisterPlayer(Actor a){
    for(int i = 0; i < cells.Count; i++){
      if(cells[i].Contains(a.gameObject.transform.position)){ 
        players.Add(a);
        return true;
      }
    }
    return false;
  }
  
  /* Adds a new player to focal cell based on prefab. */
  public void AddPlayer(string prefabName){
    focalCell.AddPlayer(prefabName);
  }
  
  /* Adds a new player to the focal cell based on Data.*/
  public void AddPlayer(Data dat, bool ignoreDoor = false){
    if(focalCell == null){ print("Focal Cell null"); return; }
    focalCell.CreateNPC(dat, ignoreDoor, true);
  }
  
  /* Saves players and tags them with their last position. */
  public List<Data> GetPlayers(){
    SavePlayers();
    Cell lp = new Cell(focalCell.cell);
    lp.items = new List<Data>();
    lp.npcs = new List<Data>();
    for(int i = 0 ; i < playerData.Count; i++){
      playerData[i].lastPos = lp;
    }
    return playerData;
  }

  /* Updates playerData. */ 
  public void SavePlayers(){
     playerData = new List<Data>();
     for(int i = 0; i < players.Count; i++){
       playerData.Add(players[i].GetData());
     }
     players = new List<Actor>();
  }
 
  /* Loads players from playerData into the active cell. */
  public void LoadPlayers(){
     for(int i = 0; i < playerData.Count; i++){
       focalCell.CreateNPC(playerData[i], false, true);
     }
     playerData = new List<Data>();
  }
}
