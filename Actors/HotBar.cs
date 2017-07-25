/*
    The HotBar tracks the location of items as they move between
    the Inventory and the EquipSlot for the purpose of cycling 
    through available weapons consistently.
    Weapons are automatically added to empty slots in the hotbar 
    and slots are emptied when the selected item is dropped.
    
    
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

  public void NextSlot(){
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

  /* Adds item into new slot if it is a weapon. */
  public void ItemStored(Data dat, int location){
    if(dat.itemType != Item.WEAPON 
      && dat.itemType != Item.MELEE 
      && dat.itemType != Item.RANGED
    ){
      int slot = -1;
      for(int i = 0; i < slots; i++){
        if(bar[i] == -3){ slot = i; break;}
      }
      if(slot == -1){ return; }
      bar[slot] = location;
    }
  }

  public void UpdateSlot(int before, int after){
    int slot = -1;
    for(int i = 0; i < slots; i++){
      if(before == bar[i]){ slot = i; break; }
    }
    if(slot == -1){ return; }
    bar[slot] = after;
  }
}
