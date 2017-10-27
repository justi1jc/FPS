/*
    Item serves as the base class for specic types of items.
*/


using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour{
  // Constants to reference class within Data
  public const int ITEM = 0;
  public const int FOOD = 1;
  public const int DECOR = 2;
  public const int CONTAINER = 3;
  public const int SPAWNER = 4;
  public const int WARPDOOR = 5;
  public const int WEAPON = 6;
  public const int MELEE = 7;
  public const int RANGED = 8;
  public const int PROJECTILE = 9;
  public const int EQUIPMENT = 10;
  public const int ABILITY = 11;
  
  // Input constants.
  public const int A_DOWN = 0;
  public const int B_DOWN = 1;
  public const int C_DOWN = 2;
  public const int D_DOWN = 3;
  public const int E_DOWN = 4;
  public const int F_DOWN = 5;
  public const int G_DOWN = 6;
  public const int A_UP = 7;
  public const int B_UP = 8;
  public const int C_UP = 9;
  public const int D_UP = 10;
  public const int E_UP = 11;
  public const int F_UP = 12;
  public const int G_UP = 13;

  // Base variables
  public string prefabName;
  public Vector3 heldPos;
  public Vector3 heldRot;
  public string displayName;
  public string itemDesc;
  public int stack;
  public int stackSize;
  public int baseValue;
  public AudioClip[] sounds;
  public int weight;
  public Actor holder;
  public bool ready = true;
  public bool oneHanded = true; // True if this item can be held in one hand.
  public bool held = false;
  public float cooldown; // Time between uses.
  
  // Common melee variables.
  public int damage;     // Damage this weapon does.
  public float damageStart; // Begin of effective swing
  public float damageEnd;   // End of effective swing
  public bool damageActive; // True if damage can be given
  public float knockBack;   // Magnitude of force exerted on target
  
  // Empty base methods
  public virtual void Use(int action){}
  public virtual string GetInfo(){ return ""; }
  
  // Implemented base methods
  /* Bonds an item to an actor */
  public virtual void Hold(Actor a){
    if(held){ return; }
    held = true;
    holder = a;
    
    Rigidbody rb = transform.GetComponent<Rigidbody>();
    if(rb != null){
      rb.isKinematic = true;
      rb.useGravity = false;
    }
    transform.localPosition = heldPos;
    Quaternion lr = Quaternion.Euler(heldRot.x, heldRot.y, heldRot.z);
    transform.localRotation = lr;
    Collider c = transform.GetComponent<Collider>();
    c.isTrigger = true;
  }
  
  /* Drop item from actor's hand. */
  public virtual void Drop(){
    held = false;
    Rigidbody rb = transform.GetComponent<Rigidbody>();
    if(rb){
      rb.isKinematic = false;
      rb.useGravity = true;
      rb.constraints = RigidbodyConstraints.None;
    }
    Collider c = transform.GetComponent<Collider>();
    c.isTrigger = false;
    transform.parent = null;
    holder = null;
  }
  
  /* Play a sound, if possible. */
  public void Sound(int i){
    if(i < 0 || sounds == null || i >= sounds.Length || sounds[i] == null){ 
      //MonoBehaviour.print("Invalid sound:" + i); 
      return; 
    }
    float vol = PlayerPrefs.HasKey("masterVolume") ? PlayerPrefs.GetFloat("masterVolume") : 1f; 
    AudioSource.PlayClipAtPoint(sounds[i], transform.position, vol);
  }
  
  /* Pick up item. */
  public virtual void Interact(Actor a, int mode = -1, string message = ""){
    a.PickUp(this);
  }
  
  public virtual Data GetData(){ return GetBaseData(); }
  public virtual void LoadData(Data dat){ LoadBaseData(dat); }
  
  /* Returns a Data containing the base variables. */
  public Data GetBaseData(){
    Data dat = new Data();
    dat.x = transform.position.x;
    dat.y = transform.position.y;
    dat.z = transform.position.z;
    Vector3 rot = transform.rotation.eulerAngles;
    dat.xr = rot.x;
    dat.yr = rot.y;
    dat.zr = rot.z;
    dat.prefabName = prefabName;
    dat.displayName = displayName;
    dat.stack = stack;
    dat.stackSize = stackSize;
    dat.ints.Add(weight);
    dat.baseValue = baseValue;
    dat.itemType = Item.ITEM;
    dat.bools.Add(oneHanded);
    return dat;
  }
  
  /* Returns true if an item stored in Data is a weapon. */
  public static bool IsWeapon(Data dat){
    if(dat == null){ return false; }
    if(dat.itemType == MELEE || dat.itemType == RANGED || dat.itemType == WEAPON){
      return true;
    }
    return false;
  }
  
  /* Returns true if an item stored in a Data is one-handed. */
  public static bool OneHanded(Data dat){
    if(dat == null || dat.bools.Count < 1){ return true; }
    return dat.bools[0];
  }
  
  /* Fills the stack of a given dat. */
  public static void FullStack(ref Data dat){
    dat.stack = dat.stackSize;
  }
  
  /* Receiving damage from external sources. */
  public virtual void ReceiveDamage(Damage dam){}
  
  /* Loads the base variables from a Data */
  public void LoadBaseData(Data dat){
    transform.position = new Vector3(dat.x, dat.y, dat.z);
    transform.rotation = Quaternion.Euler(dat.xr, dat.yr, dat.zr);
    stack = dat.stack;
    weight = dat.ints[0];
  }
  
  /* Sets item to ready after cooldown duration. */
  public IEnumerator CoolDown(float duration){
    ready = false;
    yield return new WaitForSeconds(duration);
    ready = true;
  }
  
  /* Returns an item's data based on its prefab name. */
  /* Factory that returns the data of an Item. */
  public static Data GetItem(string prefab, int quantity = 1){
    GameObject pref = (GameObject)Resources.Load("Prefabs/" + prefab, typeof(GameObject));
    if(pref == null){
      MonoBehaviour.print("Prefab null " + prefab);
      return null;
    }
    GameObject go = (GameObject)GameObject.Instantiate(
      pref,
      new Vector3(),
      Quaternion.identity
    );
    if(go == null){
      MonoBehaviour.print("Game object null " + prefab);
      return null;
    }
    Item item = go.GetComponent<Item>();
    if(item == null){ 
      MonoBehaviour.print("Item not found " + prefab);
      return null; 
    }
    Data dat = item.GetData();
    dat.stack = quantity;
    GameObject.Destroy(go);
    return dat;
  }
  
  /* Returns an Item from data */
  public static Item GetItem(Data dat){
    if(dat == null){ MonoBehaviour.print("Equipped null data"); return null; }
    GameObject prefab = Resources.Load("Prefabs/" + dat.prefabName) as GameObject;
    if(!prefab){ MonoBehaviour.print("Prefab null:" + dat.displayName); return null;}
    Vector3 position = new Vector3();
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
  
  /* Exert force and damage onto target. */
  protected void Strike(Collider col, int dmg = 0){
    if(col.gameObject == null){ MonoBehaviour.print("Gameobject missing"); return; }
    if(damageActive){
      if(holder != null){
        if(holder.GetRoot(col.gameObject.transform) == holder.transform.transform){
          return;
        }
      }
      HitBox hb = col.gameObject.GetComponent<HitBox>();
      if(hb){
        StartCoroutine(CoolDown(cooldown));
        if(dmg == 0){ dmg = damage; }
        Damage dam = new Damage(dmg, gameObject);
        hb.ReceiveDamage(dam);
        damageActive = false;
        if(hb.body != null){
          Rigidbody hbrb = hb.body.gameObject.GetComponent<Rigidbody>();
          Vector3 forward = transform.position - hb.body.transform.position;
          forward = new Vector3(forward.x, 0f, forward.y);
          if(hbrb){ hbrb.AddForce(forward * knockBack); }
        }
      }
      Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
      if(rb){ rb.AddForce(transform.forward * knockBack); Sound(0); }
      Item item = col.gameObject.GetComponent<Item>();
      if(item != null && !(item is Ability) && item.holder!= null){
        item.holder.Drop(item);
      }
    }
  }
  
  /* Returns all items as data found in Resources/items.txt */
  public static List<Data> GetKitItems(){
    List<Data> ret = new List<Data>();
    List<string> itemNames = Item.ParseItemsFile();
    foreach(string itemName in itemNames){
      Data dat = GetItem(itemName);
      if(dat == null){ MonoBehaviour.print(itemName + " was null."); }
      else{ 
        Item.FullStack(ref dat);
        ret.Add(dat);
      }
    }
    return ret;
  }
  
  /* Returns information about this item's uses for comparison purposes.
     Specifying a row allows for multiple pieces of information to be gathered
     when populating a table.
  */
  public virtual string GetUseInfo(int row = 0){ return ""; }
  
  /* Returns all the lines of the Resources/items.txt file. */
  private static List<string> ParseItemsFile(){
    List<string> ret = new List<string>();
    try{
      string path = Application.dataPath + "/Resources/items.txt";
      if(!File.Exists(path)){
        MonoBehaviour.print("Kits file does not exist at " + path); 
        return ret;
      }
      using(StreamReader sr = new StreamReader(path)){
        string line = sr.ReadLine();
        while(line != null){
          ret.Add(line);
          line = sr.ReadLine();
        }
      }
    }catch(Exception e){ MonoBehaviour.print("Exception:" + e); }
    return ret;
  }
}
