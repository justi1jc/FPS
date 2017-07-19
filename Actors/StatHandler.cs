/*
  The stat handler encapsulates the logic behind RPG stats and their calculations
  and stat checks.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StatHandler{
  
  //Conditions
  public int health, healthMax;
  public int stamina, staminaMax;
  public int mana, manaMax;
  
  // ICEPAWS attributes, max value is 10
  public int intelligence, intelligenceMod;
  public int charisma, charismaMod;
  public int endurance, enduranceMod; 
  public int perception, perceptionMod; 
  public int agility, agilityMod;     
  public int willpower, willpowerMod; 
  public int strength, strengthMod;   
  
  // Skill levels, max 100. The mod is a temporary stat modifier.
  public int ranged, rangedMod;
  public int melee, meleeMod;
  public int unarmed, unarmedMod;
  public int magic, magicMod;
  public int stealth, stealthMod;
  
  // leveling
  public int skillPoints;
  public int nextLevel;
  public int level;
  public int xp;
  
  // Abilities
  public List<int> abilities;
  
  /* Faction id 
     0 = neutral(only retaliates)
     1 = feral(attacks anything)
     2 = team Red(Arena)
     3 = team Blue(Arena)
  */
  public int faction = 0;
  
  
  
  public StatHandler(){
    abilities = new List<int>();
    intelligence = charisma = endurance = perception = agility = willpower = strength = 1;
    ranged = melee = unarmed = magic = stealth = skillPoints = xp = 0;
    level = nextLevel = 0;
    health = healthMax = stamina = staminaMax = mana = manaMax = 100;
    LevelUp();
  }
  
  /* Updates condition. */
  public void Update(){
    Regen();
  }
  
  /* Regenerates condition. */
  public void Regen(){
    int healthDifficulty = (healthMax - health) / healthMax;
    int staminaDifficulty = (staminaMax - stamina) / staminaMax;
    int manaDifficulty = (manaMax - mana) / manaMax;
    if(health < healthMax && StatCheck("ENDURANCE", healthDifficulty+5)){
      health++; 
    }
    if(stamina < staminaMax && StatCheck("AGILITY", staminaDifficulty)){ 
      stamina++; 
    }
    if(mana < manaMax && StatCheck("WILLPOWER", manaDifficulty+5)){ 
      mana++; 
    }
  }
  
  /* Performs a roll for competency.
     difficulty directly lowers effective threshold.
  */
  public bool StatCheck(string stat, int difficulty = 0){
    int threshold = 0;
    switch(stat.ToUpper()){
      case "INTELLIGENCE":
        threshold = 10 * (intelligence + intelligenceMod);
        break;
      case "CHARISMA":
        threshold = 10 * (charisma + charismaMod);
        break;
      case "ENDURANCE":
        threshold = 10 * (endurance + enduranceMod);
        break;
      case "PERCEPTION":
        threshold = 10 * (perception + perceptionMod);
        break;
      case "AGILITY":
        threshold = 10 * (agility + agilityMod);
        break;
      case "WILLPOWER":
        threshold = 10 * (willpower + willpowerMod);
        break;
      case "STRENGTH":
        threshold = 10 * (strength + strengthMod);
        break;
      case "RANGED":
        threshold = ranged + rangedMod;
        break;
      case "MELEE":
        threshold = melee + meleeMod;
        break;
      case "UNARMED":
        threshold = unarmed + unarmedMod;
        break;
      case "MAGIC":
        threshold = magic + magicMod;
        break;
      case "STEALTH":
        threshold = stealth + stealthMod;
        break;
    }
    threshold -= difficulty;
    if(threshold < 1){ return false; }
    int roll = Random.Range(0, 100);
    if(roll < threshold){ return true; }
    return false;
  }
  
  /* Applies XP and levels up if possible. */
  public void AwardXP(int amount){
    xp += amount;
    if(xp >= nextLevel){ LevelUp(); }
  }
  
  /* Awards skillpoints for levelup and recalculates nextLevel */
  public void LevelUp(){
    xp = 0;
    level++;
    nextLevel = NextLevel(level);
    skillPoints += intelligence + 10;
  }
  
  /* Returns the xp required for the next level. */
  public int NextLevel(int currentLevel){
    return ((currentLevel + 1) * (currentLevel + 1)) * 100;
  }
  
  /* Reduce a particular condition. */
  public void DrainCondition(string condition, int drain){
    switch(condition.ToUpper()){
      case "HEALTH":
        health -= drain;
        if(health < 0){ health = 0; }
        if(health > healthMax){ health = healthMax; }
        break;
      case "STAMINA":
        stamina -= drain;
        if(stamina < 0){ stamina = 0; }
        if(stamina > staminaMax){ stamina = staminaMax; }
        break;
      case "MANA":
        mana -= drain;
        if(mana < 0){ mana = 0; }
        if(mana > manaMax){ mana = manaMax; }
        break;
    }
  }
  
  
}
