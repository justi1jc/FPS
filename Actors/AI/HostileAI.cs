/*
    Hostile AI searches for enemies if manager.target is null. Once a target
    is selected, it will equip a weapon or ability and change control to 
    a corresponding 
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class HostileAI : AI{

  public HostileAI(Actor actor, AIManager manager) : base(actor, manager){}
  
  public override IEnumerator Begin(){
    yield return actor.StartCoroutine(FindEnemy());
    Equip();
  }
  
  
  /* Locates a new enemy. */
  IEnumerator FindEnemy(){
    while(manager.target == null){
      
    }
    yield return null;
  }
  
  /* Selects either a ranged or melee weapon, then changes to
     the appropriate combat ai. */
  void Equip(){
    bool ranged = false;
    if(actor.arms.handItem != null && actor.arms.handItem is Ranged){
      ranged = true;
    }
    else{
      List<int> rangedWeapons = RangedWeapons();
      if(rangedWeapons.Count > 0){
        actor.Equip(rangedWeapons[0], true);
        ranged = true;
      }
    }
    if(ranged = true){
      manager.Change("RANGEDCOMBAT");
      return;
    }
    
    bool melee = false;
    if(actor.arms.handItem != null && !(actor.arms.handItem is Melee)){
      List<int> meleeWeapons = MeleeWeapons();
      if(meleeWeapons.Count > 0){
        actor.Equip(meleeWeapons[0], true);
      }
      else{
        actor.EquipAbility(0, true);
      }
    }
    manager.Change("MELEECOMBAT");
  }
  
  
  /* Returns the slot numbers of all melee weapons in inventory. */
  public List<int> MeleeWeapons(){
    List<int> ret = new List<int>();
    if(actor == null || actor.inventory == null){ return ret; }
    Inventory inv = actor.inventory;
    for(int i = 0; i < inv.slots; i++){
      Data dat = inv.Peek(i);
      if(dat != null){
        if(dat.itemType == Item.MELEE){ ret.Add(i); }
      }
    }
    return ret;
  }
  
  
  /* Returns the slot numbers of all ranged weapons in inventory. */
  public List<int> RangedWeapons(){
    List<int> ret = new List<int>();
    if(actor == null || actor.inventory == null){ return ret; }
    Inventory inv = actor.inventory;
    for(int i = 0; i < inv.slots; i++){
      Data dat = inv.Peek(i);
      if(dat != null){
        if(dat.itemType == Item.RANGED){ ret.Add(i); }
      }
    }
    return ret;
  }
  
}
