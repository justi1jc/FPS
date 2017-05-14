/*
        The cartographer takes the master map file that contains one of each
        type of cell and creates a map by placing them randomly across a grid.
*/

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;

public class Cartographer{
  MapRecord master;
  List<Cell> interiors;
  List<Cell> exteriors;
  
  public Cartographer(MapRecord _master){
    master = _master;
    interiors = new List<Cell>();
    exteriors = new List<Cell>();
  }
  
  /* Produces a map on a grid of n width and m length. */
  public MapRecord Generate(int n, int m){
    return null;
  }
  
  /* Places a building at most one time in the world,
     given the index of the building in the master file. */
  public void PlaceBuilding(int b){
  }
  
  
  /* Places non-entrance exteriors randomly, filling in the map's grid.*/
  public void FillInExteriors(){
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
  
  
}
