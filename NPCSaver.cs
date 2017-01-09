using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSaver : MonoBehaviour, FPSItem {
   public NPC npc;
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
		return Resources.Load(GetPrefabName()) as GameObject; 
	}
	public string GetPrefabName(){
	   return "Null";
	}
	public void AssumeHeldPosition(NPC character){
	}
	public void Drop(){
	}
	public void LoadData(ItemData data){
	   if(npc != null)
	      npc.LoadData(data);
	}
	public ItemData SaveData(){
	   ItemData data  = new ItemData();
	   if(npc != null)
	      data = npc.GetData();
	   //print(npc.displayName);
	   return data;
	}
	public string GetDisplayName(){
	   return "Null";
	}
	public string GetItemInfo(){
	   return "NPCSaver";
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
      return 0;
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
