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
  public Data Peek(string slot = "NONE"){
    switch(slot.ToUpper()){
      case "NONE": return null; break;
      case "HEAD": return layers[0]; break;
      case "TORSO": return layers[1]; break;
      case "LEGS": return layers[2]; break; 
    }
    return null;
  }

  /* Removes and returns the contents of the desired slot. */
  public Data Retrieve(string slot = "NONE"){
    Data ret = null;
    Material[] materials = renderer.materials;
    int si =SlotInt(slot);
    if(si < 0){ return null; }
    ret = layers[si];
    layers[si] = null;
    materials[si+1] = null;
    renderer.materials = materials;
    return ret;
  }
  
  /* Returns integer representation of slot. */
  public int SlotInt(string slot){
    switch(slot){
      case "NONE": return -1; break;
      case "HEAD": return 0; break;
      case "TORSO": return 0; break;
      case "LEGS":  return 0; break;
    }
    return -1;
  }
  
  /* Stores item from particular slot into actor's inventory, if possible. */
  public void Store(string slot = "NONE"){
    int status = -1;
    switch(slot){
      case "HEAD": status = Inventory.HEAD; break;
      case "TORSO": status = Inventory.TORSO; break;
      case "LEGS":  status = Inventory.LEGS; break;
    }
    if(status == -1){ MonoBehaviour.print("Status was -1"); return; }
    Data dat = Retrieve(slot);
    actor.inventory.StoreEquipped(dat, status);
  }
  
  /* Equips equipment to specified slot and stores displaced if possible.*/
  public void EquipFromInventory(int index){
    if(actor == null || actor.inventory == null){ return; }
    Data dat = actor.inventory.Peek(index);
    if(dat == null || dat.itemType != Item.EQUIPMENT){ return; }
    string slot = dat.strings[0];
    int status = -1;
    switch(slot.ToUpper()){
      case "HEAD": index = 0; status = Inventory.HEAD; break;
      case "TORSO": index = 1; status = Inventory.TORSO; break;
      case "LEGS": index = 2; status = Inventory.LEGS; break;
    }
    if(index == -1){ return; }
    Data displaced = new Data(layers[index]);
    if(actor != null && actor.inventory != null){
      actor.inventory.StoreEquipped(dat, status);
    }
    actor.inventory.SetStatus(index, status);
    layers[index] = dat;
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
      materials[index+1] = material;
      renderer.materials = materials;
    }
    else{ MonoBehaviour.print(dat.prefabName + " lacks a material."); }
  }
  
  /* Returns the total modifier of equipped clothing. */
  public int Modifier(string stat){
    int sum = 0;
    for(int i = 0; i < layers.Length; i++){
      Data dat = layers[i];
      if(dat != null){
        switch(stat.ToUpper()){
          case "INTELLIGENCE": sum+= dat.ints[1]; break;
          case "CHARISMA": sum+= dat.ints[2]; break;
          case "Endurance": sum+= dat.ints[3]; break;
          case "PERCEPTION": sum+= dat.ints[4]; break;
          case "AGILITY": sum+= dat.ints[5]; break;
          case "WILLPOWER": sum+= dat.ints[6]; break;
          case "STRENGTH": sum+= dat.ints[7]; break;
          case "RANGED": sum+= dat.ints[8]; break;
          case "MELEE": sum+= dat.ints[9]; break;
          case "UNARMED": sum+= dat.ints[10]; break;
          case "MAGIC": sum+= dat.ints[11]; break;
          case "STEALTH": sum+= dat.ints[12]; break;
          case "SLOTS": sum+= dat.ints[13]; break;
        }
      }
    }
    return sum;
  }
}
