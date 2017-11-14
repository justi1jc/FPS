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
  private bool autoFireActive = false;
  private int meleeDamage;
  
  public void Start(){
    InitMuzzlePoint();
    ready = true;
  }
  
  /* Locates child muzzlepoint gameObject, if one exists. */
  void InitMuzzlePoint(){
    foreach(Transform t in transform){
      if(t.gameObject.name == "MuzzlePoint"){ 
        muzzlePoint = t;
        return;
      }
    }
  }
  
  public override void Use(Inputs action){
    if(action == Inputs.A_Down && ammo < 1){ Sound(2); }
    if(action == Inputs.A_Down && !fullAuto){ Fire(); }
    else if(action == Inputs.A_Down){ StartCoroutine(AutoFireRoutine()); }
    if(action == Inputs.A_Up && fullAuto){ autoFireActive = false; }
    else if(action == Inputs.B_Down){ ToggleAim(); }
    else if(action == Inputs.C_Down && ammo < maxAmmo && ready){
      StartCoroutine(Reload());
    }
    else if(action == Inputs.D_Down && ready){
      StartCoroutine(Melee());
    }
  }
  
  public override string GetInfo(){
    return displayName + " " + ammo + "/" + maxAmmo;
  }

  /* Fires this ranged weapon, consuming ammo. */
  public virtual void Fire(){
    if(ammo < 1 || !ready){ return; }
    Vector3 off = holder != null ? holder.stats.AccuracyPenalty() : new Vector3();
    if(hitScan){ FireHitScan(); }
    else{ FireProjectile(0f, off.x, off.y, off.z); }
    Sound(0);
    ammo--;
    if(holder != null){ holder.Recoil(recoil); }
  }

  /* Creates and propels a projectile */
  public void FireProjectile(
    float spread = 0f,
    float offX = 0.0f,
    float offY = 0.0f,
    float offZ = 0.0f
  ){
    StartCoroutine(CoolDown(cooldown));
    Vector3 relPos = transform.forward + new Vector3(offX, offY, offZ);
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
    if(hb != null){ hb.ReceiveDamage(new Damage(damage, gameObject)); }
    Rigidbody rb = target.GetComponent<Rigidbody>();
    if(rb != null){ rb.AddForce(impactForce * transform.forward); }
    print("Hit " + target.name);
  }
  
  /* Drop item from actor's hand, unzooming. */
  public override void Drop(){
    damageActive = false;
    if(holder != null){
      holder.SetAnimBool("reload", false);
      holder.SetAnimBool("rangedMelee", false);
    }
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
  
  /** Returns true if this weapon either has ammo in it, or its holder has
    * compatible ammo. 
    */
  public virtual bool HasAmmo(){
    if(ammo > 0 ){ return true; }
    if(holder == null){ return false; }
    if(holder.inventory.ItemByDisplayName(ammunition) == -1){ return false; }
    return true;
  }
  
  /* Reloading process. Note: A bool is used by the animation state machine due
    to the buggy behaviour of triggers applying to multiple layers. */
  public virtual IEnumerator Reload(){
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
    if(holder != null){
      holder.SetAnimBool("rangedMelee", true);
      meleeDamage = holder.stats.DrainCondition(StatHandler.Stats.Stamina, 25);
      meleeDamage += holder.stats.GetStat(StatHandler.Stats.Strength);
    }
    else{ meleeDamage = 25; }
    
    yield return new WaitForSeconds(0.5f);
    damageActive = true;
    yield return new WaitForSeconds(0.5f);
    damageActive = false;
    if(holder != null){ holder.SetAnimBool("rangedMelee", false); }
    ready = true;
  }
  
  void OnTriggerEnter(Collider col){ Strike(col, meleeDamage); }
  void OnTriggerStay(Collider col){ Strike(col, meleeDamage); }

  
  /* Returns the current ammo of an item's data. */
  public static int Ammo(Data dat){
    if(dat == null || dat.ints.Count < 2){ return 0; }
    return dat.ints[2];
  }
  
  /* Returns the display name of the ammo this ranged weapon uses. */
  public static string AmmoName(Data dat){
    if(dat == null || dat.strings.Count < 1){ return ""; }
    return dat.strings[0];
  }
  
  public static void MaxAmmo(ref Data dat){
    dat.ints[2] = dat.ints[3];
  }
  
  /* Adds ammo to weapon externally */
  public virtual void LoadAmmo(){
    if(holder == null){ return; }
    int available = holder.RequestAmmo(ammunition, (maxAmmo - ammo));
    if(available > 0){ ammo = ammo + available; return; }
  }
  
  /* Accounts for damage, capacity, rof, and reload speed */
  public override string GetUseInfo(int row = 0){
    string ret = "";
    switch(row){
      case 0: ret = "Damage: " + damage; break;
      case 1: ret = "Capacity: " + maxAmmo; break;
      case 2: ret = "Rate of fire: " + (60f/cooldown) + "/minute"; break;
      case 3: ret = "Reload speed: " + reloadDelay + " seconds"; break;
    }
    return ret;
  }

  /* Aims weapon or returns it to the hip.*/
  public void ToggleAim(){
    holder.ToggleAim();
  }
  
  /* Fires automatically until ammo runs out or autoFireActive is set to false. 
  */
  private IEnumerator AutoFireRoutine(){
    autoFireActive = true;
    while(autoFireActive){
      Fire();
      yield return new WaitForSeconds(cooldown);
      if(ammo < 1){ autoFireActive = false; }
    }
  }

  public override Data GetData(){
    Data dat = GetBaseData();
    AddWeaponData(dat);
    dat.ints.Add(ammo);
    dat.ints.Add(maxAmmo);
    dat.strings.Add(ammunition);
    dat.itemType = (int)Types.Ranged;
    return dat;
  }

  public override void LoadData(Data dat){
    LoadBaseData(dat);
    ammo = dat.ints[2];
  }
  
}
