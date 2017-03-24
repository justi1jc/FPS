/*
*     Author: James Justice
*       
*     Data:
*     This is a serializable data object used to store instances
*     of other classes.
*/




using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Data
{
   public Data(){
      ints = new List<int>();
      strings = new List<string>();
      floats = new List<float>();
      data = new List<Data>();
      bools = new List<bool>();
   }
   public bool readyToRead;
   public string prefabName;
   public string displayName;
   public float x;
   public float y;
   public float z;
   public float xr;
   public float yr;
   public float zr;
   public int stack;
   public int stackSize;
   public List<int> ints;
   public List<string> strings;
   public List<float> floats;
   public List<bool> bools;
   public int baseValue;
   [System.NonSerialized]
   public List<Data> data;

}

