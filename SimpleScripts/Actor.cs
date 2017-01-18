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
  }
  
  /* Before rest of code */
  void Start(){
  }
  
  /* Late-cycle loop */
  void LateUpdate(){
  }
  
  /* 0 No AI
  *  1-4 Input routine + initialize HUD
  *  >4 Initialize AI module
  */
  void AssignPlayer(int player){
  }
  
  /* Handles input from keyboard */
  IEnumerator InputRoutine(){
  }
  
  /* walk in direction at speed */
  public void Move(int direction){
  }
  
  /* Rotates head along xyz, torso over x axis*/
  void Turn(Vector3 direction){
  }
  
  /* Jumps */
  IEnumerator JumpRoutine(){
  }
  
  /* Toggles model's crouch */
  void ToggleCrouch(){
  }
  
  /* Applies damage from attack. Ignores active weapon. */
  void ReceiveDamage(int damage, GameObject weapon){
  }
  
  /* Use active item according to its type */
  void Use(int use){
  }
  
  /* Selects an item in inventory. */
  public void Equip(item itemIndex){
  }
  
  /* Removes number of available ammo, up to max, and returns that number*/
  public int RequestAmmo(string ammoName,int max){
  }
  
  /* Adds item data to inventory */
  public void StoreItem(Data item){
  }
  
  /* Drops item onto ground from inventory. */
  public void DiscardItem(int itemIndex){
  }
  
  /* Interact with item or other Actor.
     i is the argument for the interaction, if relevant */
  public void Interact(int i = -1){
  }
  
  /* Drops active item from hand */
  public void Drop(){
  }
  
  /* Returns non-prefab  data for this Actor */
  public Data GetData(){
  }
  
  /* Loads non-prefab data for this Actor */
  public void LoadData(Data dat){
  }
   
  /* Initiates conversation with other actor */
  public void TalkTo(Actor other){
  }
  
  /* Two-axis movement. used by AI */
  void Move(int x, int z){
  }
  
  /* Finds node nearest to Actor */
  NavNode NearestNode(){
  }
  
  /* move to a specific node. 
     If no node specified by reference or index,
     goes to nearest node.*/
  IEnumerator ToNode(int index = -1, NavNode node = null){
  }
  
  /* Moves to each node in order of list. */
  IENumerator TakePath(List<int> path){
  }
  
  /* Calculates path between two nodes, if one exists. */
  public List<int> GetPath(int start, int finish){
  }
  
  /* Moves to a particular point in a straight line */
  public IEnumerator MoveTo(Vector3 dest){
  }
  
  /* Orients look toward a particular orientation. */
  public IEnumerator Aim(Vector3 eulers){
  }
  
  /* Returns the current look orientation */
  public Vector3 CurrentAim(){
  }
  
}
