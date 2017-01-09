using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class TerrainSaver  {
	public static bool fileAccess = false;
	public static List<QuadNodeData> savedNodes = new List<QuadNodeData>();
	public static void Save(List<QuadNodeData> nodes){
		if (!fileAccess) {
			lock (savedNodes) {
				fileAccess = true;
				for (int i = 0; i < nodes.Count; i++) {
					QuadNodeData existing = null;

					for (int j = 0; j < savedNodes.Count; j++) {
						if (savedNodes [j].x == nodes [i].x && savedNodes [j].y == nodes [i].y) {
							existing = savedNodes [j];	

						}
					}

					if (existing == null) {
						savedNodes.Add (nodes [i]);
					} else {
						QuadNodeData newNode = nodes [i];
						savedNodes.Remove (existing);
						savedNodes.Add (newNode);
					}
				
				

				}
			}

			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Create (Application.persistentDataPath + "/savedTerrain.td");
			bf.Serialize (file, TerrainSaver.savedNodes);
			file.Close ();
			fileAccess = false;
		}
	}

	public static void Load(){
		lock (savedNodes) {
			if (!fileAccess) {
				fileAccess = true;
				if (File.Exists (Application.persistentDataPath + "/savedTerrain.td")) {
					BinaryFormatter bf = new BinaryFormatter ();
					FileStream file = File.Open (Application.persistentDataPath + "/savedTerrain.td", FileMode.Open);
					TerrainSaver.savedNodes = (List<QuadNodeData>)bf.Deserialize (file);
					file.Close ();
				}
				fileAccess = false;
			}
		}
	}

}
