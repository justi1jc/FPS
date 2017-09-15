/*
        The cartographer is responsible for accessing the master map as well as
        for generating new maps.
*/

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cartographer{
  MapRecord master;
  MapRecord derived; // The map derived from the master file.
  System.Random rand;
  int n, m; // Size of map.
  public static bool fileAccess = false;
  
  /* Constructor for map generation. */
  public Cartographer(int n, int m){
    this.master = Cartographer.GetMaster();
    this.n = n;
    this.m = m;
    rand = new System.Random();
  }
  
  /* Constructor for processing derived map. */
  public Cartographer(MapRecord derived){
    this.derived = derived;
  }
  
  
  /* Convenience method for retrieving master file. */
  public static MapRecord GetMaster(){
    string path = Application.dataPath + "/Resources/world.master";
    MapRecord ret = null;
    if(!File.Exists(path)){
      MonoBehaviour.print("File did not exist.");
      return new MapRecord();
    }
    if(fileAccess){ 
      MonoBehaviour.print("File access already in progress."); 
      return ret; 
    }
    fileAccess = true;
    using(FileStream file = File.Open(path, FileMode.Open)){
      BinaryFormatter bf = new BinaryFormatter();
      ret = (MapRecord)bf.Deserialize(file);
      file.Close();
      fileAccess = false;
    }
    return ret;
  }
  
  public static MapRecord GetMap(int n, int m){
    Cartographer cart = new Cartographer(n, m);
    cart.GenerateMap();
    return cart.derived;
  }
  
  /* Produces a randomly-generated map */
  public void GenerateMap(){
    derived = new MapRecord();
    PlaceBuildings();
    FillInOverworld();
    derived.width = n;
    derived.height = m;
  }
  
  /* Randomly places all exteriors with doors, the buildings attached, and the 
    exteriors attached to said buildings, linking them.
  */
  public void PlaceBuildings(){
    List<Cell> exteriors = GetUnlinked(false);
    List<Building> buildings = GetBuildings(exteriors);
    foreach(Building b in buildings){ PlaceBuilding(b); }
  }
  
  /* Return true if this building will not leave the edge of the map */
  public bool CanPlaceBuilding(Building b, int x, int y){
    foreach(DoorRecord dr in b.doors){
      int dx = dr.x + x;
      int dy = dr.y + y;
      if(dx < 0 || dy < 0 || dx >= n || dy >= m){ 
        return false; 
      }
    }
    return true;
  }
  
  /* Places a building at a particular position.
     placing any missing exteriors at the same time. */
  public void PlaceBuilding(Building b){
    List<Cell> existing = ExistingExteriors(b);
    int x, y;
    x = y = 0;
    if(existing.Count > 0){
      foreach(Cell c in existing){
        if(Link(b, c)){
          x = c.x;
          y = c.y;
        }
      }
      FillInUnlinked(b, x, y);
      derived.buildings.Add(b);
      return;
    }
    int[] coords = BuildingCoords(b);
    if(coords[0] == -1 && coords[1] == -1){
      MonoBehaviour.print("Couldnt place " + b.name);
      return;
    }
    FillInUnlinked(b, coords[0], coords[1]);
    derived.buildings.Add(b);
  }
  
  /* Returns random valid coordinates to place a building, or [-1,-1] */
  public int[] BuildingCoords(Building b){
    List<int[]> valid = new List<int[]>();
    for(int x = 0; x < n; x++){
      for(int y = 0; y < m; y++){
        if(CanPlaceBuilding(b, x, y)){
          int[] coords = new int[2];
          coords[0] = x;
          coords[1] = y;
          valid.Add(coords);
        }
      }
    }
    if(valid.Count == 0){
      int[] ret = {-1,-1};
      MonoBehaviour.print("Couldn't place" + b.name);
      return ret;
    }
    int choice = rand.Next(0, valid.Count);
    return valid[choice];
  }
  
  /* Places remaining unlinked exteriors that aren't on the map at provided
    coords. */
  public void FillInUnlinked(Building b, int x, int y){
    foreach(DoorRecord dr in b.doors){
      if(dr.exteriorFacing && dr.destId == -1){
        Cell c = GetMasterExterior(dr.destName);
        c.x = x + dr.x;
        c.y = y + dr.y;
        derived.exteriors.Add(c);
      }
    }
  }
  
  /* Returns all existing exteriors with unlinked doors leading to this
     building. */
  public List<Cell> ExistingExteriors(Building b){
    List<Cell> ret = new List<Cell>();
    foreach(Cell c in derived.exteriors){
      if(CanLink(c, b)){
        ret.Add(c);
      }
    }
    return ret;
  }
  
  
  /* Returns true if exterior has an unlinked door leading to this building */
  public bool CanLink(Cell c, Building b){
    foreach(DoorRecord dr in c.doors){
      if(!dr.exteriorFacing && dr.destName == b.name  && dr.building == -1){
        return true;
      }
    }
    return false;
  }
  
  /* Returns a set of buildings connected to the provided cells. */
  public List<Building> GetBuildings(List<Cell> cells){
    List<string> names = new List<string>();
    foreach(Cell c in cells){
      foreach(DoorRecord dr in c.doors){
        if(!dr.exteriorFacing && names.IndexOf(dr.destName) == -1){
          names.Add(dr.destName);
        }
        else if(names.IndexOf(dr.destName) == -1){
          MonoBehaviour.print(dr.destName + " not added " + dr.exteriorFacing);
        }
      }
    }
    List<Building> buildings = new List<Building>();
    foreach(string name in names){
      Building b = GetMasterBuilding(name);
      if(b == null){ MonoBehaviour.print(name + " is null."); }
      else{ buildings.Add(b); }
    }
    return buildings;
  }
  
  /* Places an unlinked exterior with its building, */
  public void PlaceUnlinked(Cell c){
    List<Building> buildings = GetLinkedBuildings(c);
    foreach(Building b in buildings){ 
      Link(b, c);
      derived.buildings.Add(b);
    }
    derived.exteriors.Add(c);
  }
  
  /* Ensures all doors are supplied with correct ids. 
     Returns true if this cell is the building's 0,0 coorinate.
  */
  public bool Link(Building b, Cell c){
    bool ret = false;
    if(b.id == -1){ 
      b.id = derived.NextBuildingId();
      foreach(Cell room in b.rooms){ room.id = b.id; } 
    }
    if(c.id == -1){ c.id = derived.NextExteriorId(); }
    foreach(DoorRecord dr in b.doors){
      if(dr.exteriorFacing && dr.destName == c.name){
        DoorRecord drLink = c.GetDoor(dr.destId);
        if(drLink != null){
          if(dr.x == 0 && dr.y == 0){ ret = true; }
          dr.x = c.x;
          dr.y = c.y;
        }
        drLink.building = b.id;
      }
    }
    return ret;
  }
  
  /* Returns the buildings on the other side of an exterior's doors. */
  public List<Building> GetLinkedBuildings(Cell c){
    List<Building> ret = new List<Building>();
    foreach(DoorRecord dr in c.doors){
      if(!dr.exteriorFacing){ 
        ret.Add(GetMasterBuilding(dr.destName)); 
      }
    }
    return ret;
  }
  
  
  
  /* Places non-entrance exteriors randomly, filling in the map's grid.*/
  public void FillInOverworld(){
    List<Cell> nd = GetUnlinked();
    if(nd.Count == 0){ 
      MonoBehaviour.print("No unlinked exteriors detected; aborting.");
      return;
    }
    for(int i = 0; i < n; i++){
      for(int j = 0; j < m; j++){
        if(GetExterior(i, j) == null){
          int choice = rand.Next(0, nd.Count);
          Cell c = new Cell(nd[choice]);
          c.x = i;
          c.y = j;
          PlaceUnlinked(c);
        }
      }
    }
  }
  
  /* Returns a particular from the derived map at a given coordinate. */
  public Cell GetExterior(int x, int y){
    foreach(Cell c in derived.exteriors){
      if(x == c.x && y == c.y){ return c; }
    }
    return null;
  }
  
  /* Returns exterior from derived map based on ID. */
  public Cell GetExterior(int id){
    foreach(Cell c in derived.exteriors){
      if(id == c.id){ return c; }
    }
    return null;
  }
  
  /* Returns building from derived map based on ID. */
  public Building GetBuilding(int id){
    foreach(Building b in derived.buildings){
      if(id == b.id){ return b; }
    }
    return null;
  }
  
  /* Returns all cells whose unlinked status matches the argument. */
  public List<Cell> GetUnlinked(bool val = true){
    List<Cell> ret = new List<Cell>();
    foreach(Cell c in master.exteriors){
      if(Unlinked(c) == val){ ret.Add(c); }
    }
    return ret;
  }
  
  /* Returns true if cell's buldings are contained within that cell. */
  public bool Unlinked(Cell c){
    if(c == null){ MonoBehaviour.print("Cell null"); return true; }
    foreach(DoorRecord ed in c.doors){
      Building b = GetMasterBuilding(ed.destName);
      if(b == null){ MonoBehaviour.print(ed.destName + " is null"); return true; }
      foreach(DoorRecord bd in b.doors){
        if(bd.exteriorFacing && bd.destName != c.name){
          return false; 
        }
      }
    }
    return true;
  }
  
  /* Returns an exterior from the Master based on name */
  public Cell GetMasterExterior(string name){
    if(name == ""){ return null; }
    for(int i = 0; i < master.exteriors.Count; i++){
      if(master.exteriors[i].name == name){
        return new Cell(master.exteriors[i]);
      }
    }
    return null;
  }
  
  /* Returns a Building from the Master based on name.*/
  public Building GetMasterBuilding(string building){
    if(building == ""){ return null; }
    for(int i = 0; i < master.buildings.Count; i++){
      if(master.buildings[i].name == building){
        return new Building(master.buildings[i]);
      }
    }
    return null;
  }
  
  /* Returns a particular room in a building in the Master file. */
  public Cell GetMasterRoom(string building, string room){
    Building bldg = GetMasterBuilding(building);
    if(bldg == null || room == ""){ return null; }
    for(int i = 0; i < bldg.rooms.Count; i++){
      if(bldg.rooms[i].name == room){
        return new Cell(bldg.rooms[i]);
      }
    }
    return null;

  }
}
