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
  public static string RB = "joystick button 5"; // 5
  public static string LB = "joystick button 4"; // 4
  public static string A = "joystick button 0"; // 0
  public static string B = "joystick button 1"; // 1
  public static string X = "joystick button 2"; // 2
  public static string Y = "joystick button 3"; // 3
  public static string START = "joystick button 7"; // 7
  public static string SELECT = "joystick button 6"; // 6
  public static string DUB = "joystick button 13"; // 13   For wireless controllers
  public static string DDB = "joystick button 14"; // 14
  public static string DRB = "joystick button 11"; // 11
  public static string DLB = "joystick button 12"; // 12
  public static string RSC = "joystick button 10"; // 10
  public static string LSC = "joystick button 9";  // 9
  
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
  
  /* Instantiates gameObject of a specific prefab  */
  GameObject Spawn(string prefab, Vector3 pos){
    return gameObject;
  }
  
}
