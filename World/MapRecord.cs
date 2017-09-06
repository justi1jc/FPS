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
  public int bldgId, extId, npcId;
  private readonly object syncLock = new object(); // Mutex lock
  public World world;
  
  public MapRecord(){
    world = new World();
    bldgId = extId = npcId = 0;
    buildings = new List<Building>();
    exteriors = new List<Cell>();
  }
  
  public int NextBuildingId(){
    int ret = bldgId;
    lock(syncLock){ bldgId++; }
    return ret;
  }
  
  public int NextExteriorId(){
    int ret = extId;
    lock(syncLock){ extId++; }
    return ret;
  }
  
  public int NextNPCId(){
    int ret = npcId;
    lock(syncLock){ npcId++; }
    return ret;
  }
  
  public string ToString(){
    string str = "Buildings:" + buildings.Count + " ";
    str += "Exteriors: " + exteriors.Count + " ";
    return str;
  }
}
