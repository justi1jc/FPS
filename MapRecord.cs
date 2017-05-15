/*
   The Map record is used to store the the game's Cell data for the purpose of
   initializing a GameRecord and generating interiors. 
*/

using System.Collections.Generic;

[System.Serializable]
public class MapRecord{
  
  public List<Cell[]> buildings; // The array of cells comprising each type of building.
  public List<string> buildingNames; // The name of each type of building.
  public List<Cell> exteriors; // All possible exteriors.
  public List<Cell> interiors; // All interiors
  
  public MapRecord(){
    buildings = new List<Cell[]>();
    exteriors = new List<Cell>();
    interiors = new List<Cell>();
  }
}
