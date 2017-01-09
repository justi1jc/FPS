/*
         Author: James Justice
         
         
         Implementing the NPCAI interface, HumanoidAI's key is to control
         the higher-level functions of a given NPC. 
*/﻿

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HumanoidAI : MonoBehaviour, NPCAI {
   public NPC npc;
   public bool running =false;
   public float navMargin;
   public bool paused = false;
   public void Begin(NPC npc){
      this.npc = npc;
      paused = false;
      StartCoroutine("Wander");
         
   }
   IEnumerator Wander(){
      //System.Random ran = new System.Random();
      
      yield return new WaitForSeconds(5f);
   }
   public void Pause(){
      paused = true;
   }
   public void Resume(){
      paused = false;
   }
   public ItemData GetData(){
      ItemData dat = new ItemData();
      return dat;
   }
   public void LoadData(ItemData dat){
   }
}
