/*
*
*   A singleton whose purpose is to manage the session's data.
*   
*
*
*
*
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
  static Session session;
  void Awake(){
    if(Session.session){ Destroy(this); }
    else{ Session.session = this; }
  }
  void Update(){
    if(Input.GetKey(KeyCode.Escape)){
      Application.Quit();
    }
  }
}
