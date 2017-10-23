/*
    Damage acts as a record to contain the information about the different
    effects a particular attack has.
*/
using UnityEngine;

public class Damage{
  public int health, stamina, mana;
  public string limb = "NONE";
  public GameObject source;
  
  public Damage(int health, GameObject source){
    this.health = health;
    this.source = source;
  }
  
}
