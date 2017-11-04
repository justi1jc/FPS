/*
    An EquipSlot manages equipping and using items with an actor and its hands.
    
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class EquipSlot{
  
  // Hands
  public const int RIGHT = 0;
  public const int LEFT = 1;
  
  [System.NonSerialized]public Actor actor = null; // Actor, if one is associated with this EquipSlot
  [System.NonSerialized]private GameObject[] hands;
  [System.NonSerialized]private Item[] items;
  [System.NonSerialized]private Data[] itemData;
  [System.NonSerialized]private Vector3 prevTrackPoint = new Vector3();
  
  private int updateDelay = 0; // Update time. 

  /* Constructor using the two hands of an Actor. */
  public EquipSlot(GameObject rightHand, GameObject leftHand, Actor actor){
    this.actor = actor;
    hands = new GameObject[2];
    items = new Item[2];
    itemData = new Data[2];
    hands[RIGHT] = rightHand;
    hands[LEFT] = leftHand;
    if(rightHand == null || leftHand == null){
      MonoBehaviour.print("One or more hands are null.");
    }
  }
  
  /* Prepares for serialization. */
  public void Save(){
    for(int i = 0; i < hands.Length; i++){
      if(items[i] == null){ itemData[i] = null; }
      else{ itemData[i] = items[i].GetData(); }
    }
  }
  
  /* Returns the item in this hand. */
  public Item Peek(int hand){
    if(hand < 0 || hand >= hands.Length){ return null; }
    return items[hand];
  }
  
  /* Retuns from serialization when attached to an actor. */
  public void Load(GameObject rightHand, GameObject leftHand, Actor actor){
    hands[RIGHT] = rightHand;
    hands[LEFT] = leftHand;
    for(int i = 0; i < hands.Length; i++){
      if(itemData[i] == null){ items[i] = null; }
      else{ items[i] = Item.GetItem(itemData[i]); } 
    }
  }
  
  /* Returns the index of an item's hand, or -1 if it is not currently held. */
  private int FindHand(Item item){
    if(item == null){ return -1; }
    for(int i = 0; i < items.Length; i++){
      if(item == items[i]){ return i; }
    }
    return -1;
  }
  
  /* Returns true if this item can currently be dual equipped.
     An item can only be dual equipped if it is one handed and there is a
     one handed item in the right hand.
  */
  public bool DualEquipAvailable(Data dat){
    if(items[RIGHT] == null || !items[RIGHT].oneHanded || !Item.OneHanded(dat)){
      return false;
    }
    return true;
  }
  
  
  /*
    Equips an ability for use with right hand.
  */
  public void EquipAbility(Data ability){
    if(ability == null){ return; }
    Item item = Item.GetItem(ability);
    items[RIGHT] = item;
    Mount(items[RIGHT], hands[RIGHT]);
    
    actor.SetAnimBool("rightEquip", true);
    if(!item.oneHanded){ 
      if(items[LEFT] != null && items[LEFT] is Ability){ 
        MonoBehaviour.Destroy(items[LEFT]);
        items[LEFT] = null;
      }
      else if(items[LEFT] != null){ Store(LEFT); }
      actor.SetAnimBool("twoHanded", true); 
      actor.SetAnimBool("leftEquip", false);
    }
    else{ actor.SetAnimBool("twoHanded", false); }
  }
  
  /* Equip an item to right hand, storing any displaced item into actor's 
     inventory. 
  */
  public void EquipFromInventory(int index){
    if(actor == null || actor.inventory == null){ return; }
    Data dat = actor.inventory.Peek(index);
    if(dat == null){
      MonoBehaviour.print("Dat at index " + index + "is null.");
      return;
    }
    actor.SetAnimBool("leftEquip", true);
    actor.SetAnimBool("rightEquip", true);
    actor.SetAnimBool("twoHanded", true);
    if(!Item.OneHanded(dat)){ StoreAll(); }
    else{ Store(RIGHT); }
    items[RIGHT] = Item.GetItem(dat);
    if(items[RIGHT] == null){
      MonoBehaviour.print("Dat null:" + dat.prefabName);
      return;
    }
    Mount(items[RIGHT], hands[RIGHT]);
    actor.inventory.SetStatus(index, GetStatus(RIGHT));
  }
  
  /* Equip an item to left hand if possible, storing displaced item */
  public void DualEquipFromInventory(int index){
    if(actor == null || actor.inventory == null){ return; }
    Data dat = actor.inventory.Peek(index);
    if(dat == null){
      MonoBehaviour.print("Dat at index " + index + "is null.");
      return;
    }
    if(!DualEquipAvailable(dat)){ return; }
    actor.SetAnimBool("leftEquip", true);
    actor.SetAnimBool("rightEquip", true);
    actor.SetAnimBool("twoHanded", false);
    Store(LEFT);
    items[LEFT] = Item.GetItem(dat);
    if(items[LEFT] == null){
      MonoBehaviour.print("Dat null:" + dat.prefabName);
      return;
    }
    Mount(items[LEFT], hands[LEFT]);
    actor.inventory.SetStatus(index, GetStatus(LEFT));
  }
  
  /* Stores all items into Actor's inventory. */
  public void StoreAll(){
    for(int i = 0; i < hands.Length; i++){ Store(i); }
  }
  
  /* Stores item from a particular hand into Actor's inventory. */
  public void Store(int hand){
    if(hand < 0 || hand > hands.Length || items[hand] == null){ return; }
    if(items[hand] is Ability){ return; }
    int status = GetStatus(hand);
    Data dat = items[hand].GetData();
    if(status == -1 || actor == null || actor.inventory == null || dat == null){
      return;
    }
    actor.inventory.StoreEquipped(dat, status);
    MonoBehaviour.Destroy(items[hand].gameObject);
    items[hand] = null;
  }
  
  /* Returns corresponding hand's equip status according to Inventory. */
  private int GetStatus(int hand){
    if(hand == RIGHT){ return Inventory.PRIMARY; }
    if(hand == LEFT){ return Inventory.SECONDARY; }
    return -1;
  }
  
  /* Drop particular item from hand */
  public void Drop(Item item){
    int hand = FindHand(item);
    if(hand == -1){ return; }
    if(actor != null && !(item is Ability)){
      if(hand == RIGHT){ actor.inventory.ClearEquipped(Inventory.PRIMARY); }
      if(hand == LEFT){ actor.inventory.ClearEquipped(Inventory.SECONDARY); }
    }
    items[hand].Drop();
    items[hand] = null;
  }
  
  /* Drops a single item if possible. Removes from hand with greatest index
    first.
  */
  public void Drop(){
    if(hands.Length == 0){ return; }
    for(int i = hands.Length-1; i >= 0; i--){
      if(items[i] != null){
        items[i].Drop();
        items[i] = null;
        if(actor != null){
          if(i == RIGHT){ actor.inventory.ClearEquipped(Inventory.PRIMARY); }
          if(i == LEFT){ actor.inventory.ClearEquipped(Inventory.SECONDARY); }
        }
        return;
      }
    }
  }
  
  /* Called from late update */
  public void Update(){
    int delayMax = 2;
    updateDelay++;
    if(updateDelay > delayMax){
      updateDelay = 0;
      Vector3 point = TrackPoint();
      for(int i = 0; i < hands.Length; i++){
        if(items[i] != null && items[i] is Ranged){
          Track(items[i].gameObject, point);
        }
      }
    }
  }
  
  /* Returns true if this gameObject belongs to an equipped item. */
  private bool GameObjectMatchesItem(GameObject obj){
    for(int i = 0; i < hands.Length; i++){
      if(items[i] != null && items[i].gameObject == obj){ return true; }
    }
    return false;
  }
  
  /* Returns the current point ranged weapons should aim at. */
  public Vector3 TrackPoint(){
    Vector3 offset = new Vector3();
    
    GameObject vision = actor.cam != null ? actor.cam.gameObject : actor.head;
    Vector3 pos = vision.transform.position;
    Vector3 dir = vision.transform.forward;
    RaycastHit hit; 
    if(Physics.Raycast(pos, dir, out hit) && hit.distance > 1f){ 
      GameObject go = hit.collider.gameObject;
      if(GameObjectMatchesItem(go)){ return prevTrackPoint; }
      offset *= Vector3.Distance(pos, hit.point);
      prevTrackPoint = offset + hit.point;
      return offset + hit.point;
    }
    else{
      offset *= 100;
      prevTrackPoint = pos + 100f*vision.transform.forward;
      return offset + pos + 100f*vision.transform.forward; 
    }
  }
  
  /* Aims the selected ranged weapon at the given track point. */
  public void Track(GameObject weapon, Vector3 trackPoint){
    Transform t = weapon.transform;
    t.rotation = Quaternion.LookRotation(trackPoint - t.position);
  }
  
  /* Returns true if both hands are used on single item. */
  public bool Single(){
    if(RIGHT < hands.Length && items[RIGHT] != null){
      if(LEFT > hands.Length || items[LEFT] == null){ return true; }
    }
    return false;
  }
  
  /* Returns all items equipped. */
  public List<Item> AllItems(){
    List<Item> ret = new List<Item>();
    for(int i = 0; i < items.Length; i++){
      if(items[i] != null){
        ret.Add(items[i]);
      }
    }
    return ret;
  }
  
  /* Returns true if no items are currently equipped. */
  public bool Empty(){
    for(int i = 0; i < hands.Length; i++){
      if(items[i] != null && !(items[i] is Unarmed)){ return false; }
    }
    return true;
  }
  
  /* Connects an item to a particular hand */
  public void Mount(Item item, GameObject hand){
    item.transform.parent = MountPoint(hand.transform);
    item.Hold(actor);
  }
  
  /* Returns mount point from hand. */
  private Transform MountPoint(Transform selectedHand){
    foreach(Transform t in selectedHand){ if(t.gameObject.name == "MountPoint"){ return t; } }
    return null;
  }
  
  /* Triggers melee animations when appropriate. */
  public void MeleeAnim(int use){
    if(actor == null || (use != Item.A_DOWN && use != Item.B_DOWN)){ return; }
    Melee melee = null;
    bool r = false;
    if(Single() && use == Item.A_DOWN){ 
      if(items[RIGHT] != null && items[RIGHT] is Melee){
        melee = (Melee)items[RIGHT];
      }
      r = true;
    }
    else{
      if(use == Item.A_DOWN){ 
        r = false;
        if(items[LEFT] != null && items[LEFT] is Melee){ 
          melee = (Melee)items[LEFT];
        }
      }
      else{ 
        r = true;
        if(items[RIGHT] != null && items[RIGHT] is Melee){
          melee = (Melee)items[RIGHT];
        }
      }
    }
    if(melee == null){ return; }
    string hand = r ? "right" : "left";
    string action = melee.stab ? "Stab" : "Swing";
    actor.SetAnimTrigger(hand + action);
  }
  
  
  /* Use one of the equipped items. */
  public void Use(int use){
    MeleeAnim(use);
    if(Single()){
      if(items[RIGHT] == null){ return; }
      items[RIGHT].Use(use);
    }
    else{
      switch(use){
        case Item.A_DOWN: 
          if(items[LEFT] != null){ items[LEFT].Use(Item.A_DOWN); }
          break;
        case Item.B_DOWN: 
          if(items[RIGHT] != null){ items[RIGHT].Use(Item.A_DOWN); } 
          break;
        case Item.A_UP: 
          if(items[LEFT] != null){ items[LEFT].Use(Item.A_UP); } 
          break;
        case Item.B_UP: 
          if(items[RIGHT] != null){ items[RIGHT].Use(Item.A_UP); } 
          break;
      }
    }
  }

  /* Removes an item from its specified hand and returns its data, or null. */
  public Data Remove(int hand){
    if(hand < 0 || hand > hands.Length || items[hand] == null){ return null; }
    Data ret = items[hand].GetData();
    MonoBehaviour.Destroy(items[hand].gameObject);
    items[hand] = null;
    return ret;
  }
  
  /* Returns info on the selected items or abilities. */
  public string GetInfo(){
    string ret = "";
    for(int i = 0; i < hands.Length; i++){
      if(items[i] != null){ ret += items[i].GetInfo(); }
    }
    return ret;
  }
}
