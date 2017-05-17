/*
        Author: James Justice
        
        An actor script controls a character's stats,
        model, input/AI, inventory, and speech.

        GameObject structure:
      Body| // Parent to others. Rigidbody, collider, Actor
          |
          |spine| // Pivot point for torso
          |     | Head| // Contains Camera
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
  public Menu menu;
  bool menuMove = true;// Used to govern joystick menu inputs.
  float menuMovementDelay = 0.15f; // How long to delay between menu movements
  
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
  public static readonly string[] currencies = {"Penny", "Nickel"};
  public int currency;
  public bool menuOpen;
  public GameObject primaryItem;
  public GameObject secondaryItem;
  public int primaryIndex = -1;
  public int secondaryIndex = -1;
  public GameObject actorInReach;
  public string actorInReachName;
  public GameObject itemInReach;
  public string itemInReachName;
  public List<Data> inventory = new List<Data>();
  public int weight = 0;
  public int weightMax = 100;
  
  //Player vitals
  public int health     = 100;
  public int mana       = 100;
  public int stamina    = 100;
  public int healthMax  = 100;
  public int manaMax    = 100;
  public int staminaMax = 100;
  
  // ICEPAWS attributes, max 10
  public int intelligence = 5; // Max mana
  public int charisma     = 5; //
  public int endurance    = 5; // Stamina, health regen
  public int perception   = 5; // accuracy
  public int agility      = 5; // speed, jump height
  public int willpower    = 5; // mana regen
  public int strength     = 5; // maxweight

  
  // Skill levels, max 100
  public int skillPoints = 100;
  public int ranged  = 50;
  public int melee   = 50;
  public int unarmed = 50;
  public int magic   = 50;
  public int stealth = 50;

  
  // Leveling
  public int nextLevel = 0;
  public int level = 0;
  public int xp    = 0;
  
  // abilities
  public bool[] abilities = {true, false, false, false, false};
  public int rightAbility = 1;   // Ability in right hand.
  public int leftAbility  = 1;   // Ability in left hand.
  public Item raItem = null;     // Ability Item in right hand.
  public Item laItem = null;     // Ability item in left hand.
  public bool raReady = true;    // Is right hand busy?
  public bool laReady = true;    // Is left hand busy?
  
  // Speech
  public Actor interlocutor; // Conversation partner
  public string speechTreeFile; // File for speech tree to use.
  public SpeechTree speechTree; // Speech tree if this is an NPC Actor
  
  // AI
  public AI ai;

  
  /* Before Start */
  void Awake(){
    body = gameObject;
  }
  
  /* Before rest of code */
  void Start(){
    raItem = hand.GetComponent<Item>(); 
    laItem = offHand.GetComponent<Item>();
    raItem.holder = this;
    laItem.holder = this;
    AssignPlayer(playerNumber);
    if(level == 0){ LevelUp(); xp = 50; }
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
  
  /* 0 No AI
  *  1-4 Input routine + initialize HUD
  *  >4 Initialize AI module
  */
  void AssignPlayer(int player){
    playerNumber = player;
    StartCoroutine(RegenRoutine());
    if(player <5 && player > 0){
      SetMenuOpen(false);
      if(head){ menu = head.GetComponent<Menu>(); }
      if(menu){ menu.Change(Menu.HUD);  menu.actor = this; }
      if(Session.session){ Session.session.RegisterPlayer(this, player, head.GetComponent<Camera>()); }
      if(player == 1){ StartCoroutine(KeyboardInputRoutine()); }
      else{StartCoroutine(ControllerInputRoutine()); }
    }
    else if(player == 5){
      ai = gameObject.GetComponent<AI>();
      if(ai){ ai.Begin(this); }
      if(speechTreeFile != ""){ speechTree = new SpeechTree(speechTreeFile); }
    }
  }
  
  /* Returns an empty string or the info of the active item. */
  public string ItemInfo(){
    string info = "";
    if(primaryItem){
      Item item = primaryItem.GetComponent<Item>();
      if(item){ info = item.GetInfo(); }
    }
    if(secondaryItem){
      Item item = secondaryItem.GetComponent<Item>();
      if(item){ info = item.GetInfo(); }
    }
    return info;
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
      Cursor.visible = !val;
      if(!val){ Cursor.lockState = CursorLockMode.Locked; }
      else{ Cursor.lockState = CursorLockMode.None; }
    }
  }
  
  IEnumerator RegenRoutine(){
    while(health >= 0){
      RegenCondition();
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
    if(Input.GetMouseButtonDown(0)){ Use(1); }
    if(Input.GetMouseButtonDown(1)){ Use(0); }
    if(Input.GetMouseButton(0)){ Use(4); } // Charge Left
    if(Input.GetMouseButton(1)){ Use(3); } // Charge right
    if(Input.GetMouseButtonUp(0)){ Use(6); } // Release left
    if(Input.GetMouseButtonUp(1)){ Use(5); } // Release right
    float rotx = -Input.GetAxis("Mouse Y") * sensitivityX;
    float roty = Input.GetAxis("Mouse X") * sensitivityY;
    Turn(new Vector3(rotx, roty, 0f));
    
    //Special use keys
    if(Input.GetKeyDown(KeyCode.Escape)){ menu.Change(Menu.OPTIONS); }
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
      if(menu){ menu.Change(Menu.INVENTORY); }
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
    if(Input.GetKeyDown(Session.START)){ menu.Change(Menu.OPTIONS); }
    if(Input.GetKeyDown(Session.A)){ StartCoroutine(JumpRoutine()); }
    if(Input.GetKeyDown(Session.B)){ Use(7); }
    if(Input.GetKeyDown(Session.X)){ Interact(); }
    else if(Input.GetKeyDown(Session.X)){ Use(2); }
    if(Input.GetKeyDown(Session.Y)){ SetMenuOpen(true);
    if(menu){ menu.Change(Menu.INVENTORY); } }
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
    if(EnduranceCheck(50)){ runCost = 0; }
    if(MoveCheck(dir, pace * 3)){
      if(StaminaCheck(runCost)){ rb.MovePosition(dest); }
      else{
        dest = body.transform.position +  speed * dir;
        rb.MovePosition(dest);
      }
      return;
    }
    dir += body.transform.up;
    dir = dir.normalized;
    if(MoveCheck(dir, pace * 3)){
      pace = speed;
      dest = body.transform.position +  dir * pace;
      if(StaminaCheck(runCost)){ rb.MovePosition(dest); }
      else{
        dest = body.transform.position + dir * speed;
        rb.MovePosition(dest);
      }
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
      if(item && item.gameObject == primaryItem){ holding = true; }
      if(item && item.gameObject == secondaryItem){ holding = true; }
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
    xp += amount;
    if(xp > nextLevel){ LevelUp(); }
  }
  
  /* Apply skill points, calculate xp for next level. */
  public void LevelUp(){
    level++;
    nextLevel = level * level * level * 100;
    skillPoints += 25;
  }
  
  
  /* Applies damage from attack. Ignores active weapon. */
  public void ReceiveDamage(int damage, GameObject weapon){
    
    if(health < 1 || (weapon == primaryItem && damage > 0)){ return; }
    health -= damage;
    if(health < 1){
      health = 0;
      StopAllCoroutines();
      Ragdoll(true);
      if(ai){ ai.Pause(); }
      Item item = weapon.GetComponent<Item>();
      if(item && item.holder){ item.holder.ReceiveXp(xp); }
      if(playerNumber < 5 && playerNumber > 0){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      }
      PopulateCurrencyLoot();
    }
    else{
      bool check = EnduranceCheck(damage);
      if(check){ StartCoroutine(Stagger()); }
      else{ StartCoroutine(FallDown()); }
    }
    if(health > healthMax){ health = healthMax; }
  }
  
  /* Reduce stamina.*/
  public void ReceiveFatigue(int amount){
    stamina -= amount;
    if(stamina < 0){ stamina = 0; }
  }
  
  /* Reduces mana. */
  public void ReceiveManaDrain(int amount){
    mana -= amount;
    if(mana < 0){ mana = 0; }
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
    int delay = 100 - health - Random.Range(0, (endurance * agility));
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
  
  /* Returns a string describing a given special ability */
  public string AbilityInfo(int ability){
    switch(ability){
      case 0:
        return "Unarmed";
      case 1:
        return "Fireball";
      case 2:
        return "Heal Self";
      case 3:
        return "Heal Other";
      case 4:
        return "AbilityE";
    }
    return "";
  }
  
  /* Performs a given ability. */
  void Ability(int ability, bool right, int use = 0){
    switch(ability){
      case 0:
          Punch(right, use);
        break;
      case 1:
        FireBall(right, use);
        break;
      case 2:
        HealSelf(right, use);
        break;
      case 3:
        HealOther(right, use);
        break;
      case 4:
        print("AbilityE");
        break;
      case 5:
        print("AbilityF");
        break;
      case 6:
        print("AbilityG");

        break;
    }
  }
  
  /* Melee attack
     0 Charges punch.
     1 executes punch.
  */
  void Punch(bool right, int use){
    Item item = right ? raItem : laItem;
    if(item.itemType != Item.MELEE){
      item.cooldown = 0.5f;
      item.damageStart = 0f;
      item.damageEnd = 0.25f;
      item.knockBack = strength * 50;
      item.itemType = Item.MELEE;
      item.chargeable = true;
      item.executeOnCharge = true;
      item.charge = 0;
      item.chargeMax = 25;
      item.damage = strength * (unarmed / 10 + 1);
    }
    if(item.charge == item.chargeMax -1 || use == 4){
      int dmg = (item.damage * item.charge) / item.chargeMax;
      if(StaminaCheck(dmg)){ item.Use(use); }
      else{ item.charge = 0; }
      return;
    }
    item.Use(use);
  }
  
  /* Charges a fireball, then launches it. */
  void FireBall(bool right, int use){
    Item item = right ? raItem : laItem;
    if(item.itemType != Item.RANGED){
      item.cooldown = 1.1f - (float)(willpower/10f);
      item.itemType = Item.RANGED;
      item.chargeable = true;
      item.executeOnCharge = false;
      item.projectile = "FireBall";
      item.charge = 0;
      item.chargeMax = 200 / willpower;
      item.muzzleVelocity = 50;
      item.impactForce = willpower * 50;
      item.damage = intelligence * (magic / 10 + 1);
      item.effectiveDamage = 0;
    }
    if(use == 4){
      int dmg = (item.damage * item.charge) / item.chargeMax;
      if(ManaCheck(dmg)){ item.ammo = 1; item.Use(use); }
      else{ item.charge = 0; }
      return;
    }
    item.ammo = 1;
    item.Use(use);
  }
  
  /* Instant health boost. */
  void HealSelf(bool right, int use){
    if(use != 0){ return; }
    Item item = right ? raItem : laItem;
    if(item.itemType != Item.FOOD){
      item.itemType = Item.FOOD;
      item.healing = intelligence * (magic / 10 + 1);
      item.cooldown = 1f;
    }
    if(!ManaCheck(item.healing)){ return; }
    Light l = item.gameObject.GetComponent<Light>();
    if(l){StartCoroutine(Glow(0.25f, Color.blue, l)); }
    item.stack = 2;
    item.Use(0);
  }
  
  void HealOther(bool right, int use){
    Item item = right ? raItem : laItem;
    if(item.itemType != Item.RANGED){
      item.cooldown = 1.1f - (float)(willpower/10f);
      item.itemType = Item.RANGED;
      item.chargeable = true;
      item.executeOnCharge = false;
      item.projectile = "HealBall";
      item.charge = 0;
      item.chargeMax = 200 / willpower;
      item.muzzleVelocity = 50;
      item.impactForce = willpower * 50;
      item.damage = -(intelligence * (magic / 10 + 1));
      item.effectiveDamage = 0;
    }
    if(use == 4){
      int hl = -1 * (item.damage * item.charge) / item.chargeMax;
      if(ManaCheck(hl)){ item.ammo = 1; item.Use(use); }
      else{ item.charge = 0; }
      return;
    }
    item.ammo = 1;
    item.Use(use);
  }
  
  IEnumerator Glow(float time, Color color, Light light){
    light.intensity = 2f;
    light.range = 10f;
    light.color = color;
    yield return new WaitForSeconds(time);
    light.intensity = 0f;
    light.range = 0f;
  }
  
  public void EquipAbility( int ability){
    if(primaryItem){ StorePrimary(); rightAbility = ability; return; }
    if(rightAbility == ability){ rightAbility = 0; return; }
    rightAbility = ability;
  }
  
  public void EquipAbilitySecondary(int ability){
    if(secondaryItem){ StoreSecondary(); leftAbility = ability; return; }
    if(leftAbility == ability){ leftAbility = 0; return; }
    leftAbility = ability; return;
  }
  
  /* Use primary or secondary item */
  public void Use(int use){
    Item primary, secondary;
    primary = secondary = null;
    if(primaryItem){ primary = primaryItem.GetComponent<Item>(); }
    if(secondaryItem){ secondary = secondaryItem.GetComponent<Item>(); }
    bool right = (rightAbility > -1) || primary;
    bool left  = (leftAbility > -1) || secondary;
    if(right && left){
      if(use==0){
       if(primary){primary.Use(0); return; }
       if(rightAbility > -1){ Ability(rightAbility, true); }
      }
      if(use==1){
        if(secondary){secondary.Use(0); return; }
        if(leftAbility > -1){ Ability(leftAbility, false); }
      }
      if(use==2){
        if(primary){ primary.Use(2); }
        if(secondary){ secondary.Use(2); }
      }
      if(use==3){
        if(primary){ primary.Use(3); return; }
        if(rightAbility > -1){ Ability(rightAbility, true, 3); return; }
      }
      if(use==4){
        if(secondary){ secondary.Use(3); return; }
        if(leftAbility > -1){ Ability(leftAbility, false, 3); return; }
      }
      if(use==5){
        if(primary){ primary.Use(4); return; }
        if(rightAbility > -1){ Ability(rightAbility, true, 4); return; }
      }
      if(use==6){
        if(secondary){ secondary.Use(4); return; }
        if(leftAbility > -1){ Ability(leftAbility, false, 4); return; }
      }
      if(use==7 && primary){ primary.Use(5); }
      
    }
    else if(right){
      if(use==3){
        if(primary){ primary.Use(3); return; }
        if(rightAbility > -1){ Ability(rightAbility, true, 3); return; }
      }
      if(use==5){
        if(primary){ primary.Use(4); return; }
        if(rightAbility > -1){ Ability(rightAbility, true, 4); return; }
      }
      if(primary){ primary.Use(use); return; }
      if(rightAbility > -1){ Ability(rightAbility, true); return;}
    }
    else if(left){
      if(use==4){
        if(secondary){ secondary.Use(3); return; }
        if(leftAbility > -1){ Ability(leftAbility, false, 3); return; }
      }
      if(use==6){
        if(secondary){ secondary.Use(4); return; }
        if(leftAbility > -1){ Ability(leftAbility, false, 4); return; }
      }
      if(secondary){ secondary.Use(use); return; }
      if(leftAbility > -1){ Ability(leftAbility, false); return;}
    }
  }
  
  /* Drops active item from hand */
  public void Drop(bool right = true){
    if(!right){
      if(secondaryItem){
        inventory.Remove(inventory[secondaryIndex]);
        if(secondaryIndex < primaryIndex){
          Item primary = primaryItem.GetComponent<Item>();
          inventory.Add(primary.GetData());
          Destroy(primaryItem);
          primaryIndex = -1;
          Equip(inventory.Count -1);
        }
        secondaryIndex = -1;
      }
    }
    if(primaryItem){ 
      primaryItem.transform.parent = null;
      Item item = primaryItem.GetComponent<Item>();
      if(item){ item.Drop(); }
      if( primaryIndex > -1 && primaryIndex < inventory.Count &&
          item.displayName == inventory[primaryIndex].displayName){
        if( secondaryIndex > primaryIndex &&
            secondaryIndex > -1 && secondaryIndex < inventory.Count){
          inventory.Remove(inventory[secondaryIndex]);
        }
        inventory.Remove(inventory[primaryIndex]); 
        if(secondaryIndex > primaryIndex){
          Item secondary = secondaryItem.GetComponent<Item>();
          inventory.Add(secondary.GetData());
          Destroy(secondaryItem);
          secondaryIndex = -1;
          EquipSecondary(inventory.Count -1);
        }
      }
      primaryItem = null;
      primaryIndex = -1;
    }
    else if( rightAbility > 0){ rightAbility = 0; }
    else if(secondaryItem){
      secondaryItem.transform.parent = null;
      Item item = secondaryItem.GetComponent<Item>();
      if(item){ item.Drop(); }
      if( secondaryIndex > -1 && secondaryIndex < inventory.Count &&
          item.displayName == inventory[secondaryIndex].displayName){
        if(primaryIndex < primaryIndex){ inventory.Remove(inventory[primaryIndex]); }
        inventory.Remove(inventory[secondaryIndex]);
        if(secondaryIndex < primaryIndex){
          Item primary = primaryItem.GetComponent<Item>();
          inventory.Add(primary.GetData());
          Destroy(primaryItem);
          primaryIndex = -1;
          Equip(inventory.Count -1);
        }
      }
      secondaryItem = null;
      secondaryIndex = -1;
    }
    else if(leftAbility > 0){ leftAbility = 0; }
  }
  
  /* Selects an item in inventory to equip. */
  public void Equip(int itemIndex){
    if(itemIndex < 0 || itemIndex >= inventory.Count){ return; }
    if(itemIndex == primaryIndex){ StorePrimary(); return; }
    if(itemIndex == secondaryIndex){ StoreSecondary(); return; }
    if(primaryIndex != -1 && secondaryIndex == -1){ 
      EquipSecondary(itemIndex);
      return;
    }
    StorePrimary();
    Data dat = inventory[itemIndex];
    GameObject prefab = Resources.Load(dat.prefabName) as GameObject;
    if(!prefab){ print("Prefab null:" + dat.displayName); return;}
    GameObject itemGO = (GameObject)GameObject.Instantiate(
      prefab,
      transform.position,
      Quaternion.identity
    );
    if(!itemGO){print("GameObject null:" + dat.displayName); return; }
    Item item = itemGO.GetComponent<Item>();
    item.LoadData(dat);
    itemGO.transform.parent = hand.transform;
    item.Hold(this);
    primaryItem = itemGO;
    primaryIndex = itemIndex;
    rightAbility = 0;
  }
  
  /* Selects an item in the inventory to equip to the off-hand. */
  public void EquipSecondary(int itemIndex){
    if(itemIndex < 0 || itemIndex >= inventory.Count){ return; }
    if(itemIndex == primaryIndex){ StorePrimary(); }
    if(itemIndex == secondaryIndex){ StoreSecondary(); return;}
    if(secondaryIndex != -1){ StoreSecondary(); }
    Data dat = inventory[itemIndex];
    GameObject prefab = Resources.Load(dat.prefabName) as GameObject;
    if(!prefab){ print("Prefab null:" + dat.displayName); return;}
    GameObject itemGO = (GameObject)GameObject.Instantiate(
      prefab,
      transform.position,
      Quaternion.identity
    );
    if(!itemGO){ print("GameObject null:" + dat.displayName); return; }
    Item item = itemGO.GetComponent<Item>();
    item.LoadData(dat);
    itemGO.transform.parent = offHand.transform;
    item.Hold(this);
    secondaryItem = itemGO;
    secondaryIndex = itemIndex;
    leftAbility = 0;
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
  
  /* Stores the primary item into the inventory. */
  public void StorePrimary(){
    if(!primaryItem){ return; }
    Item item = primaryItem.GetComponent<Item>();
    if(primaryIndex == -1){ StoreItem(item.GetData()); }
    else{ inventory[primaryIndex] = item.GetData(); }
    Destroy(primaryItem);
    primaryItem = null;
    primaryIndex = -1;
  }
  
  /* Stores the secondary item into the inventory. */
  public void StoreSecondary(){
    if(!secondaryItem){ return; }
    Item item = secondaryItem.GetComponent<Item>();
    if(secondaryIndex == -1){ StoreItem(item.GetData()); }
    else{ inventory[secondaryIndex] = item.GetData(); }
    Destroy(secondaryItem);
    secondaryItem = null;
    secondaryIndex = -1;
  }
  /* Adds item data to inventory */
  public void StoreItem(Data item){
    if(item.stack < 1){ return; }
    if(StoreCurrency(item.displayName, item.stack, item.baseValue)){ return; }
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
  
  /* If provided currency, adds said currency and returns true.
     otherwise returns false. */
  public bool StoreCurrency(string displayName, int amount, int val){
    int index = -1;
    for(int i = 0; i < currencies.Length; i++){
      if(displayName == currencies[i]){ index = i; break; }
    }
    if(index == -1){ return false; }
    currency +=  amount * val;
    return true;
  }
  
  /* Adds currency items to inventory.
     TODO: Make this optional. */
  public void PopulateCurrencyLoot(){
    /*
    GameObject igo = Session.session.Spawn(currencies[0], Vector3.zero);
    Data item = igo.GetComponent<Item>().GetData();
    while(currency > 0){
      if(item.stackSize <= currency){
        item.stack = item.stackSize;
        currency -= item.stack;
        StoreItem(item);
      }
      else{
        item.stack = currency;
        currency = 0;
        StoreItem(item);
      }
    }
    */
  }
  
  /* Drops item onto ground from inventory. */
  public Item DiscardItem(int itemIndex){
    if(itemIndex < 0 || itemIndex > inventory.Count){ return null; }
    if(itemIndex == primaryIndex){ StorePrimary(); }
    if(itemIndex == secondaryIndex){ StoreSecondary(); }
    Data dat = inventory[itemIndex];
    GameObject prefab = Resources.Load(dat.prefabName) as GameObject;
    GameObject itemGO = (GameObject)GameObject.Instantiate(
      prefab,
      hand.transform.position,
      Quaternion.identity
    );
    Item item = itemGO.GetComponent<Item>();
    item.LoadData(dat);
    item.stack = 1;
    itemGO.transform.position = hand.transform.position;
    dat.stack--;
    if(dat.stack < 1){ inventory.Remove(dat); }
    return item;
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
      if(actor && actor.speechTree != null && mode == -1 && actor.health > 0){
        interlocutor = actor;
        menu.Change(Menu.SPEECH);
      }
      else if(actor && mode == 0 && actor.health > 0){
        print("Steal from " + actor.gameObject.name);
      }
      else if(actor && mode == -1){
        menu.contents = actor.inventory;
        menu.Change(Menu.LOOT);
      }
      else{
        if(!actor){ print("Actor null"); }
        if(actor && actor.speechTree == null){ print("Speech tree null"); }
      }
    }
  }
  
  /* Pick up item in world. */
  public void PickUp(Item item){
    if(!item){ return; }
    Data dat = item.GetData();
    Destroy(item.gameObject);
    StoreItem(dat);
    Destroy(item.gameObject);
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
    dat.ints.Add(leftAbility);
    dat.ints.Add(rightAbility);
    dat.ints.Add(primaryIndex);
    dat.ints.Add(secondaryIndex);
    int pIndex = primaryIndex;
    int sIndex = secondaryIndex;
    StorePrimary();
    StoreSecondary();
    dat.inventory = new Inventory(inventory);
    primaryIndex = -1;
    secondaryIndex = -1;
    dat.lastPos = lastPos; 
    if(pIndex > -1){ Equip(pIndex); }
    if(sIndex > -1){ EquipSecondary(sIndex); }
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
    leftAbility = dat.ints[i]; i++;
    rightAbility = dat.ints[i]; i++;
    int pIndex = dat.ints[i]; i++;
    int sIndex = dat.ints[i]; i++;
    if(dat.inventory != null){ inventory.AddRange(dat.inventory.inv); }
    else{ print(displayName + "inventory data null");}
    if(pIndex > -1){ Equip(pIndex); }
    else{ EquipAbility(rightAbility); }
    if(sIndex > -1){ EquipSecondary(sIndex); }
    else{ EquipAbilitySecondary(leftAbility); }
    lastPos = dat.lastPos;
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


  /* Returns true and subtracts stamina if sufficient. */
  public bool StaminaCheck(int cost){
    if(stamina == 0){ return false; }
    if(cost <= stamina){ stamina-= cost; return true; }
    return false;
  }

  /* Rolls to regenerate different conditions */
  public void RegenCondition(){
    if(Random.Range(0, 100) <= health && health != 0){
      health++;
      if(health > 100){ health = 100; }
    }
    
    if(EnduranceCheck(20)){
      stamina++;
      if(stamina > 100){ stamina = 100; }
    }
    
    if(MagicCheck(25)){
      mana++;
      if(mana > 100){ mana = 100; }
    }
  }
  
  /* Returns true and subtracts mana if sufficient. */
  public bool ManaCheck(int cost){
    if(mana == 0){ return false; }
    if(cost <= mana){ mana-= cost; return true; }
    return false;
  }
  
  /* Returns true if player passes stealth check. */
  public bool StealthCheck(int viewerPerception = 0, int viewerDistance = 3){
    int sneakBonus = crouched ? 25 : 0;     // Max 25
    int sprintPenalty = sprinting ? -25 : 0;
    int skillBonus = stealth / 4;           // Max: 25
    int agilityBonus = agility * 2;         // Max: 20
    int distanceBonus = viewerDistance < 3 ? -1 : viewerDistance; 
    int viewerPenalty = viewerPerception * -1;
    int successSpace = sneakBonus + skillBonus + agilityBonus + distanceBonus;
    successSpace += sprintPenalty + viewerPenalty; // Penalties
    int roll = Random.Range(0, 100);
    if(roll <= successSpace){ return true; }
    return false;
  }
  
  /* Returns true if player passes endurance check. */
  public bool EnduranceCheck(int damage = 5){
    int enduranceBonus = health +  endurance * 5;
    int successSpace = enduranceBonus - damage; // Max 95
    int roll = Random.Range(0, 100);
    if(roll <= successSpace){ return true; }
    return false;
  }
  
  /* Returns true if player passes magic test.  */
  public bool MagicCheck(int difficulty = 0){
    int successSpace = willpower * magic;
    successSpace -= difficulty;
    int roll = Random.Range(0, 100);
    if(roll <= successSpace){ return true; }
    return false;
  }
}
