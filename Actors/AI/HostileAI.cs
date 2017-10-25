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
    ChooseWeapon();
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
  
  /* Chooses a weapon and changes to the appropriate AI for it.*/
  void ChooseWeapon(){
    actor.arms.StoreAll();
    List<int> rangedWeapons = RangedWeapons();
    if(rangedWeapons.Count > 0){
      int ranged = MaxDamage(rangedWeapons);
      actor.Equip(ranged);
      manager.Change("RANGEDCOMBAT");
      return;
    }
    List<int> meleeWeapons = MeleeWeapons();
    if(meleeWeapons.Count > 0){
      int melee = MaxDamage(meleeWeapons);
      actor.Equip(melee);
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
  
  /* Returns the weapon with the highest damage. */
  public int MaxDamage(List<int> choices){
    Inventory inv = actor.inventory;
    int choice = 0;
    int maxDamage = Weapon.Damage(inv.Peek(choice));
    for(int i = 0; i < choices.Count; i++){
      int currentDamage = Weapon.Damage(inv.Peek(choices[i]));
      if(currentDamage > maxDamage){
        maxDamage = currentDamage;
        choice = i;
      }
    }
    return choice;
  }
  
  /* Returns the slot numbers of all ranged weapons in inventory that either
    have ammo loaded, or can be loaded from ammo in the inventory.
  */
  public List<int> RangedWeapons(){
    List<int> ret = new List<int>();
    if(actor == null || actor.inventory == null){ return ret; }
    Inventory inv = actor.inventory;
    for(int i = 0; i < inv.slots; i++){
      Data dat = inv.Peek(i);
      if(dat != null){
        if(dat.itemType == Item.RANGED){ 
          bool hasAmmo = Ranged.Ammo(dat) > 0;
          if(!hasAmmo){
            string ammoName = Ranged.AmmoName(dat);
            hasAmmo = inv.ItemCount(ammoName) > 0;
          }
          if(hasAmmo){ ret.Add(i); }
        }
      }
    }
    return ret;
  }
  
}
