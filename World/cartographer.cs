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
    //List<Cell> exteriors = GetUnlinked(false);
    //List<Building> buildings = GetBuildings(exteriors);
    //foreach(Building b in buildings){ PlaceBuilding(b); }
  }
  
  /* Places a building at a particular position.
     placing any missing exteriors at the same time. */
  public void PlaceBuilding(Building b){
    
  }

  /* Places non-entrance exteriors randomly, filling in the map's grid.*/
  public void FillInOverworld(){
    /*
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
    */
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
