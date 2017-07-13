/*
    Item serves as the base class for specic types of items.
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
  bool held = false;
  
  
  // Empty base methods
  public virtual void Use(int action){}
  public virtual string GetInfo(){ return ""; }
  
  // Implemented base methods
  /* Bonds an item to  an item */
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
    transform.localRotation = Quaternion.Euler(
                                                heldRot.x,    
                                                heldRot.y, 
                                                heldRot.z
                                              );
    Collider c = transform.GetComponent<Collider>();
    c.isTrigger = true;
  }
  
  /* Drop item from actor's hand. */
  public void Drop(){
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
      MonoBehaviour.print("Invalid sound:" + i); 
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
    return dat;
  }
  
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
}
