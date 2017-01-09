/*
      Author: James Justice
      
      A kill trigger monitors an NPC and notifies a Quest
      upon the NPC's death.
      
         Initialize(string data)
      Initializes the DeathTrigger in lieu of a proper constructor.
      
      
*/
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour, QuestTrigger {
   NPC npc;
   string message;
	public void Initialize(string data){
	   message = data;
	   npc = GetComponentInChildren<NPC>() as NPC;
	   StartCoroutine("Listen");
	}
	IEnumerator Listen(){
	   while(npc.Health() > 0){
	      yield return new WaitForSeconds(1f);
	   }
	   yield return new WaitForSeconds(0f);
	   Trigger();
	}
	void Trigger(){
	   GameController.controller.Notify(message);
	}
}
