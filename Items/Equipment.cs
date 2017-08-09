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
  
  public override void Use(int action){
    if(action == 0){ Consume(); }
  }
  
  public override GetData(){
    Data dat = baseData();
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
  }
  
  public void LoadData(Data dat){
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
  }
}
