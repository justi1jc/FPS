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
    //PlaceBuildings();
    FillInOverworld();
    derived.width = n;
    derived.height = m;
  }
  
  /* Removes the cells from this list. */
  public List<Cell> RemoveRange(List<Cell> a, List<Cell> b){
    a = new List<Cell>(a);
    foreach(Cell c in b){
      a.Remove(c);
    }
    return a;
  }
  
  
  /* Randomly places all exteriors with doors, the buildings attached, and the 
    exteriors attached to said buildings, linking them.
  */
  public void PlaceBuildings(){
    List<Cell> exteriors = GetUnlinked(false);
    MonoBehaviour.print("Placing " + exteriors.Count + " linked exteriors.");
    while(exteriors.Count > 0){
      exteriors = new List<Cell>();
      List<Cell> placed = PlaceLinked(exteriors[0]);
      exteriors = RemoveRange(exteriors, placed);
    }
  }
  
  
  /* Places a grouping of exteriors connected to this one, returning all
     connected exteriors.
  */
  public List<Cell> PlaceLinked(Cell c){
    return new List<Cell>();
    
  }
  
  
  /* Returns (through out variables) the buildings and exteriors connected to
     this one. */
  public void LinkedExteriors(out List<Cell> exteriors, out List<Building> buildings){
    exteriors = null;
    buildings = null;
  }
  
  /* Places an unlinked exterior with its building, */
  public void PlaceUnlinked(Cell c){
    MonoBehaviour.print("Placing unlinked " + c.name);
    List<Building> buildings = GetLinkedBuildings(c);
    foreach(Building b in buildings){ 
      Link(b, c); 
      derived.buildings.Add(b);
    }
    derived.exteriors.Add(c);
  }
  
  /* Ensures all doors are supplied with correct ids. */
  public void Link(Building b, Cell c){
    if(b.id == -1){ b.id = derived.NextBuildingId(); }
    if(c.id == -1){ c.id = derived.NextExteriorId(); }
    foreach(DoorRecord dr in b.doors){
      if(dr.exteriorFacing && dr.destName == c.name){
        DoorRecord drLink = c.GetDoor(dr.destId);
        if(drLink != null){
          dr.x = c.x;
          dr.y = c.y;
        }
        drLink.building = b.id;
      }
    }
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
    MonoBehaviour.print(master.ToString());
    List<Cell> ret = new List<Cell>();
    foreach(Cell c in master.exteriors){
      if(Unlinked(c) == val){ ret.Add(c); }
    }
    return ret;
  }
  
  /* Returns true if cell's buldings are contained within that cell. */
  public bool Unlinked(Cell c){
    foreach(DoorRecord ed in c.doors){
      Building b = GetMasterBuilding(ed.destName);
      foreach(DoorRecord bd in b.doors){
        MonoBehaviour.print(c.name + ":" + b.name  + ":" + bd.destName );
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
        return master.exteriors[i];
      }
    }
    return null;
  }
  
  /* Returns a Building from the Master based on name.*/
  public Building GetMasterBuilding(string building){
    if(building == ""){ return null; }
    for(int i = 0; i < master.buildings.Count; i++){
      if(master.buildings[i].name == building){
        return master.buildings[i];
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
        return bldg.rooms[i];
      }
    }
    return null;

  }
}
