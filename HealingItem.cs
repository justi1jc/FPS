using UnityEngine;
using System.Collections;

public class HealingItem : MonoBehaviour, FPSItem {
   public string prefabName;
   public Vector3 heldPosition;
   public string displayName;
   public int healing;
   public int stack;
   public int stackSize;
   public AudioClip consumeSound;
   public NPC character;
   
   public void Use(NPC character){
      //empty
   }
	public void Use(int action){
	   int diff = healing + character.Health() -100;
	   if(diff > 0)
	      character.ReceiveDamage(-(healing-diff) ,gameObject);
	   else
	      character.ReceiveDamage(-healing, gameObject);
	   if(consumeSound != null)
	      AudioSource.PlayClipAtPoint (consumeSound, transform.position,
			      GameController.controller.masterVolume *
			      GameController.controller.effectsVolume);
	   stack--;
	   if(stack < 1){
	      character.Drop();
	      Destroy(gameObject);
	   }
	   
	}
	public GameObject GetGameObject(){
	   return gameObject;
	}
	public GameObject GetPrefab(){
		return Resources.Load(prefabName) as GameObject; 
	}
	
	public string GetPrefabName(){
	   return prefabName;
	}
	public void AssumeHeldPosition(NPC character){
	   this.character = character;
	   Rigidbody rb = transform.GetComponent<Rigidbody>();
	   if(rb != null){
	      rb.isKinematic = true;
		   rb.useGravity = false;
      }
	   transform.localPosition = heldPosition;
      Collider c = transform.GetComponent<Collider>();
         if(c != null)
            c.enabled = false;
      
	}
	public void Drop(){
	   Rigidbody rb = transform.GetComponent<Rigidbody>();
	   if(rb != null){
	      rb.isKinematic = false;
		   rb.useGravity = true;
      }
      Collider c = transform.GetComponent<Collider>();
         if(c != null)
            c.enabled = true;
      
	}
	public ItemData SaveData(){
	   ItemData data  = new ItemData();
	   if(data != null){
	      data.x = gameObject.transform.position.x;
	      data.y = gameObject.transform.position.y;
	      data.z = gameObject.transform.position.z;
	      data.floats.Add(transform.eulerAngles.x);
         data.floats.Add(transform.eulerAngles.y);
         data.floats.Add(transform.eulerAngles.z);
	      data.ints.Add(healing);
	      data.prefabName = prefabName;
	      data.displayName = displayName;
	      data.stack = stack;
	      data.stackSize = stackSize;
	   }
	   else if(GameController.controller.debug == 1){
	      print("item data null");
	   }
	   return data;
	}
	public void LoadData(ItemData data){
	   transform.position = new Vector3(data.x, data.y, data.z);
	   transform.rotation = Quaternion.Euler(data.floats[0],data.floats[1],data.floats[2]);
	   prefabName = data.prefabName;
	   displayName = data.displayName;
	   healing = data.ints[0];
	   stack = data.stack;
   }
   public string GetDisplayName(){
      return displayName;
   }
	public string GetItemInfo(){
	   return displayName + " +" + healing + "(" + stack + ")";
	}
	public bool IsParent(){
	   return true;
	}
	public bool IsChild(){
	   return false;
	}
	public int GetInteractionMode(){
	   return 1;
	}
   public int Stack(){
      return stack;
   }
   public int StackMax(){
      return stackSize;
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
