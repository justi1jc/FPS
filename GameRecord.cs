/*
    A GameRecord is used to store a particular session of the Adventure mode.
*/


using System;
using System.Collections.Generic;
[System.Serializable]
public class GameRecord{
  public string sessionName;
  public List<Data> playerData;
  public MapRecord map;
  public GameRecord(){
    sessionName = "Null";
    map = null;
    playerData = new List<Data>();
  }
} 
