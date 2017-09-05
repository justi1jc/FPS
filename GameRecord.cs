/*
    A GameRecord is used to store a particular session of the Adventure mode.
*/


using System;
using System.Collections.Generic;
[System.Serializable]
public class GameRecord{
  public string sessionName;
  public MapRecord map;
  public World world;
  public GameRecord(){
    sessionName = "Null";
    map = new MapRecord();
    world = new World();
  }
} 
