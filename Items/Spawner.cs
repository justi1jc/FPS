/*
    Spawner is an Item that has a chance to spawn a prefab based on a random
    chance.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : Item{
  public string spawnee;  // Prefab that should be spawned.
  public int spawnChance; // Out of 100
  
  public override Data GetData(){
    Data dat = GetBaseData();
    dat.strings.Add(spawnee);
    dat.ints.Add(spawnChance);
    return dat;
  }
  
  public override void LoadData(Data dat){
    LoadBaseData(dat);
    spawnee = dat.strings[0];
    spawnChance = dat.ints[1];
    Spawn();
  }
  
  /* Spawns whatever it will spawn and then deletes itself.  */
  void Spawn(){
    System.Random r = new System.Random();
    int roll = r.Next(101);
    if(roll < spawnChance){
      Vector3 sPos = transform.position;
      Quaternion pRot = Quaternion.identity;
      GameObject pref = (GameObject)Resources.Load("Prefabs/" + spawnee, typeof(GameObject));
      if(pref != null){ GameObject.Instantiate(pref, sPos, pRot); }
    }
    Destroy(gameObject);
  }
  
}
