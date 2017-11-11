/*
  The stat handler encapsulates the logic behind RPG stats and their calculations
  and stat checks.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StatHandler{
  
  public Actor actor;
  
  public enum Stats{
    //Conditions
    Health, Stamina, Mana, 
    
    // ICEPAWS attributes
    Intelligence, Charisma, Endurance,
    Perception, Agility, Willpower, Strength,
    
    Slots,
    //Skills
    Ranged, Melee, Unarmed, Magic, Stealth
  };
  
  //Conditions
  public bool dead;
  public int health, healthMax;
  public int stamina, staminaMax;
  public int mana, manaMax;
  public int slots;
  
  // ICEPAWS attributes, max value is 10
  public int intelligence;
  public int charisma;
  public int endurance; 
  public int perception; 
  public int agility;     
  public int willpower; 
  public int strength;   
  
  // Skill levels, max 100.
  public int ranged;
  public int melee;
  public int unarmed;
  public int magic;
  public int stealth;
  
  // leveling
  public int skillPoints;
  public int nextLevel;
  public int level;
  public int xp;
  
  // Abilities
  public List<Data> abilities;


  public Factions faction = 0;
  // Faction constants
  public enum Factions{ Neutral, Feral, RedTeam, BlueTeam };
  

  
  public StatHandler(Actor actor){
    this.actor = actor;
    dead = false;
    abilities = new List<Data>();
    intelligence = charisma = endurance = perception = agility = willpower = strength = 5;
    ranged = melee = unarmed = magic = stealth = skillPoints = xp = 0;
    level = nextLevel = 0;
    InitCondition();
    LevelUp();
  }
  
  private void InitCondition(){
    health = healthMax = 50+(endurance * 10);
    stamina = staminaMax = 50+(endurance * 10);
    mana = manaMax = 50 + (willpower * 10);
  }
  
  /* Updates condition. */
  public void Update(){
    Regen();
  }
  
  /* Regenerates condition. */
  public void Regen(){
    if(dead){ return; }
    if(stamina < staminaMax){
      stamina += agility;
      if(stamina > staminaMax){ stamina = staminaMax; }
    }
    if(mana < manaMax){
      mana += willpower;
      if(mana > manaMax){ mana = manaMax; }
    }
  }
  
  
  /* Performs a roll for competency.
     difficulty directly lowers effective threshold.
  */
  public bool StatCheck(Stats stat, int difficulty = 0){
    int threshold = 0;
    switch(stat){
      case Stats.Intelligence:
        threshold = 10 * (intelligence);
        break;
      case Stats.Charisma:
        threshold = 10 * (charisma);
        break;
      case Stats.Endurance:
        threshold = 10 * (endurance);
        break;
      case Stats.Perception:
        threshold = 10 * (perception);
        break;
      case Stats.Agility:
        threshold = 10 * (agility);
        break;
      case Stats.Willpower:
        threshold = 10 * (willpower);
        break;
      case Stats.Strength:
        threshold = 10 * (strength);
        break;
      case Stats.Ranged:
        threshold = ranged;
        break;
      case Stats.Melee:
        threshold = melee;
        break;
      case Stats.Unarmed:
        threshold = unarmed;
        break;
      case Stats.Magic:
        threshold = magic;
        break;
      case Stats.Stealth:
        threshold = stealth;
        break;
    }
    threshold -= difficulty;
    if(threshold < 1){ return false; }
    int roll = Random.Range(0, 100);
    if(roll < threshold){ return true; }
    return false;
  }
  
  /* Calculates a score between 0 and 100 based on aim penalties and bonuses. */
  public int AccuracyScore(){
    int score = 50;
    if(actor.walking){
      score -= 25;
      if(actor.sprinting){ score -= 25; }
    }
    else if(actor.crouched){ score += 10; }
    if(stamina < (staminaMax/4)){ score -= 25; }
    if(!actor.walking && actor.aiming){ score += 40; }
    if(score < 0){ return 0; }
    else if(score > 100){ return 100; }
    return score;
  }
  
  /* Calculates an offset of aim for use by ranged weapons. */
  public Vector3 AccuracyPenalty(){
    float score = (float)AccuracyScore();
    float mag = 0.1f - score/1000f;
    if(mag < 0){ new Vector3(); }
    float x = Random.Range(0f, mag);
    mag -= x;
    x = Random.Range(-x, x);
    float y = Random.Range(0f, mag);
    mag -= y;
    y = Random.Range(-y, y);
    float z = Random.Range(0f, mag);
    mag -= z;
    z = Random.Range(-z, z);
    
    return new Vector3(x,y,z);
  }
  
  public int GetStat(Stats stat){
    return BaseStat(stat) + Modifier(stat);
  }
  
  /* Returns a stat without modifiers, or -1*/
  public int BaseStat(Stats stat){
    switch(stat){
      case Stats.Intelligence: return intelligence; break;
      case Stats.Charisma: return charisma; break;
      case Stats.Endurance: return endurance; break;
      case Stats.Perception: return perception; break;
      case Stats.Agility: return agility; break;
      case Stats.Willpower: return willpower; break;
      case Stats.Strength: return strength; break;
      case Stats.Ranged: return ranged; break;
      case Stats.Melee: return melee; break;
      case Stats.Unarmed: return unarmed; break;
      case Stats.Magic: return magic; break;
      case Stats.Stealth: return stealth; break;
      case Stats.Slots: return slots; break;
    }
    return -1;
  }
  
  public int Modifier(Stats stat){
    int mod = 0;
    if(actor.doll != null){ mod += actor.doll.Modifier(stat); }
    return mod;
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
  
  /* Reduce a particular condition.
     Returns the condition drained. 
  */
  public int DrainCondition(Stats condition, int drain, GameObject weapon = null){
    int ret = drain;
    switch(condition){
      case Stats.Health:
        health -= drain;
        if(health <= 0){
          ret = drain + health;
          health = 0;
          actor.Die(weapon);
        }
        if(health > healthMax){ health = healthMax; }
        break;
      case Stats.Stamina:
        stamina -= drain;
        if(stamina < 0){
          ret = drain + stamina; 
          stamina = 0;
        }
        if(stamina > staminaMax){ stamina = staminaMax; }
        break;
      case Stats.Mana:
        mana -= drain;
        if(mana < 0){
          ret = drain + mana; 
          mana = 0;
        }
        if(mana > manaMax){ mana = manaMax; }
        break;
    }
    return ret;
  }
  
  /* Returns true if the selected actor is an enemy of this one. */
  public bool Enemy(Actor a){
    if(faction == Factions.Neutral){ return false; }
    if(faction == Factions.Feral){ return true; }
    if( faction == a.stats.faction){ return false; }
    return true;
  }
  
  /* Returns the id representation of a faction. */
  public static Factions Faction(string factionName){
    switch(factionName.ToUpper()){
      case "NONE": return Factions.Neutral; break;
      case "RED": return Factions.RedTeam; break;
      case "BLUE": return Factions.BlueTeam; break;
    }
    return Factions.Neutral;
  }
  
  /* Returns the name of a given faction ID */
  public static string Faction(int factionID){
    switch(factionID){
      case 0: return "None"; break;
      case 1: return "Red"; break;
      case 2: return "Blue"; break;
    }
    return "Invalid ID";
  }
}
