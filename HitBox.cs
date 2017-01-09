/*
*
*        Author: James Justice
*
*        HitBox is meant to use in conjunction with an FPSCharacterController
*        in order to record damage done to a particular npcpart.
*
*        ReceiveDamage() Sends the damage directly to the script at present.
*
*
*
*
*
*
*/

﻿using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour {
   public bool foot;
   public float safeFallDistance;
	public NPC npc;
	public void ReceiveDamage(int damage, GameObject weapon){
	   if(npc != null){
	      npc.ReceiveDamage(damage, weapon);
	   }
	}
	void OnCollisionEnter(Collision col){  
      if(foot && npc.IsFalling()){
         npc.Land();
         float start = npc.FallOrigin().y;
         float end = npc.GetGameObject().transform.position.y;
         float distanceFallen =  start - end;
         if(start > end && distanceFallen > safeFallDistance){
            ReceiveDamage((int)(distanceFallen-safeFallDistance)*5, gameObject);
         }  
      }
	}
	public void OnCollisionStay(Collision col){
	   if(foot && npc.IsFalling()){
	      npc.Land();
	   }
	}
}
