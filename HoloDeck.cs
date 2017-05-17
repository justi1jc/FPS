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
  int id = 0; // Id for multiple holodecks in a session. 
  int spawnDoor; // Door to derive spawnPoint from.
  public Vector3 spawnRot, spawnPos;
  public List<Actor> players;
  public List<Data> playerData;
  List<HoloCell> cells;
  HoloCell activeCell; // HoloCell that players will be placed into.
  
  /* Initialize */
  public void Start(){
    players = new List<Actor>();
    playerData = new List<Data>();
    cells = new List<HoloCell>();
    activeCell = null;
  }
  
  /* Requests an interior cell from the Session and loads it. */
  public void LoadInterior(
    string building, 
    string cellName,
    int door,
    int x, int y,
    bool saveFirst
  ){
    Cell c = Session.session.GetInterior(building, cellName, x, y);
    if(c != null){ LoadInterior(c, door, saveFirst); }
    else{ print("Could not find " + building + ":" +  name); }
  }
  
  /* Loads a given interior cell. */
  public void LoadInterior(Cell c, int door, bool saveFirst){
    //TODO
  }
  
  /* Updates interior in Session's data with current content. */
  public void SaveInterior(){
  }
  
  /* Recenters the HoloDeck on specified exterior.*/
  public void LoadExterior(
    int door,
    int x, int y,
    bool saveFirst
  ){
    //TODO
  }
  
  /* Clears the contents of all HoloCells. */
  public void ClearContents(){
  }
  
  /* Updates active cells in Session's map. */
  public void SaveExterior(int x, int y){
    for(int i = 0; i < cells.Count; i++){
      bool xmatch = x == cells[i].cell.x;
      bool ymatch = y == cells[i].cell.y;
      if(xmatch && ymatch){ 
        
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
  
  /* Adds a new player based on prefab type. */
  public void AddPlayer(string prefabName){
    //activeCell.CreateNPC(dat, false, true);
  }
  
  /* Convenience method for saving upon exit. */
  public List<Data> GetPlayers(){
    SavePlayers();
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
       activeCell.CreateNPC(playerData[i], false, true);
     }
     playerData = new List<Data>();
  }
}
