/*
    This is a utility class adding static methods pertaining to inventory
    loadouts.
*/

using System;
using System.Collections.Generic;
using UnityEngine;

public class LootTable{

  /* Factory that returns the data of an Item. */
  public static Data GetItem(string prefab, int quantity = 1){
    GameObject pref = (GameObject)Resources.Load("Prefabs/" + prefab, typeof(GameObject));
    if(pref == null){
      MonoBehaviour.print("Prefab null " + prefab);
      return null;
    }
    GameObject go = (GameObject)GameObject.Instantiate(
      pref,
      new Vector3(),
      Quaternion.identity
    );
    if(go == null){
      MonoBehaviour.print("Game object null " + prefab);
      return null;
    }
    Item item = go.GetComponent<Item>();
    if(item == null){ 
      MonoBehaviour.print("Item not found " + prefab);
      return null; 
    }
    Data dat = item.GetData();
    dat.stack = quantity;
    GameObject.Destroy(go);
    return dat;
  }
  
  /* Equips an actor with a specified starter kit. */
  public static void Kit(string name, Actor actor){
    
    switch(name.ToUpper()){
      case "GUNRUNNER":
        MonoBehaviour.print("Gunrunner selected");
        Data dat = GetItem("Weapons/Caster_Pistol");
        dat.ints[1] = 60; // Give full mag.
        actor.arms.Equip(new Data(dat), true);
        actor.arms.Equip(new Data(dat), false);
        dat = GetItem("Weapons/Ammo/Caster_Pistol_Magazine", 60);
        actor.inventory.Store(new Data(dat));
        actor.inventory.Store(new Data(dat));
        actor.inventory.Store(new Data(dat));
        actor.inventory.Store(new Data(dat));
        break;
    }
  }
  
  
}
