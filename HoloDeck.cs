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
  public Cell deck; // Active cell
  int id = 0; // Id for multiple holodecks in a session. 
  int spawnDoor; // Door to derive spawnPoint from.
  public Vector3 spawnRot, spawnPos;

  public void LoadInterior(string building, string cellName, int door){
    if(interior){
      spawnDoor = door;
      if(initialized){
        SaveInterior();
        ClearInterior();
      }
      ClearInterior();
      MapRecord map = Session.session.map;
      if(map == null){ print("Session map not initialized"); return; }
      Cell found = null;
      for(int i = 0; i < map.buildings.Count; i++){
        bool match = true; 
        if(map.buildings[i] == null){ match = false; }
        else if(map.buildings[i].Length == 0){ match = false; }
        else if(map.buildings[i][0] == null){ match = false; }
        else if(map.buildings[i][0].building != building){ match = false; }
        if(match){
          for(int j = 0; j < map.buildings[i].Length; j++){
            Cell candidate = map.buildings[i][j];
            if(candidate != null && candidate.displayName == cellName){
              found = candidate;
              break;
            }
          }
          break;
        }
      }
      if(found != null){
        deck = found;
        UnpackInterior();
      }
      else{
        print("Couldn't find " + cellName + " in " + building);
      }
      
    }
    
    initialized = true;
    interior = true;
  }
  
  /* Instantiates contents of deck. */
  public void UnpackInterior(){
    for(int i = 0; i < deck.buildings.Count; i++){ CreateItem(deck.buildings[i]); }
    for(int i = 0; i < deck.items.Count; i++){ CreateItem(deck.items[i]); }
    for(int i = 0; i < deck.npcs.Count; i++){ CreateNPC(deck.npcs[i]); }
  }
  
  public void CreateItem(Data dat){
    Vector3 sPos = new Vector3(dat.x, dat.y, dat.z);
    sPos += transform.position;
    Quaternion rot = Quaternion.Euler(new Vector3(dat.xr, dat.yr, dat.zr));
    GameObject pref = (GameObject)Resources.Load(dat.prefabName, typeof(GameObject));
    GameObject go = (GameObject)GameObject.Instantiate(
      pref,
      sPos,
      rot
    );
    go.transform.position = sPos;
    Item item = go.GetComponent<Item>();
    if(item){ 
      item.LoadData(dat);
      if(item.itemType == Item.WARP){
        item.deckId = id;
        if(item.doorId == spawnDoor){
          spawnPos = item.destPos;
          spawnRot = item.destRot;
        } 
      }
    }
  }
  
  public void CreateNPC(Data dat){
    Vector3 sPos = new Vector3(dat.x, dat.y, dat.z);
    sPos += transform.position;
    Quaternion rot = Quaternion.Euler(new Vector3(dat.xr, dat.yr, dat.zr));
    GameObject pref = (GameObject)Resources.Load(dat.prefabName, typeof(GameObject));
    if(!pref){ print(dat.prefabName + "," + dat.displayName + " null"); return; }
    GameObject go = (GameObject)GameObject.Instantiate(
      pref,
      sPos,
      rot
    );
    Actor actor = go.GetComponent<Actor>();
    if(actor){ actor.LoadData(dat); }
    go.transform.position = sPos;
  }
  
  /* Updates interior in Session's data with current content. */
  public void SaveInterior(){
    PackInterior();
    MapRecord map = Session.session.map;
    for(int i = 0; i < map.buildings.Count; i++){
        bool match = true; 
        if(map.buildings[i] == null){ match = false; }
        else if(map.buildings[i].Length == 0){ match = false; }
        else if(map.buildings[i][0] == null){ match = false; }
        else if(map.buildings[i][0].building != deck.building){ match = false; }
        if(match){
          for(int j = 0; j < map.buildings[i].Length; j++){
            Cell candidate = map.buildings[i][j];
            if(candidate != null && candidate.displayName == deck.displayName){
              map.buildings[i][j] = deck;
              return;
            }
          }
        }
      }
      print("Couldn't find " + deck.displayName + " in " + deck.building);
      
  }
  
  /* Updates the active Cell's contents. */
  public void PackInterior(){
    Cell c = new Cell();
    List<GameObject> found = GetContents();
    if(interior){
      c.items = GetItems(found);
      c.building = deck.building;
      c.displayName = deck.displayName;
    }
    else{
      c.items = GetItems(found, false, true);
      c.buildings = GetItems(found, true, false);
    }
    c.npcs = GetNpcs(found);
    c.heX = deck.heX;
    c.heY = deck.heY;
    c.heZ = deck.heZ;
    deck = c;
  }
  
  /* Clears contents of loaded interior. */
  public void ClearInterior(){
    List<GameObject> obs = GetContents();
    for(int i = 0; i < obs.Count; i++){
      Destroy(obs[i]);
    }
  }
  
  public void LoadExterior(int x, int y){
    print("Exteriors not implemented");
  }
  
  /* Returns the gameObjects caught by boxcasting with min and max.*/
  public List<GameObject> GetContents(){
    if(deck == null){ return new List<GameObject>(); }
    Vector3 center = transform.position;
    Vector3 halfExtents = new Vector3(deck.heX, deck.heY, deck.heZ);
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
        Data dat = item.GetData();
        Vector3 pos = Relative(obs[i].transform.position);
        dat.x = pos.x;
        dat.y = pos.y;
        dat.z = pos.z;
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
      if(actor){
        bool player = actor.playerNumber > 0 && actor.playerNumber < 5;
        if(!player){
          Data dat = actor.GetData();
          Vector3 pos = Relative(obs[i].transform.position);
          dat.x = pos.x;
          dat.y = pos.y;
          dat.z = pos.z;
          ret.Add(dat);
        }
      }
    }
    return ret;
  }
  
  /* Converts an absolute position into relative one. */
  Vector3 Relative(Vector3 absolute){
    return absolute - transform.position;
  }
}
