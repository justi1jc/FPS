using UnityEngine;
using System.Collections;

public class InertItem : MonoBehaviour, FPSItem {
   public string prefabName;
   public Vector3 heldPosition;
   public string displayName;
   public string itemInfo;
   public int stack;
   public int stackSize;
   
   public void Use(int action){
      //empty
   }
	public void Use(NPC character){
	   //empty
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
	   Rigidbody rb = transform.GetComponent<Rigidbody>();
	   if(rb != null){
	      rb.isKinematic = true;
		   rb.useGravity = false;
      }
      Collider c = transform.GetComponent<Collider>();
      if(c != null)
         c.enabled = false;
      
	   transform.localPosition = heldPosition;
      
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
	public void LoadData(ItemData data){
	   transform.position = new Vector3(data.x, data.y, data.z);
	   transform.rotation = Quaternion.Euler(data.floats[0],data.floats[1],data.floats[2]);
	   prefabName = data.prefabName;
	   displayName = data.displayName;
	   stack = data.stack;
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
	   data.stack = stack;
	   data.stackSize = stackSize;
	   return data;
	}
	public string GetDisplayName(){
	   return displayName;
	}
	public string GetItemInfo(){
	   return itemInfo;
	}
	public bool IsParent(){
	   return true;
	}
	public bool IsChild(){
	   return false;
	}
	public int GetInteractionMode(){
	   return 2;
	}
   public int Stack(){
      return 1;
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
