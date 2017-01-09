/*
      Author: James Justice
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechController : MonoBehaviour, FPSItem{
   public List<SpeechNode> nodes;
   public int currentNode;
   public bool played;
   public string[] visibleOptions;
   public string visibleMessage;
   public NPC npc;
   public void Start(){  
      visibleOptions = new string[4];
      for(int i =0; i<4; i++)
         visibleOptions[i] = nodes[currentNode].optionsText[i];
      visibleMessage = nodes[currentNode].messageText;
   }
   public IEnumerator PlayNode(int n){
      currentNode = n;
      
      visibleMessage = nodes[currentNode].messageText;
      if(currentNode < nodes.Count){
         AudioClip ac = Resources.Load(nodes[n].audioClipName) as AudioClip;
         if(ac != null){
            float vol = GameController.controller.masterVolume;
            AudioSource.PlayClipAtPoint(ac,transform.position,vol);
            yield return new WaitForSeconds(ac.length);
         }
         foreach(string action in nodes[currentNode].actions)
         HandleAction(action);
         played = true;
         for(int i =0; i<4; i++)
            visibleOptions[i] = nodes[currentNode].optionsText[i];
      }
      yield return new WaitForSeconds(0f);
   }
   public void HandleAction(string action){
      string[] args = action.Split(' ');
      switch(args[0]){
         case "goto":
            played = true;
            currentNode = System.Int32.Parse(args[1]);
            for(int i =0; i<4; i++)
               visibleOptions[i] = nodes[currentNode].optionsText[i];
            visibleMessage = nodes[currentNode].messageText;
            break;
         case "notify":
            GameController.controller.Notify(
               action.Substring(args[0].Length+1));
            break;
      }
   }
   public string[] GetOptions(){
      return nodes[currentNode].optionsText;  
   }
   public void ChooseOption(int option){
      for(int i = 0; i< 4; i++)
         visibleOptions[i] = "";
      played = false;
      StartCoroutine("PlayNode",nodes[currentNode].branches[option]);
   }
   public ItemData GetData(){
      ItemData dat = new ItemData();
      dat.displayName = "Speech Controller";
      dat.ints.Add(currentNode);
      dat.bools.Add(played);
      foreach(SpeechNode node in nodes)
         dat.itemData.Add(node.GetData());
      return dat;
   }
   public void LoadData(ItemData dat){
      npc.LoadData(dat);
   }
   public void LoadSpeechData(ItemData dat){
      currentNode = dat.ints[0];
      played = dat.bools[0];
      foreach(ItemData d in dat.itemData){
         GameObject prefab = 
            Resources.Load("Prefabs/Utility/SpeechNode") as GameObject;
         GameObject nodego = Instantiate(prefab) as GameObject;
         SpeechNode node = nodego.transform.GetComponent<SpeechNode>();
         if(node != null){
            node.LoadData(d);
            nodes.Add(node);
         }
      }
      
   }
   public void Use(int action){
      switch(action){
         case 0:
               StartCoroutine("PlayNode", currentNode);
            break;
         case 1:
            if(played && !visibleOptions[0].Equals(""))
               ChooseOption(0);
            break;
         case 2:
            if(played && !visibleOptions[1].Equals(""))
               ChooseOption(1);
            break;
         case 3:
            if(played && !visibleOptions[2].Equals(""))
                  ChooseOption(2);
            break;
         case 4:
            if(played && !visibleOptions[3].Equals(""))
                  ChooseOption(3);
            break;
      }
   
   }
	public void Use(NPC character){}
	public GameObject GetGameObject(){return null;}
	public GameObject GetPrefab(){return null;}
	public string GetPrefabName(){return null;}
	public void AssumeHeldPosition(NPC character){}
	public void Drop(){}
	public ItemData SaveData(){
	   ItemData dat = new ItemData();
	   dat = npc.GetData();
	   return dat;
	}
	public string GetDisplayName(){
	   if(npc != null)
	      return npc.DisplayName();
	   return "Null npc";
	}
	public string GetItemInfo(){return "Talk to " + npc.DisplayName();}
	public bool IsParent(){return true;}
	public bool IsChild(){return false;}
   public int GetInteractionMode(){return 0;}
   public int Stack(){return 0;}
   public int StackMax(){return 0;}
   public bool ConsumesAmmo(){return false;}
   public FPSItem AmmoType(){return null;}
   public void LoadAmmo(int ammount){}
   public int UnloadAmmo(){return 0;}
}
