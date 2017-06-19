/*
        Author: James Justice
        
        An actor script controls a character's stats,
        model, input/AI, inventory, and speech.

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
  public string displayName;
  public string prefabName;
  public int playerNumber; // 1 for keyboard, 3-4 for controller, 5 for NPC
  public Cell lastPos = null; // Used to place player in correct cell
  
  //Body parts
  public GameObject head;    // Gameobject containing the camera.
  public GameObject hand;    // GameObject where items are attached.
  public GameObject offHand; // Secondary hand where items are attached.
  public GameObject spine;   // Pivot point of toros
  public GameObject body;           // The base gameobject of the actor

  
  //UI
  public MenuManager menu;
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

  //Movement
  public bool ragdoll = false;
  public float speed;
  bool walking = false;
  bool sprinting = false;
  bool crouched = false;
  
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
  
  // Equipped items and abilities.
  public EquipSlot arms;
  
  // Speech
  public Actor interlocutor; // Conversation partner
  public string speechTreeFile; // File for speech tree to use.
  public SpeechTree speechTree; // Speech tree if this is an NPC Actor
  
  // AI
  public AI ai;

  
  /* Before Start */
  void Awake(){
    body = gameObject;
    inventory = new Inventory();
    arms = new EquipSlot();
    arms.hand = hand;
    arms.offHand = offHand;
    stats = new StatHandler();
  }
  
  /* Before rest of code */
  void Start(){
    if(stats.level == 0){ stats.LevelUp();}
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
    if(spine && !ragdoll){
      spine.transform.rotation = Quaternion.Euler(new Vector3(
        headRotx,
        headRoty,
        spine.transform.rotation.z));
    }                                    
  }
  
  /* Notifies menu, if it exists. */
  public void Notify(string message){
    if(menu != null){ menu.Notify(message); }
  }
  
  /* 0 No AI
  *  1-4 Input routine + initialize HUD
  *  >4 Initialize AI module
  */
  void AssignPlayer(int player){
    playerNumber = player;
    StartCoroutine(RegenRoutine());
    if(player <5 && player > 0){
      stats.abilities.Add(0);
      stats.abilities.Add(1);
      stats.abilities.Add(2);
      stats.abilities.Add(3);
      EquipAbility(0, true);
      EquipAbility(0, false);
      SetMenuOpen(false);
      if(head){ 
        menu = head.GetComponent<MenuManager>(); 
      }
      if(menu){ menu.Change("HUD");  menu.actor = this; }
      if(Session.session){ Session.session.RegisterPlayer(this, player, head.GetComponent<Camera>()); }
      if(player == 1){ StartCoroutine(KeyboardInputRoutine()); }
      else{StartCoroutine(ControllerInputRoutine()); }
    }
    else if(player == 5){
      stats.abilities.Add(0);
      EquipAbility(0, true);
      EquipAbility(0, false);
      ai = gameObject.GetComponent<AI>();
      if(ai){ ai.Begin(this); }
      if(speechTreeFile != ""){ speechTree = new SpeechTree(speechTreeFile); }
    }
  }
  
  /* Returns an empty string or the info of the active item. */
  public string ItemInfo(){
    return arms.Info();
  }
  
  /* Returns an empty string, or info about interacting with the actor in reach */
  public string ActorInteractionText(){
    string text = "";
    if(crouched){ text += "Steal from "; }
    else{ text += "Talk to "; }
    text += actorInReachName;
    return text;
  }
  
  /* Sets controls between menu and in-game contexts. */
  public void SetMenuOpen(bool val){
    menuOpen = val;
    if(playerNumber == 1){
      if(!val){ Cursor.lockState = CursorLockMode.Locked; }
      else{ Cursor.lockState = CursorLockMode.None; }
      Cursor.visible = !val;
    }
  }
  
  IEnumerator RegenRoutine(){
    while(stats.health >= 0){
      stats.Update();
      yield return new WaitForSeconds(0.2f);
    }
  }
  
  /* Handles input from keyboard. */
  IEnumerator KeyboardInputRoutine(){
    while(true){
      if(!menuOpen){
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
    sprinting = shift;
    float x = 0f;
    float z = 0f;
    if(Input.GetKey(KeyCode.W)){ z = 1f; walk = true; }
    else if(Input.GetKey(KeyCode.S)){ z = -1f; walk = true; }
    if(Input.GetKey(KeyCode.A)){ x = -1f; walk = true; }
    else if(Input.GetKey(KeyCode.D)){ x = 1f; walk = true; }
    if(x != 0f || z != 0f){ StickMove(x, z); }
    if(Input.GetKeyDown(KeyCode.Space)){ StartCoroutine(JumpRoutine()); }
    if(Input.GetKeyDown(KeyCode.LeftControl)){ToggleCrouch(); }
    if(Input.GetKeyUp(KeyCode.LeftControl)){ToggleCrouch(); }
    
    //Mouse controls
    if(Input.GetMouseButtonDown(0)){ Use(0); }
    if(Input.GetMouseButtonDown(1)){ Use(1); }
    if(Input.GetMouseButton(0)){ Use(3); } // Charge Left
    if(Input.GetMouseButton(1)){ Use(4); } // Charge right
    if(Input.GetMouseButtonUp(0)){ Use(5); } // Release left
    if(Input.GetMouseButtonUp(1)){ Use(6); } // Release right
    float rotx = -Input.GetAxis("Mouse Y") * sensitivityX;
    float roty = Input.GetAxis("Mouse X") * sensitivityY;
    Turn(new Vector3(rotx, roty, 0f));
    
    //Special use keys
    if(Input.GetKeyDown(KeyCode.Escape)){ 
      menu.Change("OPTIONS");
      SetMenuOpen(true); 
    }
    if(Input.GetKeyDown(KeyCode.R)){ Use(2); }
    if(Input.GetKeyDown(KeyCode.Q)){ Drop(); }
    if(Input.GetKeyDown(KeyCode.E)){ Interact(); }
    if(shift && Input.GetKeyDown(KeyCode.E)){ Interact(0); }
    if(Input.GetKeyDown(KeyCode.LeftArrow)){ Interact(1); }
    if(Input.GetKeyDown(KeyCode.RightArrow)){ Interact(2); }
    if(Input.GetKeyDown(KeyCode.DownArrow)){ Interact(3); }
    if(Input.GetKeyDown(KeyCode.Backspace)){ Interact(4); }
    if(Input.GetKeyDown(KeyCode.F)){ Use(7); }
    if(Input.GetKeyDown(KeyCode.Tab)){ 
      SetMenuOpen(true);
      if(menu){ 
        menu.Change("INVENTORY");
        SetMenuOpen(true); 
      }
    }
  }
  
  /* Handles pause menu keyboard input. */
  void KeyboardMenuInput(){
    if(!menu){ SetMenuOpen(false); } // Return control if menu not available
    if(Input.GetKeyDown(KeyCode.Tab)){ menu.Press(Menu.B); }
    if(Input.GetKeyDown(KeyCode.Escape)){ menu.Press(Menu.B); }
    if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)){
      menu.Press(Menu.UP);
    }
    if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)){
      menu.Press(Menu.DOWN);
    }
    if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
      menu.Press(Menu.RIGHT);
    }
    if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
      menu.Press(Menu.LEFT);
    }
    if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E)){
      menu.Press(Menu.A);
    }
    if(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.RightShift)){
      menu.Press(Menu.X);
    }
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
    bool shift = Input.GetKey(Session.LB);
    bool walk = xl != 0f || yl != 0;
    sprinting = shift;
    StickMove(xl, -yl);
    Turn(new Vector3(yr, xr, 0f));
    
    //Buttons
    if(Input.GetKeyDown(Session.START) && menu){ 
      menu.Change("OPTIONS"); 
      SetMenuOpen(true);
    }
    if(Input.GetKeyDown(Session.A)){ StartCoroutine(JumpRoutine()); }
    if(Input.GetKeyDown(Session.B)){ Use(7); }
    if(Input.GetKeyDown(Session.X)){ Interact(); }
    else if(Input.GetKeyDown(Session.X)){ Use(2); }
    if(Input.GetKeyDown(Session.Y) && menu){ 
      menu.Change("INVENTORY");
      SetMenuOpen(true); 
    }
    if(rt > 0 && !rt_down){ Use(0); rt_down = true;}    // Use right
    else if(rt > 0){ Use(3); }                          // Hold right
    if(rt == 0 && rt_down){ rt_down = false; Use(5); }  // Release right
    if(lt > 0 && !lt_down){ Use(1); lt_down = true;}    // Use left
    else if(lt > 0){ Use(4); }                          // Hold left
    if(lt == 0 && lt_down){ lt_down = false; Use(6); }  // Release left
    
    if(Input.GetKeyDown(Session.LSC)){ ToggleCrouch(); }
    if(Input.GetKeyDown(Session.RB)){ Drop(); }
  }
  
  /* Handles pause menu controller input. */
  void ControllerMenuInput(){
    if(!menu){ SetMenuOpen(false); } // Return control if menu not available
    if(Input.GetKeyDown(Session.A)){ menu.Press(Menu.A); }
    if(Input.GetKeyDown(Session.B)){ menu.Press(Menu.B); }
    if(Input.GetKeyDown(Session.X)){ menu.Press(Menu.X); }
    if(Input.GetKeyDown(Session.Y)){ menu.Press(Menu.Y); }
    
    float xl = Input.GetAxis(Session.XL);
    float yl = -Input.GetAxis(Session.YL);
    float rt = Input.GetAxis(Session.RT);
    float lt = Input.GetAxis(Session.LT);
    
    if(menuMove && xl > 0f){ menu.Press(Menu.RIGHT); StartCoroutine(MenuCooldown()); }
    else if(menuMove && xl < 0f){ menu.Press(Menu.LEFT); StartCoroutine(MenuCooldown()); }
    if(menuMove && yl > 0f){ menu.Press(Menu.UP); StartCoroutine(MenuCooldown()); }
    else if(menuMove && yl < 0f){ menu.Press(Menu.DOWN); StartCoroutine(MenuCooldown()); }
    if(menuMove && rt > 0){ menu.Press(Menu.RT); StartCoroutine(MenuCooldown()); }
    if(menuMove && lt > 0){ menu.Press(Menu.LT); StartCoroutine(MenuCooldown()); }
    
  }
  
  /* Cooldown for joystick menu movement */
  IEnumerator MenuCooldown(){
    menuMove = false;
    yield return new WaitForSeconds(menuMovementDelay);
    menuMove = true;
  }
  
  /* Move in a direction 
    0 = Forward
    1 = Backward
    2 = Left
    3 = Right
  */
  public void Move(int direction){
    if(ragdoll){ return; }
    Rigidbody rb = body.GetComponent<Rigidbody>();
    if(rb == null){ return; }
    float pace = speed;
    Vector3 pos = body.transform.position;
    Vector3 dir  = new Vector3();
    if(sprinting){ pace *= 1.75f; }
    if(!jumpReady){ pace *= 0.75f; }
    switch(direction){
      case 0:
        dir = body.transform.forward;
        break;
      case 1:
        dir = -body.transform.forward;
        break;
      case 2:
        dir = -body.transform.right;
        break;
      case 3:
        dir = body.transform.right;
        break;
    }
    ExecuteMove(pace, dir);
  }
  
  /* Move relative to transform.forward and transform.right */
  public void StickMove(float x, float y){
    if(ragdoll){  return; }
    Vector3 xdir = x * body.transform.right;
    Vector3 ydir = y * body.transform.forward;
    Vector3 dir = (xdir + ydir).normalized;
    Rigidbody rb =  body.GetComponent<Rigidbody>();
    float pace = speed;
    if(sprinting){ pace *= 1.75f; }
    if(!jumpReady){ pace *= 0.75f; }
    ExecuteMove(pace, dir);
  }
  
  /* Move relative to x and z positions.(Used for AI) */
  public void AxisMove(float x, float z){
    if(ragdoll){  return; }
    Vector3 dir = new Vector3(x, 0f, z).normalized;
    Rigidbody rb = body.GetComponent<Rigidbody>();
    float pace = speed;
    if(sprinting){ pace *= 1.75f; }
    if(!jumpReady){ pace *= 0.75f; }
    ExecuteMove(pace, dir);
  }
  
  /* Attempts to move the Actor. Applies stamina limitations.
     If cannot move, tries to move at 45 degree angle */
  void ExecuteMove(float pace, Vector3 dir){
    Vector3 dest = body.transform.position +  dir * pace;
    Rigidbody rb = body.GetComponent<Rigidbody>();
    int runCost = (int)Vector3.Magnitude(pace * dir * 6);
    if(stats.StatCheck("ENDURANCE", 50)){ runCost = 0; }
    if(MoveCheck(dir, pace * 3)){
      rb.MovePosition(dest);
      return;
    }
    dir += body.transform.up;
    dir = dir.normalized;
    if(MoveCheck(dir, pace * 3)){
      pace = speed;
      dest = body.transform.position +  dir * pace;
      rb.MovePosition(dest);
    }
  }
  
  /* Returns true if the Actor has clearance to move in a particular direction
  a particular distance. */
  bool MoveCheck(Vector3 direction, float distance){
    Vector3 center = body.transform.position;
    Vector3 halfExtents = body.transform.localScale / 2;
    Quaternion orientation = body.transform.rotation;
    int layerMask = ~(1 << 8);
    RaycastHit hit;
    bool check = !Physics.BoxCast(
      center,
      halfExtents,
      direction,
      out hit,
      orientation,
      distance,
      layerMask,
      QueryTriggerInteraction.Ignore
    );
    return check;
  }
  
  /* Boxcasts to find the current item in reach, updating itemInReach if they
    do not match. */
  void UpdateReach(){
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
    bool itemFound = false;
    for(int i = 0; i < found.Length; i++){
      Item item = found[i].collider.gameObject.GetComponent<Item>();
      bool holding = false;
      if(item == arms.handItem || item == arms.offHandItem){ holding = true; }
      if(item && !holding){
        itemInReach = item.gameObject;
        itemFound = true;
        break;
      }
    }
    if(!itemFound){ itemInReach = null; }
    bool actorFound = false;
    for(int i = 0; i < found.Length; i++){
      Actor other = found[i].collider.gameObject.GetComponent<Actor>();
      if(other && other != this){
        actorInReach = other.gameObject;
        actorInReachName = other.displayName;
        actorFound = true;
        break;
      }
    }
    if(!actorFound){ actorInReach = null; }
    return;
  }  

  
  /* Rotates head along xyz, torso over x axis*/
  public void Turn(Vector3 direction){
    if(ragdoll){ return; }
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
  
  /* Restores Jump upon landing. */
  void OnCollisionEnter(Collision col){
    if(falling){ Land(fallOrigin - transform.position.y); }
  }
  
  /* Restores jump upon standing still. */
  void OnCollisionStay(Collision col){
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
    crouched = !crouched;
  }
  
  /* Add xp and check for levelup conditions. */
  public void ReceiveXp(int amount){
    stats.AwardXP(amount);
  }
  
  /* Applies damage from attack. Ignores active weapon. */
  public void ReceiveDamage(int damage, GameObject weapon){
    if(stats.health < 1 || (weapon == arms.handItem && damage > 0)){ return; }
    stats.DrainCondition("HEALTH", damage);
    if(stats.health < 1){ Die(weapon); }
    else if (damage > 0){
      bool check = stats.StatCheck("ENDURANCE", damage);
      if(check){ StartCoroutine(Stagger()); }
      else{ StartCoroutine(FallDown()); }
    }
  }
  
  /* Enter dead state, warding experience to the killder. */
  public void Die(GameObject weapon){
    StopAllCoroutines();
    Ragdoll(true);
    if(ai){ ai.Pause(); }
    Item item = weapon.GetComponent<Item>();
    if(item && item.holder){
      item.holder.ReceiveXp(stats.NextLevel(stats.level -1)); 
    }
  }
  
  /* Actor rocks a bit. */
  IEnumerator Stagger(){
    float sa = 10; // Stagger angle
    Vector3 rot = body.transform.rotation.eulerAngles;
    body.transform.rotation = Quaternion.Euler(rot.x+ sa, rot.y, rot.z);
    speed -= 0.1f;
    yield return new WaitForSeconds(0.1f);
    speed += 0.1f;
    rot = body.transform.rotation.eulerAngles;
    body.transform.rotation = Quaternion.Euler(rot.x- sa, rot.y, rot.z);
  }
  
  /* Actor is knocked over */
  IEnumerator FallDown(){
    Ragdoll(true);
    int delay = 100 - stats.health - Random.Range(0, (stats.endurance * stats.agility));
    if(delay < 3){ delay = 3; }
    yield return new WaitForSeconds(Random.Range(0, (float)delay));
    float x = Mathf.Abs(body.transform.rotation.eulerAngles.x);
    float z = Mathf.Abs(body.transform.rotation.eulerAngles.z);
    Vector3 rot = new Vector3();
    while(z > 0 && x > 0){
      rot = body.transform.rotation.eulerAngles;
      if(rot.x > 0f){ x = -1f; }
      else if(rot.x < 0f){ x = 1f; }
      if(rot.z > 0f){ z = -1f; }
      else if(rot.z < 0f){ z = 1f; }
      body.transform.rotation = Quaternion.Euler(rot.x+x, rot.y, rot.z+z);
      rot = body.transform.rotation.eulerAngles;      
      x = Mathf.Abs(rot.x);
      z = Mathf.Abs(rot.y);
      if(x < 1f || z < 1f){
        body.transform.rotation = Quaternion.Euler(0f, rot.y, 0f);
        x = 0f;
        z = 0f;
      }
      yield return new WaitForSeconds(0.01f);
    }
    Vector3 pos = body.transform.position;
    body.transform.position = new Vector3( pos.x, pos.y+1f, pos.z);
    Ragdoll(false);
  }
  
  /* Adds or removes the ragdoll effect on the actor. */
  void Ragdoll(bool state){
    ragdoll = state;
    Rigidbody rb = body.GetComponent<Rigidbody>();
    if(state){
      if(ai){ ai.Pause(); }
      rb.constraints = RigidbodyConstraints.None; 
    }
    else{
      if(ai){ ai.Resume(); }
      rb.constraints = RigidbodyConstraints.FreezeRotationX |
                      RigidbodyConstraints.FreezeRotationY |
                      RigidbodyConstraints.FreezeRotationZ;
    }
  }
  
  /* Equips ability to selected hand, storing items displaced. */
  public void EquipAbility(int ability, bool primary){
    List<Data> displaced = arms.EquipAbility(ability, primary);
    for(int i = 0; i < displaced.Count; i++){
      displaced[i].stack = inventory.Store(displaced[i]);
      if(displaced[i].stack > 0){ DiscardItem(displaced[i]); }
    }
  }
  
  /* Use primary or secondary item */
  public void Use(int use){ arms.Use(use); }
  
  /* Drops an item from actor's arms.. */
  public void Drop(){ arms.Drop(); }
  
  /* Selects an item in inventory to equip. */
  public void Equip(int itemIndex, bool primary){
    if(itemIndex < 0 || itemIndex >= inventory.slots){ return; }
    Data dat = inventory.Retrieve(itemIndex);
    List<Data> displaced = arms.Equip(dat, primary);
    for(int i = 0; i < displaced.Count; i++){
      displaced[i].stack = inventory.Store(displaced[i]);
      if(displaced[i].stack > 0){
        DiscardItem(displaced[i]);
      }
    }
  }
  
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
  

  /* Adds item data to inventory */
  public void StoreItem(Data item){
    int remainder = inventory.Store(item);
    if(remainder > 0){
      item = new Data(item);
      item.stack = remainder;
      DiscardItem(item); 
    }
  }
  
  /* Drops item onto ground from inventory. */
  public Item DiscardItem(int slot){
    Data dat = inventory.Retrieve(slot);
    if(dat == null){ return null;  }    
    GameObject prefab = Resources.Load("Prefabs/" + dat.prefabName) as GameObject;
    GameObject itemGO = (GameObject)GameObject.Instantiate(
      prefab,
      hand.transform.position,
      Quaternion.identity
    );
    Item item = itemGO.GetComponent<Item>();
    item.LoadData(dat);
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
    item.stack = 1;
    itemGO.transform.position = hand.transform.position;
  }
  
  /* Convenience method. */
  public bool Alive(){
    return stats.health >= 1;
  }
  
  /* Interact with item in reach.
     i is the argument for the interaction, if relevant */
  public void Interact(int mode = -1){
    if(itemInReach){
      Item item = itemInReach.GetComponent<Item>();
      if(item){ item.Interact(this, mode); }
    }
    if(actorInReach){
      Actor actor = actorInReach.GetComponent<Actor>();
      if(actor && actor.speechTree != null && mode == -1 && actor.Alive()){
        interlocutor = actor;
        menu.Change("SPEECH");
        SetMenuOpen(true);
      }
      else if(actor && mode == 0 && actor.Alive()){
        print("Steal from " + actor.gameObject.name);
      }
      else if(actor && mode == -1){
        menu.contents = actor.inventory;
        menu.Change("LOOT");
        SetMenuOpen(true);
      }
      else{
        if(!actor){ print("Actor null"); }
        if(actor && actor.speechTree == null){ print("Speech tree null"); }
      }
    }
  }
  
  /* Pick up item in world. */
  public void PickUp(Item item){
    if(!item || !pickupCooldown){ return; }
    StartCoroutine(PickupCooldown());
    Data dat = item.GetData();
    Destroy(item.gameObject);
    StoreItem(dat);
    Destroy(item.gameObject);
    
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
    arms.Load(this, hand, offHand);
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
