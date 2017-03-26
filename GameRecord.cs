/*
    A game record is used to store a Session's persistent data.
*/


using System;
using System.Collections.Generic;
[System.Serializable]
public class GameRecord{
  public List<Data> interiors;
  public List<Data> exteriors;
  public bool interior;
  public List<Data> quests; // Quest data.
  public int currentCell; // Index of current cell.
  public List<Data> players;
  public GameRecord(){
    interiors = new List<Data>();
    exteriors = new List<Data>();
    quests    = new List<Data>();
    players   = new List<Data>();
  }
} 
