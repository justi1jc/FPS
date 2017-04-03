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
  public string displayName;   // This should be unique between cells in a building.
  public const string masterFile = "world.master";    // File containing all the map's cell data.     
  public GameObject  max, min; // Corners of the area that will be saved.
  public bool interior; 
  public bool saverMode = false;     // Will instantly save the current cell if true.
  // Interior
  public string building;    // The name of the building this interior resides within.
  public Data[] doorData;    // warp doors for interiors
  public GameObject[] doors; // For setting warp doors initially in editor.
  public bool[] edges;       // True if a door exists for this direction.
  public MapRecord map;      // Contents of master map file.
  public Cell packedData;    // The cell's data to populate or save with.
  
  // Exterior
  public List<Data> buildings; // The buildings in this exterior. 
  
  void Update(){
    if(Input.GetKeyDown(KeyCode.S)){ SaveToMaster(packedData); }
    if(Input.GetKeyDown(KeyCode.P)){ PackData(); }
    if(Input.GetKeyDown(KeyCode.U)){ UnpackData(); }
    if(Input.GetKeyDown(KeyCode.L)){ LoadFromMaster(); }
    if(Input.GetKeyDown(KeyCode.C)){ ClearCell(); }
    
  }
  public void Start(){
    if(saverMode){
      PackData();
      Cell c = packedData;
      for(int i = 0; i < c.items.Count; i++){ print( "Item" + c.items[i].displayName); }
      for(int i = 0; i < c.npcs.Count; i++){ print( "NPC"  + c.npcs[i].displayName); }
      LoadFromMaster();
      SaveToMaster(c);
      LoadNextScene();
    }
  }
  
  
  /* Assigns the data of contents between min and max to packedData. */
  public void PackData(){
    Cell c = new Cell();
    List<GameObject> found = GetContents();
    if(interior){
      c.items = GetItems(found);
      c.building = building;
      c.displayName = displayName;
    }
    else{
      c.items = GetItems(found, false, true);
      c.buildings = GetItems(found, true, false); 
    }
    c.npcs = GetNpcs(found);
    packedData = c;
  }
  
  /* Instantiates all gameObjects from current data. */
  public void UnpackData(){
    if(packedData == null){ print("No data present."); return; }
    List<Data> buildings = packedData.buildings;
    for(int i = 0; i < buildings.Count; i++){
      CreateItem(buildings[i]);
    }
    List<Data> items = packedData.items;
    for(int i = 0; i < items.Count; i++){
      CreateItem(items[i]);
    }
    List<Data> npcs = packedData.npcs;
    for(int i = 0; i < npcs.Count; i++){
      CreateNPC(npcs[i]);
    }
  }
  
  void CreateItem(Data dat){
    Vector3 spawnPos = new Vector3(dat.x, dat.y, dat.z);
    Quaternion rot = Quaternion.Euler(new Vector3(dat.xr, dat.yr, dat.zr));
    GameObject pref = (GameObject)Resources.Load(dat.prefabName, typeof(GameObject));
    GameObject go = (GameObject)GameObject.Instantiate(
      pref,
      spawnPos,
      rot
    );
    Item item = go.GetComponent<Item>();
    if(item){ item.LoadData(dat); }
  }
  
  void CreateNPC(Data dat){
    Vector3 spawnPos = new Vector3(dat.x, dat.y, dat.z);
    Quaternion rot = Quaternion.Euler(new Vector3(dat.xr, dat.yr, dat.zr));
    GameObject pref = (GameObject)Resources.Load(dat.prefabName, typeof(GameObject));
    GameObject go = (GameObject)GameObject.Instantiate(
      pref,
      spawnPos,
      rot
    );
    Actor actor = go.GetComponent<Actor>();
    if(actor){ actor.LoadData(dat); }
  }
  
  /* Returns the gameObjects caught by boxcasting with min and max.*/
  public List<GameObject> GetContents(){
    if(!min || !max){ return new List<GameObject>(); }
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
    List<GameObject> contents = new List<GameObject>();
    for(int i = 0; i < found.Length; i++){
      if(found[i].collider != null){
        contents.Add(found[i].collider.gameObject);
      }
    }
    return contents;
  }
  
  /* Returns the items in a GameObject list.
     if ignoreItems is true, only scenery will be returned.
     if ignoreScenery is true, only items will be returned.
  */
  public List<Data> GetItems(List<GameObject> obs,
    bool ignoreItems = false,
    bool ignoreScenery = false
  ){
    List<Data> ret = new List<Data>();
    for(int i = 0; i < obs.Count; i++){
      Item item = obs[i].GetComponent<Item>();
      if(item){
        bool scenery = item.itemType == Item.SCENERY;
        if(scenery && !ignoreScenery){ ret.Add(item.GetData()); }
        if(!scenery && !ignoreItems){ ret.Add(item.GetData()); }
      }
    }
    return ret;
  }
  
  /* Returns NPCs within GameObject list. */
  public List<Data> GetNpcs(List<GameObject> obs){
    List<Data> ret = new List<Data>();
    for(int i = 0; i < obs.Count; i++){
      Actor actor = obs[i].GetComponent<Actor>();
      if(actor){ ret.Add(actor.GetData()); }
    }
    return ret;
  }
  
  /* Destroys all gameObjecs within min and max. */
  public void ClearCell(){
    List<GameObject> contents = GetContents();
    for(int i = 0; i < contents.Count; i++){
      Destroy(contents[i]);
    }
    print("Cell cleared");
  }
  
  /* Instantiates the contents of a Cell. */
  public void LoadData(Cell c){
    bool fileFound = false;
    if(!fileFound){
      map = new MapRecord();
    }
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
