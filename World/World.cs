/*
    A World handles a particular generated world in adventure mode.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class World{
  public List<Quest> quests;
  public MapRecord map;
  
  public World(){
  }
  
  /* Creates the map for this adventure. */
  public void GenerateMap(){
    map = Cartographer.GetMap(10, 10);
    MonoBehaviour.print(map.ToString());
  }
  
  /*
    Initializes Starting quests for a new .
  */
  void CreateStartingQuests(){
    MonoBehaviour.print("method stub");
  }
  
  /* Initializes a specified quest. */
  public void StartQuest(int quest){
    MonoBehaviour.print("method stub");
  }
  
  public GameRecord GetData(){
    GameRecord record = new GameRecord();
    record.sessionName = Session.session.sessionName;
    /*
    for(int i = 0; i < decks.Count; i++){
      if(decks[i].interior){ decks[i].SaveInterior(); }
      else{ decks[i].SaveExterior(); }
    }
    record.map = map;
    record.players = GetPlayerData();
    */
    return record;
  }
  
  
}
