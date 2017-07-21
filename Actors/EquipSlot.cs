/*
    An EquipSlot is a component that uses up to two gameObjects as mount points
    as anchor points for held items and manages the equipment and use of abilities.
    
    The current mapping is as follows
    dual wield: primary = left(hand), secondary = right(offHand)
    single wield: primary = right(offHand)
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class EquipSlot{
  [System.NonSerialized]public Actor actor = null; // Actor, if one is associated with this EquipSlot
  [System.NonSerialized]public GameObject hand, offHand;
  [System.NonSerialized]public Item handItem, offHandItem; // Equipped item
  [System.NonSerialized]private Vector3 prevTrackPoint = new Vector3();
  Data handData = null;
  Data offHandData = null;
  public int handAbility = -1;
  public int offHandAbility = -1;
  
  public EquipSlot(GameObject hand = null, GameObject offHand = null, Actor actor = null){
    this.hand = hand;
    this.offHand = offHand;
    this.actor = actor;
    if(actor){
      EquipAbility(0, true);
      EquipAbility(0, false);
    }
  }
  
  /* Enters a dormant state to be serialized. */
  public void Save(){
    if(handItem != null){ handData = handItem.GetData(); }
    else{ handData = null;}
    if(offHandItem != null){  offHandData = offHandItem.GetData(); }
    else{ offHandData = null; }
    
  }
  
  /* Return to active state after being deserialized */
  public void Load(Actor actor = null, GameObject hand = null, GameObject offHand = null){
    this.actor = actor;
    this.hand = hand;
    this.offHand = offHand;
    if(handData != null){
      Equip(handData, true);
      handData = null;
    }
    else if(handAbility != -1){ EquipAbility(handAbility, true); }
    if(offHandData != null){
      Equip(offHandData, false);
      offHandData = null;
    }
    else if(offHandAbility != -1){ EquipAbility(offHandAbility, true); }
  }
  
  /* Drop item from hand, or offHand if hand is empty. */
  public void Drop(){
    if(handItem != null){
      handItem.Drop();
      handItem = null;
      EquipAbility(0, true);
      if(offHandItem != null || offHandAbility > 0){ 
        actor.SetAnimBool("twoHanded", true);
      }
      else{ actor.SetAnimBool("twoHanded", false); }
    }
    else if(offHandItem != null){
      offHandItem.Drop();
      offHandItem = null;
      EquipAbility(0, false);
      actor.SetAnimBool("twoHanded", false);
    }
  }
  
  
  /* Called from late update */
  public void Update(){
    bool lr = handItem != null && handItem is Ranged; 
    bool rr = offHand != null && handItem is Ranged;
    if(lr || rr){
      MonoBehaviour.print(lr + "," + rr);
      Vector3 point = TrackPoint();
      if(handItem != null && lr){ Track(handItem.gameObject, point); }
      if(offHandItem != null && rr){ Track(offHandItem.gameObject, point); }
    }
  }
  
  /* Returns the current point ranged weapons should aim at. */
  public Vector3 TrackPoint(){
    GameObject vision = actor.cam != null ? actor.cam.gameObject : actor.head;
    Vector3 pos = vision.transform.position;
    Vector3 dir = vision.transform.forward;
    RaycastHit hit; 
    if(Physics.Raycast(pos, dir, out hit) && hit.distance > 1f){ 
      GameObject go = hit.collider.gameObject;
      if((handItem != null && handItem.gameObject == go)
        || (offHandItem != null && offHandItem.gameObject == go)
      ){
        return prevTrackPoint; 
      }
      prevTrackPoint = hit.point;
      return hit.point;
    }
    else{
      prevTrackPoint = pos + 100f*vision.transform.forward;
      return pos + 100f*vision.transform.forward; 
    }
  }
  
  /* Aims the selected ranged weapon at the given track point. */
  public void Track(GameObject weapon, Vector3 trackPoint){
    Transform t = weapon.transform;
    t.rotation = Quaternion.LookRotation(trackPoint - t.position);
  }
  
  /* Equips an item to the desired slot, returning any items displaced. */
  public List<Data> Equip(Data dat, bool primary = true){
    if(actor != null){
      if(primary){ actor.SetAnimBool("leftEquip", true); }
      else{ actor.SetAnimBool("rightEquip", true); }
    }
    List<Data> ret = new List<Data>();
    Item item = GetItem(dat);
    if(item == null){ return ret; }
    GameObject itemGO = item.gameObject;
    
    Transform selectedHand = primary ? hand.transform : offHand.transform;
    if(!item.oneHanded){ selectedHand = offHand.transform; }
    item.transform.parent = MountPoint(selectedHand);
    item.Hold(actor);
    if(!item.oneHanded){
      ret.Add(Remove(true));
      ret.Add(Remove(false));
      handAbility = 0;
      offHandAbility = 0;
      offHandItem = item;
      if(actor != null){ actor.SetAnimBool("twoHanded", true); }
    }
    else if(primary){
      if(handItem != null){
        ret.Add(Remove(true));
        handAbility = 0;
      }
      if(offHandItem == null && offHandAbility < 1){
        GameObject.Destroy(item.gameObject);
        ret.AddRange(Equip(dat, false));
        return ret;
      }
      else{
        if(actor != null){ actor.SetAnimBool("twoHanded", false); }
        handItem = item;
      }
    }
    else{
      if(offHandItem != null){ 
        ret.Add(Remove(false)); 
        offHandAbility = 0;
      }
      offHandItem = item;
      if(actor != null){ 
        if(handItem == null && handAbility < 1){ 
          actor.SetAnimBool("twoHanded", true); 
        }
        else{
          actor.SetAnimBool("twoHanded", false);
        }
      }
    }
    
    return ret;
  }
  
  /* Returns mount point from hand. */
  Transform MountPoint(Transform selectedHand){
    foreach(Transform t in selectedHand){ if(t.gameObject.name == "MountPoint"){ return t; } }
    return null;
  }
  
  /* Returns an Item from data */
  Item GetItem(Data dat){
    if(dat == null){ MonoBehaviour.print("Equipped null data"); return null; }
    GameObject prefab = Resources.Load("Prefabs/" + dat.prefabName) as GameObject;
    if(!prefab){ MonoBehaviour.print("Prefab null:" + dat.displayName); return null;}
    Vector3 position = actor != null ? actor.transform.position : new Vector3();
    GameObject itemGO = (GameObject)GameObject.Instantiate(
      prefab,
      position,
      Quaternion.identity
    );
    if(!itemGO){MonoBehaviour.print("GameObject null:" + dat.displayName); return null; }
    Item item = itemGO.GetComponent<Item>();
    item.LoadData(dat);
    return item;
  }
  
  public void Use(int use){
    Melee melee = null;
    if(use == 2){
      if(handItem != null){ handItem.Use(2); }
      if(offHandItem != null){ offHandItem.Use(2); }
    }
    
    if(offHandItem != null && (!offHandItem.oneHanded || handItem == null && handAbility < 1)){
      switch(use){
        case 0:
          melee = offHandItem.gameObject.GetComponent<Melee>();
          if(melee != null){
            string trigger = melee.stab ? "rightStab" : "rightSwing";
            if(actor && melee.ready){ actor.SetAnimTrigger(trigger); }
          }
          offHandItem.Use(0);
          break;
        case 3: offHandItem.Use(3); break;
        case 5: offHandItem.Use(4); break;
        case 7: offHandItem.Use(5); break;
      }
      return;
    }
    
    if(handItem != null){
      switch(use){
        case 0:
          melee = handItem.gameObject.GetComponent<Melee>();
          if(melee != null){
            string trigger = melee.stab ? "leftStab" : "leftSwing";
            if(actor && melee.ready){ actor.SetAnimTrigger(trigger); }
          }
          handItem.Use(0);
          break;
        case 3: handItem.Use(3); break;
        case 5: handItem.Use(4); break;
        case 7: handItem.Use(5); break;
      }
    }
    else if(handAbility != -1){
      switch(use){
        case 0: UseAbility(0, true); break;
        case 3: UseAbility(3, true); break;
        case 5: UseAbility(4, true); break;
        case 7: UseAbility(5, true); break;
      }
    }
    
    if(offHandItem != null){
      switch(use){
        case 1:
          melee = offHandItem.gameObject.GetComponent<Melee>();
          if(melee != null){
            string trigger = melee.stab ? "rightStab" : "rightSwing";
            if(actor && melee.ready){ actor.SetAnimTrigger(trigger); }
          }
          offHandItem.Use(0);
          break;
        case 4: offHandItem.Use(3); break;
        case 6: offHandItem.Use(4); break;
      }
    }
    else if(offHandAbility != -1){
      switch(use){
        case 1: UseAbility(0, false); break;
        case 4: UseAbility(3, false); break;
        case 6: UseAbility(4, false); break;
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
      case 0: UsePunch(user, use, primary); break;
      case 1: UseFireBall(user, use); break;
      case 2: UseHealSelf(user, use); break;
      case 3: UseHealOther(user, use); break;
    }
  }
  
  
  /* Removes an item and returns its data, or null. */
  public Data Remove(bool primary = true){
    if(primary && handItem != null){
      Data ret = handItem.GetData();
      MonoBehaviour.Destroy(handItem.gameObject);
      handItem = null;
      return ret;
    }
    else if(!primary && offHandItem != null){
      Data ret = offHandItem.GetData();
      MonoBehaviour.Destroy(offHandItem.gameObject);
      offHandItem = null;
      return ret;
    }
    return null;
  }
  
  /* Initializes an ability. If primary is false, the offHand will be equipped.
     Returns a displaced item or null.
   */
  public List<Data> EquipAbility(int ability, bool primary = true){
    if(actor != null){
      if(primary){ actor.SetAnimBool("leftEquip", true); }
      else{ actor.SetAnimBool("rightEquip", true); }
    }
    List<Data> ret = new List<Data>();
    if(ability < 0 || ability > 4){ return ret; }
    GameObject selectedHand = null;
    Item displacedItem = null;
    if(hand != null && primary){
      selectedHand = hand;
      displacedItem = handItem; 
    }
    else if(offHand != null && !primary){
      selectedHand = offHand;
      displacedItem = offHandItem;
    }
    if(selectedHand == null){ MonoBehaviour.print("selected hand is null."); return ret; }
    if(displacedItem != null){
      ret.Add(displacedItem.GetData());
      MonoBehaviour.Destroy(displacedItem.gameObject);
    }
    Item oldAbility = selectedHand.GetComponent<Item>();
    if(oldAbility){ MonoBehaviour.Destroy(oldAbility); }
    if(primary){ handAbility = ability; }
    else{ offHandAbility = ability; }
    switch(ability){
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
    return ret;
  }
  
  /* Performs unarmed attack. */
  void UsePunch(GameObject user, int use, bool primary){
    Item fist = user.GetComponent<Melee>();
    if(fist == null){ return; }
    if(actor != null){
      string trigger = primary ? "leftPunch" : "rightPunch";
      if(fist.ready){
        actor.SetAnimTrigger(trigger);
        fist.Use(0);
      }
    }
    else{ MonoBehaviour.print("Actor missing"); }
    //TODO: Consume stamina
  }
  
  /* Returns info on the selected items or abilities. */
  public string Info(){
    string ret = "";
    if(handItem != null){
      ret += handItem.GetInfo();
    }
    else if(handAbility != -1){
      ret += AbilityInfo(handAbility);
    }
    ret += "\n";
    if(offHandItem != null){
      ret += offHandItem.GetInfo();
    }
    else if(offHandAbility != -1){
      ret += AbilityInfo(offHandAbility);
    }
    return ret;
  }
  
  /* Returns the name of the ability. */
  public string AbilityInfo(int ability){
    string ret = "";
    switch(ability){
        case 0:
          ret += "Unarmed";
          break;
        case 1:
          ret += "Fireball";
          break;
        case 2:
          ret += "Heal Self";
          break;
        case 3:
          ret += "Heal other";
          break;
      }
      return ret;
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
    if(l && use == 0){
      l.intensity = 2f;
      l.range = 10f;
      l.color = Color.blue;
      
    }
    else if(l && use == 4){
      l.intensity = 0f;
      l.range = 0f;
    }
    
  }
  
  /* Performs heal other spell. */
  void UseHealOther(GameObject user, int use){
    Ranged fist = user.GetComponent<Ranged>();
    if(fist == null){ return; }
    fist.ammo = 2;
    fist.Use(use);
    //TODO: Consume mana
  }
  
  /* Initializes unarmed attack. */
  void InitPunch(GameObject user){
    Melee item = user.AddComponent<Melee>();
    item.cooldown = 0.25f;
    item.damageStart = 0.25f;
    item.damageEnd = 0.75f;
    item.knockBack = 50;
    item.ready = true;
    item.damage = 20;
    item.holder = actor;
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
    item.muzzleVelocity = 100;
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
    item.cooldown = 1.5f;
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
