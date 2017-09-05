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
    switch(slot.ToUpper()){
      case "HEAD":
        ret = layers[0];
        layers[0] = null;
        materials[1] = null;
        break;
      case "TORSO":
        ret = layers[1];
        layers[1] = null;
        materials[2] = null;
        break;
      case "LEGS":
        ret = layers[2];
        layers[2] = null;
        materials[3] = null;
        break; 
    }
    renderer.materials = materials;
    return ret;
  }

  /* Equips equipment to specified slot and returns displaced
     equipment, or null. */
  public Data Equip(Data dat){
    if(dat == null){ return null; }
    string slot = dat.strings[0];
    int index = -1;
    switch(slot.ToUpper()){
      case "HEAD": index = 0; break;
      case "TORSO": index = 1; break;
      case "LEGS": index = 2; break;
    }
    if(index == -1){ return dat; }
    Data displaced = new Data(layers[index]);
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
    return null;
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
