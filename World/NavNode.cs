/*
  A NavNode is the base unit of the PathFinder's graph.
*/
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NavNode{
  public Vector3 pos;
  public int id;
  public List<int> edges;
  public int parent = -1;
  public bool empty = true;
  
  public NavNode(){
    edges = new List<int>();
  }

}
