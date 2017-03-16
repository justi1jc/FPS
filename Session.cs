/*
*
*   A singleton whose purpose is to manage the session's data.
*   
*
*
*   Note: Controls are currently hard-coded in. They should be flexible
*   in the future. Axes and buttons must be added manually in the editor to
*   a specific project.
*
*
*/



﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
public class Session : MonoBehaviour {
  public static Session session;
  /* Controller one linux values */
  public static string C1 = "Joystick 1"; //Controller 1
  public static string XL = "XL"; // "X Axis" DeadZone: 0.2 Accuracy: 1
  public static string YL = "YL"; // "Y Axis" DeadZone: 0.2 Accuracy: 1
  public static string XR = "XR"; // "4rd Axis" DeadZone: 0.2 Accuracy: 1
  public static string YR = "YR"; // "5th Axis" DeadZone: 0.2 Accuracy: 1
  public static string RT = "RT"; // "6th Axis" DeadZone: 0.1 
  public static string LT = "LT"; // "3rd Axis" DeadZone: 0.1
  public static string DX = "DX";  // "7th Axis" for wired controllers
  public static string DY = "DY";  // "8th Axis"
  public static string RB = "joystick button 5"; // 5 Right bumper
  public static string LB = "joystick button 4"; // 4 Left bumper
  public static string A = "joystick button 0"; // 0
  public static string B = "joystick button 1"; // 1
  public static string X = "joystick button 2"; // 2
  public static string Y = "joystick button 3"; // 3
  public static string START = "joystick button 7"; // 7
  public static string SELECT = "joystick button 6"; // 6
  public static string DUB = "joystick button 13"; // 13  D-pad Up For wireless controllers
  public static string DDB = "joystick button 14"; // 14  D-pad down
  public static string DRB = "joystick button 11"; // 11  D-pad right
  public static string DLB = "joystick button 12"; // 12  D-pad left
  public static string RSC = "joystick button 10"; // 10  Right stick click
  public static string LSC = "joystick button 9";  // 9   left stick click
  
  Camera cam1;
  Camera cam2;
  
  void Awake(){
    if(Session.session){ Destroy(this); }
    else{ Session.session = this; }
  }
  
  
  void Update(){
    if(Input.GetKey(KeyCode.Escape) || Input.GetKey(Session.START)){
      Application.Quit();
    }

  }
  
  Data GatherInterior(){
    return new Data();
  }
  
  public void RegisterPlayer(int player, Camera cam){
    if(player == 1){ cam1 = cam; }
    else if(player == 2){ cam2 = cam; }
    UpdateCameras();
  }
  
  public void UnregisterPlayer(int player){
    if(player == 1){ cam1 = null; }
    else if(player == 2){ cam2 = null; }
    UpdateCameras();
  }
  
  
  /* Sets up each player's Menu */
  void UpdateCameras(){
    bool split = cam1 && cam2;
    if(split){
      cam1.rect = new Rect(0f, 0f, 0.5f, 1f);
      cam2.rect = new Rect(0.5f, 0, 0.5f, 1f);
      Menu menu = cam1.gameObject.GetComponent<Menu>();
      if(menu){ menu.split = true; menu.right = false; }
      menu = cam2.gameObject.GetComponent<Menu>();
      if(menu){ menu.split = true; menu.right = true; }
    }
    else if(cam1){
      cam1.rect = new Rect(0f, 0f, 1f, 1f);
      Menu menu = cam1.gameObject.GetComponent<Menu>();
      if(menu){ menu.split = false; menu.right = false; }
    }
    else if(cam2){
      cam2.rect = new Rect(0f, 0f, 1f, 1f);
      Menu menu = cam2.gameObject.GetComponent<Menu>();
      if(menu){ menu.split = false; menu.right = false; }
    }   
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
    if(!pref){ print("Prefab null:" + prefab); return go; }
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
  
  /* Returns an array of viable positions full of empty space directly
     above colliders, This is like surveying how many stories a building
     has to it. */
  Vector3[] GroundedColumn(Vector3 pos, Vector3 scale,
                           float max=100f, float min=-100f){
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
  
}
