/*
       
     Inventory used for 
     
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//[System.Serializable] Prevents null slots.
public class Inventory{

  // Equip status constants
  public enum Statuses{
    Empty, // Empty Slot
    Stored, // Not equipped
    Primary, // Equipped to primary slot
    Secondary, // Equipped to secondary slot
    Head,  // head slot
    Torso,  // Torso slot
    Legs // Legs slot
  };

  public List<Data> inv; // Item data each slot.
  public List<Statuses> status; // Status of this slot's item.
  public int slots = 10; // Max number of slots.
  public const int favCount = 4;// Max number of fav slots.
  public List<int> favs; // Slots marked as favorite for quick access.
  int nextSlot = 0;
  
  public Inventory(){
    inv = new List<Data>();
    status = new List<Statuses>();
    favs = new List<int>();
    for(int i = 0 ; i < slots; i++){
      inv.Add(null);
      status.Add(Statuses.Empty);
    }
    for(int i = 0; i < 4; i++){
      favs.Add(-1);
    }
  }
  
  
  public void SetFav(int favSlot, int val){
    if(favSlot < 0 || val < 0 || favSlot < favs.Count || val < inv.Count){ 
      return;
    }
    favs[favSlot] = val;
  }
  
  /* Returns the item from a fav slot, updating its status. */
  public Data EquipFav(int favSlot, Inventory.Statuses slot){
    if(favSlot < 0 || favSlot > favs.Count){ return null; }
    if(status[favs[favSlot]] != Statuses.Stored){ return null; }
    status[favs[favSlot]] = slot;
    return inv[favs[favSlot]];
  }
  
  /**
    * Returns the index of the found item, or -1 if not
    * @param {string} name - display name to search for.
    * @return {int} - index of result. -1 means no result.
    */
  public int ItemByDisplayName(string name){
    for(int i = 0; i < slots; i++){
      if(inv[i] != null && inv[i].displayName == name){
        return i;
      }
    }
    return -1;
  }
  
  /* Stores an equipped item by its favslot. */
  public void StoreFav(int favSlot, Data dat){
    if(favSlot < 0 || favSlot > favs.Count){ return; }
    if(status[favs[favSlot]] == Statuses.Stored){ return; }
    inv[favs[favSlot]] = new Data(dat);
    status[favs[favSlot]] = Statuses.Stored;
  }
  
  /* Returns the first slot with the desired status, or -1. */
  public int GetSlotByStatus(Statuses stat){
    for(int i = 0; i < slots; i++){
      if(status[i] == stat){ return i; }
    }
    return -1;
  }
  
  /* Returns the status of an item if it were to be equipped. */
  public static Statuses GetStatusByData(Data dat, bool primary){
    if(dat == null){ return Statuses.Empty; }
    Statuses status = Statuses.Empty;
    if(dat.itemType == (int)Item.Types.Equipment){
      switch(Equipment.SlotType(dat)){
        case "": return Statuses.Empty; break;
        case "HEAD": status = Inventory.Statuses.Head; break;
        case "TORSO": status = Inventory.Statuses.Torso; break;
        case "LEGS": status = Inventory.Statuses.Legs; break;
      }
    }
    else if(primary){ status = Inventory.Statuses.Primary; }
    else{ status = Inventory.Statuses.Secondary; }
    return status;
  }
  
  /* Returns the total count of items with given displayName. */
  public int ItemCount(string name){
    int count = 0;
    for(int i = 0; i < slots; i++){
      if(inv[i] != null && inv[i].displayName == name){
        count += inv[i].stack;
      }
    }
    return count;
  }
  
  /* Uses an equipped item's status to locate and update its slot. */
  public void StoreEquipped(Data dat, Statuses stat){
    int slot = GetSlotByStatus(stat);
    if(slot < 0 || slot >= slots || dat == null){ return; }
    inv[slot] = dat;
    status[slot] = Inventory.Statuses.Stored;
  }
  
  /* Setter for status. */
  public void SetStatus(int slot, Statuses val){
    if(slot >= status.Count || slot < 0){ return; }
    status[slot] = val;
  }
  
  /* Getter for status. */
  public Statuses GetStatus(int slot){
    if(slot >= status.Count || slot < 0){ return Statuses.Empty; }
    return status[slot];
  }
  
  /* Returns the index of the first weapon in inventory, or -1. */
  public int FirstWeapon(){
    for(int i = 0; i < slots; i++){
      if(inv[i] != null && (inv[i].itemType == (int)Item.Types.Weapon ||
        inv[i].itemType == (int)Item.Types.Melee ||
        inv[i].itemType == (int)Item.Types.Ranged)
      ){
        return i;
      }
    }
    return -1;
  }

  public Inventory(List<Data> _inv){
    inv = _inv;
  }

  public int IndexOf(Data dat){
    for(int i = 0; i < slots; i++){
      if(dat == inv[i]){ return i; }
    }
    return -1;
  }
  
  /* Stores this item and returns the overflow if it can't fit */
  public int Store(Data dat){
    if(dat == null){ return 0; }
    for(int i = 0; i < slots; i++){ 
      if(dat.stack == 0){ return 0; }
      if(
          inv[i] != null &&
          inv[i].displayName == dat.displayName &&
          inv[i].stack < inv[i].stackSize &&
          status[i] == Statuses.Stored
      ){
        dat.stack = StackItem(i, dat);
      }
    }
    if(dat.stack == 0){ return 0; }
    
    for(int i = 0; i < slots; i++){
      if(inv[i] == null && status[i] == Statuses.Empty){
        inv[i] = dat;
        status[i] = Statuses.Stored;
        return 0;
      }
    }
    return dat.stack;
  }
  
  /* fills out selected slot and returns the remainder. */
  public int StackItem(int slot, Data dat){
    int sum = dat.stack + inv[slot].stack;
    if(sum <= inv[slot].stackSize){
      inv[slot].stack = sum;
      return 0;
    }
    else{ return sum - inv[slot].stackSize; }
  }
  
  /* Removes up to the quantity of items from a given slot, and returns it
     or null. */
  public Data Retrieve(int slot, int quantity = 1){
    if(slot < 0 || slot >= slots || inv[slot] == null){ return null; }
    Data ret = new Data(inv[slot]);
    if(quantity < inv[slot].stack){ 
      ret.stack = quantity;
      inv[slot].stack -= quantity;
    }
    else{
      inv[slot] = null;
      status[slot] = Statuses.Empty;
    }
    return ret;
  }
  
  /* Returns the contents of this slot, marking it with the given status */
  public Data Equip(int slot, Statuses stat){
    if(slot < 0 || slot >= inv.Count || status[slot] != Statuses.Stored){ 
      return null; 
    }
    status[slot] = stat;
    return new Data(inv[slot]);
  }
  
  /* Clears all slots with the given status. */
  public void ClearEquipped(Statuses stat){
    for(int i = 0; i < inv.Count; i++){
      if(status[i] == stat){
        inv[i] = null;
        status[i] = Statuses.Empty;
      }
    }
  }
  
  /* Return the contents of a slot, or null */
  public Data Peek(int slot){
    if(slot < 0 || slot >= slots || inv[slot] == null){ return null; }
    return inv[slot];
  }
  
  /* Drops item onto ground based on data. */
  public void DiscardItem(Data dat, Vector3 position = new Vector3()){
    if(dat == null){ return; }
    GameObject prefab = Resources.Load("Prefabs/" + dat.prefabName) as GameObject;
    GameObject itemGO = (GameObject)GameObject.Instantiate(
      prefab,
      position,
      Quaternion.identity
    );
    Item item = itemGO.GetComponent<Item>();
    item.LoadData(dat);
    item.stack = 1;
  }
  
  /* Load from serialized record */
  public void LoadData(InventoryRecord rec){
    inv = new List<Data>(rec.inv);
  }
  
  /* Returns a record for serialization. */
  public InventoryRecord GetData(){
    return new InventoryRecord(inv); 
  }
}
