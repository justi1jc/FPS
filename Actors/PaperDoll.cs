/*
     Author: James Justice
       
     The PaperDoll manages the application of clothing and
     Armor to the provided Humanoid model.
     
     Note: The PaperDoll assumes provided data is Equipment.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class PaperDoll{
  [System.NonSerialized]public SkinnedMeshRenderer renderer = null;
  [System.NonSerialized]public Mesh mesh = null;
  [System.NonSerialized]public Actor actor = null;
  public const int HEAD = 0;
  public const int TORSO = 1;
  public const int LEGS = 2;
  
  
  Data[] layers;

  public PaperDoll(Actor a){
    int layerCount = 3;
    layers = new Data[layerCount];
    for(int i = 0; i < layerCount; i++){ layers[i] = null; }
    if(a != null){
      actor = a;
      renderer = a.GetComponentInChildren<SkinnedMeshRenderer>();
      Material[] materials = new Material[4];
      materials[0] = renderer.materials[0];
      renderer.materials = materials;
    }
  }
  
  
  /* Shows the contents of the desired slot. */
  public Data Peek(int slot){
    switch(slot){
      case HEAD: return layers[0]; break;
      case TORSO: return layers[1]; break;
      case LEGS: return layers[2]; break; 
    }
    return null;
  }

  /* Removes and returns the contents of the desired slot. */
  public Data Retrieve(int slot){
    Data ret = null;
    Material[] materials = renderer.materials;
    if(slot < 0){ return null; }
    ret = layers[slot];
    layers[slot] = null;
    materials[slot+1] = null;
    renderer.materials = materials;
    return ret;
  }
  
  /* Returns the integer representation of a slot from its name. 
     Returns -1 on failure.
  */
  public static int GetSlotByName(string slotName){
    switch(slotName.ToUpper()){
      case "HEAD": return HEAD; break;
      case "TORSO": return TORSO; break;
      case "LEGS": return LEGS; break;
    }
    return -1;
  }
  
  /* Returns the Inventory status associated with a given slot, or -1 on 
     failure.
  */
  public static int MapSlotToStatus(int slot){
    switch(slot){
      case HEAD: return Inventory.HEAD; break;
      case TORSO: return Inventory.TORSO; break;
      case LEGS: return Inventory.LEGS; break;
    }
    return -1;
  }
  
  /* Stores item from particular slot into actor's inventory, if possible. */
  public void Store(int slot = -1){
    int status = MapSlotToStatus(slot);
    if(status == -1){ MonoBehaviour.print("Status was -1"); return; }
    Data dat = Retrieve(slot);
    if(dat == null){ return; }
    actor.inventory.StoreEquipped(dat, status);
  }
  
  /* Equips equipment to specified slot and stores displaced if possible.*/
  public void EquipFromInventory(int invIndex){
    if(actor == null || actor.inventory == null){ return; }
    Data dat = actor.inventory.Peek(invIndex);
    if(dat == null || dat.itemType != Item.EQUIPMENT){ return; }
    int slot = GetSlotByName(dat.strings[0]);
    int status = MapSlotToStatus(slot);
    if(invIndex == -1){ return; }
    Data displaced = new Data(layers[slot]);
    if(actor != null && actor.inventory != null){
      actor.inventory.StoreEquipped(displaced, status);
    }
    actor.inventory.SetStatus(invIndex, status);
    layers[slot] = dat;
    if(dat.strings.Count > 1){
      Material material = Resources.Load(dat.strings[1], typeof(Material)) as Material;
      material = new Material(material);
      if(dat.floats.Count > 3){
        float r = dat.floats[0];
        float g = dat.floats[1];
        float b = dat.floats[2];
        float a = dat.floats[3];
        material.color = new Color(r, g, b, a);
      }
      else{ MonoBehaviour.print(dat.prefabName + " lacks a color."); }
      Material[] materials = renderer.materials;
      materials[invIndex+1] = material;
      renderer.materials = materials;
    }
    else{ MonoBehaviour.print(dat.prefabName + " lacks a material."); }
  }
  
  /* Returns the total modifier of equipped clothing. */
  public int Modifier(int stat){
    int sum = 0;
    for(int i = 0; i < layers.Length; i++){
      Data dat = layers[i];
      if(dat != null){ sum += Equipment.GetMod(dat, stat); }
    }
    return sum;
  }
}
