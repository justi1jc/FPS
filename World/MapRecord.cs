/*
   The Map record is used to store the the game's Cell data for the purpose of
   initializing a GameRecord and generating interiors. 
*/

using System.Collections.Generic;

[System.Serializable]
public class MapRecord{
  
  public List<Building> buildings; // All possible buildings.
  public List<Cell> exteriors; // All possible exteriors.
  public int width, height; // Size of overworld in cells.
  public int bldgId, extId;
  public MapRecord(){
    bldgId = extId = 0;
    buildings = new List<Building>();
    exteriors = new List<Cell>();
  }
  
  public int NextBuildingId(){
    int ret = bldgId;
    bldgId++;
    return ret;
  }
  
  public int NextExteriorId(){
    int ret = extId;
    extId++;
    return ret;
  }
  
  public string ToString(){
    string str = "Buildings:";
    foreach(Building b in buildings){ str += b.ToString(); }
    str += "Exteriors:";
    foreach(Cell e in exteriors){ str += e.ToString(); }
    return str;
  }
}
