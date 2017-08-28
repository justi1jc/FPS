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
      manager.sighted = ScanForActors();
      if(manager.sighted.Count > 0){ SelectTarget(); }
      yield return new WaitForSeconds(1f);
    }
    yield return new WaitForSeconds(0);
  }
  
  void SelectTarget(){
    Actor a = manager.sighted[0];
    float leastDistance = Vector3.Distance(manager.sighted[0].transform.position, actor.transform.position);
    for(int i = 1; i < manager.sighted.Count; i++){
      float dist = Vector3.Distance(manager.sighted[i].transform.position, actor.transform.position);
      if(dist < leastDistance){
        leastDistance = dist;
        a = manager.sighted[i];
      }
    }
    manager.target = a.gameObject;
  }
  
  /* Returns true if this ranged weapon has ammo in it, or in the actor's
     inventory. */
  public bool HasAmmo(Ranged r){
    if(r.ammo > 0){ return true; }
    Inventory inv = actor.inventory;
    for(int i = 0; i < inv.slots; i++){
      Data dat = inv.Peek(i);
      if(dat != null && dat.displayName == r.ammunition){ return true; }
    }
    return false;
  }
  
  /* overloaded to accept data */
  public bool HasAmmo(Data rdat){
    if(rdat.ints[1] > 0){ return true; }
    Inventory inv = actor.inventory;
    for(int i = 0; i < inv.slots; i++){
      Data dat = inv.Peek(i);
      if(dat != null && dat.displayName == rdat.strings[0]){ return true; }
    }
    return false;
  }
  
  /* Selects either a ranged or melee weapon, then changes to
     the appropriate combat ai. */
  void Equip(){
    bool ranged = false;
    Ranged r = null;
    Inventory inv = actor.inventory;
    if(actor.arms.handItem != null && actor.arms.handItem is Ranged){
      r = (Ranged)actor.arms.handItem;
    }
    else if(actor.arms.offHandItem != null && actor.arms.offHandItem is Ranged){
      r = (Ranged)actor.arms.offHandItem;
    }
    if(r != null && HasAmmo(r)){
      ranged = true;
    }
    else{
      List<int> rangedWeapons = RangedWeapons();
      for(int i = 0; i < rangedWeapons.Count; i++){
        if(HasAmmo(inv.Peek(rangedWeapons[i]))){
          actor.Equip(rangedWeapons[0], true);
          ranged = true;
          break;
        }
      }
    }
    if(ranged == true){
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
