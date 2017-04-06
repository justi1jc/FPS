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


  public void LoadInterior(string building, string cellName){
    if(interior){
      if(initialized){
        SaveInterior();
        ClearInterior();
      }
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
        print("Loaded " + found.displayName);
      }
      
    }
    
    initialized = true;
    interior = true;
  }
  
  /* Instantiates contents of deck. */
  public void UnpackInterior(){
    //for(int i = 0; i < map.buildings.Count; i++){ CreateItem(map.buildings[i]); }
      //for(int i = 0; i < map.items.Count; i++){ CreateItem(map.items[i]); }
      //for(int i = 0; i < map.npcs.Count; i++){ CreateNPC(map.npcs[i]); }
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
}
