/*
       
     Inventory used for 
     
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//[System.Serializable] Prevents null slots.
public class Inventory{

  // Equip status constants 
  public const int EMPTY = 0; // Empty slot
  public const int STORED = 1; // Item is not equipped
  public const int PRIMARY = 2; // Item is equipped to primary slot.
  public const int SECONDARY = 3; // Item is eqipped to Secondary slot.
  public const int HEAD = 4; // Item is equipped to head slot.
  public const int TORSO = 5; // Item is equipped to torso slot.
  public const int LEGS = 6; // Item is equipped to leg slot.

  public List<Data> inv; // Item data each slot.
  public List<int> status; // Status of this slot's item.
  public int slots = 10; // Max number of slots.
  public const int favCount = 4;// Max number of fav slots.
  public List<int> favs; // Slots marked as favorite for quick access.
  int nextSlot = 0;
  
  public Inventory(){
    inv = new List<Data>();
    status = new List<int>();
    favs = new List<int>();
    for(int i = 0 ; i < slots; i++){
      inv.Add(null);
      status.Add(EMPTY);
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
  public Data EquipFav(int favSlot, int slot){
    if(favSlot < 0 || favSlot > favs.Count){ return null; }
    if(status[favs[favSlot]] != STORED){ return null; }
    status[favs[favSlot]] = slot;
    return inv[favs[favSlot]];
  }
  
  /* Stores an equipped item by its favslot. */
  public void StoreFav(int favSlot, Data dat){
    if(favSlot < 0 || favSlot > favs.Count){ return; }
    if(status[favs[favSlot]] == STORED){ return; }
    inv[favs[favSlot]] = new Data(dat);
    status[favs[favSlot]] = STORED;
  }
  
  /* Returns the first slot with the desired status, or -1. */
  public int GetSlotByStatus(int stat){
    for(int i = 0; i < slots; i++){
      if(status[i] == stat){ return i; }
    }
    return -1;
  }
  
  /* Returns the status of an item if it were to be equipped. */
  public static int GetStatusByData(Data dat, bool primary){
    if(dat == null){ return -1; }
    int status = -1;
    if(dat.itemType == Item.EQUIPMENT){
      switch(Equipment.SlotType(dat)){
        case "": return -1; break;
        case "HEAD": status = Inventory.HEAD; break;
        case "TORSO": status = Inventory.TORSO; break;
        case "LEGS": status = Inventory.LEGS; break;
      }
    }
    else if(primary){ status = Inventory.PRIMARY; }
    else{ status = Inventory.SECONDARY; }
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
  public void StoreEquipped(Data dat, int stat){
    int slot = GetSlotByStatus(stat);
    if(slot < 0 || slot >= slots || dat == null){ return; }
    inv[slot] = dat;
    status[slot] = STORED;
  }
  
  /* Setter for status. */
  public void SetStatus(int slot, int val){
    if(slot >= status.Count || slot < 0){ return; }
    status[slot] = val;
  }
  
  /* Getter for status. */
  public int GetStatus(int slot){
    if(slot >= status.Count || slot < 0){ return EMPTY; }
    return status[slot];
  }
  
  /* Returns next weapon starting from nextSlot or null. */
  public Data NextWeapon(){
    int fromBeginning = -1;
    for(int i = 0; i < slots; i++){
      if(inv[i] != null && (inv[i].itemType == Item.WEAPON ||
        inv[i].itemType == Item.MELEE ||
        inv[i].itemType == Item.RANGED)
      ){
        if(i < nextSlot){ fromBeginning = i; }
        else if(i >= nextSlot){
          nextSlot = i + 1;
          if(nextSlot >= slots){ nextSlot = 0; }
          return Retrieve(i, inv[i].stack);
        }
      }
    }
    if(fromBeginning == -1){ return null; }
    nextSlot = fromBeginning -1;
    if(nextSlot < 0){ nextSlot = slots -1; }
    return Retrieve(fromBeginning, inv[fromBeginning].stack);
  }
  
  /* Returns the previous weapon starting from next slot or null. */
  public Data PreviousWeapon(){
    int fromEnd = -1;
    for(int i = slots-1; i > -1; i--){
      if(inv[i] != null && (inv[i].itemType == Item.WEAPON ||
        inv[i].itemType == Item.MELEE ||
        inv[i].itemType == Item.RANGED)
      ){
        if(i > nextSlot){ fromEnd = i; }
        else if(i < nextSlot){
          nextSlot = i - 1;
          if(nextSlot < 0){ nextSlot = slots-1; }
          return Retrieve(i, inv[i].stack);
        }
      }
    }
    if(fromEnd == -1){ return null; }
    nextSlot = fromEnd + 1;
    if(nextSlot >= slots){ nextSlot = 0; }
    return Retrieve(fromEnd, inv[fromEnd].stack);
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
          status[i] == STORED
      ){
        dat.stack = StackItem(i, dat);
      }
    }
    if(dat.stack == 0){ return 0; }
    
    for(int i = 0; i < slots; i++){
      if(inv[i] == null && status[i] == EMPTY){
        inv[i] = dat;
        status[i] = STORED;
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
      status[slot] = EMPTY;
    }
    return ret;
  }
  
  /* Returns the contents of this slot, marking it with the given status */
  public Data Equip(int slot, int stat){
    if(slot < 0 || slot >= inv.Count || status[slot] != STORED){ return null; }
    status[slot] = stat;
    return new Data(inv[slot]);
  }
  
  /* Clears all slots with the given status. */
  public void ClearEquipped(int stat){
    MonoBehaviour.print("Clearing for stat" + stat);
    for(int i = 0; i < inv.Count; i++){
      if(status[i] == stat){
        inv[i] = null;
        status[i] = EMPTY;
        MonoBehaviour.print("Cleared slot " + i);
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
