/*
    The PathFinder is tasked with creating a matrix of nodes with
    which paths can be found between points.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PathFinder{
  List<NavNode> nodes;
  MonoBehaviour mb;
  public float boxSize = 5f;
  public PathFinder(MonoBehaviour mb, Vector3 max, Vector3 min){
    this.mb = mb;
    nodes = new List<NavNode>();
    mb.StartCoroutine(Initialize(max, min));
  }

  /* Creates the vertices for this map. */
  public IEnumerator Initialize(Vector3 max, Vector3 min){
    int id = 0;
    for(float x = min.x; x < max.x; x = x+= boxSize){
      for(float y = min.y; y < max.y; y = y+= boxSize){
        for(float z = min.z; z < max.z; z = z+= boxSize){
          NavNode node = new NavNode();
          node.id = id;
          id++;
          node.pos = new Vector3(x, y, z);
          nodes.Add(node);
        }
      }
      yield return new WaitForSeconds(0.00001f);
    }
    MonoBehaviour.print("Created nodes.");
    yield return InitEmpty();
    MonoBehaviour.print("Checked for empty nodes.");
    yield return InitEdges();
    MonoBehaviour.print("Checked for edges.");
  }
  
  public IEnumerator InitEmpty(){
    foreach(NavNode node in nodes){
      node.empty = Empty(node.pos);
      yield return new WaitForSeconds(0.00001f);
    }
    yield return new WaitForSeconds(0f);
  }
  
  public IEnumerator InitEdges(){
    foreach(NavNode node in nodes){
      foreach(NavNode nodeB in nodes){
        float dist = Vector3.Distance(node.pos, nodeB.pos);
        if(node != nodeB && dist < boxSize + 1f){
          node.edges.Add(nodeB.id);
        }
        yield return new WaitForSeconds(0.00001f);
      }
    }
    yield return new WaitForSeconds(0f);
  }

  /* Perform a boxcast at this position. */
  public bool Empty(Vector3 center){
    float he = boxSize/2f;
    Vector3 halfExtents = new Vector3(he, he, he);
    Quaternion orientation = Quaternion.identity;
    Vector3 direction = new Vector3(0f, 0.01f, 0f);
    float maxDistance = boxSize;
    return !Physics.BoxCast(center, halfExtents, direction, orientation, maxDistance);
  }

  public void Clear(){
  }
  
  /* Create graph with path from one point to another. */
  public void CalculatePath(){
  }

  /* Returns the next vertex in a path. */
  public Vector3 NextVertex(){
    return new Vector3();
  }

}
