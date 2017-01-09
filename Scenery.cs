using UnityEngine;
using System.Collections;

public class Scenery : MonoBehaviour, FPSItem {

   public string prefabName;
   public string displayName;
   public bool terrain;
   
   public void Use(int action){}
	public void Use(NPC character){}
	public GameObject GetGameObject(){
	   return gameObject;
	}
	public GameObject GetPrefab(){
	   return (GameObject)Resources.Load(prefabName, typeof(GameObject));
	}
	
	public string GetPrefabName(){
	   return prefabName;
	}
	public void AssumeHeldPosition(NPC character){}
	public void Drop(){}
	public string GetDisplayName(){
	   return displayName;
	}
	public string GetItemInfo(){
	   return displayName;
	}
	public ItemData SaveData(){
      ItemData dat = new ItemData();
      dat.displayName = displayName;
      dat.prefabName = prefabName;
      dat.floats.Add(transform.position.x);
      dat.floats.Add(transform.position.y);
      dat.floats.Add(transform.position.z);
      dat.floats.Add(transform.eulerAngles.x);
      dat.floats.Add(transform.eulerAngles.y);
      dat.floats.Add(transform.eulerAngles.z);
      dat.bools.Add(terrain);
      return dat;
   }
   public void LoadData(ItemData dat){
      displayName = dat.displayName;
      transform.position = new Vector3(dat.floats[0],
            dat.floats[1],dat.floats[2]);
      transform.rotation = Quaternion.Euler(dat.floats[3],
            dat.floats[4],dat.floats[5]);
      terrain = dat.bools[0];
   }
	public bool IsParent(){
	   return true;
	}
	public bool IsChild(){
	   return false;
	}
   public int GetInteractionMode(){
      return 0;
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
   public void LoadAmmo(int ammount){}
   public int UnloadAmmo(){
      return 0;
   }	
}
