/*
*     Author: James Justice
*       
*     Serializeable list of Data objects
*     TODO: Migrate inventory logic here from Actor for modularity
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Inventory{
  public List<Data> inv;
  public int slots = 20; // Max number of slots.
  public int hotSlots = 3;
  int nextSlot = 0;
  
  public Inventory(){
    inv = new List<Data>();
    for(int i = 0 ; i < slots+hotSlots; i++){
      inv.Add(null);
    }
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
          inv[i].stack < inv[i].stackSize 
      ){
        dat.stack = StackItem(i, dat);
      }
    }
    if(dat.stack == 0){ return 0; }
    
    for(int i = 0; i < slots; i++){
      if(inv[i] == null){
        inv[i] = dat;
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
    }
    return ret;
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
