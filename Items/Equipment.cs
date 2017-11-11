/*
    Equipment represents clothing and armor worn on the
    player's body with the PaperDoll.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Equipment : Item{
  // Stat mods
  public int intelligence, charisma, endurance, perception, agility, willpower, strength;
  public int ranged, melee, unarmed, magic, stealth;
  public int slots;
  public string slot; // What slot does this occupy?
  public string material; // Material for texture-based clothing.
  public bool textureBased = true; // True for texture-based clothing.
  public Color mainColor; // Color for this Equipment.
  
  public void Start(){
    SetColor(mainColor);
  }
  
  public void SetColor(Color color){
    mainColor = color;
    if(color == null){ 
      mainColor = new Color(0f, 0f, 0f, 1f);
      MonoBehaviour.print("Color missing: " + displayName);
    }
  }
  
  /* Changes the color of Equipment in its data form. */
  public static void SetColor(Color color, ref Data dat){
    if(dat == null || dat.itemType != (int)Item.Types.Equipment){ return; }
    dat.floats[0] = color.r;
    dat.floats[1] = color.g;
    dat.floats[2] = color.b;
    dat.floats[3] = color.a;
  }
  
  public override Data GetData(){
    Data dat = GetBaseData();
    dat.ints.Add(intelligence);// Starts with index 1
    dat.ints.Add(charisma);
    dat.ints.Add(endurance);
    dat.ints.Add(perception);
    dat.ints.Add(agility);
    dat.ints.Add(willpower);
    dat.ints.Add(strength);
    
    dat.ints.Add(ranged);
    dat.ints.Add(melee);
    dat.ints.Add(unarmed);
    dat.ints.Add(magic);
    dat.ints.Add(stealth);
    
    dat.ints.Add(slots);
    
    dat.floats.Add(mainColor.r);
    dat.floats.Add(mainColor.g);
    dat.floats.Add(mainColor.b);
    dat.floats.Add(mainColor.a);
    
    dat.strings.Add(slot);
    dat.strings.Add(material);
    dat.itemType = (int)Item.Types.Equipment;
    
    return dat;
  }
  
  /* Returns the value of the stat modifier of this Equipment Data */
  public static int GetMod(Data dat, StatHandler.Stats stat){
    if(dat.ints.Count < 13){
      MonoBehaviour.print("Invalid equipment data.");
      return 0;
    }
    switch(stat){
      case StatHandler.Stats.Intelligence: return dat.ints[1]; break;
        case StatHandler.Stats.Charisma: return dat.ints[2]; break;
        case StatHandler.Stats.Endurance: return dat.ints[3]; break;
        case StatHandler.Stats.Perception: return dat.ints[4]; break;
        case StatHandler.Stats.Agility: return dat.ints[5]; break;
        case StatHandler.Stats.Willpower: return dat.ints[6]; break;
        case StatHandler.Stats.Strength: return dat.ints[7]; break;
        case StatHandler.Stats.Ranged: return dat.ints[8]; break;
        case StatHandler.Stats.Melee: return dat.ints[9]; break;
        case StatHandler.Stats.Unarmed: return dat.ints[10]; break;
        case StatHandler.Stats.Magic: return dat.ints[11]; break;
        case StatHandler.Stats.Stealth: return dat.ints[12]; break;
        case StatHandler.Stats.Slots: return dat.ints[13]; break; 
    }
    return 0;
  }
  
  public override void LoadData(Data dat){
    LoadBaseData(dat);
    
    intelligence = dat.ints[1];
    charisma     = dat.ints[2];
    endurance    = dat.ints[3];
    perception   = dat.ints[4];
    agility      = dat.ints[5];
    willpower    = dat.ints[6];
    strength     = dat.ints[7];
    
    ranged       = dat.ints[8];
    melee        = dat.ints[9];
    unarmed      = dat.ints[10];
    magic        = dat.ints[11];
    stealth      = dat.ints[12];
    
    slots        = dat.ints[13];
    
    float r = dat.floats[0];
    float g = dat.floats[1];
    float b = dat.floats[2];
    float a = dat.floats[3];
    Color color = new Color(r, g, b, a);
    SetColor(color);
    
    slot = dat.strings[0];
    material = dat.strings[1];
  }
  
  /* Returns the slot this equipment should be associated with. */
  public static string SlotType(Data dat){
    if(dat.itemType == (int)Item.Types.Equipment && dat.strings.Count > 0){
      return dat.strings[0];
    }
    return "";
  }
}
