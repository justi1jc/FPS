/*
      Author: James Justice
      The purpose of sentinal AI is to guard a specific area from potential
      threats.
*/
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinalAI : MonoBehaviour, NPCAI {
   public Vector3 outerMin;
   public Vector3 outerMax;
   public Vector3 innerMin;
   public Vector3 innerMax;
   public NPC npc;
	public void Begin(NPC npc){
	}
   public void Pause(){
   }
   public void Resume(){
   }
   public ItemData GetData(){
      return new ItemData();
   }
   public void LoadData(ItemData dat){
      
   }
}
