/*
    A World handles a particular generated world in adventure mode,
    including quests and the HoloDecks.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class World{
  public List<Quest> quests;
  public MapRecord map;
  public List<HoloDeck> decks;
  public GameObject gameObject;
  
  public World(){
    decks = null;
    this.gameObject = new GameObject();
  }
  
  /* Creates a new adventure. */
  public void CreateAdventure(){
    map = Cartographer.GetMap(5, 5);
    DeployHoloDeck();
    Building b = map.buildings[0];
    decks[0].LoadRoom(b.id, b.rooms[0].name, 0, false);
    decks[0].AddPlayer("player1");
  }
  
  
  /* Sets up HoloDecks for adventure. */
  public void DeployHoloDeck(){
    decks = new List<HoloDeck>();
    HoloDeck hd = CreateDeck(0);
  }
  
  public void LoadOverworld(
    int x,
    int y, 
    int door,
    int deck,
    bool saveFirst
  ){
    //MonoBehaviour.print(x + "," + y + "," + door + "," + deck + "," + saveFirst);
    HoloDeck hd = GetDeck(deck);
    if(hd == null){ MonoBehaviour.print("Deck " + deck + " is null"); return; }
    hd.LoadExterior(x, y, door, saveFirst);
    
  }
  
  public void LoadRoom(
    int building, 
    string room, 
    int door, 
    int deck, 
    bool saveFirst
  ){
    HoloDeck hd = GetDeck(deck);
    if(hd == null){ MonoBehaviour.print("Deck " + deck + " is null"); return; }
    hd.LoadRoom(building, room, door, saveFirst);
  
  }
  
  public HoloDeck CreateDeck(int id){
    HoloDeck hd = gameObject.AddComponent<HoloDeck>();
    decks.Add(hd);
    hd.id = id;
    return hd;
  }
  
  /* Returns HoloDeck of specified id, or null. */
  public HoloDeck GetDeck(int id){
    foreach(HoloDeck deck in decks){
      if(deck.id == id){ return deck; }
    }
    return null;
  }
  
  /*
    Initializes Starting quests for a new adventure.
  */
  void CreateStartingQuests(){
    MonoBehaviour.print("method stub");
  }
  
  /* Returns specified room from map, or null. */
  public Cell GetRoom(int building, string room){
    foreach(Building b in map.buildings){
      if(b.id == building){
        foreach(Cell c in b.rooms){
          if(c.name == room){ return new Cell(c); }
        }
      }
    }
    return null;
  }
  
  /* Return specific exterior from map, or null */
  public Cell GetExterior(int x, int y){
    foreach(Cell c in map.exteriors){
      if(c.x == x && c.y == y){ return new Cell(c); }
    }
    return null;
  }
  
  /* Returns bulding of specified id, or null. */
  public Building GetBuilding(int building){
    foreach(Building b in map.buildings){
      if(b.id == building){ return b; }
    }
    return null;
  }
  
  /* Returns this Adventure's save. */
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
  
  public void RegisterPlayer(Actor actor){
    foreach(HoloDeck deck in decks){
      if(deck.RegisterPlayer(actor)){ return; }
    }
  }
  
  /* Returns a door from building */
  public DoorRecord GetBuildingDoor(int building, string room, int doorId){
    Building b = GetBuilding(building);
    if(b == null){ MonoBehaviour.print("Couldn't find building"); return null; }
    foreach(DoorRecord dr in b.doors){
      if(dr.room == room && dr.id == doorId){ return dr; }
      else{ MonoBehaviour.print(dr.room + ":"+ room + "," + dr.id + ":" + doorId); }
    }
    return null;
  }
  
  /* Loads the Data from this GameRecord */
  public void LoadData(GameRecord gr){
    MonoBehaviour.print("method stub");
  }
  
  
  
  
  
}
