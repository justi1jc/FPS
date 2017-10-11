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
  
  /* Equip an item to right hand, storing any displaced item into actor's 
     inventory. 
  */
  public void EquipFromInventory(int index){
    if(actor == null || actor.inventory == null){ return; }
    Data dat = actor.inventory.Peek(index);
    if(dat == null){ return; }
    if(!Item.OneHanded(dat)){ StoreAll(); }
    else{ Store(RIGHT); }
    items[RIGHT] = Item.GetItem(dat);
    Mount(items[RIGHT], hands[RIGHT]);
    actor.inventory.SetStatus(index, GetStatus(RIGHT));
  }
  
  /* Equip an item to left hand if possible, storing displaced item */
  public void DualEquipFromInventory(int index){
    if(actor == null || actor.inventory == null){ return; }
    Data dat = actor.inventory.Peek(index);
    if(dat == null || !DualEquipAvailable(dat)){ return; }
    Store(LEFT);
    items[LEFT] = Item.GetItem(dat);
    Mount(items[LEFT], hands[LEFT]);
    actor.inventory.SetStatus(index, GetStatus(LEFT));
  }
  
  /* Stores all items into Actor's inventory. */
  public void StoreAll(){
    for(int i = 0; i < hands.Length; i++){ Store(i); }
  }
  
  /* Stores item from a particular hand into Actor's inventory. */
  private void Store(int hand){
    if(hand < 0 || hand > hands.Length || items[hand] == null){ return; }
    int status = GetStatus(hand);
    Data dat = items[hand].GetData();
    if(status == -1 || actor == null || actor.inventory == null || dat == null){
      return;
    }
    actor.inventory.StoreEquipped(dat, status);
    MonoBehaviour.Destroy(items[hand]);
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
    items[hand].Drop();
    items[hand] = null;
  }
  
  /* Drops a single item if possible. Removes from hand with greatest index
    first.
  */
  public void Drop(){
    if(hands.Length == 0){ return; }
    for(int i = hands.Length-1; i > 0; i--){
      if(items[i] != null){
        items[i].Drop();
        items[i] = null;
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
  
  /* Get offset for trackpoint. */
  public Vector3 TrackPointOffset(){
    float offset = actor.stats.AccuracyOffset();
    float x = Random.Range(-offset, offset);
    float y = Random.Range(-offset, offset);
    float z = Random.Range(-offset, offset);
    return new Vector3(x, y, z);
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
    if(LEFT > hands.Length || items[LEFT] == null){ return true; }
    return false;
  }
  
  /* Returns true if no items are currently equipped. */
  public bool Empty(){
    for(int i = 0; i < hands.Length; i++){
      if(items[i] != null){ return false; }
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
  
  public void Use(int use){
  }

  /* Removes an item from its specified hand and returns its data, or null. */
  public Data Remove(int hand){
    if(hand < 0 || hand > hands.Length || items[hand] == null){ return null; }
    Data ret = items[hand].GetData();
    MonoBehaviour.Destroy(items[hand]);
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
  
  
  /* Initializes unarmed attack. */
  void InitPunch(GameObject user){
    Melee item = user.AddComponent<Melee>();
    item.ability = true;
    item.cooldown = 0.25f;
    item.damageStart = 0.25f;
    item.damageEnd = 0.75f;
    item.knockBack = 50;
    item.ready = true;
    item.damage = 20;
    item.holder = actor;
  }

  /* Performs unarmed attack. */
  public void UsePunch(GameObject user, int use, bool primary){
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
  }
  
}
