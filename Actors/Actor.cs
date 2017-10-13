/*
        Author: James Justice
        
        An actor script controls a character's stats,
        model, input/AI, inventory, and speech.

      Note: Player's root collider(The one ignored by bodyparts) should be on layer 9.

        GameObject structure:
      Body| // Parent to others. Rigidbody, collider, Actor
          |
          |spine| // Pivot point for torso
          |     | Head| // Contains Camera, Menu
          |     |     |RHand| // Mount points for held items. 
          |     |     |     |primaryItem
          |     |     |     | // Need trigger box and Item for abilities
          |     |     |LHand|
          |     |     |     |secondaryItem
          |     |     |     | // Need trigger box and Ttem for abilities
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
  private bool pickupCooldown = true;
  public bool init = false; // True after Start() has finished.
  
  public string displayName;
  public string prefabName;
  public int playerNumber; // 1 for keyboard, 3-4 for controller, 5 for NPC
  public Cell lastPos = null; // Used to place player in correct cell
  
  //Body parts
  public GameObject head;    // Gameobject containing the camera.
  public GameObject hand;    // GameObject where items are attached.
  public GameObject offHand; // Secondary hand where items are attached.
  public GameObject spine;   // Pivot point of toros
  public GameObject body;    // The base gameobject of the actor, for external use
  public GameObject rFoot;    // Used for jumping.

  
  //UI
  public MenuManager menu;
  public Camera cam; // Camera used for players.
  bool menuMove = true;// Used to govern joystick menu inputs.
  float menuMovementDelay = 0.15f; // How long to delay between menu movements
  public bool menuOpen;
  public GameObject actorInReach;
  public string actorInReachName;
  public GameObject itemInReach;
  public string itemInReachName;
  
  //Looking
  public bool rt_down = false;
  public bool lt_down = false;
  public float sensitivityX =1f;
  public float sensitivityY =1f;
  float rotxMax = 60f;
  public float headRotx = 0f;
  public float headRoty= 0f;
  float bodyRoty = 0f;
  public bool aiming = false;
  bool doneAiming = true;

  //Movement
  public bool ragdoll = false;
  public float speed;
  public bool walking = false;
  public bool sprinting = false;
  public bool crouched = false;
  public int moveX, moveY; // Current direction.
  
  //Jumping
  public bool jumpReady = true;
  public bool falling;
  public float fallOrigin;
  public float safeFallDistance;

  //Animation
  public Animator anim;
  
  //Inventory
  public Inventory inventory;
    
  //Stats
  public StatHandler stats;
  public int id = -1;
  public int killerId = -1;
  
  // Equipped items and abilities.
  public EquipSlot arms;
  public PaperDoll doll;
  public bool armsReady = true;
  
  // Speech
  public Actor interlocutor; // Conversation partner
  public string speechTreeFile; // File for speech tree to use.
  public SpeechTree speechTree; // Speech tree if this is an NPC Actor
  
  // AI
  public AIManager ai;
  public string defaultAI = "";
  
  // Controller
  ActorInputHandler input;
  
  /* Before Start */
  void Awake(){
    body = gameObject;
    inventory = new Inventory();
    arms = new EquipSlot(offHand, hand, this);
    doll = new PaperDoll(this);
    arms.actor = this;
    stats = new StatHandler(this);
  }
  
  /* Before rest of code */
  void Start(){    
    if(stats.level == 0){ stats.LevelUp();}
    AssignPlayer(playerNumber);
    Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
    foreach(Rigidbody rb in rbs){ 
      if(rb != GetComponent<Rigidbody>()){rb.isKinematic = true;} 
    }
    Collider[] colliders = GetComponentsInChildren<Collider>();
    Collider col = GetComponent<Collider>();
    foreach(Collider c in colliders){
      if(c != col){
        Physics.IgnoreCollision(c, col); 
        c.isTrigger = true;
      }
    }
    init = true;
  }
  
  /* Cycle loop. Runs once per frame. */
  void Update(){
    UpdateReach();
    UpdateFall(); 
  }
  
  /* Listens for faling. */
  void UpdateFall(){
    Rigidbody rb = GetComponent<Rigidbody>();
    if(!falling && rb && rb.velocity.y < 0f){ falling = true; }
  }
  
  /* Late-cycle loop. Orients the model before render.*/
  void LateUpdate(){
    if(spine && !ragdoll){
      float offset = -90; // Offset of model's spine.
      Vector3 rot = new Vector3(headRotx, headRoty, offset);
      spine.transform.rotation = Quaternion.Euler(rot);
    }
    if(arms != null){ arms.Update(); }
  }
  
  /* Notifies menu, if it exists. */
  public void Notify(string message){
    if(menu != null){ menu.Notify(message); }
  }
  
  /* Sets the animator boolean if the animator exists. */
  public void SetAnimBool(string boolean, bool val){
    if(!anim){ print("animator null"); return; }
    anim.SetBool(boolean, val);
  }
  
  /* Returns the value of the boolean, or false. */
  public bool GetAnimBool(string boolean){
    if(!anim){ return false; }
    return anim.GetBool(boolean);
  }
  
  
  /* Toggles between 60 and 30 field of view. */
  public void ToggleAim(){
    aiming = !aiming;
    if(cam != null && doneAiming){
      StartCoroutine(SetAim());
    }
  }
  
  /* Transitions between 60 and 30 field of view for camera */
  public IEnumerator SetAim(){
    doneAiming = false;
    int fov = aiming ? 30 : 60;
    while(cam.fieldOfView != fov){
      fov = aiming ? 30 : 60;
      if(cam.fieldOfView > fov){ cam.fieldOfView--; }
      else if(cam.fieldOfView < fov){ cam.fieldOfView++; }
      if(cam.fieldOfView > fov){ cam.fieldOfView--; }
      else if(cam.fieldOfView < fov){ cam.fieldOfView++; }
      yield return new WaitForSeconds(1f/120f);
    }
    doneAiming = true;
    yield return new WaitForSeconds(0f);
  }
  
  /* Sets the specified trigger if the animator exists. */
  public void SetAnimTrigger(string trigger){
    if(!anim){ return; }
    anim.SetTrigger(trigger);
  }
  
  /* 0 No AI
  *  1-4 Input routine + initialize HUD
  *  >4 Initialize AI module
  */
  void AssignPlayer(int player){
    playerNumber = player;
    if(arms.Empty()){
      arms.EquipAbility(Item.GetItem("Abilities/Unarmed"));
    }
    StartCoroutine(RegenRoutine());
    if(player <5 && player > 0){
      SetMenuOpen(false);
      if(menu){ menu.Change("HUD");  menu.actor = this; }
      if(Session.session != null && cam != null){
        Session.session.RegisterPlayer(this, player, cam); 
      }
      else{ print("Session or cam is null"); }
      if(player == 1){ input = new ActorInputHandler(this, "KEYBOARD AND MOUSE"); }
      else{ input = new ActorInputHandler(this, "XBOX 360 CONTROLLER"); }
      StartCoroutine(InputRoutine());
    }
    else if(player == 5){
      if(defaultAI == ""){ ai = new AIManager(this, "PASSIVE"); }
      else{ ai = new AIManager(this, defaultAI); }
      if(speechTreeFile != ""){ speechTree = new SpeechTree(speechTreeFile); }
    }
  }
  
  public IEnumerator InputRoutine(){
    while(Alive()){
      input.Update();
      yield return new WaitForSeconds(0.01f);
    }
  }
  
  /* Returns an empty string or the info of the active item. */
  public string ItemInfo(){
    return arms.GetInfo();
  }
  
  /* Returns an empty string, or info about interacting with the actor in reach */
  public string ActorInteractionText(){
    string text = "";
    Actor a = actorInReach.GetComponent<Actor>();
    if(a == null){ return text; }
    if(crouched){ text += "Steal from "; }
    else if(a.Alive()){ text += "Talk to "; }
    else{ text += "Loot "; }
    text += actorInReachName;
    return text;
  }
  
  /* Sets controls between menu and in-game contexts. */
  public void SetMenuOpen(bool val){
    menuOpen = val;
    if(playerNumber == 1){
      if(!val){ Cursor.lockState = CursorLockMode.Locked; }
      else{ Cursor.lockState = CursorLockMode.None; }
    }
    if(input != null){ input.SetMenuOpen(val); }
  }
  
  IEnumerator RegenRoutine(){
    while(stats.health >= 0){
      stats.Update();
      yield return new WaitForSeconds(0.2f);
    }
  }
  
  
  /* Changes the menu. */
  public void ChangeMenu(string choice){
    if(menu != null){
      menu.Change(choice);
      if(choice == "HUD"){ SetMenuOpen(false); }
      else{ SetMenuOpen(true); }
    }
  }
  
  public MenuManager GetMenu(){
    return menu;
  }
  
  /* External jump method. */
  public void Jump(){
    StartCoroutine(JumpRoutine());
  }
  
  /* Set sprinting. */
  public void SetSprinting(bool sprint){
    sprinting = sprint;
  }

  /* Move relative to transform.forward and transform.right */
  public void StickMove(float x, float y){
    if(ragdoll){  return; }
    Vector3 xdir = x * transform.right;
    Vector3 ydir = y * transform.forward;
    Vector3 dir = (xdir + ydir).normalized;
    Rigidbody rb =  GetComponent<Rigidbody>();
    float pace = speed;
    if(sprinting){ pace *= 1.75f; }
    if(!jumpReady){ pace *= 0.75f; }
    ExecuteMove(pace, dir);
  }
  
  /* Move relative to x and z positions.(Used for AI) */
  public void AxisMove(float x, float z){
    if(ragdoll){  return; }
    Vector3 dir = new Vector3(x, 0f, z).normalized;
    Rigidbody rb = GetComponent<Rigidbody>();
    float pace = speed;
    if(sprinting){ pace *= 1.75f; }
    if(!jumpReady){ pace *= 0.75f; }
    ExecuteMove(pace, dir);
  }
  
  /* Attempts to move the Actor. Applies stamina limitations.
     If cannot move, tries to move at 45 degree angle */
  void ExecuteMove(float pace, Vector3 dir){
    Vector3 dest = transform.position +  dir * pace;
    if(dir.x == 0f){ moveX = 0; }
    if(dir.y == 0f){ moveY = 0; }
    if(walking && moveX == 0 && moveY == 0){ SetAnimBool("walking", false); }
    else if(!walking && (moveX != 0 || moveY != 0)){ 
      SetAnimBool("walking", true); 
    }
    Vector3 pos = transform.position;
    Rigidbody rb = GetComponent<Rigidbody>();
    int runCost = (int)Vector3.Magnitude(pace * dir * 6);
    if(stats.StatCheck("ENDURANCE", 50)){ runCost = 0; }
    if(MoveCheck(dir, pace * 3)){
      transform.position += (dir * pace);
      return;
    }
    dir += transform.up;
    dir = dir.normalized;
    dest = transform.position +  dir * pace;
    if(MoveCheck(dir, pace * 3)){ transform.position += (dir * pace); }
  }
  
  /* Returns the root transform for a given transform. */
  public Transform GetRoot(Transform t){
    Transform current = t;
    Transform last = t;
    while(current != null){
      last = current;
      current = current.parent;
    }
    return last;
  }
  
  /* Returns true if the Actor has clearance to move in a particular direction
  a particular distance. */
  bool MoveCheck(Vector3 direction, float distance){
    Vector3 center = transform.position + new Vector3(0,3f,0);
    Vector3 halfExtents = transform.localScale / 2f;
    Quaternion orientation = transform.rotation;
    int layerMask = ~(1 << 8);
    RaycastHit hit;
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
      Transform f = found[i].collider.gameObject.transform;
      if(GetRoot(f) != transform){ return false; } 
    }
    return true;
  }
  
  /* Boxcasts to find the current item in reach, updating itemInReach if they
    do not match. */
  void UpdateReach(){
    Vector3 center = cam ? cam.transform.position : head.transform.position;
    Vector3 halfExtents = new Vector3(0.25f,0.25f,0.25f);
    Vector3 direction = cam ? cam.transform.forward : head.transform.forward;
    Quaternion orientation = cam ? cam.transform.rotation : head.transform.rotation;
    float distance = 5f;
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
    bool itemFound = false;
    for(int i = 0; i < found.Length; i++){
      Item item = found[i].collider.gameObject.GetComponent<Item>();
      if(item && GetRoot(item.gameObject.transform) != transform && item.displayName != ""){
        itemInReach = item.gameObject;
        itemFound = true;
        break;
      }
    }
    if(!itemFound){ itemInReach = null; }
    bool actorFound = false;
    for(int i = 0; i < found.Length; i++){
      Actor other = found[i].collider.gameObject.GetComponent<Actor>();
      HitBox hb = found[i].collider.gameObject.GetComponent<HitBox>();
      if(other && other != this){
        actorInReach = other.gameObject;
        actorInReachName = other.displayName;
        actorFound = true;
        break;
      }
      else if(hb != null){
        if(hb.body && hb.body != this){
          actorInReach = hb.body.gameObject;
          actorInReachName = hb.body.displayName;
          actorFound = true;
          break;
        }
      }
    }
    if(!actorFound){ actorInReach = null; }
    return;
  }  
  
  public void Recoil(float recoil){
    stats.aimPenalty += 1;
    float x = recoil;
    x = Random.Range(-x, x);
    float y = Random.Range(recoil, recoil*1.5f);
    Turn(new Vector3(-y, x, 0f) );
  }
  
  /* Rotates head along xyz, torso over x axis*/
  public void Turn(Vector3 direction){
    if(ragdoll){ return; }
    headRotx += direction.x;
    headRoty += direction.y;
    bodyRoty += direction.y;
    if(headRotx > rotxMax){ headRotx = rotxMax; }
    if(headRotx < -rotxMax){ headRotx = -rotxMax; }
    transform.rotation = Quaternion.Euler(0f, bodyRoty, 0f);
  }
  
  /* Collider behaviour for body. */
  public void OnCollisionEnter(Collision col){LandCheck(col.collider); }
  public void OnColisionStay(Collision col){ LandCheck(col.collider); }
  
  void LandCheck(Collider col){
    if(col == null){ return; }
    if(falling && transform.position.y > col.transform.position.y){ 
      Land(fallOrigin - transform.position.y);
    }
  }
  
  /* Jumps */
  IEnumerator JumpRoutine(){
    if(jumpReady){
      jumpReady = false;
      Rigidbody rb = GetComponent<Rigidbody>();
      int lifts = 5;
      int jumpForce = 60;
      while(lifts > 0){
        lifts--;
        rb.AddForce(transform.up * jumpForce);
        yield return new WaitForSeconds(0.001f);
      }
    }
    yield return new WaitForSeconds(0f);
  }
  
  /* Replenishes Jump and applies fall damage. */
  public void Land(float distanceFallen){
    falling = false;
    jumpReady = true;
    if(distanceFallen > safeFallDistance){
      int damage = (int)(distanceFallen - safeFallDistance) * 5;
      ReceiveDamage(damage, gameObject);
    }
  }
  
  /* Toggles model's crouch */
  public void ToggleCrouch(){
    crouched = !crouched;
    SetAnimBool("crouching", crouched);
  }
  
  /* Add xp and check for levelup conditions. */
  public void ReceiveXp(int amount){
    stats.AwardXP(amount);
  }
  
  /* Applies damage from attack. Ignores active weapon. */
  public void ReceiveDamage(int damage, GameObject weapon){
    if(stats.dead){ return; }
    if(weapon == null || GetRoot(weapon.transform) == transform){ return; }
    if(stats.health < 1){ return; }
    stats.DrainCondition("HEALTH", damage);
    Actor attacker = Attacker(weapon);
    if(ai != null && attacker != null){ ai.ReceiveDamage(damage, attacker); }
    if(stats.health < 1){ Die(weapon); }
    else if (damage > 0){
      bool check = stats.StatCheck("ENDURANCE", damage);
      if(check){ StartCoroutine(Stagger()); }
      else{ StartCoroutine(FallDown()); }
    }
  }
  
  /* Returns the actor responsible for damage, provided it is not this actor. */
  public Actor Attacker(GameObject weapon){
    Item item = weapon.GetComponent<Item>();
    if(item == null){ return null; }
    if(item.holder == this || item.holder == null){ return null; }
    return item.holder;
  }
  
  /* Enter dead state, warding experience to the killder. */
  public void Die(GameObject weapon){
    stats.dead = true;
    //StopAllCoroutines();
    Ragdoll(true);
    arms.Drop();
    arms.Drop();
    if(ai != null){ ai.Pause(); }
    Item item = weapon.GetComponent<Item>();
    if(item && item.holder){
      item.holder.ReceiveXp(stats.NextLevel(stats.level -1));
      killerId = item.holder.id;
    }
    GetComponent<BoxCollider>().isTrigger = false;
    BoxCollider[] colliders = GetComponentsInChildren<BoxCollider>();
    foreach(BoxCollider bc in colliders){ bc.isTrigger = false; }
    if(anim){ anim.enabled = false; }
    Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
    foreach(Rigidbody rb in rbs){ rb.isKinematic = false; }
  }
  
  /* Actor rocks a bit. */
  IEnumerator Stagger(){
    yield return null;
  }
  
  /* Actor is knocked over */
  IEnumerator FallDown(){
    yield return null;
  }
  
  /* Adds or removes the ragdoll effect on the actor. */
  void Ragdoll(bool state){
    ragdoll = state;
    Rigidbody rb = GetComponent<Rigidbody>();
    if(state){
      if(ai != null){ ai.Pause(); }
      rb.constraints = RigidbodyConstraints.None; 
    }
    else{
      if(ai != null){ ai.Resume(); }
      rb.constraints = RigidbodyConstraints.FreezeRotationX |
                      RigidbodyConstraints.FreezeRotationY |
                      RigidbodyConstraints.FreezeRotationZ;
    }
  }
  
  /* Use primary or secondary item */
  public void Use(int use){ arms.Use(use); }
  
  /* Drops an item from actor's arms. */
  public void Drop(){ 
    arms.Drop();
    if(arms.Empty()){ arms.EquipAbility(Item.GetItem("Abilities/Unarmed")); }
  }
  
  /* Public equip method for an unstored object. */
  public void Equip(Data dat){
    int slot = StoreItem(dat);
    if(slot == -1){ print("Inventory full."); return; }
    Equip(slot);
  }
  
  /* Equips item in inventory by its index. */
  public void Equip(int itemIndex){
    Data dat = inventory.Peek(itemIndex);
    if(dat == null){ print("Tried to equip null item."); return; }
    if(dat.itemType == Item.EQUIPMENT){ doll.EquipFromInventory(itemIndex); }
    else{ arms.EquipFromInventory(itemIndex); }
  }
  
  /* Attempts to dual equip to arms by its index. */
  public void DualEquip(int itemIndex){ arms.DualEquipFromInventory(itemIndex);}
  
  
  /* Removes number of available ammo, up to max, and returns that number*/
  public int RequestAmmo(string ammoName,int max){
    for(int i = 0; i < inventory.slots; i++){
      if(
          inventory.inv != null 
          && inventory.inv[i] != null
          && inventory.inv[i].displayName == ammoName
        ){
        int available = inventory.Retrieve(i, max).stack;
        return available;
      }
    }
    return 0;
  }

  /* Adds item data to inventory, returning the slot it was stored in. */
  public int StoreItem(Data item){
    int remainder = inventory.Store(item);
    int slot = inventory.IndexOf(item);
    if(remainder > 0){
      item = new Data(item);
      item.stack = remainder;
      DiscardItem(item); 
    }
    return slot;
  }
  
  /* Discards all items in inventory. */
  public void DiscardAllItems(){
    for(int i = 0; i < inventory.slots; i++){ 
      DiscardItem(i); 
    }
  }  
  
  /* Drops item onto ground from inventory. */
  public Item DiscardItem(int slot ){
    if(inventory.Peek(slot) == null){ return null; }
    Data dat = inventory.Retrieve(slot, inventory.Peek(slot).stack);
    if(dat == null){ return null;  }  
    GameObject prefab = Resources.Load("Prefabs/" + dat.prefabName) as GameObject;
    if(prefab == null){ print("Prefab null:" + dat.prefabName);  return null;}
    GameObject itemGO = (GameObject)GameObject.Instantiate(
      prefab,
      hand.transform.position,
      Quaternion.identity
    );
    Item item = itemGO.GetComponent<Item>();
    item.LoadData(dat);
    itemGO.transform.position = hand.transform.position;
    return item;
  }
  
  /* Drops item onto ground based on data. */
  public void DiscardItem(Data dat){
    if(dat == null){ return; }
    GameObject prefab = Resources.Load("Prefabs/" + dat.prefabName) as GameObject;
    GameObject itemGO = (GameObject)GameObject.Instantiate(
      prefab,
      hand.transform.position,
      Quaternion.identity
    );
    Item item = itemGO.GetComponent<Item>();
    item.LoadData(dat);
    item.stack = dat.stack;
    itemGO.transform.position = hand.transform.position;
  }
  
  /* Convenience method. */
  public bool Alive(){
    return !stats.dead;
  }
  
  /* Interact with item in reach.
     i is the argument for the interaction, if relevant 
     -2 will not reload.
  */
  public void Interact(int mode = -1){
    if(itemInReach){
      Item item = itemInReach.GetComponent<Item>();
      if(item){ item.Interact(this, mode); }
    }
    else if(actorInReach){
      Actor actor = actorInReach.GetComponent<Actor>();
      if(actor == null){ return; }
      if(actor.speechTree != null && mode == -1 && actor.Alive()){
        interlocutor = actor;
        menu.Change("SPEECH");
        SetMenuOpen(true);
      }
      else if(mode == 0 && actor.Alive()){
        print("Steal from " + actor.gameObject.name);
      }
      else if(!actor.Alive()){
        menu.contents = actor.inventory;
        menu.Change("LOOT");
        SetMenuOpen(true);
      }
    }
    else if( mode != -2){
      Use(2);
    }
    
  }
  
  /* Pick up item in world. */
  public void PickUp(Item item){
    if(!item || !pickupCooldown || item is Ability){ return; }
    StartCoroutine(PickupCooldown());
    Data dat = item.GetData();
    Destroy(item.gameObject);
    StoreItem(dat);
  }
  
  /* Prevents picking up the same items. */
  public IEnumerator PickupCooldown(){
    pickupCooldown = false;
    yield return new WaitForSeconds(0.1f);
    pickupCooldown = true;
  }
  
  /* Returns data not stored in prefab for this Actor */
  public Data GetData(){
    Data dat = new Data();
    dat.displayName = displayName;
    dat.prefabName = prefabName;
    dat.x = transform.position.x;
    dat.y = transform.position.y;
    dat.z = transform.position.z;
    Vector3 rot = head.transform.rotation.eulerAngles;
    dat.xr = rot.x;
    dat.yr = rot.y;
    dat.zr = rot.z;
    dat.stack = 1;
    dat.stackSize = 1;
    dat.ints.Add(id);
    arms.Save();
    dat.equipSlot = arms;
    dat.doll = doll;
    dat.inventoryRecord = inventory.GetData();
    dat.lastPos = lastPos;
    dat.strings.Add(speechTreeFile);
    dat.speechTree = speechTree;
    return dat;
  }
  
  /* Loads data not specified for this prefab for this Actor */
  public void LoadData(Data dat){
    displayName = dat.displayName;
    transform.position = new Vector3(dat.x, dat.y, dat.z);
    transform.rotation = Quaternion.identity;
    headRotx = dat.xr;
    headRoty = dat.yr;
    bodyRoty = dat.yr;
    int i = 0;
    arms = dat.equipSlot;
    doll = dat.doll;
    arms.Load(offHand, hand, this);
    inventory.LoadData(dat.inventoryRecord);
    id = dat.ints[i]; i++;
    lastPos = dat.lastPos;
    speechTree = dat.speechTree;
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
