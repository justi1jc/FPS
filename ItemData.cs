/*
*     Author: James Justice
*
*     ItemData
*     This C# class acts as a collection of variables used by Monobehaviors
*     to save data in files. The X, Y, and Z variables are used in conjunction
*     with the prefabname in order to instantiate the desired item's gameobject.
*     
*/




using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class ItemData
{
   public ItemData(){
      ints = new List<int>();
      strings = new List<string>();
      floats = new List<float>();
      itemData = new List<ItemData>();
      bools = new List<bool>();
   }
   public bool readyToRead;
   public string prefabName;
   public string displayName;
   public int stack;
   public int stackSize;
   public float x;
   public float y;
   public float z;
   public List<int> ints;
   public List<string> strings;
   public List<float> floats;
   public List<ItemData> itemData;
   public List<bool> bools;

}

