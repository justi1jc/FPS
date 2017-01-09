/*
*
*        Author: James Justice
*
*
*
*
*
*
*
*
*
*/
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class QuadNode : MonoBehaviour {
	public List<GameObject> objects;
	public QuadNode[] neighbors;
	public Terrain terrain;
	public TerrainData terrainData;
	public int x;
	public int y;
	//A quad node stores neighbor information for terrain
	//0 1 2
	//3 4 5
	//6 7 8

	void Start () {
		if(!terrain){
			terrain = GetComponent<Terrain> ();
		}
		if(terrain && !terrainData){
			terrainData = terrain.terrainData;
		}
		neighbors = new QuadNode[9];
		neighbors [4] = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	Terrain GetNeighbor(int direction){
		return neighbors [direction].GetComponent<Terrain> ();
	}

	public void SetNeighborNode(int direction, QuadNode neighbor){
		neighbors [direction] = neighbor;
	}
	public void SetNeighbor(int direction, Terrain neighbor){
		SetNeighborNode (direction, neighbor.GetComponent<QuadNode>());
	}

	public void CheckForNeighbors(QuadNode[] nodes, int count){
		//Linear search for adjacent nodes
		List<QuadNode> candidates = new List<QuadNode>();

		for(int i =0; i< count; i++){
			if (i<nodes.Length && nodes[i] != null && nodes[i] != this && nodes[i].x < x + 2 && nodes[i].x > x - 2) {
				lock (candidates) {
					candidates.Add (nodes [i]);
				}
			}
		}
		List<QuadNode> finalists = new List<QuadNode> ();
		for(int i = 0; i< candidates.Count; i++){
			if(candidates[i].y < y+2 && candidates[i].y > y-2){
				lock (candidates) {
					finalists.Add (candidates [i]);
				}
			}
		}
		//0 1 2
		//3 4 5
		//6 7 8
		for(int i = 0; i< finalists.Count; i++){
			if (finalists[i].x < x) {
				if (finalists[i].y < y) {
					if(!neighbors[6]){
						SetNeighborNode (6, finalists[i]);
					}
				} else if (finalists[i].y > y) {
					if(!neighbors[0]){
						SetNeighborNode (0, finalists[i]);
					}
				} else {
					if(!neighbors[3]){
						SetNeighborNode (3, finalists[i]);
					}
				}
			} else if (finalists[i].x > x) {
				if (finalists[i].y < y) {
					if(!neighbors[8]){
						SetNeighborNode (8, finalists[i]);
					}
				} else if (finalists[i].y > y) {
					if(!neighbors[2]){
						SetNeighborNode (2, finalists[i]);
					}
				} else {
					if(!neighbors[5]){
						SetNeighborNode (5, finalists[i]);
					}
				}
			} else {
				if (finalists[i].y < y) {
					if(!neighbors[7]){
						SetNeighborNode (7, finalists[i]);
					}
				} else if (finalists[i].y > y) {
					if(!neighbors[1]){
						SetNeighborNode (1, finalists[i]);
					}
				} else {
					if(!neighbors[4]){
						SetNeighborNode (4, finalists[i]);
					}
				}
			}
		}

	}
	public void LoadData(QuadNodeData data){
		while(objects.Count > 0){
			GameObject obj = objects [0];
			objects.Remove (obj);
			Destroy(obj);
		}
		gameObject.GetComponent<Terrain> ().terrainData.SetHeights(0,0, data.heightMap);
		for (int i = 0; i < data.posX.Count; i++) {
			
			GameObject prefab = (GameObject)Resources.Load (data.prefabName [i], typeof(GameObject));

			if (prefab) {
				Vector3 pos = new Vector3 (data.posX [i], data.posY [i], data.posZ [i]);
				Quaternion rot = Quaternion.LookRotation (new Vector3 (data.rotX [i], data.rotY [i], data.rotZ [i]));
				GameObject go = (GameObject)Instantiate (prefab, pos, rot);
			
				go.name = data.prefabName [i];
				if (go) {
					
					objects.Add (go);


				}
			} else {
				print ("prefab null");
			}
		}
	}
}
