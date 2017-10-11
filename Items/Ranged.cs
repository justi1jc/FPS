/*
    Ranged is a Weapon that fires a projectile in order to inflict damage.
    sounds[0] = firing sound
    sounds[1] = reloading sound
    sounds[2] = dry fire
    sounds[3] = melee sound.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Ranged : Weapon{
  public int ammo;
  public int maxAmmo;
  public string projectile;
  public float reloadDelay;
  public float muzzleVelocity;
  public float impactForce;
  public string ammunition;
  public bool fullAuto = false;
  public bool hitScan = false; // True if this weapon doesn't use a projectile.
  Transform muzzlePoint; // Source of projectile
  public float recoil; // Muzzle climb of the weapon when fired.
  public bool meleeActive = false;
  
  public void Start(){
    InitMuzzlePoint();
    ready = true;
  }

  void InitMuzzlePoint(){
    foreach(Transform t in transform){
      if(t.gameObject.name == "MuzzlePoint"){ 
        muzzlePoint = t;
        return;
      }
    }
  }

  public override void Use(int action){
    if(action == 0 && ammo < 1){ Sound(2); }
    if(action == 0 || (fullAuto && action == 3)){ Fire(); }
    else if(action == 1){ ToggleAim(); }
    else if(action == 2 && ammo < maxAmmo){
      if(ready){ StartCoroutine(Reload()); }
    }
    else if(action == 5 && ready){
      StartCoroutine(Melee());
    }
  }
  
  public override string GetInfo(){
    return displayName + " " + ammo + "/" + maxAmmo;
  }

  /* Fires ranged weapon. */
  public virtual void Fire(){
    if(ammo < 1 || !ready){ return; }
    if(hitScan){ FireHitScan(); }
    else{ FireProjectile(); }
    Sound(0);
    ammo--;
    if(holder != null){ holder.Recoil(recoil); }
  }

  /* Creates and propels a projectile */
  public void FireProjectile(float spread = 0f){
    StartCoroutine(CoolDown(cooldown));
    Vector3 relPos = transform.forward;
    Vector3 spawnPos = muzzlePoint != null ? muzzlePoint.position : transform.position;
    Quaternion projRot = Quaternion.LookRotation(relPos);
    GameObject pref = (GameObject)Resources.Load(
      "Prefabs/"+projectile,
      typeof(GameObject)
    );
    GameObject proj = (GameObject)GameObject.Instantiate(
      pref,
      spawnPos,
      projRot
    );
    Collider col = proj.GetComponent<Collider>();
    col.isTrigger = true;
    Physics.IgnoreCollision(col, GetComponent<Collider>());
    Item item = proj.GetComponent<Item>();
    if(item is Projectile){
      Projectile p = (Projectile)item;
      p.weaponOfOrigin = gameObject;
      p.holder = holder;
      p.impactForce = impactForce;
      p.damageActive = true;
      p.damage = damage;
      p.Despawn();
      if(chargeable){
        p.damage = effectiveDamage;
        effectiveDamage = 0;
        charge = 0;
        Light light = item.gameObject.GetComponent<Light>();
        if(light){
          if(p.damage < 0){
            light.intensity = -(float)p.damage/10f;
            light.range = -p.damage;
          }
          else{
            light.intensity = ((float)p.damage)/10f;
            light.range = p.damage;
          }
        }
      }
    }
    float x = Random.Range(-spread, spread);
    float y = Random.Range(-spread, spread);
    float z = Random.Range(-spread, spread);
    Vector3 trajectory = new Vector3(x, y, z);
    proj.GetComponent<Rigidbody>().velocity = (relPos + trajectory) * muzzleVelocity;
  }

  /* Does a raycast and impacts a target. */
  void FireHitScan(){
    Sound(0);
    Vector3 dir = transform.rotation.eulerAngles;
    Vector3 pos = muzzlePoint != null ? muzzlePoint.position : transform.position;
    float dist = Mathf.Infinity;
    RaycastHit hit;
    if(Physics.Raycast(pos, dir, out hit, dist)){ Impact(hit.collider.gameObject); }
    StartCoroutine(CoolDown(cooldown));
  }

  /* Applies damage if the target has a HitBox and force if it has a rigidBody */
  void Impact(GameObject target){
    HitBox hb = target.GetComponent<HitBox>();
    if(hb != null){ hb.ReceiveDamage(damage, gameObject); }
    Rigidbody rb = target.GetComponent<Rigidbody>();
    if(rb != null){ rb.AddForce(impactForce * transform.forward); }
    print("Hit " + target.name);
  }
  
  /* Drop item from actor's hand, unzooming. */
  public override void Drop(){
    held = false;
    if(holder != null && holder.cam != null){
      holder.cam.fieldOfView = 60;
    }
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
  
  /* Reloading process. Note: A bool is used by the animation state machine due
    to the buggy behaviour of triggers applying to multiple layers. */
  public IEnumerator Reload(){
    ready = false;
    Sound(1);
    if(holder != null){ holder.SetAnimBool("reload", true); }
    yield return new WaitForSeconds(reloadDelay);
    if(holder != null){ holder.SetAnimBool("reload", false); }
    LoadAmmo();
    ready = true;
  }
  
  public IEnumerator Melee(){
    ready = false;
    if(holder != null){ holder.SetAnimBool("rangedMelee", true); }
    yield return new WaitForSeconds(0.5f);
    meleeActive = true;
    yield return new WaitForSeconds(0.5f);
    meleeActive = false;
    if(holder != null){ holder.SetAnimBool("rangedMelee", false); }
    ready = true;
  }
  
  void OnTriggerEnter(Collider col){ Strike(col); }
  void OnTriggerStay(Collider col){ Strike(col); }
  
  /* Exert force and damage onto target. Damage and knockback are hardcoded. */
  void Strike(Collider col){
    if(col.gameObject == null){ MonoBehaviour.print("Gameobject missing"); return; }
    if(meleeActive){
      float knockBack = 100f;
      int dmg = 35;
      if(holder != null){
        if(holder.GetRoot(col.gameObject.transform) == holder.transform.transform){
          return;
        }
      }
      HitBox hb = col.gameObject.GetComponent<HitBox>();
      if(hb){
        StartCoroutine(CoolDown(cooldown));
        hb.ReceiveDamage(dmg, gameObject);
        meleeActive = false;
        if(hb.body != null){
          Rigidbody hbrb = hb.body.gameObject.GetComponent<Rigidbody>();
          Vector3 forward = transform.position - hb.body.transform.position;
          forward = new Vector3(forward.x, 0f, forward.y);
          if(hbrb){ hbrb.AddForce(forward * knockBack); }
        }
      }
      Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
      if(rb){ 
        rb.AddForce(transform.forward * knockBack); 
        Sound(3);
      }
      Item item = col.gameObject.GetComponent<Item>();
      if(item != null && item.holder!= null && !item.ability){
        item.holder.arms.Drop(item);
      }
    }
  }
  
  /* Returns the current ammo of an item's data. */
  public static int Ammo(Data dat){
    if(dat == null || dat.ints.Count < 2){ return 0; }
    return dat.ints[1];
  }
  
  /* Returns the display name of the ammo this ranged weapon uses. */
  public static string AmmoName(Data dat){
    if(dat == null || dat.strings.Count < 1){ return ""; }
    return dat.strings[0];
  }
  
  public static void MaxAmmo(ref Data dat){
    dat.ints[1] = dat.ints[2];
  }
  
  /* Adds ammo to weapon externally */
  public virtual void LoadAmmo(){
    if(holder == null){ return; }
    int available = holder.RequestAmmo(ammunition, (maxAmmo - ammo));
    if(available > 0){ ammo = ammo + available; return; }
  }

  /* Aims weapon or returns it to the hip.*/
  public void ToggleAim(){
    holder.ToggleAim();
  }

  public override Data GetData(){
    Data dat = GetBaseData();
    AddWeaponData(dat);
    dat.ints.Add(ammo);
    dat.ints.Add(maxAmmo);
    dat.strings.Add(ammunition);
    dat.itemType = Item.RANGED;
    return dat;
  }

  public override void LoadData(Data dat){
    LoadBaseData(dat);
    ammo = dat.ints[2];
  }
  
}
