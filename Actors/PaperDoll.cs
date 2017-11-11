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
  public enum Slots{ None, Head, Torso, Legs };
  
  
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
  public Data Peek(Slots slot){
    switch(slot){
      case Slots.Head: return layers[0]; break;
      case Slots.Torso: return layers[1]; break;
      case Slots.Legs: return layers[2]; break; 
    }
    return null;
  }

  /* Removes and returns the contents of the desired slot. */
  public Data Retrieve(Slots slot){
    Data ret = null;
    Material[] materials = renderer.materials;
    if(slot < 0){ return null; }
    ret = layers[SlotToLayer(slot)];
    layers[SlotToLayer(slot)] = null;
    materials[SlotToMaterial(slot)] = null;
    renderer.materials = materials;
    return ret;
  }
  
  /* Returns the integer representation of a slot from its name. 
     Returns -1 on failure.
  */
  public static Slots GetSlotByName(string slotName){
    switch(slotName.ToUpper()){
      case "HEAD": return Slots.Head; break;
      case "TORSO": return Slots.Torso; break;
      case "LEGS": return Slots.Legs; break;
    }
    return Slots.None;
  }
  
  /* Returns the Inventory status associated with a given slot, or -1 on 
     failure.
  */
  public static Inventory.Statuses MapSlotToStatus(Slots slot){
    switch(slot){
      case Slots.Head: return Inventory.Statuses.Head; break;
      case Slots.Torso: return Inventory.Statuses.Torso; break;
      case Slots.Legs: return Inventory.Statuses.Legs; break;
    }
    return Inventory.Statuses.Empty;
  }
  
  /* Stores item from particular slot into actor's inventory, if possible. */
  public void Store(Slots slot = Slots.None){
    Inventory.Statuses status = MapSlotToStatus(slot);
    if(status == Inventory.Statuses.Empty){ 
      MonoBehaviour.print("Status was Empty"); 
      return; 
    }
    Data dat = Retrieve(slot);
    if(dat == null){ return; }
    actor.inventory.StoreEquipped(dat, status);
  }
  
  /* Returns the index of the data in layers corresponding to this slot. */
  public int SlotToLayer(Slots slot){
    return(int)slot - 1;
  }
  
  /* Returns the index of the material in materials corresponding to this slot. */
  public int SlotToMaterial(Slots slot){
    return (int)slot;
  }
  
  /* Equips equipment from specified inventory.*/
  public void EquipFromInventory(int invIndex){
    if(actor == null || actor.inventory == null){ return; }
    Data dat = actor.inventory.Peek(invIndex);
    if(dat == null || dat.itemType != (int)Item.Types.Equipment){ return; }
    Slots slot = GetSlotByName(dat.strings[0]);
    Inventory.Statuses status = MapSlotToStatus(slot);
    if(invIndex == -1){ return; }
    Data displaced = new Data(layers[SlotToLayer(slot)]);
    if(actor != null && actor.inventory != null){
      actor.inventory.StoreEquipped(displaced, status);
    }
    actor.inventory.SetStatus(invIndex, status);
    layers[SlotToLayer(slot)] = dat;
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
      materials[SlotToMaterial(slot)] = material;
      renderer.materials = materials;
    }
    else{ MonoBehaviour.print(dat.prefabName + " lacks a material."); }
  }
  
  /* Returns the total modifier of equipped clothing. */
  public int Modifier(StatHandler.Stats stat){
    int sum = 0;
    for(int i = 0; i < layers.Length; i++){
      Data dat = layers[i];
      if(dat != null){ sum += Equipment.GetMod(dat, stat); }
    }
    return sum;
  }
}
