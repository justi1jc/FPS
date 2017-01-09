/*
*
*        Author: James Justice
*
*
*
*
*
*
*
*
*
*
*
*/
﻿using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
   public float flightTime;
   public float knockback;
   public int damage;
   public bool active = false;
   public void  Start(){
   }
   public void Activate(){
      active = true;
      StartCoroutine("Disappear");
   }
   public IEnumerator Disappear(){
      yield return new WaitForSeconds(flightTime);
      Destroy(gameObject);
   }
	void OnCollisionEnter(Collision col){
	   if(active){
		   HitBox hb = col.collider.gameObject.GetComponent<HitBox>();
		   if(hb){
			   hb.ReceiveDamage(damage, gameObject);
			   Destroy (gameObject);
		   }
		   Rigidbody rb = col.collider.gameObject.GetComponent<Rigidbody>();
		   if(rb != null){
		      rb.AddForce( knockback * transform.forward );
		   }
		}
	}
}
