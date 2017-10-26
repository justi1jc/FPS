/*
  This class represents an individual map in the Arena and provides the means
  to parse map data from /Resources/maps.txt and associated images in the 
  textures directory.
  
  Note: Available maps should be defined in /Resources/maps.txt with the
  following format:
  
  MAP
  <Map name>
  <Description>
  <thumnail location>
  <GameMode>*[:<other GameMode>]
  
  
  END
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class ArenaMap{
  const string mapPath = "/Resources/maps.txt"; // The location of the map file.
  const string texturePath = "Textures/Thumbnails/"; // Location of textures.
  public string name, description;
  public Texture thumbnail; // Preview image for map.
  public int[] gameModes; // Gamemodescompatible with this map.
  
  public string ToString(){
    string ret = name;
    if( gameModes == null){ return ret; }
    for(int i = 0; i < gameModes.Length; i++){
      ret += "," + Arena.GameModeName(gameModes[i]);
    }
    return ret;
  }
  
  /* Returns maps parsed from MapParser. */
  public static List<ArenaMap> GetMaps(){
    MapParser mp = new MapParser();
    return mp.Parse();
  }
  
  private class MapParser{
    int lineCount;
    
    /* Public constructor to allow ArenaMap access. */
    public MapParser(){}
    
    public List<ArenaMap> Parse(){
      lineCount = 0;
      List<ArenaMap> ret = new List<ArenaMap>();
      try{
        string path = Application.dataPath + mapPath;
        if(!File.Exists(path)){
          MonoBehaviour.print("Maps file does not exist at " + path); 
          return ret;
        }
        using(StreamReader sr = new StreamReader(path)){
          string line = sr.ReadLine();
          while(line != null && line.ToUpper() != "END"){
            if(line.ToUpper() == "MAP"){
              ArenaMap am = (ParseMap(sr));
              if(am == null){ 
                MonoBehaviour.print("Map null" + lineCount);
                return new List<ArenaMap>();
              }
              ret.Add(am);
            }
            line = sr.ReadLine();
            lineCount++;
          }
        }
      }catch(Exception e){ MonoBehaviour.print("Exception:" + e); }
      return ret;
    }
    
    /* Parses all the maps defined in the maps.txt file. */
    private ArenaMap ParseMap(StreamReader sr){
      ArenaMap ret = new ArenaMap();
      try{
        string line = sr.ReadLine();
        lineCount++;
        ret.name = line;
        
        line = sr.ReadLine();
        ret.description = line;
        
        line = sr.ReadLine();
        ret.thumbnail = Resources.Load(texturePath + line) as Texture;
        
        line = sr.ReadLine();
        ret.gameModes = LineToGameModes(line);
        
      }catch(Exception e){ MonoBehaviour.print("Exception:" + e); }
      return ret;
    }
    
    /* Parses all the : delimited integers from a string and returns them in
       an array. 
    */
    private int[] LineToGameModes(string line){
      string[] entries = line.Split(':');
      List<int> ret = new List<int>();
      for(int i = 0; i < entries.Length; i++){
        int x = -1;
        Int32.TryParse(entries[0], out x);
        if(x != -1){ ret.Add(x); }
      }
      return ret.ToArray();
    }
  }

}
