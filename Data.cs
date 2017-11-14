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
  public float x, y, z; // Position
  public float xr, yr, zr; // Rotation
  public int itemType = -1; // Corresponds to Item.Types
  public int stack, stackSize;
  public List<int> ints;
  public List<string> strings;
  public List<float> floats;
  public List<bool> bools;
  public int baseValue; // Monetary value of item.
  public InventoryRecord inventoryRecord;
  
  // Variables for Actor
  public SpeechTree speechTree;
  public EquipSlot equipSlot;
  public PaperDoll doll;
  
  /**
   * Default constructor.
   */
  public Data(){
     ints = new List<int>();
     strings = new List<string>();
     floats = new List<float>();
     inventoryRecord = null;
     bools = new List<bool>();
     speechTree = null;
     equipSlot = null;
     doll = null;
  }
   
  /**
    * Constructor for cloning a Data.
    * @param {Data} dat - The data to clone.
    */
  public Data(Data dat){
    if(dat == null){ return; }
    readyToRead = dat.readyToRead;
    prefabName = dat.prefabName;
    displayName = dat.displayName;
    x = dat.x;
    y = dat.y;
    z = dat.z;
    xr = dat.xr;
    yr = dat.yr;
    zr = dat.zr;
    itemType = dat.itemType;
    stack = dat.stack;
    stackSize = dat.stackSize;
    baseValue = dat.baseValue;
    ints = dat.ints;
    strings = dat.strings;
    floats = dat.floats;
    inventoryRecord = dat.inventoryRecord;
    bools = dat.bools;
    speechTree = dat.speechTree;
    equipSlot = dat.equipSlot;
    doll = dat.doll;
  }

}

