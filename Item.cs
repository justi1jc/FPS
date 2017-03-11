﻿using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour{
/*
*   You'll quickly realize I'm using switch statements in place of polymorphism.
*   Unity3d doesn't support script polymorphism. Creating an Item interface
*   in the absence of abstract classes or inheritence was unmanageable, so this 
*   is my solution.
*
*   GameObject structure
*   
*   General item
*   Item|(ItemController)
*       |Model(rigidbody, collider, meshrenderer)
*   
*   Ranged Weapon
*   Item|(ItemController)
*       |Model|(rigidbody, collider, meshrenderer)
*             |MuzzlePoint//Forward position of barrel
*             |RearPoint  //Rear position of barrel
*
*   Variables that must be set for each instance
*   displayName
*   prefabName
*   itemType
*   stack
*   stackSize
*   For food: healing
*   For Melee weapon: damage, cooldown, damageStart, damageEnd
*   For ranged weapon: damage, maxAmmo, projectile, rangeType, cooldown, muzzlePoint, rearPoint
*     reloadDelay, muzzleVelocity, impactForce
*/


  // Item Types
  public const int SCENERY   = 0; // Can't be picked up.
  public const int MISC      = 1; // No inherent use
  public const int FOOD      = 2; // Restore health
  public const int MELEE     = 3; // Melee weapon
  public const int RANGED    = 4; // Ranged weapon
  public const int WARP      = 5; // Warps player to new area.
  public const int CONTAINER = 6; // Access contents, but not pick up.
  public const int PROJECTILE= 7; // Flies forward when used.
  
  // Ranged weapon types
  const int RIFLE  = 0; // Two-handed firearm.
  const int PISTOL = 1; // One-handed firearm.
  const int BOW    = 2; // Drawn back on click, then fired on release.
  const int THROWN = 4; // Requires no ammo, destroyed on use.
 
  
  // General item variables
  public string prefabName;
  public Vector3 heldPos;
  public Vector3 heldRot;
  public string displayName;
  public string itemDesc;
  public int stack;
  public int stackSize;
  public int itemType;
  public AudioClip[] sounds;
  public int weight;
  public Actor holder;
  
  // Food variables.
  public int healing;
  
  // Weapon variables
  public int damage;
  public float cooldown;
  public bool ready = true;
  
  // Melee variables
  public float damageStart;
  public float damageEnd;
  public bool damageActive;
  public string swingString;
  public int swingHash;
  public float knockBack;
  
  // Charging
  public bool chargeable = false;
  public int charge;
  public int chargeMax;
  public bool executeOnCharge = false;
  public int effectiveDamage = 0;
  
  // Ranged weapon variables 
  public int ammo;
  public int maxAmmo;
  public int activeAmmoType;
  public string[] ammoTypes;
  public string projectile;
  public string aimString;
  public string fireString;
  public int aimHash;
  public int rangedType;
  public int fireHash;
  public float reloadDelay;
  public GameObject muzzlePoint;
  public GameObject rearPoint;
  public float muzzleVelocity;
  public float impactForce;
  
  // WARP variables
  public string destName;
  public Vector3 destPos;
  public Vector3 destRot;
  
  // projectile variables
  GameObject weaponOfOrigin;
  
  // Container variables
  public Data[] contents;
  
  public void Start(){
    switch(itemType){
      case MELEE:
        swingHash = Animator.StringToHash(swingString);
        aimHash = Animator.StringToHash(aimString);
        chargeable = true;
        executeOnCharge = true;
        break;
      case RANGED:
        fireHash = Animator.StringToHash(fireString);
        break;
      default:
        break;
    }
  }
  
  public void Use(int action){
    // Main
    if(action == 0){
      switch(itemType){
        case FOOD:
          Consume();
          break;
        case MELEE:
          chargeable = true;
          break;
        case RANGED:
          Fire();
          break;
        case WARP:
          Warp();
          break;
        default:
          break;
      }
    }
    // Secondary
    else if (action == 1){
      switch(itemType){
        case RANGED:
          ToggleAim();
          break;
        default:
          break;
      }
    }
    // Tertiary
    else if (action == 2){
      switch(itemType){
        case RANGED:
          StartCoroutine(Reload());
          break;
        default:
          break;
      }
    }
    // Charge
    else if (action == 3){
      switch(itemType){
        case MELEE:
          if(chargeable){ ChargeSwing(); };
          break;
      }
    }
    // Charge release
    else if(action == 4){
      switch(itemType){
        case MELEE:
          if(chargeable && ready){
            charge = 0;
            chargeable = false;
            StartCoroutine(Swing()); 
          }
          break;
      }
    }
  }
  
  public string GetInfo(){
    string  info = displayName;
    switch(itemType){
      case FOOD:
        info += " +" + healing + " HP";
        break;
      case RANGED:
        info += " " + ammo + "/" + maxAmmo;
        break;
      case MELEE:
        info += " " + effectiveDamage + "DMG";
        break;
    }
    return info;
  }
  
  /* Response to interaction from non-holder Actor */
  public void Interact(Actor a, int mode = -1, string message = ""){
    // TODO: Account for other interaction modes.
    if(itemType != SCENERY && holder == null){ a.PickUp(this); };
  }
  
  /* Pick the item up. */
  public void Hold(Actor a){
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
  
  /* Drop the item. */
  public void Drop(){
    Rigidbody rb = transform.GetComponent<Rigidbody>();
    if(rb != null){
      rb.isKinematic = false;
      rb.useGravity = true;
    }
    Collider c = transform.GetComponent<Collider>();
    c.isTrigger = false;
    holder = null;
  }
  
  /* handle trigger collision */
  void OnTriggerEnter(Collider col){
    if(itemType == MELEE && damageActive){
      HitBox hb = col.gameObject.GetComponent<HitBox>();
      if(hb){
        StartCoroutine(CoolDown());
        hb.ReceiveDamage(effectiveDamage, gameObject);
        chargeable = false;
        effectiveDamage = 0;
      }
      Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
      if(rb){ rb.AddForce(transform.forward * knockBack); }
    }
    else if(
        itemType == PROJECTILE
        && damageActive
        && weaponOfOrigin != col.gameObject
      ){
      Destroy(this.gameObject);
    }
  }
  
  /* handle continuing trigger collision */
  void OnTriggerStay(Collider col){
    if(itemType == MELEE && damageActive){
      HitBox hb = col.gameObject.GetComponent<HitBox>();
      if(hb){
        StartCoroutine(CoolDown());
        hb.ReceiveDamage(effectiveDamage, gameObject);
        chargeable = false;
        effectiveDamage = 0;
      }
      Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
      if(rb){ rb.AddForce(transform.forward * knockBack); }
    }
  }
  
  /* Consume food. */
  public void Consume(){
    holder.ReceiveDamage(-healing, gameObject);
    holder.Drop();
    Destroy(this.gameObject);
  }
  
  public void ChargeSwing(){
    if(chargeable && charge < chargeMax){
      charge++;
      effectiveDamage = (damage * charge) / chargeMax;
    }
    if(chargeable && executeOnCharge && (charge >= chargeMax) && ready){
      charge = 0;
      chargeable = false;
      StartCoroutine(Swing());
    }
  }
  
  /* Swings melee weapon. */
  public IEnumerator Swing(){
    ready = false;
    damageActive = false;
    yield return new WaitForSeconds(damageStart);
    damageActive = true;
    transform.position += transform.forward;
    yield return new WaitForSeconds(damageEnd);
    damageActive = false;
    ready = true;
    transform.position -= transform.forward;
  }
  
  /* Sets weapon to ready after cooldown duration. */
  public IEnumerator CoolDown(){
    ready = false;
    damageActive = false;
    yield return new WaitForSeconds(cooldown);
    ready = true;
  } 
  
  /* Fires ranged weapon. */
  public void Fire(){
  if(!muzzlePoint || !rearPoint || ammo < 1){ return; }
    if(sounds.Length > 0){
      float vol = 1f;
      AudioSource.PlayClipAtPoint(
        sounds[0],
        transform.position,
        vol
      );
    }
    ready = false;
    if(holder && holder.anim){ holder.anim.SetTrigger(fireHash); }
    ammo--;
    Vector3 muzzlePos = muzzlePoint.transform.position;
    Vector3 rearPos = rearPoint.transform.position;
    Vector3 relPos = muzzlePos - rearPos;
    Quaternion projRot = Quaternion.LookRotation(relPos);
    GameObject pref = (GameObject)Resources.Load(
      projectile,
      typeof(GameObject));
    GameObject proj = (GameObject)GameObject.Instantiate(
      pref,
      muzzlePos,
      projRot);
    proj.GetComponent<Collider>().isTrigger = true;
    Item item = proj.GetComponent<Item>();
    if(item){
      item.weaponOfOrigin = gameObject;
      item.impactForce = impactForce;
      item.damageActive = true;
      item.damage = damage;
    }
    proj.GetComponent<Rigidbody>().velocity = relPos * muzzleVelocity;
    StartCoroutine(CoolDown());
  }
 
  
  /* Warps to destination. */
  public void Warp(){
    //TODO
  }
  
  /* Returns true if this weapon consumes ammo. */
  public bool ConsumesAmmo(){
    switch(itemType){
      case RANGED:
        return true;
    }
    return false;
  }
  
  /* Returns a list of valid ammo names */
  public string[] GetAmmoTypes(){
    return ammoTypes;
  } 
  
  /* Adds ammo to weapon externally */
  public void LoadAmmo(){
    /* TODO: Impliment multiple ammo types*/
    int available = holder.RequestAmmo(projectile, (maxAmmo - ammo));
    if(available > 0){ ammo = ammo + available; return; }
  }
  
  /* Reloads weapon from player inventory if possible */
  public IEnumerator Reload(){
    //TODO: use ammoTypes instead
    if(sounds.Length > 1){
      float vol = 0f;//GameController.controller.masterVolume *
                  //GameController.controller.effectsVolume;
      AudioSource.PlayClipAtPoint(
                                  sounds[1],
                                  transform.position, 
                                  vol );
    }
    yield return new WaitForSeconds(reloadDelay);
    LoadAmmo();
  }
  
  /* Aims weapon or returns it to the hip.*/
  public void ToggleAim(){
    if(!holder || !holder.anim){ return; }
    if(holder.anim.GetBool(aimHash)){
   	   holder.anim.SetBool(aimHash, false);
   	}
   	else{
   	   holder.anim.SetBool(aimHash, true);
   	}
  }
  
  /* Get the item's data. */
  public Data GetData(){
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
    switch(itemType){
      case FOOD:
        dat.ints.Add(healing);
        break;
      case MELEE:
        dat.ints.Add(damage);
        dat.floats.Add(cooldown);
        dat.floats.Add(damageStart);
        dat.floats.Add(damageEnd);
        break;
      case RANGED:
        dat.ints.Add(damage);
        dat.ints.Add(ammo);
        dat.ints.Add(rangedType);
        dat.floats.Add(cooldown);
        dat.strings.Add(projectile);
        break;
      case WARP:
        dat.strings.Add(destName);
        dat.floats.Add(destPos.x);
        dat.floats.Add(destPos.y);
        dat.floats.Add(destPos.z);
        dat.floats.Add(destRot.x);
        dat.floats.Add(destRot.y);
        dat.floats.Add(destRot.z);
        break;
      case CONTAINER:
        for(int j = 0; j < contents.Length; j++){
          dat.data.Add(contents[j]);
        }
        break;
      default:
        break;
    }
    return dat;
  }
  
  /* Load the item's data. */
  public void LoadData(Data dat){
    int i, s, f;
    i = s = f = 0;

    transform.position = new Vector3(dat.x, dat.y, dat.z);
    transform.rotation = Quaternion.Euler(dat.xr, dat.yr, dat.zr);
    stack = dat.stack;
    weight = dat.ints[i];
    i++;
    switch(itemType){
      case FOOD:
        healing = dat.ints[i];
        i++;
        break;
      case MELEE:
        damage = dat.ints[i];
        i++;
        cooldown = dat.floats[f];
        f++;
        damageStart = dat.floats[f];
        f++;
        damageEnd = dat.floats[f];
        f++;
        break;
      case RANGED:
        damage = dat.ints[i];
        i++;
        ammo = dat.ints[i];
        i++;
        rangedType = dat.ints[i];
        i++;
        projectile = dat.strings[s];
        s++;
        break;
      case WARP:
        destName = dat.strings[s];
        s++;
        destPos = new Vector3(
                              dat.floats[f],
                              dat.floats[f+1],
                              dat.floats[f+2]
                              );
        f+=3;
        destRot = new Vector3(
                              dat.floats[f],
                              dat.floats[f+1],
                              dat.floats[f+2]
                              );
        f+=3;
        break;
      case CONTAINER:
        contents = dat.data.ToArray();
        break;
      default:
        break;
    }
  }
  
}
