/*
    Projectile is an Item that inflicts damage and disappers upon collision.
    sound[0] = Impact sound.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : Item{
  public GameObject weaponOfOrigin;
  public float impactForce;
  public int damage;
  public bool damageActive;
  
  void OnTriggerEnter(Collider col){
    if(damageActive && weaponOfOrigin != col.gameObject){ Impact(col); }
  }
  
  void OnTriggerstay(Collider col){
    if(damageActive && weaponOfOrigin != col.gameObject){ Impact(col); }
  }
  
  public void FixedUpdate(){
    Rigidbody rb = GetComponent<Rigidbody>();
    Vector3 dir = rb.velocity.normalized;
    float dist = rb.velocity.magnitude;
    RaycastHit hit;
    Vector3 pos = transform.position;
    if(Physics.Raycast(pos, dir, out hit, dist)){
      Impact(hit.collider);
    }
  }
  
  /* Exert damage and impulse, then destroy self. */
  void Impact(Collider col){
    if(col == null){ return; }
    damageActive = false;
    HitBox hb = col.gameObject.GetComponent<HitBox>();
    if(hb){ hb.ReceiveDamage(damage, weaponOfOrigin); }
    Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
    if(rb) rb.AddForce(transform.forward * impactForce);
    Sound(0);
    Destroy(this.gameObject);
  }
  
  /* Triggers destruction of self. */
  public void Despawn(){
    StartCoroutine(DespawnTime());
  }
  
  /* Destroy self on timer. */
  public IEnumerator DespawnTime(){
    yield return new WaitForSeconds(20f);
    if(gameObject){ Destroy(gameObject); }
  }
}
