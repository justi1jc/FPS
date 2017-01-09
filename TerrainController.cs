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

public class TerrainController : MonoBehaviour {
	public QuadNode center;
	public GameObject player;
	public QuadNode[] activeNodes;
	public List<QuadNodeData> loadedNodes;


	void Start () {
		TerrainSaver.Load ();
		loadedNodes = TerrainSaver.savedNodes;
		activeNodes = new QuadNode[9];
		activeNodes [4] = center;
		for(int i = 0; i< 9; i++){
			if(i!=4){
				
				activeNodes [i] = AddNode (center, i);
			}
		}

	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			for(int i = 0; i < 9; i++){
				SaveNode (activeNodes[i]);
			}
			TerrainSaver.Save (loadedNodes);
			Application.Quit();
		}
		if(Input.GetKeyDown(KeyCode.P)){
			AddObject (activeNodes [Direction (center, player)], player.transform.position + (2 * player.transform.forward), "Prefabs/Rifle");
		}
		if( Direction(center, player) != 4){
			Extend (Direction(center, player));

		}
	}
	void Extend(int direction){
		for(int i = 0; i < 9; i++){
			SaveNode (activeNodes[i]);
		}
		//print("Extended in direction " + direction);
		switch(direction){
		case 0:
			{
				RemoveNode (activeNodes[2]);
				RemoveNode (activeNodes[5]);
				RemoveNode (activeNodes[6]);
				RemoveNode (activeNodes[7]);
				RemoveNode (activeNodes[8]);
				activeNodes[8] = activeNodes[4];
				activeNodes[7] = activeNodes[3];
				activeNodes[6] = activeNodes[1];
				activeNodes[4] = activeNodes[0];
				center = activeNodes [4];
				activeNodes[0] = AddNode(center, 0);
				activeNodes[1] = AddNode(center, 1);
				activeNodes[2] = AddNode(center, 2);
				activeNodes[3] = AddNode(center, 3);
				activeNodes[6] = AddNode(center, 6);

			}
			break;
		case 1:
			{
				RemoveNode (activeNodes[6]);
				RemoveNode (activeNodes[7]);
				RemoveNode (activeNodes[8]);
				activeNodes[8] = activeNodes[5];
				activeNodes[7] = activeNodes[4];
				activeNodes[6] = activeNodes[3];
				activeNodes[5] = activeNodes[2];
				activeNodes[4] = activeNodes[1];
				activeNodes[3] = activeNodes[0];
				center = activeNodes [4];
				activeNodes[0] = AddNode(center,0);
				activeNodes[1] = AddNode(center,1);
				activeNodes[2] = AddNode(center,2);
			}
			break;
		case 2:
			{
				RemoveNode (activeNodes[0]);
				RemoveNode (activeNodes[3]);
				RemoveNode (activeNodes[6]);
				RemoveNode (activeNodes[7]);
				RemoveNode (activeNodes[8]);
				activeNodes[7] = activeNodes[5];
				activeNodes[6] = activeNodes[4];
				activeNodes[4] = activeNodes[2];
				activeNodes[3] = activeNodes[1];
				center = activeNodes [4];
				activeNodes[0] = AddNode(center, 0);
				activeNodes[1] = AddNode(center, 1);
				activeNodes[2] = AddNode(center, 2);
				activeNodes[3] = AddNode(center, 5);
				activeNodes[6] = AddNode(center, 8);

			}
			break;
		case 3:
			{
				RemoveNode (activeNodes[2]);
				RemoveNode (activeNodes[5]);
				RemoveNode (activeNodes[8]);
				activeNodes[8] = activeNodes[7];
				activeNodes[7] = activeNodes[6];
				activeNodes[5] = activeNodes[4];
				activeNodes[4] = activeNodes[3];
				activeNodes[2] = activeNodes[1];
				activeNodes[1] = activeNodes[0];
				center = activeNodes [4];
				activeNodes[0] = AddNode(center,0);
				activeNodes[3] = AddNode(center,3);
				activeNodes[6] = AddNode(center,6);
			}
			break;
		case 5:
			{
				RemoveNode (activeNodes[0]);
				RemoveNode (activeNodes[3]);
				RemoveNode (activeNodes[6]);
				activeNodes[0] = activeNodes[1];
				activeNodes[1] = activeNodes[2];
				activeNodes[3] = activeNodes[4];
				activeNodes[4] = activeNodes[5];
				activeNodes[6] = activeNodes[7];
				activeNodes[7] = activeNodes[8];
				center = activeNodes [4];
				activeNodes[2] = AddNode(center,2);
				activeNodes[5] = AddNode(center,5);
				activeNodes[8] = AddNode(center,8);
			}
			break;
		case 6:
			{
				RemoveNode (activeNodes[0]);
				RemoveNode (activeNodes[3]);
				RemoveNode (activeNodes[6]);
				RemoveNode (activeNodes[7]);
				RemoveNode (activeNodes[8]);
				activeNodes[7] = activeNodes[5];
				activeNodes[6] = activeNodes[4];
				activeNodes[4] = activeNodes[2];
				activeNodes[3] = activeNodes[1];
				center = activeNodes [4];
				activeNodes[0] = AddNode(center, 0);
				activeNodes[3] = AddNode(center, 3);
				activeNodes[6] = AddNode(center, 6);
				activeNodes[7] = AddNode(center, 7);
				activeNodes[8] = AddNode(center, 8);

			}
			break;
		case 7:
			{
				RemoveNode (activeNodes[0]);
				RemoveNode (activeNodes[1]);
				RemoveNode (activeNodes[2]);
				activeNodes[0] = activeNodes[3];
				activeNodes[1] = activeNodes[4];
				activeNodes[2] = activeNodes[5];
				activeNodes[3] = activeNodes[6];
				activeNodes[4] = activeNodes[7];
				activeNodes[5] = activeNodes[8];
				center = activeNodes [4];
				activeNodes[6] = AddNode(center,6);
				activeNodes[7] = AddNode(center,7);
				activeNodes[8] = AddNode(center,8);
			}
			break;
		case 8:
			{
				RemoveNode (activeNodes[0]);
				RemoveNode (activeNodes[1]);
				RemoveNode (activeNodes[2]);
				RemoveNode (activeNodes[3]);
				RemoveNode (activeNodes[6]);
				activeNodes[0] = activeNodes[4];
				activeNodes[1] = activeNodes[5];
				activeNodes[3] = activeNodes[7];
				activeNodes[4] = activeNodes[8];
				center = activeNodes [4];
				activeNodes[2] = AddNode(center, 2);
				activeNodes[5] = AddNode(center, 5);
				activeNodes[6] = AddNode(center, 6);
				activeNodes[7] = AddNode(center, 7);
				activeNodes[8] = AddNode(center, 8);
			}
			break;
		}
		for(int i = 0; i < 9; i++){
			LoadNode (activeNodes[i]);
		}
	}
	QuadNode AddNode(QuadNode parent, int direction){
		
		int x = 0;
		int y = 0;
		switch(direction){
		case 0:
			x = parent.x - 1;
			y = parent.y + 1;
			break;
		case 1:
			x = parent.x;
			y = parent.y + 1;
			break;
		case 2:
			x = parent.x + 1;
			y = parent.y + 1;
			break;
		case 3:
			x = parent.x - 1;
			y = parent.y;
			break;
		case 5:
			x = parent.x + 1;
			y = parent.y;
			break;
		case 6:
			x = parent.x - 1;
			y = parent.y - 1;
			break;
		case 7:
			x = parent.x;
			y = parent.y - 1;
			break;
		case 8:
			x = parent.x + 1;
			y = parent.y - 1;
			break;
		}

		float sizeX = parent.terrain.terrainData.size.x;
		if(parent.x < x){
			sizeX *= -1;
		}
		else if(parent.x == x){
			sizeX = 0;
		}
		float sizeZ = parent.terrain.terrainData.size.z;
		if(parent.y < y){
			sizeZ *= -1;
		}
		else if(parent.y == y){
			sizeZ = 0;
		}
		Vector3 pos = new Vector3(parent.transform.position.x + sizeX, parent.transform.position.y, parent.transform.position.z + sizeZ);
		GameObject obj = (GameObject)Instantiate (parent.gameObject, pos, parent.transform.rotation);
		QuadNode node = obj.GetComponent<QuadNode> ();
		node.x = x;
		node.y = y;
		LoadNode (node);

		return node;

	}
	void RemoveNode(QuadNode node){
		SaveNode (node);
		for(int i = node.objects.Count-1; i > -1; i--){
			GameObject obj = node.objects [i];
			node.objects.Remove (obj);
			Destroy (obj);
		}
		QuadNodeData data = new QuadNodeData (node);
		bool added = false;
		for(int i = 0; i< loadedNodes.Count; i++){
			if(data.x == loadedNodes[i].x && data.y == loadedNodes[i].y){
				loadedNodes[i] = data;
				added = true;
				break;
			}
		}
		if(!added){
			loadedNodes.Add (data);
		}

		Destroy (node.gameObject);
	}
	void AddObject(QuadNode node, Vector3 pos, string prefabName){
		GameObject prefab = (GameObject)Resources.Load (prefabName, typeof(GameObject));
		GameObject obj = (GameObject)Instantiate (prefab, pos, node.transform.rotation);
		obj.name = prefabName;
		node.objects.Add (obj);
		SaveNode (node);
	}
	void LoadNode(QuadNode node){
		foreach(QuadNodeData data in loadedNodes){
			if(data.x == node.x && data.y == node.y){
				node.LoadData (data);

				break;
			}
		}
	}
	void SaveNode(QuadNode node){
		for(int i =0; i< loadedNodes.Count; i++){
			if(node.x == loadedNodes[i].x && node.y == loadedNodes[i].y){
				loadedNodes [i] = new QuadNodeData (node);

				return;
			}
		}
		loadedNodes.Add(new QuadNodeData(node));
	}
	int Direction(QuadNode node, GameObject target){
		Vector3 pos = target.transform.position;
		Terrain terrain = node.GetComponent<Terrain> ();
		Vector2 max = new Vector2(node.transform.position.x + terrain.terrainData.size.x, terrain.transform.position.z + terrain.terrainData.size.z);
		Vector2 min = new Vector2 (node.transform.position.x, node.transform.position.z);
		if(pos.x < max.x && pos.x > min.x && pos.z < max.y && pos.z > min.y ){
			return  4;
		}
		if (pos.x < min.x) {
			if (pos.z < min.y) {
				return 2;
			} else if (pos.z > max.y) {
				return 8;
			} else {
				return 5;
			}
		} else if (pos.x > max.x) {
			if (pos.z < min.y) {
				return 0;
			} else if (pos.z > max.y) {
				return 6;
			} else {
				return 3;
			}
		} else {
			if (pos.z < min.y) {
				return 1;
			} else if (pos.z > max.y) {
				return 7;
			} else {
				print ("TerrainController direction computation error");
				return -1;
			}
		}

	}
}
