/*
*   An AI for NPC Actors. 
*   
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class AI: MonoBehaviour{
  bool paused;
  
  /* Initial setup. */
  public void Begin(){
    paused = false;
  }
  
  /* Cease activity. */
  public void Pause(){
    paused = true;
  }
  
  /* Resume activity. */
  public void Resume(){
    paused = false;
  }
}
