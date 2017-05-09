/*
*     Author: James Justice
*       
*     Serializeable list of Data objects
*     TODO: Migrate inventory logic here from Actor for modularity
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Inventory{
  public List<Data> inv;
  
  public Inventory(){
    inv = new List<Data>();
  }
  
  public Inventory(List<Data> _inv){
    inv = _inv;
  }
}
