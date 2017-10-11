/*
    Item serves as the base class for specic types of items.
    Use codes
    0 Main Click
    1 Secondary Click
    2 Reload
    3 Charge
    4 Relase
*/


﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour{
  // Constants to reference class within Data
  public const int ITEM = 0;
  public const int FOOD = 1;
  public const int DECOR = 2;
  public const int CONTAINER = 3;
  public const int SPAWNER = 4;
  public const int WARPDOOR = 5;
  public const int WEAPON = 6;
  public const int MELEE = 7;
  public const int RANGED = 8;
  public const int PROJECTILE = 9;
  public const int EQUIPMENT = 10;
  
  
  // Input constants.
  public const int A_DOWN = 0;
  public const int B_DOWN = 1;
  public const int C_DOWN = 2;
  public const int D_DOWN = 3;
  public const int E_DOWN = 4;
  public const int F_DOWN = 5;
  public const int G_DOWN = 6;
  public const int A_UP = 7;
  public const int B_UP = 8;
  public const int C_UP = 9;
  public const int D_UP = 10;
  public const int E_UP = 11;
  public const int F_UP = 12;
  public const int G_UP = 13;

  // Base variables
  public string prefabName;
  public Vector3 heldPos;
  public Vector3 heldRot;
  public string displayName;
  public string itemDesc;
  public int stack;
  public int stackSize;
  public int baseValue;
  public AudioClip[] sounds;
  public int weight;
  public Actor holder;
  public bool ready = true;
  public bool oneHanded = true; // True if this item can be held in one hand.
  public bool held = false;
  public bool ability = false;
  
  
  // Empty base methods
  public virtual void Use(int action){}
  public virtual string GetInfo(){ return ""; }
  
  // Implemented base methods
  /* Bonds an item to an actor */
  public void Hold(Actor a){
    if(held){ return; }
    held = true;
    holder = a;
    
    Rigidbody rb = transform.GetComponent<Rigidbody>();
    if(rb != null){
      rb.isKinematic = true;
      rb.useGravity = false;
    }
    transform.localPosition = heldPos;
    Quaternion lr = Quaternion.Euler(heldRot.x, heldRot.y, heldRot.z);
    transform.localRotation = lr;
    Collider c = transform.GetComponent<Collider>();
    c.isTrigger = true;
  }
  
  /* Drop item from actor's hand. */
  public virtual void Drop(){
    print(displayName + " dropped");
    held = false;
    Rigidbody rb = transform.GetComponent<Rigidbody>();
    if(rb){
      rb.isKinematic = false;
      rb.useGravity = true;
      rb.constraints = RigidbodyConstraints.None;
    }
    Collider c = transform.GetComponent<Collider>();
    c.isTrigger = false;
    transform.parent = null;
    holder = null;
  }
  
  /* Play a sound, if possible. */
  public void Sound(int i){
    if(i < 0 || sounds == null || i >= sounds.Length || sounds[i] == null){ 
      //MonoBehaviour.print("Invalid sound:" + i); 
      return; 
    }
    float vol = PlayerPrefs.HasKey("masterVolume") ? PlayerPrefs.GetFloat("masterVolume") : 1f; 
    AudioSource.PlayClipAtPoint(sounds[i], transform.position, vol);
  }
  
  /* Pick up item. */
  public virtual void Interact(Actor a, int mode = -1, string message = ""){
    a.PickUp(this);
  }
  
  public virtual Data GetData(){ return GetBaseData(); }
  public virtual void LoadData(Data dat){ LoadBaseData(dat); }
  
  /* Returns a Data containing the base variables. */
  public Data GetBaseData(){
    Data dat = new Data();
    dat.x = transform.position.x;
    dat.y = transform.position.y;
    dat.z = transform.position.z;
    Vector3 rot = transform.rotation.eulerAngles;
    dat.xr = rot.x;
    dat.yr = rot.y;
    dat.zr = rot.z;
    dat.prefabName = prefabName;
    dat.displayName = displayName;
    dat.stack = stack;
    dat.stackSize = stackSize;
    dat.ints.Add(weight);
    dat.baseValue = baseValue;
    dat.itemType = Item.ITEM;
    dat.bools.Add(oneHanded);
    return dat;
  }
  
  /* Returns true if an item stored in a Data is one-handed. */
  public static bool OneHanded(Data dat){
    if(dat == null || dat.bools.Count < 1){ return true; }
    return dat.bools[0];
  }
  
  /* Fills the stack of a given dat. */
  public static void FullStack(ref Data dat){
    dat.stack = dat.stackSize;
  }
  
  public virtual void ReceiveDamage(int damage, GameObject weapon){}
  
  /* Loads the base variables from a Data */
  public void LoadBaseData(Data dat){
    transform.position = new Vector3(dat.x, dat.y, dat.z);
    transform.rotation = Quaternion.Euler(dat.xr, dat.yr, dat.zr);
    stack = dat.stack;
    weight = dat.ints[0];
  }
  
  /* Sets item to ready after cooldown duration. */
  public IEnumerator CoolDown(float duration){
    ready = false;
    yield return new WaitForSeconds(duration);
    ready = true;
  }
  
  /* Returns an Item from data */
  public static Item GetItem(Data dat){
    if(dat == null){ MonoBehaviour.print("Equipped null data"); return null; }
    GameObject prefab = Resources.Load("Prefabs/" + dat.prefabName) as GameObject;
    if(!prefab){ MonoBehaviour.print("Prefab null:" + dat.displayName); return null;}
    Vector3 position = new Vector3();
    GameObject itemGO = (GameObject)GameObject.Instantiate(
      prefab,
      position,
      Quaternion.identity
    );
    if(!itemGO){MonoBehaviour.print("GameObject null:" + dat.displayName); return null; }
    Item item = itemGO.GetComponent<Item>();
    item.LoadData(dat);
    return item;
  }
}
