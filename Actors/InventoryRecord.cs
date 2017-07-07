/*
    The inventory record exists to serialize the contents of an inventory
    so that it can be stored in a Data.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InventoryRecord{
  public List<Data> inv;
  public InventoryRecord(List<Data> inv){
    this.inv = new List<Data>(inv);
  }

}
