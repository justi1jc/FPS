/*
*
*        Author: James Justice
*
*
*     FPSitem is an interface for items in this unity project. Because
*     Unity bogs down proper inheretence using the MonoBehavior class, 
*     this interface aims to provide other scrips with a means to interact
*     with the scripts of all items in the same fashion.
* 
*     Use(int action) Provides an item's script with a single argument.
*     By default 1 and 2 will function as primary and secondary uses, 
*     respectively.
*
*     GetGameObject() returns an instance of the item's GameObject
*     
*     LoadData(ItemData data) provides the item with data to load. 
*     
*     ItemData SaveData() returns all the item's data saved in an ItemData object.
*
*     int GetInteractionMode() returns a value describing how the item is used
*     0 = environment: Can't be picked up, only used remotely(ie a lightswitch)
*     Use(int action)
*     1 = general use: Can be picked up and used with Use(int action)
*     2 = character specific: Can be picked up and used on a particular character
*     using Use(NPC character)
*/

﻿using UnityEngine;
using System.Collections;

public interface FPSItem{
	void Use(int action);
	void Use(NPC character);
	GameObject GetGameObject();
	GameObject GetPrefab();
	string GetPrefabName();
	void AssumeHeldPosition(NPC character);
	void Drop();
	void LoadData(ItemData data);
	ItemData SaveData();
	string GetDisplayName();
	string GetItemInfo();
	bool IsParent();
	bool IsChild();
   int GetInteractionMode();
   int Stack();
   int StackMax();
   bool ConsumesAmmo();
   FPSItem AmmoType();
   void LoadAmmo(int ammount);
   int UnloadAmmo();
}
