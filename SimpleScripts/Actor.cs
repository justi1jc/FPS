/*
        Author: James Justice
        
        An actor script controls a character's stats,
        model, input/AI, inventory, and speech.

        GameObject structure:
      Body|//Parent to others.
          |
          |spine|//Pivot point for torso
          |     | Head|//Contains Camera
          |     |     | Hand|// Mount point for held items.
          |     |     |     |Fist // Default weapon
          |     |     |     |activeItem//Item in current use, if applicable.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Actor{
  public string displayName;
  public string prefabName;
  public playerNumber;
  
  //Body parts
  public GameObject head;  // Gameobject containing the camera.
  public GameObject hand;  // GameObject where items are attached.
  public GameObject spine; // Pivot point of toros
  public GameObject body;  // The parent gameobject of the actor
  
  //Looking
  public float sensitivityX =1f;
  public float sensitivityY =1f;
  float rotxMax = 60f;
  float headRotx = 0f;
  float headRoty= 0f;
  float bodyRoty = 0f;

  //Walking 
  public float speed;
  bool walking = false;
  bool sprinting = false;

  //Jumping
  public bool jumpReady = true;
  public bool falling;
  public Vector3 fallOrigin;

  //Animation
  public Animator anim;
  int aliveHash, crouchedHash, walkingHash, holdRifleHash, aimRifleHash;
  public string aliveString, crouchedString, walkingString;
  public string holdRifleString, aimRifleString;
  
  //Inventory
  public GameObject defaultItem;
  public GameObject activeItem;
  public GameObject itemInReach;
  public string itemInReachName;
  public List<Data> inventory = new List<Data>();
  
  public HUDController hc;
  public Region reg;
  
  //Player stats
  public int hp = 100;
  public float speed = 0.1;
  
  //Speech
  public Actor interlocutor; // Actor with whom you are speaking
  
  /* Before Start */
  void Awake(){
    //TODO
  }
  
  /* Before rest of code */
  void Start(){
    AssignPlayer(playerNumber);
  }
  
  /* Late-cycle loop. Orients the model before render.*/
  void LateUpdate(){
    spine.rotation = Quaternion.Euler(new Vector3(
                                                  headRotx,
                                                  headRoty,
                                                  spine.rotation.z);
                                                  
  }
  
  /* 0 No AI
  *  1-4 Input routine + initialize HUD
  *  >4 Initialize AI module
  */
  void AssignPlayer(int player){
    player = _player;
    if(player <5 && player > 0){
      //TODO Register with gamecontroller
      StartCoroutine(InputRoutine());
    }
    else if(player == 5){
      //TODO assign ai
      
      
    }
  }
  
  /* Handles input */
  IEnumerator InputRoutine(){
    while(true){
      if(!GameController.controller.paused){
        KeyboardActorInput();
      }
      else{
        KeyboardMenuInput();
      }
    }
  }
  
  /* Handle keyboard input when not paused. */
  void KeyboardActorInput(){
    //Basic movement
    bool shift = Input.GetKey(KeyCode.LeftShift);
    bool walk = false;
    if(Input.GetKey(KeyCode.W)){ Move(0,left); walk = true; }
    if(Input.GetKey(KeyCode.S)){ Move(1,left); walk = true; }
    if(Input.GetKey(KeyCode.A)){ Move(2,left); walk = true; }
    if(Input.GetKey(KeyCode.D)){ Move(3,left); walk = true; }
    if(walk != walking){
      walking = walk;
      anim.SetBool(walkingHash, walking);
    }
    if(Input.GetKeyDown(KeyCode.Space)){ StartCoroutine(JumpRoutine(); )}
    if(Input.GetKeyDown(KeyCode.LeftControl)){ToggleCrouch(); }
    if(Input.GetKeyUp(KeyCode.LeftControl)){ToggleCrouch(); }
    
    //Mouse controls
    if(Input.GetMouseButtonDown(0)){ Use(0); }
    if(Input.GetMouseButtonDown(1)){ Use(1); }
    float rotx = -Input.GetAxis("Mouse Y") * sensitivityX;
    float roty = Input.GetAxis("Mouse X") * sensitivityY;
    Turn(new Vector3(rotx, roty, 0f));
    
    //Special use keys
    if(Input.GetKeyDown(KeyCode.R)){ Use(2); }
    if(Input.GetKeyDown(KeyCode.Q)){ Drop(); }
    if(Input.GetKeyDown(KeyCode.E)){ Interact(); }
    if(Input.GetKeyDown(KeyCode.LeftArrow)){ Interact(1); }
    if(Input.GetKeyDown(KeyCode.RightArrow)){ Interact(2); }
    if(Input.GetKeyDown(KeyCode.DownArrow)){ Interact(3); }
    if(Input.GetKeyDown(KeyCode.Backspace)){ Interact(4); }
  }
  
  /* Handles pause menu keyboard input. */
  void KeyboadMenuInput(){
    
  }
  
  /* Move in a direction 
    0 = Forward
    1 = Backward
    2 = Right
    3 = Left
  */
  public void Move(int direction){
    Rigidbody rb = body.getComponent<Rigidbody>();
    if(rb == null){ return; }
    float pace = speed;
    Vector3 pos = body.transform.position;
    Vector3 dest = Vector3();
    Vector3 dir  = Vector3();
    if(sprinting){ pace *= 1.25f; }
    switch(direction){
      case 0:
        dest = pos + body.transform.forward * pace;
        dir = body.transform.forward;
        break;
      case 1:
        dest = pos + body.transform.forward * -pace;
        dir = -body.transform.forward;
        break;
      case 2:
        dest = pos + body.transform.right * pace;
        dir = body.transform.right;
        break;
      case 3:
        dest = pos + body.transform.right * -pace;
        dir = -body.transform.right;
        break;
    }
    int layerMask = 1 << 8;
       layerMask = ~layerMask;
       RaycastHit[] hits =
          Physics.RaycastAll(pos + new Vector3(0,1,0),dir,3*pace, layerMask);
       if(hits.Length == 0){ rb.MovePosition(dest); } 
  }
  
  /* Two-axis movement. used by AI
   0 = no motion on axis
   1 = motion in positive direction
  -1 = motion in negative direction */
  void Move(int x, int z){
  //TODO
    switch(x){
      case -1:
        Move(1);
        break;
      case 1:
        Move(0);
        break;
    }
    switch(z){
      case -1:
        Move(3);
        break;
      case 1:
        Move(2);
        break;
    }
  }
  
  /* Rotates head along xyz, torso over x axis*/
  void Turn(Vector3 direction){
    headRotx += direction.x;
    headRoty += direction.y;
    bodyRoty += direction.y;
    if(headRotx > rotxMax){ headRotx = rotxMax; }
    if(headRotx < -rotxMax){ headRotx = -rotxMax; }
    head.transform.rotation = Quaternion.Euler(headRotx, headRoty, 0f);
    body.transform.rotation = Quaternion.Euler(0f,bodyRoty, 0f);
  }
  
  /* Jumps */
  IEnumerator JumpRoutine(){
    if(jumpCoolDown){
      Rigidbody rb = body.getComponent<Rigidbody>();
      int lifts = 5;
      int jumpForce = 150;
      while(lifts > 0){
        lifts--;
        rb.AddForce(body.transform.up * jumpForce);
        yield return new WaitForSeconds(0.001f);
      }
    }
    yield return new WaitForSeconds(0f);
  }
  
  /* Toggles model's crouch */
  void ToggleCrouch(){
    anim.SetBool(crouchedHash, !anim.GetBool(crouchedHash));
  }
  
  /* Applies damage from attack. Ignores active weapon. */
  void ReceiveDamage(int damage, GameObject weapon){
    if(health < 0 || weapon == activeItem){ return; }
    health -= damage;
    if(health < 1){
      health = 0;
      StopAllCoroutines();
      anim.SetBool(aliveHash, false);
      Ragdoll(true);
    }
  }
  
  /* Adds or removes the ragdoll effect on the actor. */
  void Ragdoll(bool state){
    //TODO
  }
  
  /* Use active item according to its type */
  void Use(int use){
    if(activeItem == null){ return; }
    Item item = activeItem.GetComponent<Item>();
    if(item == null){ print("Item missing:" + activeItem.name); return; }
    item.use(use);
  }
  
  /* Selects an item in inventory. */
  public void Equip(item itemIndex){
  //TODO: Ensure animations are correct
    if(itemIndex < 0 || itemIndex => inventory.Count){ return; }
    anim.setBool(aimRifleHash, false);
    anim.setBool(holdRifleHash, false);
    Data dat = inventory[itemIndex];
    GameObject prefab = Resources.Load(dat.prefabName) as GameObject;
    if(prefab == null){ print("Prefab null:" + dat.name); return;}
    GameObject itemGO = (GameObject)GameObject.Instantiate(
      prefab,
      transform.position,
      Quaternion.identity
    );
    if(itemGO == null){print("GameObject null:" + dat.name); return; }
    Item item = itemGO.GetComponent<Item>();
    item.LoadData(dat);
    itemGO.transform.parent = hand.transform;
    item.Hold(this);
    activeItem = itemGO;
    inventory.Remove(inventory[itemIndex]);
    if(item.ConsumesAmmo()){ LoadAmmo(item); }
  }
  
  /* Removes number of available ammo, up to max, and returns that number*/
  public int RequestAmmo(string ammoName,int max){
    for(int i = 0; i< inventory.Count; i++){
      if(inventory[i].name == ammoName){
        int available = inventory.stack;
        if(available <= max){
          inventory.Remove(inventory[i]);
          return available;
        }
        if(available > max){
          available = max;
          inventory[i].stack -= max;
          return available;
        }
      }
    }
    return 0;
  }
  
  /* Adds item data to inventory */
  public void StoreItem(Data item){
    inventory.Add(item);
  }
  
  /* Drops item onto ground from inventory. */
  public void DiscardItem(int itemIndex){
    if(itemIndex < 0 || itemIndex > inventory.Count){ return; }
    Data dat = inventory[itemIndex];
    GameObject prefab = Resources.Load(dat.prefabName) as GameObject;
    GameObject itemGO = (GameObject)GameObject.Instantiate(
      prefab,
      transform.position,
      Quaternion.identity
    );
    Item item = itemGO.GetComponent<Item>();
    item.LoadData(dat);
    inventory.Remove(inventory[itemIndex]);
  }
  
  /* Interact with item in reach.
     i is the argument for the interaction, if relevant */
  public void Interact(int mode = -1){
    if(itemInReach == null){ return; }
    Item item = itemInReach.GetComponent<Item>();
    if(item == null){ return; }
    item.Interact(this, mode); 
  }
  
  /* Drops active item from hand */
  public void Drop(){
    if(activeItem == defaultItem || activeItem == null){ return; }
    activeItem.transform.parent = null;
  }
  
  /* Returns data not found in prefab for this Actor */
  public Data GetData(){
  //TODO
  }
  
  /* Loads data not specified for this prefab for this Actor */
  public void LoadData(Data dat){
  //TODO
  }
   
  /* Initiates conversation with other actor */
  public void TalkTo(Actor other, int option = -1){
    if(option == -1 && other != interlocutor && other.interlocutor == null){
      interlocutor = other;
      interlocutor.interlocutor = this;
      interlocutor.ReceiveSpeech(option);
      return;
    }
    if(interlocutor == other && other.interlocutor == this){
      interlocutor.ReceiveSpeech(option);
    }
  }
  
  /* Respond to being talked to by other Actor. */
  public void ReceiveSpeech(int option = -1){
    //TODO Make one response for NPC, one for Player
  }
    
  /* Finds node nearest to Actor */
  public NavNode NearestNode(){
  //TODO
  }
  
  /* move to a specific node. 
     If no node specified by reference or index,
     goes to nearest node.*/
  public IEnumerator ToNode(int index = -1, NavNode node = null){
  //TODO
  }
  
  /* Moves to each node in order of list. */
  public IENumerator TakePath(List<int> path){
  //TODO
  }
  
  /* Calculates path between two nodes, if one exists. */
  public List<int> GetPath(int start, int finish){
  //TODO
  }
  
  /* Moves to a particular point in a straight line */
  public IEnumerator MoveTo(Vector3 dest){
  //TODO
  }
  
  /* Orients look toward a particular orientation. */
  public IEnumerator Aim(Vector3 eulers){
  //TODO
  }
  
  /* Returns the current look orientation */
  public Vector3 CurrentAim(){
  //TODO
  }
  
}
