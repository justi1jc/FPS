/*
*
*     Author: James Justice
*  
*     MeleeWeapon implements the FPSinterface in order to function as an item
*     used by other scripts.
*     
*     The following components are required for function:
*     rigidbody      
*     animator
*     
*
*
*
*/


﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeWeapon : MonoBehaviour, FPSItem 
{
   public string prefabName;
   public AudioClip swingSound;
   public AudioClip impactSound;
   public AudioClip drawSound;
   public string swingName;
   public string holdName;
          int swingHash;
          
   public int damage;
   public float knockback;
   public Vector3 heldPos;
   public Vector3 heldRot;
   public Vector3 childHeldRot;
   public Vector3 childHeldPos;
   public string displayName;
   public bool isParent;
   public bool isChild;
   public bool cooled = true;
   public float damageCoolDown;
   public bool damageEnabled;
   public float damageBegin;
   public float damageEnd;
   NPC npc;
   void Awake(){
	   swingHash= Animator.StringToHash(swingName);
   }
	void Start () {
	}
	public void AssumeHeldPosition(NPC npc){
	   this.npc = npc;
	   gameObject.layer = 8;
	   if(drawSound != null)
	      AudioSource.PlayClipAtPoint (drawSound, transform.position, 
	         GameController.controller.masterVolume * GameController.controller.effectsVolume);
		if(isChild){
		   transform.parent.localPosition = heldPos;
		   transform.parent.localRotation = 
		   Quaternion.Euler(heldRot.x, heldRot.y, heldRot.z);
		   transform.localPosition = childHeldPos;
		   transform.localRotation = Quaternion.Euler(childHeldRot.x,
		       childHeldRot.y, childHeldRot.z);
		}
		else if(isParent){
		   BoxCollider bc = gameObject.GetComponent<BoxCollider>();
		   if(bc != null)
		      bc.isTrigger = true;
         MeshCollider mc = transform.GetComponentInChildren<MeshCollider>();
         if(mc != null)
            mc.enabled = true;
		   transform.localPosition = heldPos;
		   transform.localRotation = Quaternion.Euler(heldRot.x,
		       heldRot.y, heldRot.z);;
		   foreach(Transform child in transform){
		      child.localPosition = childHeldPos;
		      child.localRotation = Quaternion.Euler(childHeldRot.x,
		          childHeldRot.y, childHeldRot.z);
		   }
		}
		else{
		   transform.localPosition = heldPos;
		   transform.localRotation = Quaternion.Euler(heldRot.x,
		       heldRot.y, heldRot.z);
      }
		Rigidbody rb = transform.GetComponent<Rigidbody>();
		if(isChild && rb == null){
		   rb = transform.parent.GetComponent<Rigidbody>();
		}
		if(isParent && rb == null){
		   foreach(Transform child in transform){
            rb = child.GetComponent<Rigidbody>();
            if(rb != null)
               break;
         }
		}
		if(rb != null){
		   rb.isKinematic = true;
		   rb.useGravity = false;
      }
	}
	public void Drop(){
	   gameObject.layer = 0;
	   if(isParent){
	      BoxCollider bc = gameObject.GetComponent<BoxCollider>();
		   if(bc != null)
		      bc.isTrigger = false;
		   MeshCollider mc = transform.GetComponentInChildren<MeshCollider>();
         if(mc != null)
            mc.enabled = false;
		   foreach(Transform child in transform){
		      MeshCollider col = child.GetComponent<MeshCollider>();
		      if(col != null)
		         col.enabled = false;
		   }

	   }
	   
	   Rigidbody rb = transform.GetComponent<Rigidbody>();
	   if(rb == null && transform.parent != null){
		   rb = transform.parent.GetComponent<Rigidbody>();
		}
		if(rb == null){
		   foreach(Transform child in transform){
            rb = child.GetComponent<Rigidbody>();
            if(rb != null)
               break;
         }
		}
		if(rb != null){
		   rb.isKinematic = false;
		   rb.useGravity = true;
      }
	}
	public void BladeImpact(Collider col){
	   if(cooled && damageEnabled){
	      
	      HitBox hb = col.gameObject.GetComponent<HitBox>();
	      if(hb && hb.npc.GetActiveItem() != (FPSItem)this){
	         hb.ReceiveDamage(damage, gameObject);
	         if(impactSound != null)
	            AudioSource.PlayClipAtPoint (impactSound, transform.position, 
	               GameController.controller.masterVolume * GameController.controller.effectsVolume);
	         cooled = false;
	         StartCoroutine("CoolDown");
	      }
	      
         Rigidbody enemyrb = col.gameObject.GetComponent<Rigidbody>();
         if(enemyrb != null)
            enemyrb.AddForce(knockback * transform.parent.forward);
      }
	}
	IEnumerator EnableDamage(){
	   damageEnabled = false;
	   yield return new WaitForSeconds(damageBegin);
	   damageEnabled = true;
	   yield return new WaitForSeconds(damageEnd);
	   damageEnabled = false;
	}
	public IEnumerator CoolDown(){
	   yield return new WaitForSeconds(damageCoolDown);
	   cooled = true;
	   
   }
	public void Use(int action){
	      if(action == 0)
	         npc.SetAnimationTrigger(swingHash);
	         StartCoroutine("EnableDamage");
	         if(swingSound != null)
	            AudioSource.PlayClipAtPoint (swingSound, transform.position, 
	               GameController.controller.masterVolume * GameController.controller.effectsVolume);       
	}
	public void Use(NPC npc){
	}
	public GameObject GetGameObject(){
	   return gameObject;
	}
	public GameObject GetPrefab(){
	   return Resources.Load(prefabName) as GameObject;
	}
	public void LoadData(ItemData data){
	   gameObject.transform.position = new Vector3(data.x, data.y, data.z);
	   transform.rotation = Quaternion.Euler(data.floats[0],data.floats[1],data.floats[2]);
	   prefabName = data.prefabName;
	   damage = data.ints[0];
	   displayName = data.displayName;
	}
	public ItemData SaveData(){
	   ItemData data  = new ItemData();
	   data.x = gameObject.transform.position.x;
	   data.y = gameObject.transform.position.y;
	   data.z = gameObject.transform.position.z;
      data.floats.Add(transform.eulerAngles.x);
      data.floats.Add(transform.eulerAngles.y);
      data.floats.Add(transform.eulerAngles.z);
	   data.ints.Add(damage);
	   data.prefabName = prefabName;
	   data.displayName = displayName;
	   data.stack = 1;
	   data.stackSize = 1;
	   return data;
	}
	public string GetDisplayName(){
	   return displayName;
   }
   public bool IsParent(){
      return isParent;
   }
   public bool IsChild(){
      return isChild;
   }
   public string GetItemInfo(){
      return displayName + " " + damage + " damage"; 
   }
   public string GetPrefabName(){
      return prefabName;
   }
   public int GetInteractionMode(){
      return 1;
   }
   public int Stack(){
      return 1;
   }
   public int StackMax(){
      return 1;
   }
   public bool ConsumesAmmo(){
      return false;
   }
   public FPSItem AmmoType(){
      return null;
   }
   public void LoadAmmo(int ammount){
   }
   public int UnloadAmmo(){
      return 0;
   }
   
}
