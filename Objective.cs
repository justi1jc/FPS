using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour {
	public int index;
	public int state;//0 inactive, 1 active, 2 complete, 3 failed
   public int progress;
   public int progressMax;
   public int progressInit;
   public string progressMessage;
   public string[] completionActions;
   public string[] failureActions;
   public Quest quest;
   public string[] targets;
   public string[] triggers;
   public string[] triggerArgs;
   
   public void InitializeObjective(Quest q){
      progress = progressInit;
      state = 0;
      quest = q;
   }
   public ItemData GetData(){
      ItemData dat = new ItemData();
      dat.ints.Add(index);
      dat.ints.Add(state);
      dat.ints.Add(progress);
      dat.ints.Add(progressMax);
      dat.ints.Add(progressInit);
      dat.strings.Add(progressMessage);
      
      dat.ints.Add(completionActions.Length);
      foreach(string s in completionActions)
         dat.strings.Add(s);
      dat.ints.Add(failureActions.Length);
      foreach(string s in failureActions)
         dat.strings.Add(s);
      dat.ints.Add(targets.Length);
      foreach(string s in targets)
         dat.strings.Add(s);
      dat.ints.Add(triggers.Length);
      foreach(string s in triggers)
         dat.strings.Add(s);
      dat.ints.Add(triggerArgs.Length);
      foreach(string s in triggerArgs)
         dat.strings.Add(s);
      return dat;
   }
   public void LoadData(ItemData dat){
      index = dat.ints[0];
      state = dat.ints[1];
      progress = dat.ints[2];
      progressMax = dat.ints[3];
      progressInit = dat.ints[4];
      progressMessage = dat.strings[0];
      List<string> strings = new List<string>();
      int j = 1;
      for(int i = j; i< j+dat.ints[5]; i++)
         strings.Add(dat.strings[i]);
      completionActions = strings.ToArray();
      strings.Clear();
      j += dat.ints[5];
      for(int i = j; i< j+dat.ints[6]; i++)
         strings.Add(dat.strings[i]);
      failureActions = strings.ToArray();
      strings.Clear();
      j += dat.ints[6];
      for(int i = j; i< j+dat.ints[7]; i++)
         strings.Add(dat.strings[i]);
      targets = strings.ToArray();
      strings.Clear();
      j += dat.ints[7];
      for(int i = j; i< j+dat.ints[8]; i++)
         strings.Add(dat.strings[i]);
      triggers = strings.ToArray();
      strings.Clear();
      j += dat.ints[8];
      for(int i = j; i< j+dat.ints[9]; i++)
         strings.Add(dat.strings[i]);
      triggerArgs = strings.ToArray();
      
   }
   public void PlaceTriggers(){
      for(int i =0; i<targets.Length && i<triggers.Length; i++){
         GameObject go = GameObject.Find(targets[i]);
         if(go != null){
            go.AddComponent(System.Type.GetType(triggers[i]));
            QuestTrigger trigger = go.GetComponent<QuestTrigger>() as QuestTrigger;
            if(trigger !=null)
               trigger.Initialize(triggerArgs[i]);
         }
      }
   }
	public void ObjectiveAction(string update){
	   switch(update){
	      case "complete":
	         state = 2;
	         GameController.controller.Notify("display " + progressMessage + " complete!");
	         if(quest != null)
	            foreach(string action in completionActions)
                  quest.QuestAction(action);
                  
	         break;
	      case "fail":
	         state = 3;
	         GameController.controller.Notify("display " + progressMessage + " failed!");
	         if(quest != null)
	            foreach(string action in failureActions)
                  quest.QuestAction(action);
	         break;
	      case "progress":
	         progress++;
	         if(progress == progressMax){
	            ObjectiveAction("complete");
	         }
	         break;
	      case "regress":
	         progress--;
	         if(progress <0)
	            ObjectiveAction("fail");
	         break;
	      case "activate":
	         state = 1;
	         break;
	      case "deactivate":
	         state = 0;
	         break;
	   }
	}
	public string DisplayProgress(){
	   string message = progressMessage;
	   if(progressMax > 1)
	      message += " :" + progress + "/" + progressMax;
	   if(progressMax == 0)
	      message+= " :" + progress;
	   return message;
	}
}
