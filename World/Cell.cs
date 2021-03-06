/*
  A cell is the base unit of the map. 
  
  The overworld is comprised of multiple cells filling a 2D grid.
  
  Buildings are made up of Cells representing individual rooms.
*/

using System.Collections.Generic;
[System.Serializable]
public class Cell{
  
  public string name;
  public bool interior;
  public string building; // The parent building of this room cell resides in.
  public int x, y; // Position on the map this exterior cell resides in.
  public int id = -1; // Id of this cell, or its building.
  public List<Data> items;
  public List<Data> npcs;
  public List<DoorRecord> doors;
  public float heX, heY, heZ; // Half extents for boxcasts
  
  /* Cloning constructor */
  public Cell(Cell c){
    x = c.x;
    y = c.y;
    id = c.id;
    items = new List<Data>(c.items);
    npcs = new List<Data>(c.npcs);
    doors = new List<DoorRecord>(c.doors);
    heX = c.heX;
    heY = c.heY;
    heZ = c.heZ;
    name = c.name;
    interior = c.interior;
  }
  
  public Cell(){
    items = new List<Data>();
    npcs = new List<Data>();
    doors = new List<DoorRecord>();
  }
  
  /* Returns the door matching this id, or null. */
  public DoorRecord GetDoor(int doorID){
    DoorRecord ret = null;
    foreach(DoorRecord dr in doors){
      if(dr.id == doorID){
        ret = dr;
        break;
      }
    }
    return ret;
  }
  
  public string ToString(){
    string ret = "[";
    ret += name;
    ret += "(" + x + "," + y + ")," + id;
    return ret;
  }
  
}
