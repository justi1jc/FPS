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
  List<Cell> interiors;
  List<Cell> exteriors;
  System.Random rand;
  int n, m; // Size of map.
  
  /* Convenience method. */
  public static MapRecord Generate(MapRecord map, int _n, int _m){
    Cartographer c = new Cartographer(map, _n, _m);
    return c.GenerateMap();
  }
  
  /* Uses the CellSaver to return acess of map from master file. */
  public static MapRecord GetMaster(){
    GameObject go = new GameObject();
    CellSaver cs = go.AddComponent<CellSaver>();
    cs.LoadMaster();
    MapRecord ret = cs.map;
    GameObject.Destroy(go);
    return ret;
  }
  
  public Cartographer(MapRecord _master, int _n, int _m){
    master = _master;
    interiors = new List<Cell>();
    exteriors = new List<Cell>();
    n = _n;
    m = _m;
    rand = new System.Random();
  }
  
  /* Produces a randomly-generated map */
  public MapRecord GenerateMap(){
    for(int i = 0; i < master.buildings.Count; i++){ PlaceBuilding(i); }
    FillInExteriors();
    MapRecord ret = new MapRecord();
    ret.exteriors = exteriors;
    ret.interiors = interiors;
    return ret;
  }
  
  /* Places a building at most one time in the world,
     given the index of the building in the master file. */
  public void PlaceBuilding(int b){
    List<int[]> footprint = GetFootprint(b);
    List<int[]> candidates = GetPlacementCandidates(footprint);
    int choice = rand.Next(0, candidates.Count);
    for(int i = 0; i < master.buildings[b].Length; i++){
      Cell c = master.buildings[b][i];
      c.x += candidates[choice][0];
      c.y += candidates[choice][1];
      interiors.Add(c);
      Cell e = GetMasterExterior(c.exteriorName);
      if(e != null){
        e.x = candidates[choice][0];
        e.y = candidates[choice][1];
        exteriors.Add(e);
      }
    }
  }
  
  /* Returns a list of coordinates covered by a building. */
  public List<int[]> GetFootprint(int b){
    List<int[]> ret =  new List<int[]>();
    for(int i = 0; i < master.buildings[b].Length; i++){
      int[] coord = new int[2];
      coord[0] = master.buildings[b][i].x;
      coord[1] = master.buildings[b][i].y;
      bool blank = master.buildings[b][i].exteriorName == "";
      if(!ret.Contains(coord) && !blank){ ret.Add(coord); }
    }
    return ret;
  }
  
  /* Returns a list of valid coordinates for placing a building with
     a particular footprint. */
  public List<int[]> GetPlacementCandidates(List<int[]> footprint){
    List<int[]> ret = new List<int[]>();
    for(int i = 0; i < n; i++){
      for(int j = 0; j < m; j++){
        if(ValidPlacement(footprint, i, j)){
          int[] coord = new int[2];
          coord[0] = i;
          coord[1] = j;
          ret.Add(coord);
        }
      }
    }
    return ret;
  }
  
  /* Returns true if each coord in the  */
  public bool ValidPlacement(List<int[]> footprint, int x, int y){
    for(int i = 0; i < footprint.Count; i++){
      int xr = x + footprint[i][0];
      int yr = y + footprint[i][1];
      if(xr > n || yr > m){ return false; }
      if(GetExterior(xr, yr) > -1){ return false; }
    }
    return true;
  }
  
  /* Places non-entrance exteriors randomly, filling in the map's grid.*/
  public void FillInExteriors(){
    List<Cell> ne = GetNonEntrances();
    for(int i = 0; i < n; i++){
      for(int j = 0; j < m; j++){
        int ext = GetExterior(i, j);
        if(ext == -1){
          int choice = rand.Next(0, ne.Count);
          Cell c = new Cell(ne[choice]);
          c.x = i;
          c.y = j;
          exteriors.Add(c);
        }
        
      }
    }
  }
  
  /* Returns a list of exteriors that are not entrances. */
  public List<Cell> GetNonEntrances(){
    List<Cell> ret = new List<Cell>();
    for(int i = 0; i < master.exteriors.Count; i++){
      if(!master.exteriors[i].entrance){ ret.Add(master.exteriors[i]); }
    }
    return ret;
  }
  
  /* Returns the index of a given interior, or -1. */
  public int GetInterior(int x, int y){
    int found = -1;
    for(int i = 0; i < interiors.Count; i++){
      if(interiors[i].x == x && interiors[i].y == y){ found = i; break; }
    }
    return found;
  }

  /* Returns the index of a given exterior, or -1. */
  public int GetExterior(int x, int y){
    int found = -1;
    for(int i = 0; i < exteriors.Count; i++){
      if(exteriors[i].x == x && exteriors[i].y == y){ found = i; break; }
    }
    return found;
  }
  
  /* Returns an exterior from the Master based on name */
  public Cell GetMasterExterior(string name){
    if(name == ""){ return null; }
    for(int i = 0; i < master.exteriors.Count; i++){
      if(master.exteriors[i].displayName == name){
        return master.exteriors[i];
      }
    }
    return null;
  }
}
