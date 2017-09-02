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
    interior = true;
  }
  
  public void Update(){
    if(!interior){ ManageShifting(); }
    if(Input.GetKeyDown(KeyCode.P)){
      Vector3 dir = new Vector3(0,0,-100);
      //PositionShift(dir);
    }
  }
  
  /* Shifts the position of cells such that the cells closest to the destination
     are moved first, averting cells merging contents.*/
  void PositionShift(Vector3 dir){
    List<HoloCell> cs = new List<HoloCell>(cells);
    List<HoloCell> sorted = new List<HoloCell>();
    Vector3 dest = focalCell.position + 10*dir; // Points in direction of movement.
    while(cs.Count > 0){
      HoloCell min = cs[0];
      for(int i = 1; i < cs.Count; i++){
        float mdist = Vector3.Distance(dest, min.position);
        float cdist = Vector3.Distance(dest, cs[i].position);
        if(cdist < mdist){ min = cs[i]; }
      }
      cs.Remove(min);
      sorted.Add(min);
    }
    for(int i = 0; i < sorted.Count; i++){ sorted[i].Move(dir); }
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
    else{ print("Could not find " + building + ":" +  name + " at " + x + "," + y); }
  }
  
  /* Loads a given interior cell. */
  public void LoadInterior(Cell c, int door, bool saveFirst){
    MonoBehaviour.print("method stub");
    /*
    SavePlayers();
    if(saveFirst && interior){ SaveInterior(); }
    else if(saveFirst){ SaveExterior(); }
    ClearContents();
    cells.Add(new HoloCell(transform.position, this));
    focalCell = cells[0];
    focalCell.LoadData(c, door);
    LoadPlayers();
    interior = true;
    */
  }
  
  /* Updates interior in Session's data with current content. */
  public void SaveInterior(){
    MonoBehaviour.print("method stub");
    /*
    Cell c = focalCell.GetData();
    Session.session.SetInterior(c.building, c.displayName, c.x, c.y, c);
    */
  }
  
  /* Recenters the HoloDeck on specified exterior and loads relevant cells.
     This is called as the result of a warp door being used.
  */
  public void LoadExterior(
    int door, // If -1, ignores doors.
    int x, int y,
    bool saveFirst
  ){
    SavePlayers();
    if(saveFirst && interior){ SaveInterior(); }
    else if(saveFirst){ SaveExterior(); }
    ClearContents();
    for(int i = 0; i < 3; i++){
      for(int j = 0; j < 3; j++){
        int cx = x - 1 + i;
        int cy = y - 1 + j; 
        Cell c = Session.session.GetExterior(cx, cy);
        HoloCell hc = AddExteriorCell(i,j);
        if(i == 1 && j == 1){
          hc.LoadData(c, door);
          focalCell = hc;
        }
        else{ hc.LoadData(c); }
      }
    }
    LoadPlayers();
    interior = false;
    BuildWalls();
  }
  
  /* Listens for the player leaving the active cell. In the event that this
     happens, the holodeck shifts its focus to the player's current position. */
  public void ManageShifting(){
    if(players == null){ return; }
    if(players[0] != null && !focalCell.Contains(players[0].transform.position)){
      ShiftExterior();
    }
  }
  
  
  /* Instantiates a cell to the correct position, adds it to cells, 
     and returns it. */
  HoloCell AddExteriorCell(int x, int y){
    HoloCell hc = new HoloCell(GridPosition(x,y), this);
    cells.Add(hc);
    return hc;
  }
  
  /* Returns the cell that contains this pos, or null. */
  public HoloCell ContainingCell(Vector3 pos){
    float DISTANCE = 50;
    for(int i = 0; i < cells.Count; i++){
      HoloCell hc = cells[i];
      if(hc.Contains(pos)){ return hc; }
    }
    return null;
  }
  
  /* Re-centers the holodeck around the player's location, pruning
     HoloCells that are not adjacent whilst loading any additional adjacent
     HoloCells. */
  public void ShiftExterior(){
    HoloCell fc = focalCell;
    focalCell = ContainingCell(players[0].transform.position);
    ClearWalls();
    Prune();
    Expand();
    Vector3 dir = transform.position - focalCell.position;
    PositionShift(dir);
    BuildWalls();
  }
  
  /* Removes all non-adjacent Cells. */
  void Prune(){
    List<HoloCell> orphans = new List<HoloCell>();
    for(int i = 0; i < cells.Count; i++){
      HoloCell hc = cells[i];
      if(!CellAdjacency(focalCell, hc)){
        Session.session.SetExterior(hc.GetData());
        hc.Clear();
        orphans.Add(hc);
      }
    }
    for(int i = 0; i < orphans.Count; i++){ cells.Remove(orphans[i]); }
  }
  
  /* Loads any unloaded adjacent cells. */
  void Expand(){
    List<int[]> neighbors = Adjacencies(focalCell.cell.x, focalCell.cell.y);
    for(int i = 0; i < neighbors.Count; i++){
      int[] ne = neighbors[i];
      if(!Loaded(ne[0], ne[1])){
        int[] fr = FocalRelation(ne[0], ne[1]); 
        HoloCell hc = AddExteriorCell(fr[0], fr[1]);
        Cell c = Session.session.GetExterior(ne[0], ne[1]);
        hc.LoadData(c);
      }
    }
  }
  
  /* Returns true if this cell is already loaded. */
  bool Loaded(int x, int y){
    for(int i = 0; i < cells.Count; i++){
      bool exists = cells[i].cell != null;
      if(exists && x == cells[i].cell.x && y == cells[i].cell.y){ return true; }
    }
    return false;
  }
  
  /* Returns the 8 coords that should surround the given coordinates. */
  List<int[]> Adjacencies(int x, int y){
    List<int[]> ret = new List<int[]>();
    for(int i = -1; i < 2; i++){
      for(int j = -1; j < 2; j++){
        if(j != 0 || i != 0){ 
          int[] coord = {x+i, y+j};
          ret.Add(coord); 
        }
      }
    }
    return ret;
  }
  
  /* Returns the relative 3x3 grid position of a cell to a center cell.*/
  int[] GridRelation(HoloCell center, HoloCell other){
    int[] ret = new int[2];
    if(center.cell == null || other.cell == null){ return null; }
    ret[0] = other.cell.x - center.cell.x; 
    ret[1] = other.cell.y - center.cell.y;
    ret[0] += 1; // This pushes the grid from [-1,1] to [0,2]
    ret[1] += 1;
    return ret;
  }
  
  /* Returns the relative 3x3 grid position of a cell to the focalCell*/
  int[] FocalRelation(int x, int y){
    int[] ret = new int[2];
    ret[0] = x - focalCell.cell.x; 
    ret[1] = y - focalCell.cell.y;
    ret[0] += 1; // This pushes the grid from [-1,1] to [0,2]
    ret[1] += 1;
    return ret;
  }
  
  /* Given two coordinates, returns whether they are adjacent. */
  bool GridAdjacency(int[] first, int[] second){
    int x = first[0] - second[0];
    int y = first[1] - second[1];
    if(x < 0){ x *= -1; }
    if(y < 0){ x *= -1; }
    if(x < 2 && y < 2){ return true; }
    return false;
  }

  /* Compares two Cells using GridAdjacency. */
  bool CellAdjacency(HoloCell a, HoloCell b){
    int DISTANCE = 150; // Distance between two cells, allowing corners.
    float dist = Vector3.Distance(a.position, b.position);     
    if(dist < DISTANCE){ return true; }
    return false;
  }
  
  /* Returns an in-scene position according to a 3x3 grid. */
  Vector3 GridPosition(int x, int y){
    float xpos = focalCell != null ? focalCell.position.x : transform.position.x;
    float zpos = focalCell != null ? focalCell.position.z : transform.position.z;
    if(x == 2){ xpos += 100; }
    else if(x == 0){ xpos -= 100; }
    if(y == 2){ zpos += 100; }
    else if(y == 0){ zpos -= 100; }
    Vector3 ret = new Vector3(xpos, transform.position.y, zpos);
    Vector3 ter = focalCell != null ? focalCell.position : transform.position;    
    //print("Moving from " + ter + " to " + ret);
    return ret;
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
      if(c != null){ Session.session.SetExterior(c.x, c.y, c); }
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
  
  /* Returns all active actors. */
  public List<Actor> GetActors(){
    List<Actor> ret = new List<Actor>();
    for(int i = 0; i < cells.Count; i++){
      ret.AddRange(cells[i].GetActors());
    }
    return ret;
  }
  
  /* Loads players from playerData into the active cell. */
  public void LoadPlayers(){
     for(int i = 0; i < playerData.Count; i++){
       if(focalCell != null){ focalCell.CreateNPC(playerData[i], false, true); }
     }
     playerData = new List<Data>();
  }
  
  /* Apply invisible walls to exterior holocells that need them. */
  public void BuildWalls(){
    for(int i = 0; i < cells.Count; i++){ BuildWalls(cells[i]); }
  }
  
  /* Builds walls for individual exterior cell */
  public void BuildWalls(HoloCell c){
    if(c.cell == null){ return; }
    int x = c.cell.x;
    int y = c.cell.y;
    List<int[]> sides = new List<int[]>(); // neighboring coordinates
    sides.Add(new int[]{x+1,y});
    sides.Add(new int[]{x-1,y});
    sides.Add(new int[]{x,y+1});
    sides.Add(new int[]{x,y-1});
    Session s = Session.session;
    for(int i = 0; i < sides.Count; i++){
      bool needWall = !Loaded(sides[i][0], sides[i][1]);
      if(needWall){ c.BuildWall(i); }
    }
  }
  
  /* Tear down existing invisible walls. */
  public void ClearWalls(){
    for(int i = 0; i < cells.Count; i++){ cells[i].ClearWalls(); }
  }
}
