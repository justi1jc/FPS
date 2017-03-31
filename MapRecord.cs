/*
   The Map record is used to store the the game's Cell data for the purpose of
   initializing a GameRecord and generating interiors. 
*/

using System.Collections.Generic;

public class MapRecord{
  
  List<Cell[]> buildings; // The array of cells comprising each type of building.
  List<string> buildingNames; // The name of each type of building.
  List<Cell> exteriors; // All possible exteriors.
  
}
