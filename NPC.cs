/*
   Author: James Justice
   
   The purpose of the NPC interface is to provide HitBoxes, NPCAI, and 
   HUDControllers with a means to interact with all npcs, whether they
   use an FPSCharacterController or not.
*/


﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NPC {
   string DisplayName();
   ItemData GetData();
   void LoadData(ItemData dat);
   void MoveTo(Vector3 dest);
   void Aim(Vector3 eulers);
   Vector3 CurrentAim();
   ItemData[] GetInventory();
   void Equip(int item);
   void Drop();
   void Interact(int i);
   GameObject GetItemInReach();
   void SetItemInReach(GameObject item);
   void Use(int action);
   int Health();
   GameObject GetGameObject();
   void ReceiveDamage(int damage, GameObject weapon);
   void SetAnimationTrigger(int hash);
   void SetAnimationBool(int hash, bool val);
   bool GetAnimationBool(int hash);
   FPSItem GetActiveItem();
   bool IsFalling();
   Vector3 FallOrigin();
   void Land();
   SpeechController GetSpeechController();
   void TalkTo(NPC other);
   void TalkTo(int option);
   int GetFaction();
}
