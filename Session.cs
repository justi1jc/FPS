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
  public static string RB = "RB"; // 5
  public static string LB = "LB"; // 4
  public static string A = "A"; // 0
  public static string B = "B"; // 1
  public static string X = "X"; // 2
  public static string Y = "Y"; // 3
  public static string START = "START"; // 7
  public static string SELECT = "SELECT"; // 6
  public static string DUB = "DUB"; // 13   For wireless controllers
  public static string DDB = "DDB"; // 14
  public static string DRB = "DRB"; // 11
  public static string DLB = "DLB"; // 12
  
  
  void Awake(){
    if(Session.session){ Destroy(this); }
    else{ Session.session = this; }
  }
  
  
  void Update(){
    if(Input.GetKey(KeyCode.Escape)){
      Application.Quit();
    }
  }
  
  Data GatherInterior(){
    return new Data();
  }
  
}
