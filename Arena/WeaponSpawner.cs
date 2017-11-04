/*
    WeaponSpawner spawns a single weapon (or any arbitrary item, really) and
    monitors it. When the item no longer exists, the weaponSpawner waits a
    specified amount of time and then spawns a new weapon.

    A weaponspawner will abstain from this behavior if it has an associated 
    Arena with spawnWeapons set to false.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSpawner : MonoBehaviour{
  public string weaponName;
  private GameObject weapon;
  public float spawnDelay; // Delay for replacing a weapon.
  public Arena arena;
  
  /* Starts the spawning of weapons. */
  public void Begin(){
    StartCoroutine(SpawnWeapons());
  }
  
  
  /* Spawns weapons as appropriate. */
  private IEnumerator SpawnWeapons(){
    yield return new WaitForSeconds(1f);
    if(arena == null || arena.spawnWeapons){ Spawn(); }
    while(arena == null || arena.spawnWeapons){
      if(weapon == null){
        yield return new WaitForSeconds(spawnDelay);
        Spawn();
      }
      yield return new WaitForSeconds(2f);
    }
  }
  
  /* Spawns this weapon, giving it full ammo if applicable. */
  private void Spawn(){
    Data dat = Item.GetItem(weaponName);
    if(dat != null){
      if(dat.itemType == Item.RANGED){ Ranged.MaxAmmo(ref dat); }
      Item item = Item.GetItem(dat);
      if(item != null){
        weapon = item.gameObject;
        weapon.transform.position = transform.position;
      }
    }
  }

}
