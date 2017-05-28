/*
    A game record is used to store a Session's persistent data.
*/


using System;
using System.Collections.Generic;
[System.Serializable]
public class GameRecord{
  public string sessionName;
  public MapRecord map;
  public int currentID; // Current ID for identifying NPCs
  public List<Data> quests; // Quest data.
  public List<Data> players;
  public GameRecord(){
    map = new MapRecord();
    quests    = new List<Data>();
    players   = new List<Data>();
    quests = new List<Data>();
  }
} 
