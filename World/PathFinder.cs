/*
    The PathFinder is tasked with creating a graph of nodes and
    calculating paths between nodes for use by AI.
    
    The pathFinder creates a layer of uninitialized NavNodes.
    Real nodes are added in the process of calculating paths.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PathFinder{
  int nextId;
  List<NavNode> nodes;
  MonoBehaviour mb;
  public float boxSize = 5f;
  public PathFinder(MonoBehaviour mb, Vector3 max, Vector3 min){
    this.mb = mb;
    nodes = new List<NavNode>();
    nextId = 0;
    mb.StartCoroutine(Initialize(max, min));
  }

  /* Creates the vertices for this map. */
  public IEnumerator Initialize(Vector3 max, Vector3 min){
    for(float x = min.x; x < max.x; x += boxSize){
      for(float z = min.y; z < max.z; z+= boxSize){
        NavNode node = new NavNode();
        node.id = nextId;
        nextId++;
        node.pos = new Vector3(x, 0f, z);
        node.initialized = false;
      }
    }
    
    yield return new WaitForSeconds(0f);
  }

  /* Select all grounded nodes in one verticle space. */
  public void InitColumn(Vector2 pos, float max, float min){
    int id = 0;
    // Note: Vector3 cannot be null, so below is a sentinal value that should never be valid.
    Vector3 sv = new Vector3(-999999f,-999999f,-999999f);
    Vector3 prevEmpty = sv;
    for(float y = min; y < max; y+=boxSize){
      Vector3 vec = new Vector3(pos.x, y, pos.y);
      if(Empty(vec)){ prevEmpty = vec; }
      else if(prevEmpty != sv){
        NavNode node = new NavNode();
        node.id = id;
        id++;
        node.pos = prevEmpty;
        nodes.Add(node);
      }
    }
  }

  /* Returns the node closest to the point, or -1. */
  public int NearestNode(Vector3 point){
    int least = -1;
    float leastDistance = 0f;
    foreach(NavNode node in nodes){
      float dist = Vector3.Distance(point, node.pos);
      if(least == -1 || leastDistance > dist){
        least = node.id;
        leastDistance = dist;
      }
    }
    return least;
  }

  /* Returns the desired node, or null. */
  public NavNode Node(int id){
    foreach(NavNode node in nodes){
      if(node.id == id){ return node; }
    }
    return null;
  }

  /* Finds adj based on distance. */
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

  /* Create graph with path from one point to another. */
  public void CalculatePath(){
  }

}
