/*
    An EquipSlot is a component that uses up to two gameObjects as mount points
    as anchor points for held items and manages the equipment and use of abilities.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class EquipSlot{
  Actor actor = null; // Actor, if one is associated with this EquipSlot
  GameObject hand, offHand;
  Item handItem, offHandItem; // Equipped item
  Data handData = null;
  Data offHandData = null;
  int handAbility, offHandAbility;
  
  public EquipSlot(GameObject hand = null, GameObject offHand = null){
    this.hand = hand;
    this.offHand = hand;
    handAbility = offHandAbility = -1;
  }
  
  /* Enters a dormant state to be serialized. */
  public void Save(){
    if(handItem != null){ handData = handItem.GetData(); }
    else{ handData = null;}
    if(offHand != null){  offHandData = offHandItem.GetData(); }
    else{ offHandData = null; }
    
  }
  
  /* Return to active state after being deserialized */
  public void Load(GameObject hand = null, GameObject offHand = null){
    this.hand = hand;
    this.offHand = offHand;
  }
  

  
  /* Drop item from hand, or offHand if hand is empty. */
  public void Drop(){
    if(handItem != null){
      handItem.Drop();
      handItem = null;
    }
    else if(offHandItem != null){
      offHandItem.Drop();
      offHandItem = null;
    }
  }
  
  /* Equips an item to the desired slot, returning any items displaced. */
  public List<Data> Equip(Data dat, bool primary = true){
    List<Data> ret = new List<Data>();
    if(primary){
      if(handItem != null){
        ret.Add(Remove(true));
      }
    }
    else{
      if(offHandITem != null){
        ret.Add(Remove(true));
      }
    }
    return ret;
  }
  
  public void Use(int use){
    if(handItem != null){
      switch(use){
        case 0:
          handItem.Use(0);
          break;
        case 2:
          handItem.Use(2);
          break;
        case 3:
          handItem.Use(3);
          break;
        case 5:
          handItem.Use(4);
          break;
        case 7:
          handItem.Use(5);
          break;
      }
    }
    else if(handAbility != -1){
      switch(){
        case 0:
          UseAbility(0, true);
          break;
        case 2:
          UseAbility(2, true);
          break;
        case 3:
          UseAbility(3, true);
          break;
        case 5:
          UseAbility(4, true);
          break;
        case 7:
          UseAbility(5, true);
          break;
      }
    }
    
    if(offHandItem != null){
      switch(use){
        case 1:
          offHandItem.Use(0);
          break;
        case 2:
          offHandItem.Use(2);
          break;
        case 4:
          offHandItem.Use(3);
          break;
        case 6:
          offHandItem.Use(4);
          break;
      }
    }
    else if(offHandAbility != -1){
      switch(use){
        case 1:
          UseAbility(0, false);
          break;
        case 2:
          UseAbility(2, false);
          break;
        case 4:
          UseAbility(3, false);
          break;
        case 6:
          UseAbility(4, false);
          break;
      }
    }
  }
  
  
  /* Use an ability. Primary is false if offhand is selected. */
  public void UseAbility(int use, bool primary){
    GameObject user = primary ? hand : offHand;
    if(user == false){ return; }
    int ability = primary ? handAbility : offHandAbility;
    if(ability == -1){ return; }
    
    switch(ability){
      case 0:
        UsePunch(user, use);
        break;
      case 1:
        UseFireBall(user, use);
        break;
      case 2:
        UseHealSelf(user, use);
        break;
      case 3:
        UseHealOther(user, use);
        break;
    }
  }
  
  
  /* Removes an item and returns its data, or null. */
  public Data Remove(bool primary = true){
    if(primary && handItem != null){
      Data ret = handItem.GetData();
      Destroy(handItem.gameObject);
      handItem = null;
      return ret;
    }
    else if(!primary && offHandItem != null){
      Data ret = offHandItem.GetData();
      Destroy(handItem.gameObject);
      offHandItem = null;
      return ret;
    }
    return null;
  }
  
  /* Initializes an ability. If primary is false, the offHand will be equipped. */
  public List<Data> EquipAbility(int ability, bool primary = true){
    GameObject selectedHand = null;
    if(hand != null && primary){ selectedHand = hand; }
    else if(offHand != null && !primary){ selectedHand = offHand; }
    if(selectedHand == null){ return; }
    Item oldAbility = selectedHand.GetComponent<Item>();
    if(oldAbility){ Destroy(oldAbility); }
    
    switch(int ability){
      case 0:
        InitPunch(selectedHand);
        break;
      case 1:
        InitFireBall(selectedHand);
        break;
      case 2:
        InitHealSelf(selectedHand);
        break;
      case 3:
        InitHealOther(selectedHand);
        break;
    }
  }
  
  /* Performs unarmed attack. */
  void UsePunch(GameObject user, int use){
    Item fist = user.GetComponent<Item>();
    if(fist == null){ return; }
    fist.Use(use);
    //TODO: Consume stamina
  }
  
  /* Performs fireball spell. */
  void UseFireBall(GameObject user, int use){
    Ranged fist = user.GetComponent<Ranged>();
    if(fist == null){ return; }
    fist.ammo = 2;
    fist.Use(use);
    //TODO: Consume stamina
  }
  
  /* Performs heal self spell.*/
  void UseHealSelf(GameObject user, int use){
    Food fist = user.GetComponent<Food>();
    if(fist == null){ return; }
    fist.stack = 2;
    fist.Use(use);
    Light l = user.gameObject.GetComponent<Light>();
    if(l){StartCoroutine(Glow(0.25f, Color.blue, l)); }
    //TODO: Consume stamina
  }
  
  /* Performs heal other spell. */
  void UseHealother(GameObject user, int use){
    Ranged fist = user.GetComponent<Ranged>();
    if(fist == null){ return; }
    fist.ammo = 2;
    fist.Use(use);
    //TODO: Consume mana
  }
  
  IEnumerator Glow(float time, Color color, Light light){
    light.intensity = 2f;
    light.range = 10f;
    light.color = color;
    yield return new WaitForSeconds(time);
    light.intensity = 0f;
    light.range = 0f;
    //TODO: Consume mana
  }
  
  /* Initializes unarmed attack. */
  void InitPunch(GameObject user){
    Melee item = user.AddComponent<Melee>();
    item.cooldown = 0.5f;
    item.damageStart = 0f;
    item.damageEnd = 0.25f;
    item.knockBack = 50;
    item.chargeable = true;
    item.executeOnCharge = true;
    item.charge = 20;
    item.chargeMax = 25;
    item.damage = 20;
  }
  
  /* Initializes fireball spell. */
  void InitFireBall(GameObject user){
    Ranged item = user.AddComponent<Ranged>();
    item.cooldown = 0.5f;
    item.chargeable = true;
    item.executeOnCharge = false;
    item.projectile = "FireBall";
    item.charge = 10;
    item.chargeMax = 25;
    item.muzzleVelocity = 50;
    item.impactForce = 150;
    item.damage = 10;
    item.effectiveDamage = 10;
  }
  
  /* Initializes self healing spell. */
  void InitHealSelf(GameObject user){
    if(actor == null){ return; }
    Light l = user.GetComponent<Light>();
    if(!l){ user.AddComponent<Light>(); }
    Food item  = user.AddComponent<Food>();
    item.healing = 25;
    item.cooldown = 3f;
  }
  
  /* Initializes heal other spell. */
  void InitHealOther(GameObject user){
    Ranged item = user.AddComponent<Ranged>();
    item.cooldown = 1.5f);
    item.chargeable = true;
    item.executeOnCharge = false;
    item.projectile = "HealBall";
    item.charge = 10;
    item.chargeMax = 25;
    item.muzzleVelocity = 50;
    item.impactForce = 200;
    item.damage = -10;
    item.effectiveDamage = -10;
  }
  
}
