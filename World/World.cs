/*
    A World handles a particular generated world in adventure mode,
    including quests and the HoloDecks.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class World{
  public string name;
  public List<Quest> quests;
  public MapRecord map;
  public List<HoloDeck> decks;
  public GameObject gameObject;
  public List<Data> playerData;
  private readonly object syncLock = new object(); // Mutex lock
  
  public World(){
    decks = null;
    playerData = null;
    this.gameObject = new GameObject();
  }
  
  /* Creates a new adventure. */
  public void CreateAdventure(){
    name = "game";
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
    if(decks == null){ decks = new List<HoloDeck>(); }
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
  
  public void SetRoom(int building, string room, Cell c){
    if(c == null){ MonoBehaviour.print("Cell null"); return; }
    foreach(Building b in map.buildings){
      if(b.id == building){
        for(int i = 0; i < b.rooms.Count; i++){
          if(b.rooms[i].name == room){
            b.rooms[i] = new Cell(c);
            return;
          }
        }
      }
    }
  }
  
  /* Return specific exterior from map, or null */
  public Cell GetExterior(int x, int y){
    foreach(Cell c in map.exteriors){
      if(c.x == x && c.y == y){ return new Cell(c); }
    }
    return null;
  }
  
  public void SetExterior(Cell c){
    if(c == null){ return; }
    for(int i = 0; i < map.exteriors.Count; i++){
      Cell otherC = map.exteriors[i];
      if(otherC.x == c.x && otherC.y == c.y){
        map.exteriors[i] = c;
        return;
      }
    }
  }
  
  /* Returns bulding of specified id, or null. */
  public Building GetBuilding(int building){
    foreach(Building b in map.buildings){
      if(b.id == building){ return b; }
    }
    return null;
  }
  
  public void RegisterPlayer(Actor actor){
    foreach(HoloDeck deck in decks){
      if(deck.RegisterPlayer(actor)){ return; }
    }
  }
  
  public List<Data> GetPlayerData(){
    List<Data> ret = new List<Data>();
    foreach(HoloDeck deck in decks){
      ret.AddRange(deck.GetPlayers());
    }
    MonoBehaviour.print("Saved" + ret.Count + " players.");
    return ret;
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
  
  public void SaveGame(){
    MonoBehaviour.print("Saving game.");
    lock(syncLock){
      BinaryFormatter bf = new BinaryFormatter();
      string path = Application.persistentDataPath + "/" + name + ".save"; 
      using(FileStream file = File.Create(path)){
        GameRecord record = GetData();
        bf.Serialize(file, record);
        file.Close();
      }
    }
  }
  
  public void Clear(){
    foreach(HoloDeck deck in decks){ deck.ClearContents(); }
    GameObject.Destroy(gameObject);
  }
  
  public void LoadGame(string fileName){
    GameRecord record = LoadFile(fileName);
    if(record == null){ MonoBehaviour.print("Null game record"); return; }
    LoadData(record);
    if(playerData.Count == 0){ MonoBehaviour.print("There are no players."); return; }
    HoloDeck hd = CreateDeck(0);
    Data player = playerData[0];
    Cell c = player.lastPos;
    if(c.interior){
      hd.LoadRoom(c.id, c.name, -1, false);
    }
    else{
      hd.LoadExterior(c.x, c.y, -1, false);
    }
    hd.AddPlayer(player, true);
    playerData.Remove(player);
  }
  
  /* Returns a GameRecord containing data from a specified file, or null.*/
  GameRecord LoadFile(string fileName){
    lock(syncLock){
      BinaryFormatter bf = new BinaryFormatter();
      string path = Application.persistentDataPath + "/" + fileName + ".save";
      if(!File.Exists(path)){ return null; }
      using(FileStream file = File.Open(path, FileMode.Open)){
        GameRecord record = (GameRecord)bf.Deserialize(file);
        file.Close();
        return record;
      }
    }
  }
  
  /* Returns this Adventure's save. */
  public GameRecord GetData(){
    GameRecord record = new GameRecord();
    record.sessionName = Session.session.sessionName;
    for(int i = 0; i < decks.Count; i++){
      if(decks[i].interior){ decks[i].SaveInterior(); }
      else{ decks[i].SaveExterior(); }
    }
    record.sessionName = name;
    record.map = map;
    record.playerData = GetPlayerData();
    return record;
  }
  
  /* Loads the Data from this GameRecord */
  public void LoadData(GameRecord gr){
    map = gr.map;
    name = gr.sessionName;
    playerData = gr.playerData;
  }
  
  
  
  
  
}
