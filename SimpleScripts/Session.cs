using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
public class Session : MonoBehaviour {

  void Update(){
    if(Input.GetKey(KeyCode.Escape)){
      Application.Quit();
    }
  }
}
