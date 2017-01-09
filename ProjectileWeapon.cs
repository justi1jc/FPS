/*
*
*        Author: James Justice
*
*
*
*
*
*
* Use()
* 0- left mouse
* 1- right mouse
* 2- R
*
*
*
*
*/



using UnityEngine;
using System.Collections;

public class ProjectileWeapon : MonoBehaviour, FPSItem {
	public GameObject projectile;
	public int capacity;
	public int capacityMax;
	public int reserve;
	public float velocity;
	public bool ready = true;
	public float fireDelay;
	public float reloadDelay;
	public float aimDelay;
   public GameObject muzzlePoint;
   public GameObject rearPoint;
	public Animator anim;
	public AudioClip fireSound;
	public AudioClip emptySound;
	public AudioClip reloadSound;
	public Vector3 heldPos;
	public Vector3 heldRot;
	public Vector3 childHeldRot;
	public string heldString;
	public string aimString;
	public string fireString;
	public string reloadString;
	int heldHash;
	int aimHash;
	int fireHash;
	int reloadHash;
	public string displayName;
	public string prefabName;
	public string ammoPrefabName;
	public bool isParent;
	public bool isChild;
	NPC npc;
	
	public void Awake(){
	   anim = transform.GetComponent<Animator>();
	   heldHash = Animator.StringToHash(heldString);
	   aimHash = Animator.StringToHash(aimString);
	   fireHash = Animator.StringToHash(fireString);
	   reloadHash = Animator.StringToHash(reloadString);
	}
	public void Use(int action){
		if(action == 0 && ready){
			if(capacity > 0){
				StartCoroutine("Fire");	
			}
			else{
				if(emptySound){
					AudioSource.PlayClipAtPoint (emptySound, transform.position,
					 GameController.controller.masterVolume * GameController.controller.effectsVolume);
				}
			}
			
		}
		if(action == 1 && ready){
			ToggleAim();
		}
		if(action == 2 && ready && reserve > 0){
			StartCoroutine("Reload");
		}
		
		
	}
	public void Use(NPC npc){
	}
	public void AssumeHeldPosition(NPC npc){
	   this.npc = npc;
	   if(isParent){
         BoxCollider bc = transform.GetComponent<BoxCollider>();
         
         if(bc != null)
            bc.enabled = false;
      }
	   npc.SetAnimationBool(heldHash, true);
		transform.localPosition = heldPos;
		transform.localRotation = Quaternion.Euler(heldRot.x, heldRot.y, heldRot.z);
		foreach(Transform child in transform){
		   child.localPosition = new Vector3();
		   child.localRotation = 
		      Quaternion.Euler(childHeldRot.x, childHeldRot.y, childHeldRot.z);
		}
		Rigidbody rb = transform.GetComponent<Rigidbody>();
		if(rb == null){
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
		   rb.isKinematic = true;
		   rb.useGravity = false;
      }
	}
	public void Drop(){
	   npc.SetAnimationBool(heldHash, false);
	   npc.SetAnimationBool(aimHash,false);
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
		   rb.isKinematic = false;
		   rb.useGravity = true;
      }
      if(isParent){
         BoxCollider bc = transform.GetComponent<BoxCollider>();
         if(bc != null)
            bc.enabled = true;
      }
	}
	public GameObject GetGameObject(){
		return gameObject;
	}
	public GameObject GetPrefab(){
		return Resources.Load(prefabName) as GameObject;
	}
	IEnumerator Fire(){
		ready = false;
		npc.SetAnimationTrigger(fireHash);
		npc.SetAnimationTrigger(fireHash);
		capacity--;
		Vector3 muzzlePosition;
		if(muzzlePoint != null)
		   muzzlePosition = muzzlePoint.transform.position;
		else
		   muzzlePosition = transform.position;
		Vector3 relPos = muzzlePoint.transform.position-rearPoint.transform.position;
		Quaternion projRot = Quaternion.LookRotation(relPos);
		GameObject active = (GameObject)GameObject.Instantiate(projectile, 
			muzzlePosition, projRot);
		Rigidbody activeRB = active.GetComponent<Rigidbody>();
		Projectile proj = active.transform.GetComponent<Projectile>();
		if(proj != null){
		   proj.Activate();
		}
		if(activeRB){
			activeRB.velocity = relPos * velocity;
			if(fireSound != null)
			   AudioSource.PlayClipAtPoint (fireSound, transform.position,
			      GameController.controller.masterVolume * GameController.controller.effectsVolume);
		}
		yield return new WaitForSeconds(fireDelay);
		ready = true;
	}
	IEnumerator Reload(){
		ready = false;
		if(reloadSound != null)
         AudioSource.PlayClipAtPoint (reloadSound, transform.position,
            GameController.controller.masterVolume * GameController.controller.effectsVolume);
         anim.SetTrigger(reloadHash);
		yield return new WaitForSeconds(reloadDelay);
		ready = true;
		if(reserve >= capacityMax){
		   reserve -= capacityMax;
		   capacity = capacityMax;
		}
		else if(reserve > 0){
		   capacity = reserve;
		   reserve = 0;
		}
		   
	}
	void ToggleAim(){
	   	if(npc.GetAnimationBool(aimHash)){
	   	   npc.SetAnimationBool(aimHash, false);
	   	}
	   	else{
	   	   npc.SetAnimationBool(aimHash, true);
	   	}
	}
	
	public ItemData SaveData(){
	   ItemData data  = new ItemData();
	   data.x = gameObject.transform.position.x;
	   data.y = gameObject.transform.position.y;
	   data.z = gameObject.transform.position.z;
      data.floats.Add(transform.eulerAngles.x);
      data.floats.Add(transform.eulerAngles.y);
      data.floats.Add(transform.eulerAngles.z);
	   data.prefabName = prefabName;
	   data.displayName = displayName;
	   data.stack = 1;
	   data.stackSize = 1;
	   return data;
	}
	public void LoadData(ItemData data){
	   transform.position = new Vector3(data.x, data.y, data.z);
	   transform.rotation = Quaternion.Euler(data.floats[0],data.floats[1],data.floats[2]);
	   prefabName = data.prefabName;
	   displayName = data.displayName;
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
      return displayName + " " + capacity + "/" + capacityMax + "/" + reserve;
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
      return true;
   }
   public FPSItem AmmoType(){
      GameObject ammoPre = Resources.Load(ammoPrefabName) as GameObject;
      GameObject ammoGo = (GameObject)GameObject.Instantiate(ammoPre, 
			transform.position, transform.rotation);
      FPSItem ammo = ammoGo.transform.GetComponent<FPSItem>();
      return ammo;
   }
   public void LoadAmmo(int ammount){
      reserve = ammount;
      StartCoroutine("Reload");
   }
   public int UnloadAmmo(){
      int total = reserve + capacity;
      reserve = 0;
      capacity = 0;
      
      return total;
   }
}
