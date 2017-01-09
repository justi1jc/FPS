using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour {
   public static GameController controller = null;
   public string gameName;
   public int debug;
   public bool paused;
   public float masterVolume;
   public float musicVolume;
   public float effectsVolume;
   public string regionName;
   public int coordX;
   public int coordY;
   [System.NonSerialized]
   public List<ItemData> questData = new List<ItemData>();
   public List<Quest> quests = new List<Quest>();
   public bool fileAccess = false;
   List<HumanoidController> players = new List<HumanoidController>();
   [System.NonSerialized]
   ItemData[] playersData;
   public Vector3 spawnPoint;
   public Vector3 spawnRot;
   public Region[] overworld = new Region[9]; 
   public Region interiorRegion;
   [System.NonSerialized]
   private List<ItemData> interiorsData;
   [System.NonSerialized]
   private List<ItemData> overworldData;
	void Awake () {
	   playersData = new ItemData[4];
	   DontDestroyOnLoad(gameObject);
	   overworld = new Region[9];
	   interiorsData = new List<ItemData>();
	   overworldData = new List<ItemData>();
	   if(controller == null){
	      controller = this;
	   }
	   else{
	      Destroy(gameObject);
	   }
	}
	public void AddPlayer(HumanoidController player){
	   if(players.Count < playersData.Length)
	      players.Add(player);
	}
   public void AddRegion(Region r){
      if(r.x < 0 || r.y< 0){
         interiorRegion = r;
         bool found = false;
         foreach(ItemData d in interiorsData){
            if(d.displayName == r.displayName)
                found = true;
         }
         if(!found)
            interiorsData.Add(r.GetData());
      }
      else{
         print(r.displayName + "x:" + r.x + " Y:" + r.y);
      }
   }
   public void RemoveRegion(Region r){
      if(r.x < 0 || r.y < 0){
         interiorRegion = null;
      }
      else{
         print("TODO: 3x3 region loading");
      }
   }
   public Region GetRegion(Vector3 pos){
      if(interiorRegion != null && interiorRegion.Contains(pos))
         return interiorRegion;
      else{
         for(int i =0; i< 9; i++){
            if(overworld[i] != null && overworld[i].Contains(pos))
               return overworld[i];
         }
      }
      return null;
   }
   public void PlaceTriggers(){
      foreach(Quest q in quests){
         if(q != null && q.state == 1)
            q.PlaceTriggers();
         
      }
   }
   public void Notify(string message){
      string[] args = message.Split(' ');
      foreach(Quest q in quests){
         if(q.displayName.Equals(args[0])){
            q.QuestAction(message.Substring(args[0].Length+1));
            return;
         }
      }
      switch(args[0]){
         case "display":
            foreach(HumanoidController p in players){
               Transform pParent = p.transform.parent;
               HUDController hc = 
                  pParent.GetComponentInChildren<HUDController>();
               hc.Display(message.Substring(args[0].Length+1));
            }
            break;
         case "speech":
            GameObject go = GameObject.Find(args[2]);
            NPC npc = null;
            int index = System.Convert.ToInt32(args[3]);
            if(go != null)
               npc = go.GetComponent<NPC>() as NPC;
            if(npc != null){      
               if(args[1].Equals("hide")|| args[1].Equals("unhide")){
                  bool visible = args[1].Equals("hide");
                  npc.GetSpeechController().nodes[index].visible = visible; 
               }
               else if(args[1].Equals("say")){
                  npc.GetSpeechController().PlayNode(index); 
               }
            }
            break;
         case "warp":
            float x = float.Parse(args[3]);
            float y = float.Parse(args[4]);
            float z = float.Parse(args[5]);
            spawnPoint = new Vector3(x,y,z);
            if(args[2].Equals("overworld")){
               int xCoord = System.Convert.ToInt32(args[6]);
               int yCoord = System.Convert.ToInt32(args[7]);
               LoadOverworld(xCoord,yCoord);
            }
            else
               LoadInterior(args[2]);
            break;  
      }
      
   }
   public void SaveGame(GameFile gf){
      if(!fileAccess){
         fileAccess = true;
         BinaryFormatter bf = new BinaryFormatter();
         FileStream file = File.Create(Application.persistentDataPath + "/" +
            gf.gameName + ".gf");
         bf.Serialize(file, gf);
         file.Close();
         fileAccess = false;
      }
   }
   public void LoadGame(string fileName){
      if(!fileAccess){
         fileAccess = true;
         if(File.Exists(Application.persistentDataPath+ "/" +fileName + ".gf")){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath+"/"+
               fileName+".gf", FileMode.Open);
            GameFile gfile = (GameFile)bf.Deserialize(file);
            file.Close();
            LoadData(gfile);
         }
         else{
            print("File not found!");
         }
         fileAccess = false;
      }
   }
   public GameFile GetData(){
      List<ItemData> activeRegionData = new List<ItemData>();
      if(interiorRegion != null){
         activeRegionData.Add(interiorRegion.GetData());
      }
      else{
         for(int i = 0; i<9; i++)
            activeRegionData.Add(overworld[i].GetData());
      }
      GameFile gf = new GameFile();
      gf.activeRegionData = activeRegionData;
      gf.overworldData = overworldData;
      gf.interiorsData = interiorsData;
      gf.gameName = gameName;
      gf.regionName = regionName;
      gf.coordX = coordX;
      gf.coordY = coordY;
      gf.spawnX = spawnPoint.x;
      gf.spawnY = spawnPoint.y;
      gf.spawnZ = spawnPoint.z;
      gf.spawnrX = spawnRot.x;
      gf.spawnrX = spawnRot.y;
      gf.spawnrX = spawnRot.z;
      foreach(HumanoidController player in players){
         if(player != null)
            gf.playersData.Add(player.GetData());
      }
      foreach(Quest q in quests)
         gf.quests.Add(q.GetData());
      return gf;
   }
   public void LoadData(GameFile gf){
      quests.Clear();
      gameName = gf.gameName;
      regionName = gf.regionName;
      coordX = gf.coordX;
      coordY = gf.coordY;
      interiorsData = gf.interiorsData;
      overworldData = gf.overworldData;
      for(int i =0; i<playersData.Length&&i<gf.playersData.Count; i++){
         playersData[i] = gf.playersData[i];
      }
      if(regionName != ""){
         SceneManager.LoadScene(1);
         SceneManager.sceneLoaded += DeployInterior;
      }
      else{
         SceneManager.LoadScene(2);
      }
      spawnPoint = new Vector3(gf.spawnX, gf.spawnY, gf.spawnZ);
      spawnRot = new Vector3(gf.spawnrX, gf.spawnrY, gf.spawnrZ);
      questData = gf.quests;
   }
   public void DeployInterior(Scene scene, LoadSceneMode mode){
      SceneManager.sceneLoaded -= DeployInterior;
      foreach(ItemData dat in playersData){
         if(dat != null){
            dat.floats[0] = spawnPoint.x;
            dat.floats[1] = spawnPoint.y;
            dat.floats[2] = spawnPoint.z;
            dat.floats[3] = spawnRot.x;
            dat.floats[4] = spawnRot.y;
            dat.floats[5] = spawnRot.z;
            GameObject prefab = Resources.Load(dat.prefabName) as GameObject;
            GameObject go = 
               Instantiate(prefab,transform.position,transform.rotation)
               as GameObject;
            HumanoidController npc = 
               go.GetComponentInChildren<HumanoidController>();
            if(npc != null){
               npc.LoadData(dat);
            }
         }
      }
      foreach(ItemData q in questData){
         GameObject qgo = new GameObject("Quest");
         Quest quest = qgo.AddComponent<Quest>();
         quest.LoadData(q);
         quests.Add(quest);
      }
      LoadInterior(regionName);
   }
   public void PackRegions(){
      if(interiorRegion != null){
         interiorsData.Add(interiorRegion.GetData());
         interiorRegion = null;
      }
      else if(overworld[0] != null){
         for(int i = 0; i<9; i++){
            overworldData.Add(overworld[i].GetData());
            overworld[i] = null;
         }
      }
   }
   public void WarpTo(string destination,
         Vector3 destPos,Vector3 destRot,int destX,int destY){
      spawnPoint = destPos;
      spawnRot = destRot;
      regionName = destination;
      for(int i =0; i<playersData.Length&&i<players.Count; i++){
         playersData[i] = players[i].GetData();
      }
      players.Clear();
      if(destX < 0 || destY < 0){
         SceneManager.sceneLoaded += DeployInterior;
         SceneManager.LoadScene(1);
      }
      else{
         print("Overworld not yet implemented");
      }
   }
   public void LoadOverworld(int x, int y){
      PackRegions();
      List<int> xCoords =new List<int>{x-4,x-3,x-2,
                                       x-1,x  ,x+1,
                                       x+2,x+3,x+3}; 
      List<int> yCoords =new List<int>{y-4,y-3,y-2,
                                       y-1,y  ,y+1,
                                       y+2,y+3,y+4};
      for(int i = 0; i<9; i++){
         
         foreach(ItemData dat in overworldData){
            if(dat.ints[0]==xCoords[i] && dat.ints[1] == yCoords[i]){
               GameObject regGO = Resources.Load(dat.prefabName) as GameObject;
               overworld[i] = regGO.transform.GetComponent<Region>();
               overworld[i].LoadData(dat);
               break;
            }
         }
      }
   }
   public void LoadInterior(string regionName){
      PackRegions();
      foreach(ItemData dat in interiorsData){
         if(dat.displayName.Equals(regionName)){
            GameObject regGO = Resources.Load(dat.prefabName) as GameObject;
            interiorRegion = regGO.transform.GetComponent<Region>();
            interiorRegion.LoadData(dat);
            break;
         }
      }
      PlaceTriggers();
   }
   public List<GameFile> GetFiles(){
      List<GameFile> files = new List<GameFile>();
      DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath+ "/");
      FileInfo[] info = dir.GetFiles("*.gf");
      if(!fileAccess){
         fileAccess = true;
         BinaryFormatter bf = new BinaryFormatter();
         
         foreach(FileInfo f in info){
            if(File.Exists(f.FullName)){
               FileStream file = File.Open(f.FullName, FileMode.Open);
               GameFile gf = (GameFile)bf.Deserialize(file);
               file.Close();
               if(gf != null)
                  files.Add(gf);
            }
         }
         fileAccess = false;
      }
      return files;
   }
   public void SaveToMaster(Region reg){
      if(!fileAccess){
         GameFile master;
         if(File.Exists(Application.persistentDataPath+ "/" + "World.MASTER")){
               master = LoadFromMaster();
            if(reg.Interior()){
               for(int i = 0; i<master.interiorsData.Count; i++){
                  if(master.interiorsData[i].displayName.Equals(reg.displayName)){
                     master.interiorsData.Remove(master.interiorsData[i]);
                     i--;
                     print("Duplicate removed:" + reg.displayName);
                  }
               }
               master.interiorsData.Add(reg.GetData());
            }
            else{
               master.overworldData.Add(reg.GetData());
            }
         }
         else{
            AddRegion(reg);
            master = GetData();
         }
         BinaryFormatter bf = new BinaryFormatter();
         FileStream file = File.Create(Application.persistentDataPath+
            "/"+"World.MASTER");
         bf.Serialize(file, master);
         file.Close();
         fileAccess = false;
      }
   }
   public GameFile LoadFromMaster(){
      GameFile master = null;
      if(!fileAccess){
         fileAccess = true;
         if(File.Exists(Application.persistentDataPath+ "/" + "World.MASTER")){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath+"/"+
               "World.MASTER", FileMode.Open);
            master = (GameFile)bf.Deserialize(file);
            file.Close();
         }
         else{
            print("File not found!");
         }
         fileAccess = false;
      }
      return master;
   }
   public void InitializeGame(string name){
      GameFile master = LoadFromMaster();
      if(master != null){
         LoadData(master);
      }
      else
         print("Master file missing!");
      gameName = name;
   }
}
