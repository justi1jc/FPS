using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NavNode{
   public Vector3 position;
   public int id;
   public int island;
   public int distance;
   public int parent;
   public List<int> edges;
   public NavNode(){
      position = new Vector3();
      id = -1;
      edges = new List<int>();
   }
   public NavNode(ItemData dat){
      position = new Vector3();
      id = -1;
      edges = new List<int>();
      LoadData(dat);
   }
   public ItemData GetData(){
      ItemData dat = new ItemData();
      dat.floats.Add(position.x);
      dat.floats.Add(position.y);
      dat.floats.Add(position.z);
      dat.ints.Add(id);
      dat.ints.Add(island);
      dat.ints.Add(edges.Count);
      
      foreach(int edge in edges)
         dat.ints.Add(edge);
      return dat;
   }
   public void LoadData(ItemData dat){
      position = new Vector3(dat.floats[0], dat.floats[1], dat.floats[2]);
      id = dat.ints[0];
      island = dat.ints[1];
      for(int i = 0; i< dat.ints[2]; i++)
         edges.Add(dat.ints[i+3]);
      
   }
}
