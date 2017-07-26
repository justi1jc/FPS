/*
    The HotBar tracks the location of items as they move between
    the Inventory and the EquipSlot for the purpose of cycling 
    through available weapons consistently.
    Weapons are automatically added to empty slots in the hotbar 
    and slots are emptied when the selected item is dropped.
    
    The EquipSlot and Inventory are responsible for updating
    the HotBar as they gain and lose items.
    
    The eventual use of the hotbar will be to represent items
    that are holstered, slung, or otherwise quick-access. 
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class HotBar{
  public Actor actor;
  public int slots = 3; // Number of slots in HotBar
  public int selectedSlot;
  /*
    -6 transitioning from left hand
    -5 transitioning from right hand
    -4 transitioning from inventory 
    -3 is Empty
    -2 is left hand
    -1 is right hand
     0 and above is an inventory slot.
  */
  public List<int> bar;

  public HotBar(Actor a){
    actor = a;
    bar = new List<int>();
    for(int i = 0; i < slots; i++){ bar.Add(-3); }
  }

  public Data Peek(int slot){
    if(slot < -3 || slot >= slots){ return null; }
    if(bar[slot] == -3){ return null; }
    if(bar[slot] == -2){
      if(actor != null && actor.arms.handItem != null){
        return actor.arms.handItem.GetData();
      }
      return null;
    }
    if(bar[slot] == -1){
      if(actor != null && actor.arms.offHandItem != null){
        return actor.arms.offHandItem.GetData();
      }
      return null;
    }
    if(actor != null && bar[slot] > -1){
      return actor.inventory.Peek(bar[slot]);
    }
    return null;
  } 

  /* Equips the item in the next slot. */
  public void NextSlot(){
    selectedSlot++;
    if(selectedSlot >= slots){ selectedSlot = 0; }
    Equip(selectedSlot);
  }

  /* Equips the item in the previous slot. */
  public void PreviousSlot(){
    selectedSlot--;
    if(selectedSlot < 0){ selectedSlot = slots-1; }
    Equip(selectedSlot);
  }
  
  /* Equips a specific item to the player's EquipSlot */
  public void Equip(int slot){
    MonoBehaviour.print("Slot " + slot);
    Data item = GetItem(slot);
    if(item == null){ 
      MonoBehaviour.print("Slot " + slot + " null");
      return; 
    }
    actor.arms.Store(true);
    actor.arms.Store(false);
    actor.arms.Equip(item);
  }
  
  /* Returns the item in a slot, or null. */
  private Data GetItem(int slot){
    if(actor == null){ return null; }
    int loc = bar[slot];
    switch(loc){
      case -3: return null; break;
      case -2: return actor.arms.Remove(true); break;
      case -1: return actor.arms.Remove(false); break;
    }
    if(loc > -1){ return actor.inventory.Retrieve(loc); }
    return null;
  }

  /* Updates items as they move between inventory and arms. */
  public void Update(int before, int after, Data dat = null){
    //MonoBehaviour.print("Hotbar Updated from " + before + " to " + after);
    int slot = -1;
    for(int i = 0; i < slots; i++){
      if(before == bar[i]){ slot = i; break; }
    }
    if(slot == -1){ return; }
    if(bar[slot] == -3 && dat != null && dat.itemType != Item.WEAPON 
      && dat.itemType != Item.MELEE 
      && dat.itemType != Item.RANGED
    ){
      MonoBehaviour.print(dat.displayName + dat.itemType + " Not a weapon."); 
      return; 
    }
    if(dat != null){
      MonoBehaviour.print(dat.displayName + " was added to slot" + slot);
    }
    bar[slot] = after;
  }
}
