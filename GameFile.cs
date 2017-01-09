using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class GameFile {
   public string gameName;
   public string regionName;
   public float spawnX,spawnY,spawnZ;
   public float spawnrX, spawnrY, spawnrZ;
   public List<ItemData> activeRegionData;
   public List<ItemData> interiorsData;
   public List<ItemData> overworldData;
   public List<ItemData> playersData;
   public List<ItemData> quests;
   public int coordX;
   public int coordY;
   public GameFile(){
      activeRegionData = new List<ItemData>();
      interiorsData = new List<ItemData>();
      overworldData = new List<ItemData>();
      playersData = new List<ItemData>();
      quests = new List<ItemData>();
   }
	
}
