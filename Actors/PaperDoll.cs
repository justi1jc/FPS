/*
*     Author: James Justice
*       
*     The PaperDoll manages the application of clothing and
*     Armor to the provided Humanoid model.
*     
*     Note: The PaperDoll assumes provided data is Equipment.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class PaperDoll{
  public MeshRenderer renderer = null;
  public Actor actor = null;
  
  
  Data[] layers;
  


  public PaperDoll(Actor a){
    int layerCount = 3;
    layers = new Data[layerCount];
    for(int i = 0; i < layerCount; i++){ layers[i] = null; }
    if(a != null){
      actor = a;
      if(a.renderer != null){ renderer = a.renderer; }
    }
    if(renderer != null){ InitRenderer(); }
  }
  
  public void InitRenderer(){
    
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
    switch(slot.ToUpper()){
      case "HEAD": ret = layers[0]; layers[0] = null; break;
      case "TORSO": ret = layers[1]; layers[1] = null; break;
      case "LEGS": ret = layers[2]; layers[2] = null; break; 
    }
    return ret;
  }

  /* Equips equipment to specified slot and returns displaced
     equipment, or null. */
  public Data Equip(Data dat){
    if(dat == null){ return null; }
    MonoBehaviour.print("Equipped" + dat.displayName);
    string slot = dat.strings[0];
    int index = -1;
    switch(slot.ToUpper()){
      case "NONE": return dat; break;
      case "HEAD": index = 0; break;
      case "TORSO": index = 1; break;
      case "LEGS": index = 2; break;
    }
    displaced = new Data(layers[index]);
    layers[index] = dat;
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
