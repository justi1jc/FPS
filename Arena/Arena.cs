/*
    Arena handles the arena gameplay.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Arena : MonoBehaviour{
  GameObject[] spawnPoints; //Stores direction and position to spawn players in.
  MenuManager menu;
  public void Start(){
    Initialize();
  }
  
  /* Begin a new round. */
  public void Initialize(){
    menu = gameObject.AddComponent<MenuManager>();
  }
  

}
