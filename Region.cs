/*
      Author: James Justice
      
      A Region stores a the pathfinding information 
      of a given space and is responsible for saving the
      objects active in this region.
      
      Ints x and y are used to keep track of the region's location on the
      overworld, but -1 is used for interior regions(Dungeons).
      
      
      GeneratePaths()
         
      
*/


﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class Region : MonoBehaviour {
	public GameObject boundaryMin;
	public GameObject boundaryMax;
	public string displayName;
	public string prefabName;
   public int x;
   public int y;
   public int nextId;
   public float nodeDistance;
	public List<NavNode> nodes = new List<NavNode>();
	public NavNode[] visibleNodes;
	public Vector3 nodeExtents;
	void Awake () {
	   
	}
	void Start(){
	   GameController.controller.AddRegion(this);
	   if(GameController.controller.debug == 2)
	      StartCoroutine("MapGenInputs");
	   if(GameController.controller.debug == 3){  
	     GameController.controller.SaveToMaster(this);
	     print(displayName + " saved to master.");
	     if(SceneManager.GetActiveScene().buildIndex+1 <
               SceneManager.sceneCountInBuildSettings){
           SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
      }
        
      if(GameController.controller.debug == 4){
        StartCoroutine("GeneratePaths");
	   }
	}
	IEnumerator MapGenInputs(){
	   print("P-Gen paths\nM-Save to Master");
	   while(true){
	      if(Input.GetKeyDown(KeyCode.P))
	         StartCoroutine("GeneratePaths");
	      if(Input.GetKeyDown(KeyCode.M)){
            print("Saved "+ displayName +" to master.");
	         GameController.controller.SaveToMaster(this);
	      }
	      yield return new WaitForSeconds(0.01f);
	   }
	}
	void SaveRegion(){
	   GameController.controller.RemoveRegion(this);
	}
   void LoadRegion(){
      
   }
	
	IEnumerator GeneratePaths(){
	   print("Path generation initiated.");
	   nextId = 0;
	   Vector2 nodePos = new Vector2(boundaryMin.transform.position.x, boundaryMin.transform.position.z);
	   while(nodePos.x < boundaryMax.transform.position.x){
	      while(nodePos.y < boundaryMax.transform.position.z){
	         yield return StartCoroutine("AddNode",nodePos);
	         nodePos = new Vector2(nodePos.x, nodePos.y+nodeDistance);
	         yield return new WaitForSeconds(0.01f);
	      }
	      nodePos = new Vector2(nodePos.x+nodeDistance, boundaryMin.transform.position.z);
	      yield return new WaitForSeconds(0.01f);
      }
      print("Nodes added.");
      AddEdges();
      print("Edges added.");
      CalculateIslands();
      print("Islands calculated.");
      visibleNodes = nodes.ToArray();
      yield return new WaitForSeconds(0.1f);
      if(GameController.controller.debug == 3){
         GameController.controller.SaveToMaster(this);
         if(SceneManager.GetActiveScene().buildIndex+1 < SceneManager.sceneCountInBuildSettings){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
         }
      }
	}
   IEnumerator AddNode(Vector2 location){
      Vector3 termPos = new Vector3(location.x, boundaryMax.transform.position.y,location.y);
      Vector3 curPos = termPos;
      bool space = false;
      bool floor = false;
      RaycastHit[] hits = new RaycastHit[0];
      while(curPos.y > boundaryMin.transform.position.y){
         yield return new WaitForSeconds(0.0001f);
         while(hits.Length == 0 && curPos.y > boundaryMin.transform.position.y){
            //yield return new WaitForSeconds(0.001f);
            termPos = curPos;
            curPos =new Vector3(curPos.x, curPos.y-=0.2f, curPos.z);
            Vector3 halfExtents = nodeExtents; 
            hits = Physics.BoxCastAll(curPos,halfExtents, transform.forward,
                transform.rotation, halfExtents.y);
            if(!space && hits.Length == 0)
               space = true;
            if(space && hits.Length > 0)
               floor = true;
         }
         if(space && floor){
            NavNode node = new NavNode();
            node.position = termPos;
            node.id = nextId;
            nextId++;
            nodes.Add(node);
            space = false;
            floor = false;
            print("Node added.");
         }
         hits = new RaycastHit[0];
      }
      
      yield return new WaitForSeconds(0.01f);
   }
   public void AddEdges(){
      foreach(NavNode node in nodes){
         List<NavNode> candidates = new List<NavNode>();
         RaycastHit[] hits;
         foreach(NavNode other in nodes){
            if(Vector3.Distance(node.position, other.position) < nodeDistance)
               candidates.Add(other);
         }
         foreach(NavNode candidate in candidates){
            Vector3 center = Vector3.Lerp(node.position,
            candidate.position,0.5f);
            Vector3 halfExtents = nodeExtents;
            hits = Physics.BoxCastAll(center,halfExtents, transform.forward,
                  transform.rotation,halfExtents.x);
            if(hits.Length == 0){
               node.edges.Add(candidate.id);
            }
         }
         
      }
      
   }
   public void CalculateIslands(){
      int currentIsland = 0;
      for(int i = 0; i< nodes.Count; i++){
         if(nodes[i].island < 0){
            CalculateIsland(currentIsland, nodes[i]);
            currentIsland++;
         }
      }
   }
   public void CalculateIsland(int currentIsland, NavNode node){
      node.island = currentIsland;
      
      foreach(int i in node.edges){
         if(i < nodes.Count && nodes[i] != null && nodes[i].island <0)
            CalculateIsland(currentIsland, nodes[i]);
      }
   }
   void RemoveNode(){
      
   }
   public List<ItemData> GatherItems(){
      List<ItemData> items = new List<ItemData>();
      RaycastHit[] hits;
      Vector3 center = Vector3.Lerp(boundaryMax.transform.position,
         boundaryMin.transform.position,0.5f);
      Vector3 halfExtents = new Vector3((boundaryMax.transform.position.x-boundaryMin.transform.position.x/2f),
            (boundaryMax.transform.position.y-boundaryMin.transform.position.y)/2f,
            (boundaryMax.transform.position.z-boundaryMin.transform.position.z)/2f);
      hits = Physics.BoxCastAll(center,halfExtents, transform.forward, transform.rotation,halfExtents.x);
      foreach(RaycastHit hit in hits){
         FPSItem item = hit.collider.gameObject.GetComponent<FPSItem>();
         if(item!= null){
            items.Add(item.SaveData());
         }
         
         
      }
      return items;
   }
   
   public bool Contains(Vector3 pos){
      if(pos.x < boundaryMax.transform.position.x &&
            pos.x > boundaryMin.transform.position.x &&
            pos.y < boundaryMax.transform.position.y &&
            pos.y > boundaryMin.transform.position.y &&
            pos.z < boundaryMax.transform.position.z &&
            pos.z > boundaryMin.transform.position.z )
         return true;
      return false;
   }
   public ItemData GetData(){
      ItemData dat = new ItemData();
      dat.floats.Add(boundaryMin.transform.position.x);
      dat.floats.Add(boundaryMin.transform.position.y);
      dat.floats.Add(boundaryMin.transform.position.z);
      dat.floats.Add(boundaryMax.transform.position.x);
      dat.floats.Add(boundaryMax.transform.position.y);
      dat.floats.Add(boundaryMax.transform.position.z);
      dat.strings.Add(displayName);
      dat.strings.Add(prefabName);
      dat.ints.Add(nodes.Count);
      dat.displayName = displayName;
      dat.prefabName = prefabName;
      foreach(NavNode node in nodes){
         dat.itemData.Add(node.GetData());
      }
      List<ItemData> contents =  GatherItems();
      foreach(ItemData item in contents){
         dat.itemData.Add(item);
      }
      return dat;
   }
   /*
   TODO: Consider the fate of this method
   public List<ItemData> GatherScenery(){
      List<ItemData> contents = new List<ItemData>();
      RaycastHit[] hits;
      Vector3 center = Vector3.Lerp(boundaryMax.transform.position,
         boundaryMin.transform.position,0.5f);
      Vector3 halfExtents = new Vector3((boundaryMax.transform.position.x-boundaryMin.transform.position.x/2f),
            (boundaryMax.transform.position.y-boundaryMin.transform.position.y)/2f,
            (boundaryMax.transform.position.z-boundaryMin.transform.position.z)/2f);
      hits = Physics.BoxCastAll(center,halfExtents, transform.forward, transform.rotation,halfExtents.x);
      foreach(RaycastHit hit in hits){
         FPSItem scenery = hit.collider.gameObject.transform.GetComponent(typeof(FPSItem)) as FPSItem;
         if(scenery != null)
            contents.Add(scenery.SaveData());
         
      }
      return contents;
   }
   */
   public void DeployContents(List<ItemData> contents){
      foreach(ItemData dat in contents){
         GameObject prefab = (GameObject)Resources.Load(dat.prefabName, typeof(GameObject));
         if(prefab != null){
            GameObject itemgo = (GameObject)GameObject.Instantiate(prefab, 
			   transform.position, transform.rotation) as GameObject;
			   FPSItem item = itemgo.transform.GetComponent(typeof(FPSItem)) as FPSItem;
			   if(item != null){
			      item.LoadData(dat);
			   }
			   else{
			      print("FPSItem null" + dat.displayName);
			   }
			}
			else
			   print("Prefab null" + dat.prefabName + ","+ dat.displayName);
      }
   }
   public void ClearContents(){
      print("Clearing contents");
      RaycastHit[] hits;
      Vector3 center = Vector3.Lerp(boundaryMax.transform.position,
         boundaryMin.transform.position,0.5f);
      Vector3 halfExtents = new Vector3((boundaryMax.transform.position.x-boundaryMin.transform.position.x/2f),
            (boundaryMax.transform.position.y-boundaryMin.transform.position.y)/2f,
            (boundaryMax.transform.position.z-boundaryMin.transform.position.z)/2f);
      hits = Physics.BoxCastAll(center,halfExtents, transform.forward, transform.rotation,halfExtents.x);
      foreach(RaycastHit hit in hits){
         FPSItem scenery = hit.collider.gameObject.transform.GetComponent(typeof(FPSItem)) as FPSItem;
         if(scenery != null)
            Destroy(hit.collider.gameObject);
         else{
            HumanoidController character = hit.collider.gameObject.GetComponent<HumanoidController>();
            if(character != null)
               Destroy(character.gameObject);
         }   
      }
   }
   public void LoadData(ItemData dat){
      displayName = dat.displayName;
      print("Loading " + displayName);
      prefabName = dat.prefabName;
      boundaryMin.transform.position= new Vector3(dat.floats[0], dat.floats[1],dat.floats[2]);
      boundaryMax.transform.position= new Vector3(dat.floats[3], dat.floats[4],dat.floats[5]);
      for(int i = 0; i< dat.ints[0]; i++)
         nodes.Add(new NavNode(dat.itemData[i]));
      DeployContents(dat.itemData);
   }
   public bool Interior(){
      if(x < 0 || y < 0)
         return true;
      return false;
   }
}
