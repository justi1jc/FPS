/*
  A cell is the basic unit of the game's world. It can describe either the
  an interior space, or an exterior space.

  An interior cell exists within a building and has up to four doors corresponding
  to the four cardinal directions. The locations of each door are likewise
  maintained in order to programatically generate building interiors.
  
  An exterior cell is loaded to create a grid. By breaking up the contents
  into buildings, NPCs, and Items, cells distant from the player can load
  buildings first, then later load NPCs and Items as the player nears.
*/

using System.Collections.Generic;
[System.Serializable]
public class Cell{
  // Cardinal Directions 
  public const int NORTH = 0;
  public const int EAST  = 1;
  public const int WEST  = 2;
  public const int SOUTH = 3;
  public int x, y; // Position on the map.
  public bool interior;   // True if this cell is an interior.
  public List<Data> items;
  public List<Data> npcs;
  public float heX, heY, heZ; // Half extents for boxcasts
  
  // Interior
  public string building; // The name of the building this interior resides within.
  public string displayName; // Base name for room.
  public string exteriorName; //Linked exterior, if any
  
  // Exterior
  public bool entrance; // True if this contains a door to an interior.
  
  public Cell(){
    items = new List<Data>();
    npcs = new List<Data>();
  }
  
}
