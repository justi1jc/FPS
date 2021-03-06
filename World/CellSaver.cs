/*
    CellSaver is the MonoBehaviour responsibile for saving and loading the
    contents of a Cell for editing the master map file via the unity editor.
    
    The master map file contains all the cells available when generating 
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class CellSaver : MonoBehaviour {
  
  /* Cell Data to be set in editor */
  public int x, y; /// Coords for exteriors.
  public string name; // Unique within building or overworld  
  public GameObject  max, min; // Corners of the area that will be saved.
  public bool interior; // True if this is a room
  public string building;    // The name of the building this interior resides within
  public GameObject[] doors; // For setting warp doors initially in editor.
  
  /* Saver settings. */
  public bool saverMode = false;  // Will instantly save the current cell if true.
  public bool cascade = false; // Will continue to save other scenes in build settings.
  
  public const string masterFile = "world";    // File containing all the map's cell data.
  bool fileAccess = false; // True during file io
  public MapRecord map;      // Contents of master map file.
  public Cell packedCell;    // The cell's data to populate or save with.
  
  
  public void Start(){
    if(saverMode){
      PackCell();
      LoadMaster();
      UpdateMaster();
      SaveMaster();
      if(cascade){ LoadNextScene(); }
      else{ print(map.ToString()); }
    }
  }
  
  
  /* Assigns the data of contents between min and max to packedCell. */
  public void PackCell(){
    
    Cell c = new Cell();
    List<GameObject> found = GetContents();
    c.interior = interior;
    if(interior){ c.building = building; }
    c.items = GetItems(found);
    c.npcs = GetNpcs(found);
    c.doors = GetDoors();
    c.name = name;
    c.building = building;
    Vector3 he = (max.transform.position - min.transform.position) / 2;
    c.heX = he.x;
    c.heY = he.y;
    c.heZ = he.z;
    c.x = x;
    c.y = y;
    packedCell = c;
  }
  
  /* Returns DoorRecords from doors. */
  public List<DoorRecord> GetDoors(){
    List<DoorRecord> ret = new List<DoorRecord>();
    foreach(GameObject door in doors){
      WarpDoor wd = door.GetComponent<WarpDoor>();
      if(wd != null){ ret.Add(wd.GetRecord()); }
    }
    return ret;
  }
  
  /* Instantiates all gameObjects from current data. */
  public void UnpackCell(){
    if(packedCell == null){ print("No data present."); return; }
    if(interior){
      name = packedCell.name;
      building = packedCell.building;
    }
    List<Data> items = packedCell.items;
    for(int i = 0; i < items.Count; i++){
      CreateItem(items[i]);
    }
    List<Data> npcs = packedCell.npcs;
    for(int i = 0; i < npcs.Count; i++){
      CreateNPC(npcs[i]);
    }
    
  }
  
  /* Converts an absolute position into relative one. */
  Vector3 Relative(Vector3 absolute){
    return absolute - transform.position;
  }
  
  void CreateItem(Data dat){
    Vector3 spawnPos = new Vector3(dat.x, dat.y, dat.z);
    spawnPos += transform.position;
    Quaternion rot = Quaternion.Euler(new Vector3(dat.xr, dat.yr, dat.zr));
    GameObject pref = (GameObject)Resources.Load("Prefabs/" + dat.prefabName, typeof(GameObject));
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
    GameObject pref = (GameObject)Resources.Load("Prefabs/" + dat.prefabName, typeof(GameObject));
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
    int layerMask = -1;
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
    bool ignoreDecor = false
  ){
    List<Data> ret = new List<Data>();
    for(int i = 0; i < obs.Count; i++){
      Item item = obs[i].GetComponent<Item>();
      if(item){
        Data dat = item.GetData();
        Vector3 pos = Relative(obs[i].transform.position);
        dat.x = pos.x;
        dat.y = pos.y;
        dat.z = pos.z;
        bool decor = item is Decor;
        if(decor && !ignoreDecor){ ret.Add(item.GetData()); }
        if(!decor && !ignoreItems){ ret.Add(item.GetData()); }
      }
    }
    print("Saved " + ret.Count + " items.");
    return ret;
  }
  
  /* Returns NPCs within GameObject list. */
  public List<Data> GetNpcs(List<GameObject> obs){
    List<Data> ret = new List<Data>();
    for(int i = 0; i < obs.Count; i++){
      Actor actor = obs[i].GetComponent<Actor>();
      if(actor){ ret.Add(actor.GetData()); }
    }
    print("Saved " + ret.Count + " NPCs.");
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
  
  
  
  /* Saves packedCell to map.  */
  public void UpdateMaster(){
    if(map == null){ print("Master not loaded"); return; }
    if(interior){ UpdateMasterInterior(); }
    else{ UpdateMasterExterior(); }
  }
  
  /* Caller ensures the map is not null. */
  public void UpdateMasterInterior(){
    Building bld = null;
    foreach(Building b in map.buildings){
      if(b.name == building){
        bld = b;
        break;
      }
    }
    
    if(bld == null){
      print("New building added");
      bld = new Building();
      bld.name = building;
      bld.doors = new List<DoorRecord>(packedCell.doors);
      bld.rooms = new List<Cell>();
      bld.rooms.Add(packedCell);
      map.buildings.Add(bld);
      return;
    }
    
    print("Added room to existing building.");
    
    bld.doors.AddRange( new List<DoorRecord>(packedCell.doors));
    bld.rooms.Add(packedCell);
  }
  
  /* Caller ensures the map is not null. */
  public void UpdateMasterExterior(){
    print("Saving exterior to master...");
    int found = -1;
    for(int i = 0; i < map.exteriors.Count; i++){
      if(map.exteriors[i].name == name){ found = i; break;}
    }
    if(found < 0){
      map.exteriors.Add(packedCell);
      print("New exterior added.");
    }
    else{
      map.exteriors[found] = packedCell;
      print("Updated existing interior.");
    }
  }
  
  /* Saves map to master map file. */
  public void SaveMaster(){
    if(map == null){ print("Master not loaded"); return; }
    if(fileAccess){ print("File access already in progress."); return; }
    fileAccess = true;
    string path = Application.dataPath + "/Resources/" + masterFile + ".master";
    using(FileStream file  = File.Create(path)){
      BinaryFormatter bf =  new BinaryFormatter();
      bf.Serialize(file, map);
      file.Close();
      fileAccess = false;
      print("Master saved to " + path);
    }
  }
  
  /* Loads the master map file into map or creates new one. */
  public void LoadMaster(){
    map = Cartographer.GetMaster();
  }
  
  /* Loads the next scene in the build order, if possible. */
  public void LoadNextScene(){
    int index = SceneManager.GetActiveScene().buildIndex;
    int count = SceneManager.sceneCountInBuildSettings;
    if(index + 1  >= count){ print("End;"); return; }
    print("Loading next scene.");
    SceneManager.LoadScene(index + 1);
  }
}
