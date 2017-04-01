/*
    CellSaver is the MonoBehaviour responsibile for saving and loading the
    contents of a Cell. It can be used by the Session to load a particular active
    Cell, or it can be used in the Unity Editor to save the contents of a scene
    as a Cell.  
    
    The master map file represents all the cells available when generating 
    a new map, whereas a GameRecord represents a particular session's data. The
    CellSaver will not save directly to the GameRecord's file.
    
    Directions for using to store scenes from Unity Editor:
    0. Place in scene whose contents represents a Cell.
    1. Set saverMode to true.
    2, Set this cell's information in the editor.
    3. Place a child gameObject Max at the higher corner and Min at lower corner
       of the scene's content.
    4. Add all such scenes to your project's build settings.
    5. Open the scene with the lowest build order.
    6. Run the scene and each scene will be loaded and saved in order.
    
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CellSaver : MonoBehaviour {
  public string displayName;   // This should be unique between all interior cells.
  public const string masterFile = "world.master";    // File containing all the map's cell data.     
  public GameObject  max, min; // Corners of the area that will be saved.
  public bool interior; 
  public bool saverMode = false;     // Will instantly save the current cell if true.
  // Interior
  public string building;    // The name of the building this interior resides within.
  public Data[] doorData;    // warp doors for interiors
  public GameObject[] doors; // For setting warp doors initially in editor.
  public bool[] edges;       // True if a door exists for this direction.  

  // Exterior
  public List<Data> buildings; // The buildings in this exterior. 
  
    
  public void Start(){
    if(saverMode){
      Cell c =  GetData();
      LoadFromMaster();
      SaveToMaster(c);
      LoadNextScene();
    }
  }
  
  
  /* Returns a Cell containing data of contents between min and max. */
  public Cell GetData(){
    Cell c = new Cell();
    if(!min || !max){ return c; }
    Vector3 center = Vector3.Lerp(min.transform.position, max.transform.position, 0.5f);
    Vector3 halfExtents = (max.transform.position - min.transform.position) / 2;
    Vector3 direction = transform.forward;
    Quaternion orientation = transform.rotation;
    float distance = 1f;
    int layerMask = ~(1 << 8);
    RaycastHit[] found = Physics.BoxCastAll(
      center,
      halfExtents,
      direction,
      orientation,
      distance,
      layerMask,
      QueryTriggerInteraction.Ignore
    );
    for(int i = 0; i < found.Length; i++){
      print(found[i].collider.gameObject.name);
    }
    return c;
  }
  
  /* Destroys all gameObjecs within min and max. */
  public void ClearScene(){
    print("Cleared scene");
  }
  
  /* Instantiates the contents of a Cell. */
  public void LoadData(Cell c){
    print("Loaded data");
  }
  
  /* Saves a cell to master map file.  */
  public void SaveToMaster(Cell c){
    print("Saved to master");
  }
  
  /* Loads the master map file. */
  public void LoadFromMaster(){
    print("Loaded from master");
  }
  
  /* Loads the next scene in the build order, if possible. */
  public void LoadNextScene(){
    print("Loaded next scene");
  }
}
