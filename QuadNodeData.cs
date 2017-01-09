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
public class QuadNodeData{
	public int x;
	public int y;
	public float[,] heightMap;
	public List<float> posX;
	public List<float> posY;
	public List<float> posZ;
	public List<float> rotX;
	public List<float> rotY;
	public List<float> rotZ;
	public List<string> prefabName;



	public QuadNodeData(QuadNode node){
		posX = new List<float> ();
		posY = new List<float> ();
		posZ = new List<float> ();
		rotX = new List<float> ();
		rotY = new List<float> ();
		rotZ = new List<float> ();
		prefabName = new List<string> ();
		heightMap = node.terrainData.GetHeights (0, 0, node.terrainData.heightmapWidth, node.terrainData.heightmapHeight);
		x = node.x;
		y = node.y;
		for(int i = 0; i< node.objects.Count; i++){
			GameObject obj = node.objects [i];
			posX.Add (obj.transform.position.x);
			posY.Add (obj.transform.position.y);
			posZ.Add (obj.transform.position.z);
			rotX.Add (obj.transform.rotation.x);
			rotY.Add (obj.transform.rotation.y);
			rotZ.Add (obj.transform.rotation.z);
			prefabName.Add (obj.name);

		}
	}

	public QuadNodeData(){
		
	}

}
