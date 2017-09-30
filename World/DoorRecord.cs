/*
  A DoorRecord more conveniently stores a particular Door's data that is
  relevant to mapping specific Rooms to exteriors.
  
*/

using System.Collections.Generic;
[System.Serializable]

public class DoorRecord{
  public int x, y; // Position relative to center door of a building.
  public int building = -1; // Id of destination building.
  public string destName; // Name of destination exterior/building.
  public string room; // Name of destination room within building.
  
  public bool linked = false;
  public bool exterior; // True if this door is in a cell, false if in a building.
  public bool exteriorFacing; // True if this door leads to the overworld 
  
  public int destId = -1; // Id of destination door.
  public int id; // Id of this door within its room. 
  
  /* WarpDoor constructor. */
  public DoorRecord(WarpDoor wd){
    x = wd.x;
    y = wd.y;
    building = wd.building;
    destName = wd.destName;
    room = wd.room;
    
    exterior = wd.exterior;
    exteriorFacing = wd.exteriorFacing;
    
    destId = wd.destId;
    id = wd.id;
  }
  
  /* Cloning constructor */
  public DoorRecord(DoorRecord dr){
    x = dr.x;
    y = dr.y;
    building = dr.building;
    destName = dr.destName;
    room = dr.room;
    
    exterior = dr.exterior;
    exteriorFacing = dr.exteriorFacing;
    
    destId = dr.destId;
    id = dr.id;
  }
  
  public DoorRecord(){}
  
  
}
