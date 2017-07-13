/*
       
     Data:
     This is a serializable multi-purpose record made for storing a variety of
     arbitrary data.
*/




using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Data{
  public bool readyToRead;
  public string prefabName;
  public string displayName;
  public float x;
  public float y;
  public float z;
  public float xr;
  public float yr;
  public float zr;
  public int itemType = -1;
  public int stack;
  public int stackSize;
  public List<int> ints;
  public List<string> strings;
  public List<float> floats;
  public List<bool> bools;
  public int baseValue;
  public InventoryRecord inventoryRecord;
  public Cell lastPos = null;
  public SpeechTree speechTree;
  public EquipSlot equipSlot;
  public Data(){
     ints = new List<int>();
     strings = new List<string>();
     floats = new List<float>();
     inventoryRecord = null;
     bools = new List<bool>();
     speechTree = null;
     equipSlot = null;
  }
   
  public Data(Data dat){
    readyToRead = dat.readyToRead;
    prefabName = dat.prefabName;
    displayName = dat.displayName;
    x = dat.x;
    y = dat.y;
    z = dat.z;
    xr = dat.xr;
    yr = dat.yr;
    zr = dat.zr;
    stack = dat.stack;
    stackSize = dat.stackSize;
    lastPos = dat.lastPos;
    baseValue = dat.baseValue;
    ints = dat.ints;
    strings = dat.strings;
    floats = dat.floats;
    inventoryRecord = dat.inventoryRecord;
    bools = dat.bools;
    speechTree = dat.speechTree;
    equipSlot = dat.equipSlot;
  }
   

}

