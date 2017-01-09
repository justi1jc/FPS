using UnityEngine;
using System.Collections;

public class InventorySlot{
   public int stack = 1;
   public int stackSize = 1;
   public float unitWeight = 0.0f;
   public string prefabName = "";
   public string displayName = "";
   public ItemData data;
   
   public InventorySlot(string _displayName){
      displayName = _displayName;
   }
   public InventorySlot(ItemData item){
      data = item;
      displayName = item.displayName;
      prefabName = item.prefabName;
      stack = item.stack;
      stackSize = item.stackSize;
   }
   public ItemData GetData(){
      data.stack = stack;
      return data;
   }
   public float GetWeight(){
      return unitWeight * (float)stack;
   }
   public void Empty(){
   }
}
