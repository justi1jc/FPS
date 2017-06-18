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
  int slots = 20; // Max number of slides.
  
  public Inventory(){
    inv = new List<Data>();
    for(int i = 0 ; i < slots; i++){
      inv.Add(null);
    }
  }
  
  public Inventory(List<Data> _inv){
    inv = _inv;
  }
  
  /* Stores this item and returns the overflow if it can't fit */
  public int Store(Data dat){
    for(int i = 0; i < inv.Count; i++){ 
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
    
    for(int i = 0; i < inv.Count; i++){
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
    if(slot < 0 || slot >= inv.Count || inv[slot] == null){ return null; }
    Data ret = inv[slot];
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
    if(slot < 0 || slot >= inv.Count || inv[slot] == null){ return null; }
    return inv[slot];
  }
}
