using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour{
/*
*
*   GameObject structure
*   
*   General item
*   Item|(ItemController)
*       |Model(rigidbody, collider, meshrenderer)
*   
*   Ranged Weapon
*   Item|(ItemController)
*       |Model|(rigidbody, collider, meshrenderer)
*             |MuzzlePoint//Forward position of barrel
*             |RearPoint  //Rear position of barrel
*/


  // Item Types
  const int SCENERY   = 0; // Can't be picked up.
  const int MISC      = 1; // No inherent use
  const int FOOD      = 2; // Restore health
  const int MELEE     = 3; // Melee weapon
  const int RANGED    = 4; // Ranged weapon
  const int WARP      = 5; // Warps player to new area.
  const int CONTAINER = 6; // Access contents, but not pick up.
  const int PROJECTILE= 7; // Flies forward when used.
  
  // General item variables
  public string prefabName;
  public Vector3 heldPos;
  public Vector3 heldRot;
  public string displayname;
  public string itemDesc;
  public int stack;
  public int stackSize;
  public int itemType;
  public AudioClip[] sounds;
  public int weight;
  public Actor holder;
  
  // Food variables.
  public int healing;
  
  // Weapon variables
  public int damage;
  public float cooldown;
  public bool ready;
  
  //Melee variables
  public float damageStart;
  public float damageEnd;
  public bool damageActive;
  public string swingString;
  public int swingHash;
  
  //Types of ranged weapons
  const int RIFLE  = 0;
  const int PISTOL = 1;
  const int BOW    = 2;
  
  //Ranged weapon variables 
  public int ammo;
  public int maxAmmo;
  public string ammoName;
  public string projectile;
  public bool aimed;
  public int rangedType;
  
  //WARP variables
  public string destName;
  public Vector3 destPos;
  public Vector3 destRot;
  
  //Container variables
  public Data[] contents;
  
  public void Start(){
    switch(itemType){
      case MELEE:
        swingHash = Animator.StringToHash(swingString);
        break;

      default:
        break;
    }
  }
  
  public void Use(int action){
    //Left mouse
    if(action == 0){
      switch(itemType){
        case FOOD:
          Consume();
          break;
        case MELEE:
          StartCoroutine(Swing());
          break;
        case RANGED:
          Fire();
          break;
        case WARP:
          Warp();
          break;
        case PROJECTILE:
          // Launch
          break;
        default:
          break;
      }
    }
    //Right mouse
    else if (action == 1){
      switch(itemType){
        case RANGED:
          ToggleAim();
          break;
        default:
          break;
      }
    }
    //R key
    else if (action == 2){
      switch(itemType){
        case RANGED:
          StartCoroutine(Reload());
          break;
        default:
          break;
      }
    }
  }
  
  /* Pick the item up. */
  public void Hold(Actor a){
    holder = a;
    
    Rigidbody rb = transform.GetComponent<Rigidbody>();
    if(rb != null){
      rb.isKinematic = true;
      rb.useGravity = false;
    }
    transform.localPosition = heldPos;
    transform.localRotation = Quaternion.Euler(
                                              heldRot.x,    
                                              heldRot.y, 
                                              heldRot.z
                                              );
    Collider c = transform.GetComponent<Collider>();
    if(c != null){
      c.enabled = false;
    }
  }
  
  /* Drop the item. */
  public void Drop(){
    Rigidbody rb = transform.GetComponent<Rigidbody>();
    if(rb != null){
      rb.isKinematic = false;
      rb.useGravity = true;
    }
    Collider c = transform.GetComponent<Collider>();
  }
  
  /* Consume food. */
  public void Consume(){
    //TODO when ACTOR is set up.
    //holder.hp+= healing;
  }
  
  /* Swings melee weapon. */
  public IEnumerator Swing(){
    //TODO when Actor is set up.
    if(sounds.Count > 0){
      float vol = GameController.controller.masterVolume *
                  GameController.controller.effectsVolume;
      AudioSource.PlayClipAtPoint(
                                  sounds[0],
                                  transform.position,
                                  vol
                                  );
    }
    damageActive = false;
    yield return new WaitForSeconds(damageBegin);
    damageActive = true;
    yield return new WaitForSeconds(damageEnd);
    damageActive = false;
  }
  
  /* Sets weapon to ready after cooldown duration. */
  public IEnumerator CoolDown(){
    yield return new WaitForSeconds(cooldown);
    ready = true;
  } 
  
  /* Fires ranged weapon. */
  public void Fire(){
    if(sounds.Count > 0){
      vol = GameController.controller.masterVolume *
            GameController.controller.effectsVolume;
      AudioSource.PlayClipAtPoint(
                                  sounds[0],
                                  transform.position,
                                  vol
                                  );
    }
    if(muzzlePoint != null && rearPoint != null){
      ready = false;
      //holder.SetAnimationTrigger(fireHash);
      //holder.SetAnimationTrigger(fireHash);
      ammo--;
      Vector3 muzzlePos = muzzlePoint.transform.position;
      Vector3 rearPos = rearPoint.transform.positon;
      Vector3 relPos = muzzlePos - rearPos;
      Quaternion projRot = Quaternion.LookRotation(relPos);
      Rigidbody activeRB = active.GetComponent<Rigidbody>();
      GameObject pref = (GameObject)Resources.Load(
                                                     dat.prefabName,
                                                     typeof(GameObject));
                                                     );
      GameObject proj = (GameObject)GameObject.Instantiate(pref.)
      StartCoroutine(CoolDown());
    }
  }
  
  /* Reloads ranged weapon*/
  publi void Reload(){
    int displaced = ammo;
    ammo = 0;
    //TODO when Actor is set up
  }
  
  /* Warps to destination. */
  public void Warp(){
    //TODO
  }
  
  /* Reloads weapon from player inventory if possible */
  public IEnumerator Reload(){
    //TODO
    yield return new WaitForSeconds(1f);
  }
  
  /* Aims weapon or returns it to the hip.*/
  public void ToggleAim(){
    //TODO
  }
  
  /* Get the item's data. */
  public Data GetData(){
    Data dat = new Data();
    dat.x = transform.position.x;
    dat.y = transform.position.y;
    dat.z = transform.position.z;
    Vector3 rot = transform.rotation.eulerAngles;
    dat.xr = rot.x;
    dat.yr = rot.y;
    dat.zr = rot.z;
    dat.prefabName = prefabName;
    dat.displayName = displayname;
    dat.stack = stack;
    dat.stackSize = stackSize;
    dat.ints.Add(weight);
    switch(itemType){
      case FOOD:
        dat.ints.Add(healing);
        break;
      case MELEE:
        dat.ints.Add(damage);
        dat.floats.Add(cooldown);
        break;
      case RANGED:
        dat.ints.Add(damage);
        dat.floats.Add(cooldown);
        break;
      case WARP:
        dat.strings.Add(destName);
        dat.floats.Add(destPos.x);
        dat.floats.Add(destPos.y);
        dat.floats.Add(destPos.z);
        dat.floats.Add(destRot.x);
        dat.floats.Add(destRot.y);
        dat.floats.Add(destRot.z);
        break;
      case CONTAINER:
        for(int j = 0; j < contents.Length; j++){
          dat.data.Add(contents[j]);
        }
        break;
      default:
        break;
    }
    return dat;
  }
  
  /* Load the item's data. */
  public void LoadData(Data dat){
    int i, s, f, d, b;
    i = s = f = d =b = 0;
    
    transform.position = new Vector3(dat.x, dat.y, dat.z);
    transform.rotation = Quaternion.Euler(dat.xr, dat.yr, dat.zr);
    stack = dat.stack;
    switch(itemType){
      case FOOD:
        healing = dat.ints[i];
        i++;
        break;
      case MELEE:
        damage = dat.ints[i];
        i++;
        damageStart = dat.floats[f];
        f++;
        damageEnd = dat.floats[f];
        f++;
        break;
      case RANGED:
        damage = dat.ints[i];
        i++;
        ammo = dat.ints[i];
        i++;
        projectile = dat.strings[s];
        s++;
        rangedType = dat.ints[i];
        i++;
        break;
      case WARP:
        destName = dat.strings[s];
        s++;
        destPos = new Vector3(
                              dat.floats[f],
                              dat.floats[f+1],
                              dat.floats[f+2]
                              );
        f+=3;
        destRot = new Vector3(
                              dat.floats[f],
                              dat.floats[f+1],
                              dat.floats[f+2]
                              );
        f+=3;
        break;
      case CONTAINER:
        contents = dat.data.ToArray();
        break;
      default:
        break;
    }
  }
  
}
