/*
    HoloDeck is responsible for loading cells from the session and updating
    the existing cells in the session. 
    
    Interior rendering consists of loading the contents of a single cell.
    
    Exterior rendering consists of loading a cell and a number of layers of
    cells surrounding it.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HoloDeck : MonoBehaviour{
  public bool initialized = false; // True if a cell of some type is currently loaded. 
  public bool interior; // True if an interior is currently loaded.


  public void LoadInterior(string building, string cellName){
    if(initialized && interior){
      SaveInterior();
      ClearInterior();
      print("Loaded " + building + " " + cellName);
    }
    
    initialized = true;
    interior = true;
  }
  
  /* Updates interior in Session's data. */
  public void SaveInterior(){
    print("Saved Interior");
  }
  
  /* Clears contents of loaded interior. */
  public void ClearInterior(){
    print("ClearedInterior");
  }
  
  public void LoadExterior(int x, int y){
    print("Exteriors not implemented");
  }
}
