using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Actor{
  public string displayName;
  public string prefabName;
  public playerNumber;
  
  //Body parts
  public GameObject spine;
  
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

  //Inventory
  public GameObject defaultItem;
  public GameObject activeItem;
  public GameObject itemInReach;
  public string itemInReachName;
  public List<Data> inventory = new List<Data>();
  
  public HUDController hc;
  public Region reg;
  
  //Player stats
  public hp = 100;
  
  /* Before Start */
  void Awake(){
    //TODO
  }
  
  /* Before rest of code */
  void Start(){
  //TODO
  }
  
  /* Late-cycle loop */
  void LateUpdate(){
  //TODO
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
  
  /* walk in direction at speed */
  public void Move(int direction){
  //TODO
  }
  
  /* Rotates head along xyz, torso over x axis*/
  void Turn(Vector3 direction){
  //TODO
  }
  
  /* Jumps */
  IEnumerator JumpRoutine(){
  //TODO
  }
  
  /* Toggles model's crouch */
  void ToggleCrouch(){
  //TODO
  }
  
  /* Applies damage from attack. Ignores active weapon. */
  void ReceiveDamage(int damage, GameObject weapon){
  //TODO
  }
  
  /* Use active item according to its type */
  void Use(int use){
  //TODO
  }
  
  /* Selects an item in inventory. */
  public void Equip(item itemIndex){
  //TODO
  }
  
  /* Removes number of available ammo, up to max, and returns that number*/
  public int RequestAmmo(string ammoName,int max){
  //TODO
  }
  
  /* Adds item data to inventory */
  public void StoreItem(Data item){
  //TODO
  }
  
  /* Drops item onto ground from inventory. */
  public void DiscardItem(int itemIndex){
  //TODO
  }
  
  /* Interact with item or other Actor.
     i is the argument for the interaction, if relevant */
  public void Interact(int i = -1){
  //TODO
  }
  
  /* Drops active item from hand */
  public void Drop(){
  //TODO
  }
  
  /* Returns non-prefab  data for this Actor */
  public Data GetData(){
  //TODO
  }
  
  /* Loads non-prefab data for this Actor */
  public void LoadData(Data dat){
  //TODO
  }
   
  /* Initiates conversation with other actor */
  public void TalkTo(Actor other){
  //TODO
  }
  
  /* Two-axis movement. used by AI */
  void Move(int x, int z){
  //TODO
  }
  
  /* Finds node nearest to Actor */
  NavNode NearestNode(){
  //TODO
  }
  
  /* move to a specific node. 
     If no node specified by reference or index,
     goes to nearest node.*/
  IEnumerator ToNode(int index = -1, NavNode node = null){
  //TODO
  }
  
  /* Moves to each node in order of list. */
  IENumerator TakePath(List<int> path){
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
