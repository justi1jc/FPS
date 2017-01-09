using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour {

	public string displayName;
	public int state; //0 inactive, 1 started, 2 complete, 3 failed.
	public List<Objective> objectives=new List<Objective>();
	public List<int> startingObjectives=new List<int>();
	public void PlaceTriggers(){
	   foreach(Objective o in objectives)
	      if(o.state == 1)
	         o.PlaceTriggers();
	}
	public void InitializeQuest(){
	   foreach(Transform t in transform){
	      Objective o = t.GetComponent<Objective>();
	      if(o != null){
	         objectives.Add(o);
	         o.InitializeObjective(this);
	      }
	   }
	}
	public ItemData GetData(){
	   ItemData dat = new ItemData();
	   dat.strings.Add(displayName);
	   dat.ints.Add(state);
	   foreach(int i in startingObjectives)
	      dat.ints.Add(i);
	   foreach(Objective o in objectives){
	      dat.itemData.Add(o.GetData());
	   } 
	   return dat;
	}
	public void LoadData(ItemData dat){
	   displayName = dat.strings[0];
	   state = dat.ints[0];
	   foreach(ItemData odat in dat.itemData){
	      GameObject opref = Resources.Load("Prefabs/Utility/Objective") as GameObject;
	      GameObject ogo = Instantiate(opref, transform.position, transform.rotation) as GameObject;
	      Objective o = ogo.GetComponent<Objective>();
	      o.LoadData(odat);
	      objectives.Add(o);
	      o.quest = this;
	   }
	   for(int i = 1; i< dat.ints.Count; i++)
	      startingObjectives.Add(dat.ints[i]);
	      
   }
	public void QuestAction(string message){
	   string[] args = message.Split(' ');
	   int obj = -1;
	   switch(args[0]){
	      case"fail":
	         switch(args[1]){
	            case "quest":
	               state = 3;
	               GameController.controller.Notify("display sound Audio/SFX/fire3 " + displayName + " failed!");
	            break;
	            case "objective":
	               obj = System.Int32.Parse(args[2]);
	               objectives[obj].ObjectiveAction("fail");
	            break;
	      }
	      break;
	      case"complete":
	         switch(args[1]){
	            case "quest":
	               state =2;
	               GameController.controller.Notify("display sound Audio/SFX/menubing1 " + displayName + " complete!");
	            break;
	            case "objective":
	               obj = System.Int32.Parse(args[2]);
	               objectives[obj].ObjectiveAction("complete");
	            break;
	      }
	      break;
	      case "progress":
	      case "regress":
	      case "activate":
	      case "deactivate":
	         obj = System.Int32.Parse(args[1]);
	         objectives[obj].ObjectiveAction(args[0]);
	         break;
	   }
	}
}
