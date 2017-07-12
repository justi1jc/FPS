/*
    Ranged is a Weapon that fires a projectile in order to inflict damage.
    sounds[0] = firing sound
    sounds[1] = reloading sound
    sounds[2] = dry fire
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
  
  public void Start(){
    ready = true;
  }
  
  public override void Use(int action){
    if(action == 0 && ammo < 1){ Sound(2); }
    if(action == 0 || (fullAuto && action == 3)){ Fire(); }
    else if(action == 1){ ToggleAim(); }
    else if(action == 2){ 
      if(ready){ StartCoroutine(Reload()); }
    }
  }
  
  public override string GetInfo(){
    return displayName + " " + ammo + "/" + maxAmmo;
  }
  
  /* Fires ranged weapon. */
  void Fire(){
    if(ammo < 1 || !ready){ return; }
    if(hitScan){ FireHitScan(); }
    else{ FireProjectile(); }
  }
  
  /* Creates and propels a projectile */
  void FireProjectile(){
    StartCoroutine(CoolDown(cooldown));
    ammo--;
    Sound(0);
    Vector3 relPos = transform.forward;
    Vector3 spawnPos = transform.position + transform.forward;
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
    proj.GetComponent<Collider>().isTrigger = true;
    Item item = proj.GetComponent<Item>();
    
    if(item is Projectile){
      Projectile p = (Projectile)item;
      p.weaponOfOrigin = gameObject;
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
    proj.GetComponent<Rigidbody>().velocity = relPos * muzzleVelocity;
  }
  
  /* Does a raycast and impacts a target. */
  void FireHitScan(){
    Sound(0);
    Vector3 dir = transform.rotation.eulerAngles;
    Vector3 pos = transform.position;
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
  
  /* Reloading process. */
  IEnumerator Reload(){
    Sound(1);
    yield return new WaitForSeconds(reloadDelay);
    LoadAmmo();
  }
  
  /* Adds ammo to weapon externally */
  public void LoadAmmo(){
    if(holder == null){ return; }
    int available = holder.RequestAmmo(ammunition, (maxAmmo - ammo));
    if(available > 0){ ammo = ammo + available; return; }
  }
  
  /* Aims weapon or returns it to the hip.*/
  public void ToggleAim(){
    /*
    if(!holder || !holder.anim){ return; }
    if(holder.anim.GetBool(aimHash)){
   	   holder.anim.SetBool(aimHash, false);
   	}
   	else{
   	   holder.anim.SetBool(aimHash, true);
   	}
   	*/
  }
  
  public override Data GetData(){
    Data dat = GetBaseData();
    dat.ints.Add(ammo);
    return dat;
  }
  
  public override void LoadData(Data dat){
    LoadBaseData(dat);
    ammo = dat.ints[1];
  }
  
}
