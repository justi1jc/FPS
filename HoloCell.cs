/*
    The HoloCell is concerned with saving or loading the contents of a single
    cell at any one time. The HoloDeck employs HoloCells to 

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HoloCell{
  Vector3 position;
  Vector3 spawnRot, spawnPos; // Rotation and position players will spawn into.
  public Cell cell; // Active cell.
  int spawnDoor; // ID of the spawn door
  HoloDeck deck; // Id of the HoloDeck using this HoloCell.
  
  public HoloCell(Vector3 position, HoloDeck deck = null){
    this.position = position;
    this.deck = deck;
  }
  
  /* Returns the updated cell.*/
  public Cell GetData(){
    List<GameObject> contents = GetContents();
    cell.items = GetItems(contents);
    cell.npcs = GetNpcs(contents);
    return cell;
  }
  
  /* Places the contents of a cell into the scene. */
  public void LoadData(Cell c, int doorId = -1){
    spawnDoor = doorId;
    cell = c;
    //MonoBehaviour.print("HOLOCELL Interior:" + c.interior);
    for(int i = 0; i < cell.items.Count; i++){ CreateItem(cell.items[i]); }
    for(int i = 0; i < cell.npcs.Count; i++){ CreateNPC(cell.npcs[i]); }
  }
  
  /* Simply destroys the active contents of this HoloCell. */
  public void Clear(){
    List<GameObject> obs = GetContents();
    for(int i = 0; i < obs.Count; i++){
      Object.Destroy(obs[i]);
    }
  }
  
  public void CreateItem(Data dat){
    Vector3 sPos = new Vector3(dat.x, dat.y, dat.z);
    sPos += position;
    Quaternion rot = Quaternion.Euler(new Vector3(dat.xr, dat.yr, dat.zr));
    GameObject pref = (GameObject)Resources.Load(dat.prefabName, typeof(GameObject));
    GameObject go = (GameObject)GameObject.Instantiate(pref, sPos, rot );
    go.transform.position = sPos;
    Item item = go.GetComponent<Item>();
    if(item){ 
      item.LoadData(dat);
      if(item.itemType == Item.WARP){
        item.deck = deck;
        if(item.doorId == spawnDoor){
          spawnPos = item.destPos;
          spawnRot = item.destRot;
        } 
      }
    }
  }
  
  /* Creates a new player from prefab and places them at spawnpoint. */
  public void AddPlayer(string prefabName){
    GameObject pref = (GameObject)Resources.Load(prefabName, typeof(GameObject));
    if(!pref){ MonoBehaviour.print(prefabName + " does not exist."); return; }
    Vector3 sPos = position;
    Quaternion rot = Quaternion.identity;
    GameObject go = (GameObject)GameObject.Instantiate(pref, sPos, rot);
    Actor a = go.GetComponent<Actor>();
    if(a == null){ MonoBehaviour.print(prefabName + " had no actor."); return; }
    Data dat = a.GetData();
    dat.x = spawnPos.x;
    dat.y = spawnPos.y;
    dat.z = spawnPos.z;
    dat.xr = spawnRot.x;
    dat.yr = spawnRot.y;
    dat.zr = spawnRot.z;
    a.LoadData(dat);
  }
  
  /* Creates an NPC with the given data. 
    IgnoreDoor is true if file has just been loaded and the player should remain in
    their last known position. Otherwise it is assumed players are entering the 
    cell via a spawnpoint.  */
  public void CreateNPC(Data dat, bool ignoreDoor = false, bool player = false){
    if(!ignoreDoor && player){
      dat.x = spawnPos.x;
      dat.y = spawnPos.y;
      dat.z = spawnPos.z;
      dat.xr = spawnRot.x;
      dat.yr = spawnRot.y;
      dat.zr = spawnRot.z;
    }
    Vector3 sPos = new Vector3(dat.x, dat.y, dat.z);
    sPos += position;
    Quaternion rot = Quaternion.Euler(new Vector3(dat.xr, dat.yr, dat.zr));
    GameObject pref = (GameObject)Resources.Load(dat.prefabName, typeof(GameObject));
    if(!pref){
      MonoBehaviour.print(dat.prefabName + "," + dat.displayName + " null");
      return;
    }
    GameObject go = (GameObject)GameObject.Instantiate(pref, sPos, rot);
    Actor actor = go.GetComponent<Actor>();
    if(actor){ actor.LoadData(dat); }
    go.transform.position = sPos; 
  }
  
  /* Returns the gameObjects caught by boxcasting with min and max.*/
  public List<GameObject> GetContents(){
    if(cell == null){ return new List<GameObject>(); }
    Vector3 center = position;
    Vector3 halfExtents = new Vector3(cell.heX, cell.heY, cell.heZ);
    Vector3 direction = new Vector3(0f,0.001f,0f);
    Quaternion orientation = Quaternion.identity;
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
  */
  public List<Data> GetItems(List<GameObject> obs ){
    List<Data> ret = new List<Data>();
    for(int i = 0; i < obs.Count; i++){
      Item item = obs[i].GetComponent<Item>();
      if(item){
        Data dat = item.GetData();
        Vector3 pos = Relative(obs[i].transform.position);
        dat.x = pos.x;
        dat.y = pos.y;
        dat.z = pos.z;
        ret.Add(item.GetData());
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
    return absolute - position;
  }
  
  /* Returns true if the point resides within this Cell, */
  public bool Contains(Vector3 point){
    float dist = Vector3.Distance(point, position);
    float mag = new Vector3(cell.heX, cell.heY, cell.heZ).magnitude;
    if(dist < mag){ return true; }
    return false;
  }
  
  /* Instantiates gameObject of a specific prefab  
     as close to the desired location as possible.
     will spawn gameObject directly on top of map if
     there's no space large enough for the movecheck. */
  public GameObject Spawn(string prefab, Vector3 pos){
    GameObject go = null;
    GameObject pref = (GameObject)Resources.Load(
      prefab,
      typeof(GameObject)
    );
    if(!pref){ MonoBehaviour.print("Prefab null:" + prefab); return go; }
    Vector3[] candidates = GroundedColumn(pos, pref.transform.localScale);
    int min = 0;
    float minDist, dist;
    for(int i = 0; i < candidates.Length; i++){
      minDist = Vector3.Distance(candidates[min], pos);
      dist = Vector3.Distance(candidates[i], pos);
      if(minDist > dist){
        min = i;
      }
    }
    go = (GameObject)GameObject.Instantiate(
      pref,
      candidates[min],
      Quaternion.identity
    );
    return go;
  }
  
  /* Returns an array of viable positions consisting of empty space directly
     above colliders, This is like surveying how many stories a building
     has to it. */
  Vector3[] GroundedColumn(
    Vector3 pos,
    Vector3 scale,
    float max=100f,
    float min=-100f
  ){
    Vector3 origin = pos;
    List<Vector3> grounded = new List<Vector3>();
    pos = new Vector3( pos.x, max, pos.z);
    Vector3 last = pos;
    bool lastPlace = true;
    while(pos.y > min){
      bool check = PlacementCheck(pos, scale);
      if(!check && lastPlace){ grounded.Add(last + Vector3.up); }
      last = pos;
      pos = new Vector3(pos.x, pos.y-scale.y, pos.z);
      lastPlace = check;
    }
    if(grounded.Count == 0){ grounded.Add(origin); } // Don't return an empty array.
    return grounded.ToArray();
  }
  
  /* Performs a boxcast at a certain point and scale. Returns true on collison. */
  bool PlacementCheck(Vector3 pos, Vector3 scale){
    Vector3 direction = Vector3.up;
    float distance = scale.y;
    Vector3 center = pos;
    Vector3 halfExtents = scale / 2;
    Quaternion orientation = Quaternion.identity;
    int layerMask = ~(1 << 8);
    RaycastHit hit;
    bool check = !Physics.BoxCast(
      center,
      halfExtents,
      direction,
      out hit,
      orientation,
      distance,
      layerMask,
      QueryTriggerInteraction.Ignore
    );
    return check;
  }
  
  public string ToString(){
    string ret = "Holocell:";
    ret += cell == null ? "Null cell" : cell.ToString();
    return ret; 
  }
}
