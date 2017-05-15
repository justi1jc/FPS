/*
    The HoloCell is concerned with saving or loading the contents of a single
    cell at any one time. The HoloDeck employs HoloCells to 

*/

public class HoloCell{
  Cell cell; // Active cell.
  
  public HoloCell(){
  }
  
  /* Returns the updated cell, clearing the contents in the process.*/
  public Cell Pack(){
    return null;
  }
  
  /* Places the contents of a cell into the scene. */
  public void Unpack(Cell c){
    cell = c;
    
  }
  
}
