using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, FPSItem {
   public Vector3 destPos;
   public Vector3 destRot;
   public string destination;
   public int destX;
   public int destY;
   public string displayName;
   public string prefabName;
   public bool warpDoor;
   public bool open;
	public void Use(int action){
	   if(warpDoor){
	      GameController.controller.WarpTo(destination, destPos, destRot, destX, destY);
	   }
	   else{
	      if(open){
	         print("closing");
	      }
	      else{
	         print("Opening");
	      }
	   }
	
	}
	public void Use(NPC player){}
	public GameObject GetGameObject(){return gameObject;}
	public GameObject GetPrefab(){return null;}
	public string GetPrefabName(){return "";}
	public void AssumeHeldPosition(NPC character){}
	public void Drop(){}
	public void LoadData(ItemData dat){
	   displayName = dat.displayName;
      transform.position = new Vector3(dat.floats[0],
            dat.floats[1],dat.floats[2]);
      transform.rotation = Quaternion.Euler(dat.floats[3],
            dat.floats[4],dat.floats[5]);
      destination = dat.strings[0];
      destPos = new Vector3(dat.floats[6],dat.floats[7],dat.floats[8]);
      destRot = new Vector3(dat.floats[9],dat.floats[10], dat.floats[11]);
      destX = dat.ints[0];
      destY = dat.ints[1];
	}
	public ItemData SaveData(){
	   ItemData dat = new ItemData();
	   dat.prefabName = prefabName;
	   dat.displayName = displayName;
	   dat.strings.Add(destination);
	   dat.floats.Add(transform.position.x);
      dat.floats.Add(transform.position.y);
      dat.floats.Add(transform.position.z);
      dat.floats.Add(transform.eulerAngles.x);
      dat.floats.Add(transform.eulerAngles.y);
      dat.floats.Add(transform.eulerAngles.z);
      dat.floats.Add(destPos.x);
      dat.floats.Add(destPos.y);
      dat.floats.Add(destPos.z);
      dat.floats.Add(destRot.x);
      dat.floats.Add(destRot.y);
      dat.floats.Add(destRot.z);
      dat.ints.Add(destX);
      dat.ints.Add(destY);
	   return dat;
	}
	public string GetDisplayName(){return displayName;}
	public string GetItemInfo(){return displayName;}
	public bool IsParent(){return true;}
	public bool IsChild(){return false;}
   public int GetInteractionMode(){return 0;}
   public int Stack(){ return 0;}
   public int StackMax(){return 0;}
   public bool ConsumesAmmo(){return false;}
   public FPSItem AmmoType(){return null;}
   public void LoadAmmo(int ammount){}
   public int UnloadAmmo(){return 0;}
   
	
}
