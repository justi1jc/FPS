/*
        Author: James Justice
        
        An actor script controls a character's stats,
        model, input/AI, inventory, and speech.

        GameObject structure:
      Body| // Parent to others. Rigidbody, collider, Actor
          |
          |spine| // Pivot point for torso
          |     | Head| // Contains Camera
          |     |     | Hand| // Mount point for held items.
          |     |     |     |Fist // Default weapon
          |     |     |     |activeItem // Item in current use, if applicable.
          
      Variables that must be set in animator or prefab
      head, hand, spine
      Speed
      All animation strings
      playerNumber
      safeFallDistance
      rigidody constraints: rotation on x, y z axes.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Actor : MonoBehaviour{
  public string displayName;
  public string prefabName;
  public int playerNumber; // 1 for keyboard, 3-4 for controller, 5 for NPC
  
  //Body parts
  public GameObject head;  // Gameobject containing the camera.
  public GameObject hand;  // GameObject where items are attached.
  public GameObject spine; // Pivot point of toros
  GameObject body;  // The base gameobject of the actor
  
  //Looking
  public bool rt_down = false;
  public bool lt_down = false;
  public float sensitivityX =1f;
  public float sensitivityY =1f;
  float rotxMax = 60f;
  public float headRotx = 0f;
  public float headRoty= 0f;
  float bodyRoty = 0f;

  //Walking 
  public float speed;
  bool walking = false;
  bool sprinting = false;

  //Jumping
  public bool jumpReady = true;
  public bool falling;
  public float fallOrigin;
  public float safeFallDistance;

  //Animation
  public Animator anim;
  int aliveHash, crouchedHash, walkingHash, holdRifleHash, aimRifleHash;
  public string aliveString, crouchedString, walkingString;
  public string holdRifleString, aimRifleString;
  
  //Inventory
  bool menuOpen;
  public GameObject defaultItem;
  public GameObject activeItem;
  public GameObject itemInReach;
  public string itemInReachName;
  public List<Data> inventory = new List<Data>();
  
  //Player stats
  public int health = 100;
  
  //Speech
  public Actor interlocutor; // Actor with whom you are speaking
  
  /* Before Start */
  void Awake(){
    //TODO
    aliveHash = Animator.StringToHash(aliveString);
    crouchedHash = Animator.StringToHash(crouchedString);
    walkingHash = Animator.StringToHash(walkingString);
    holdRifleHash = Animator.StringToHash(holdRifleString);
    aimRifleHash = Animator.StringToHash(aimRifleString);
  }
  
  /* Before rest of code */
  void Start(){
    body = gameObject;
    AssignPlayer(playerNumber);
  }
  
  
  /* Cycle loop. Listens for falling. */
  void Update(){
    UpdateReach();
    Rigidbody rb = body.GetComponent<Rigidbody>();
    if(!falling && rb && rb.velocity.y < 0f){
      falling = true;
      fallOrigin = transform.position.y;
		}
  }
  
  /* Late-cycle loop. Orients the model before render.*/
  void LateUpdate(){
    if(spine){
      spine.transform.rotation = Quaternion.Euler(new Vector3(
        headRotx,
        headRoty,
        spine.transform.rotation.z));
    }                                    
  }
  
  /* 0 No AI
  *  1-4 Input routine + initialize HUD
  *  >4 Initialize AI module
  */
  void AssignPlayer(int player){
    if(player == 1){
      SetMenuControls(false);
      StartCoroutine(KeyboardInputRoutine()); 
      if(Session.session){ Session.session.RegisterPlayer(player, head.GetComponent<Camera>()); }
    }
    if(player <5 && player > 1){
      if(Session.session){ Session.session.RegisterPlayer(player, head.GetComponent<Camera>()); }
      StartCoroutine(ControllerInputRoutine());
    }
    else if(player == 5){
      //TODO assign ai
    }
  }
  
  /* Sets controls between menu and in-game contexts. */
  void SetMenuControls(bool val){
    Cursor.visible = !val;
    if(!val){ Cursor.lockState = CursorLockMode.Locked; }
    else{ Cursor.lockState = CursorLockMode.None; }
  }
  
  
  /* Handles input from keyboard. */
  IEnumerator KeyboardInputRoutine(){
    while(true){
      if(!menuOpen){//TODO: Toggle menu controls properly
        KeyboardActorInput();
      }
      else{
        KeyboardMenuInput();
      }
      yield return new WaitForSeconds(0.01f);
    }
  }
  
  /* Handles input from controller. */
  IEnumerator ControllerInputRoutine(){
    while(true){
      if(!menuOpen){
        ControllerActorInput();
      }
      else{
        ControllerMenuInput();
      }
      yield return new WaitForSeconds(0.01f);
    }
  }
  
  /* Handle keyboard input when not paused. */
  void KeyboardActorInput(){
    //Basic movement
    bool shift = Input.GetKey(KeyCode.LeftShift);
    bool walk = false;
    //TODO: if(shift != sprinting && anim){ anim.SetBool(sprintingHash, shift)}
    sprinting = shift;
    if(Input.GetKey(KeyCode.W)){ Move(0); walk = true; }
    if(Input.GetKey(KeyCode.S)){ Move(1); walk = true; }
    if(Input.GetKey(KeyCode.A)){ Move(2); walk = true; }
    if(Input.GetKey(KeyCode.D)){ Move(3); walk = true; }
    if(Input.GetKeyDown(KeyCode.Space)){ StartCoroutine(JumpRoutine()); }
    if(Input.GetKeyDown(KeyCode.LeftControl)){ToggleCrouch(); }
    if(Input.GetKeyUp(KeyCode.LeftControl)){ToggleCrouch(); }
    if(walk != walking && anim){ anim.SetBool(walkingHash, walking); }
    
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
  void KeyboardMenuInput(){
    
  }
  
  /* Handles controller input when not paused. */
  void ControllerActorInput(){
    //Get axis input
    float xl = Input.GetAxis(Session.XL);
    float yl = Input.GetAxis(Session.YL);
    float xr = Input.GetAxis(Session.XR);
    float yr = Input.GetAxis(Session.YR);
    float rt = Input.GetAxis(Session.RT);
    float lt = Input.GetAxis(Session.LT);
    
    // Basic movement
    bool shift = Input.GetKey(Session.RB);
    bool walk = xl != 0f || yl != 0;
    //TODO: if(shift != sprinting && anim){ anim.SetBool(sprintingHash, shift)}
    sprinting = shift;
    AxisMove(xl, yl);
    Turn(new Vector3(yr, xr, 0f));
    if(walk != walking && anim){ anim.SetBool(walkingHash, walking); }
    
    //Buttons
    if(Input.GetKeyDown(Session.A)){ StartCoroutine(JumpRoutine()); }
    if(Input.GetKeyDown(Session.X) && itemInReach){ Interact(); }
    else if(Input.GetKeyDown(Session.X)){ Use(2); }
    if(rt > 0 && !rt_down){ Use(0); rt_down = true;}
    if(rt == 0){ rt_down = false; }
    if(lt > 0 && !lt_down){ Use(1); lt_down = true;}
    if(lt == 0){ lt_down = false; }
    if(Input.GetKeyDown(Session.LSC)){ ToggleCrouch(); }
  }
  
  /* Handles pause menu controller input. */
  void ControllerMenuInput(){
    
  }
  
  /* Move in a direction 
    0 = Forward
    1 = Backward
    2 = Left
    3 = Right
  */
  public void Move(int direction){
    Rigidbody rb = body.GetComponent<Rigidbody>();
    if(rb == null){ return; }
    float pace = speed;
    Vector3 pos = body.transform.position;
    Vector3 dest = new Vector3();
    Vector3 dir  = new Vector3();
    if(sprinting){ pace *= 1.75f; }
    if(!jumpReady){ pace *= 0.75f; }
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
        dest = pos + body.transform.right * -pace;
        dir = -body.transform.right;
        break;
      case 3:
        dest = pos + body.transform.right * pace;
        dir = body.transform.right;
        break;
    }
    if(MoveCheck(dir, 3 * pace)){ rb.MovePosition(dest); }
  }
  
  /* Move according to x and y axes. */
  public void AxisMove(float x, float y){
    Vector3 xdir = x * body.transform.right;
    Vector3 ydir = -y * body.transform.forward;
    Vector3 dir = xdir + ydir;
    Rigidbody rb =  body.GetComponent<Rigidbody>();
    float pace = speed;
    if(sprinting){ pace *= 1.75f; }
    if(!jumpReady){ pace *= 0.75f; } 
    Vector3 dest = body.transform.position + (pace * dir);
    if(MoveCheck(dir, 3 * pace)){ rb.MovePosition(dest); }
  }
  
  /* Returns true if the Actor has clearance to move in a particular direction
  a particular distance. */
  bool MoveCheck(Vector3 direction, float distance){
    Vector3 center = body.transform.position;
    Vector3 halfExtents = body.transform.localScale / 2;
    Quaternion orientation = body.transform.rotation;
    int layerMask = ~(1 << 8);
    RaycastHit hit;
    return !Physics.BoxCast(
      center,
      halfExtents,
      direction,
      out hit,
      orientation,
      distance,
      layerMask,
      QueryTriggerInteraction.Ignore
    );
  }
  
  /* Boxcasts to find the current item in reach, updating itemInReach if they
    do not match. */
  void UpdateReach(){
    if(!hand){ print(gameObject.name + " hand missing"); return; }
    Vector3 center = hand.transform.position;
    Vector3 halfExtents = hand.transform.localScale / 2;
    Vector3 direction = hand.transform.forward;
    Quaternion orientation = body.transform.rotation;
    float distance = 1f;
    int layerMask = ~(1 << 8);
    RaycastHit[] found = Physics.BoxCastAll(
      center,
      halfExtents,
      direction,
      orientation,
      distance,
      layerMask,
      QueryTriggerInteraction.Ignore
    );
    for(int i = 0; i < found.Length; i++){
      Item item = found[i].collider.gameObject.GetComponent<Item>();
      if(item){ itemInReach = item.gameObject; return; }
    }
    itemInReach = null;
    return;
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
    if(head){
      head.transform.rotation = Quaternion.Euler(headRotx, headRoty, 0f);
    }
    body.transform.rotation = Quaternion.Euler(0f, bodyRoty, 0f);
  }
  
  /* Jumps */
  IEnumerator JumpRoutine(){
    if(jumpReady){
      jumpReady = false;
      Rigidbody rb = body.GetComponent<Rigidbody>();
      int lifts = 5;
      int jumpForce = 60;
      while(lifts > 0){
        lifts--;
        rb.AddForce(body.transform.up * jumpForce);
        yield return new WaitForSeconds(0.001f);
      }
    }
    yield return new WaitForSeconds(0f);
  }
  
  /* Refreshes Jump upon landing. */
  void OnCollisionEnter(Collision col){
    if(falling){ Land(fallOrigin - transform.position.y); }
  }
  
  /* Replenishes Jump and applies fall damage. */
  void Land(float distanceFallen){
    falling = false;
    jumpReady = true;
    if(distanceFallen > safeFallDistance){
      int damage = (int)(distanceFallen - safeFallDistance) * 5;
      ReceiveDamage(damage, gameObject);
    }
  }
  
  /* Toggles model's crouch */
  void ToggleCrouch(){
    print("Crouch!");
    if(!anim){ return; }
    anim.SetBool(crouchedHash, !anim.GetBool(crouchedHash));
  }
  
  /* Applies damage from attack. Ignores active weapon. */
  public void ReceiveDamage(int damage, GameObject weapon){
    if(health < 0 || (weapon == activeItem && damage > 0)){ return; }
    health -= damage;
    if(health < 1){
      health = 0;
      StopAllCoroutines();
      if(anim){ anim.SetBool(aliveHash, false); }
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
    item.Use(use);
  }
  
  /* Drops active item from hand */
  public void Drop(){
    if(activeItem == defaultItem || activeItem == null){ return; }
    activeItem.transform.parent = null;
    Item item = activeItem.GetComponent<Item>();
    if(item){ item.Drop(); }
    activeItem = defaultItem;
  }
  
  /* Selects an item in inventory to equip. */
  public void Equip(int itemIndex){
  //TODO: Ensure animations are correct
    if(itemIndex < 0 || itemIndex >= inventory.Count){ return; }
    if(anim){
      anim.SetBool(aimRifleHash, false);
      anim.SetBool(holdRifleHash, false);
    }
    Data dat = inventory[itemIndex];
    GameObject prefab = Resources.Load(dat.prefabName) as GameObject;
    if(prefab == null){ print("Prefab null:" + dat.displayName); return;}
    GameObject itemGO = (GameObject)GameObject.Instantiate(
      prefab,
      transform.position,
      Quaternion.identity
    );
    if(itemGO == null){print("GameObject null:" + dat.displayName); return; }
    Item item = itemGO.GetComponent<Item>();
    item.LoadData(dat);
    itemGO.transform.parent = hand.transform;
    item.Hold(this);
    activeItem = itemGO;
    inventory.Remove(inventory[itemIndex]);
    if(item.ConsumesAmmo()){ item.Reload(); }
  }
  
  /* Removes number of available ammo, up to max, and returns that number*/
  public int RequestAmmo(string ammoName,int max){
    for(int i = 0; i < inventory.Count; i++){
      if(inventory[i].displayName == ammoName){
        int available = inventory[i].stack;
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
    if(item.stack == 0){ return; }
    for(int i = 0; i < inventory.Count; i++){
      if(item.stack < 1){ return; }
      Data dat = inventory[i];
      if(item.displayName == dat.displayName
         && dat.stack < dat.stackSize){
        int freeSpace = dat.stackSize - dat.stack;
        if(freeSpace > item.stack){ dat.stack += item.stack; return; }
        else{ dat.stack += freeSpace; item.stack -= freeSpace; }
      }
    }
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
    dat.stack--;
    if(dat.stack < 1){ inventory.Remove(dat); }
  }
  
  /* Interact with item in reach.
     i is the argument for the interaction, if relevant */
  public void Interact(int mode = -1){
    if(itemInReach == null){ return; }
    Item item = itemInReach.GetComponent<Item>();
    if(item == null){ return; }
    item.Interact(this, mode);
  }
  
  /* Pick up item in world. */
  public void PickUp(Item item){
    if(!item){ return; }
    Data dat = item.GetData();
    Destroy(item.gameObject);
    StoreItem(dat);
    if(activeItem == defaultItem){ Equip(inventory.Count - 1); }
    Destroy(item.gameObject);
  }
  
  /* Returns data not found in prefab for this Actor */
  public Data GetData(){
  //TODO
    return new Data();
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
     
}
