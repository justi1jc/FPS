/*
  A building contains a collection of cells and DoorRecords for the
  purpose of mapping exterior-facing doors within rooms to their interior-facing
  counterparts within Cells ofs the overworld.
*/

using System.Collections.Generic;
[System.Serializable]

public class Building{
  public int x, y; // Center position on overworld.
  public string name;
  public int id = -1;
  public List<DoorRecord> doors;
  public List<Cell> rooms;
  
  /* Cloning constructor */
  public Building(Building b){
    x = b.x;
    y = b.y;
    id = b.id;
    name = b.name;
    doors = new List<DoorRecord>(b.doors);
    rooms = new List<Cell>(b.rooms);
  }
  
  public Building(){
    doors = new List<DoorRecord>();
    rooms = new List<Cell>();
  }
  
  /* Returns true if this building has exterior doors across multiple cells. */
  public bool MultiCell(){
    if(doors.Count < 2){ return false; }
    List<DoorRecord> efDoors = ExteriorFacingDoors();
    if(efDoors.Count < 2){ return false; }
    DoorRecord dr = efDoors[0];
    for(int i = 1; i < efDoors.Count; i++){
      if(efDoors[i].x != dr.x || efDoors[i].y != dr.y){ return true; }
    }
    return false;
  }
  
  public List<DoorRecord> ExteriorFacingDoors(){
    List<DoorRecord> ret = new List<DoorRecord>();
    foreach(DoorRecord door in doors){
      if(door.exteriorFacing){ ret.Add(door); }
    }
    return ret;
  }
  
  public string ToString(){
    string str = "";
    str += name;
    str += "(" + x + "," + y + ")";
    str += rooms.Count + " rooms";
    return str;
  }
}
