/*
    This is a utility class adding static methods pertaining to inventory
    loadouts.
*/

using System;
using System.Collections;
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
  public static void Kit(string name, ref Actor actor){
    Data dat = null;
    int choice = 0;
    switch(name.ToUpper()){
      case "RANDOM":
        choice = UnityEngine.Random.Range(0, 5);
        switch(choice){
          case 0: Kit("GUNRUNNER", ref actor); break;
          case 1: Kit("RIFLEMAN", ref actor); break;
          case 2: Kit("SHOTGUNNER", ref actor); break;
          case 3: Kit("SWORDSMAN", ref actor); break;
          case 4: Kit("ASSASSIN", ref actor); break;
        }
      break;
      case "GUNRUNNER": // Get a random pistol kit.
        choice = UnityEngine.Random.Range(0, 4);
        switch(choice){
          case 0: Kit("AUTO_PISTOL", ref actor); break;
          case 1: Kit("DUAL_AUTO_PISTOL", ref actor); break;
          case 2: Kit("CASTER_PISTOL", ref actor); break;
          case 3: Kit("DUAL_CASTER_PISTOL", ref actor); break;
        }
        break;
      case "AUTO_PISTOL":
        dat = GetItem("Weapons/Auto_Pistol");
        dat.ints[1] = 8; // Provide ammo for the pistol.
        actor.Equip(new Data(dat), true);
        dat = GetItem("Weapons/Ammo/Caster_Pistol_Magazine", 20);
        actor.StoreItem(new Data(dat));
        actor.StoreItem(new Data(dat));
        break;
      case "DUAL_AUTO_PISTOL":
        dat = GetItem("Weapons/Auto_Pistol");
        dat.ints[1] = 8; // Load ammo.
        actor.Equip(new Data(dat), true);
        actor.Equip(new Data(dat), true);
        break;
      case "CASTER_PISTOL":
        dat = GetItem("Weapons/Caster_Pistol");
        dat.ints[1] = 20;
        actor.Equip(new Data(dat), true);
        dat = GetItem("Weapons/Ammo/Caster_Pistol_Magazine", 20);
        actor.StoreItem(new Data(dat));
        break;
      case "DUAL_CASTER_PISTOL":
        dat = GetItem("Weapons/Caster_Pistol");
        dat.ints[1] = 20;
        actor.Equip(new Data(dat), true);
        actor.Equip(new Data(dat), true);
        break;
      case "RIFLEMAN": // Random rifle.
        choice = UnityEngine.Random.Range(0, 2);
        switch(choice){
          case 0: Kit("CASTER_RIFLE", ref actor); break;
          case 1: Kit("AUTO_RIFLE", ref actor); break; 
        }
        break;
      case "CASTER_RIFLE":
        dat = GetItem("Weapons/Caster_Rifle");
        dat.ints[1] = 60; // Give full mag.
        actor.Equip(new Data(dat), true);
        dat = GetItem("Weapons/Ammo/Caster_Rifle_magazine", 60);
        actor.StoreItem(new Data(dat));
        break;
      case "AUTO_RIFLE":
        dat = GetItem("Weapons/AutoRifle");
        dat.ints[1] = 15; // Give full ammo.
        actor.Equip(new Data(dat), true);
        dat = GetItem("Weapons/Ammo/Caster_Rifle_magazine", 60);
        actor.StoreItem(new Data(dat));
        break;
      case "SWORDSMAN":
        dat = GetItem("Weapons/Sword");
        actor.Equip(dat, true);
        break;
      case "SHOTGUNNER": // Random shotgun kit
        choice = UnityEngine.Random.Range(0, 2);
        switch(choice){
          case 0: Kit("PUMP_SHOTGUN", ref actor); break;
          case 1: Kit("SINGLE_SHOTGUN", ref actor); break;
        }
        break;
      case "PUMP_SHOTGUN":
        dat = GetItem("Weapons/Shotgun");
        dat.ints[1] = 8; // Give full tube.
        actor.Equip(dat, true);
        
        dat = GetItem("Weapons/Ammo/Shotgun_Cartridge", 16);
        actor.StoreItem(new Data(dat));
        break;
      case "SINGLE_SHOTGUN":
        dat = GetItem("Weapons/Single_Shotgun");
        dat.ints[1] = 1; // Give ammo.
        actor.Equip(dat, true);
        
        dat = GetItem("Weapons/Ammo/Shotgun_Cartridge", 16);
        actor.StoreItem(new Data(dat));
        break;
      case "ASSASSIN":
        dat = GetItem("Weapons/Knife");
        actor.Equip(new Data(dat), true);
        break;
      case "RED":
        dat = GetItem("Equipment/Shirt");
        
        //Set shirt color to red
        dat.floats[0] = 1f; // r
        dat.floats[1] = 0f; // g
        dat.floats[2] = 0f; // b
        dat.floats[3] = 1f; // a
        
        actor.Equip(new Data(dat));
        Kit("PANTS", ref actor);
        break;
      case "BLUE":
        dat = GetItem("Equipment/Shirt");
        
        //Set shirt color to blue
        dat.floats[0] = 0f; // r
        dat.floats[1] = 0f; // g
        dat.floats[2] = 1f; // b
        dat.floats[3] = 1f; // a
        
        actor.Equip(new Data(dat));
        Kit("PANTS", ref actor);
        break;
      case "PANTS":
        dat = GetItem("Equipment/Pants");
        actor.Equip(new Data(dat));
        break;
      default:
        MonoBehaviour.print("Invalid kit selection:" + name.ToUpper());
        break;
    }
  }
}
