/*
    Directions for use:
    1. Place in scene whose contents represents a Cell.
    2, Set this cell's information in the editor.
    3. Place a child gameObject Max at the higher corner and Min at lower corner
       of the scene's content.
    3. Add all such scenes to your project's build settings.
    4. Open the scene with the lowest build order.
    5. Run the scene and each scene will be loaded and saved in order.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CellSaver : MonoBehaviour {
  public string displayName; // This should be unique between all interior cells.
  public bool interior; 
  public string gameFile;
  public GameObject  Max, Min; // Corners of the area that will be saved.
  public List<Data> contents;
  
  public void Start(){
    LoadGameRecord();
    CaptureContents();
    SaveGameRecord();
    LoadNextScene();
  }
  
  /* Attempts to load file from gameFile location, and otherwise creates a new
     one. Sets  */
  public void LoadGameRecord(){
    print("GameRecord Loaded.");
  }
  
  /* Captures contents between Max and Min points.*/
  public void CaptureContents(){
    print("Contents captured.");
  }
  
  /* Saves the game record. To gameFile location */
  public void SaveGameRecord(){
    print("Game Record Saved");
  }
  
  /* Loads next scene, if possible. */
  public void LoadNextScene(){
    print("Next scene loaded");
  }
}
