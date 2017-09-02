/*
    A warp door is Decor that moves the HoloDeck to a new location.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarpDoor : Decor{
  // DoorRecord data
  public int x, y;
  public int building;
  public string room, destName;
  public bool exterior, exteriorFacing;
  public int destId;
  public int id;
  
  public int deck; // Deck this door is loaded in.
  public Vector3 destPos;
  public Vector3 destRot;
  bool warped = false;
  
  public void Start(){
    destPos = transform.position + transform.forward * 2;
    destRot = transform.rotation.eulerAngles;
  }
  
  public override void Interact(Actor a, int mode = -1, string message = ""){
    Warp();
  }
  
  /* Warps to destination. */
  public void Warp(){
    if(exteriorFacing){
      Session.session.LoadOverworld(x, y, destId, deck, true);
    }
    else{
      Session.session.LoadRoom(building, room, destId, deck, true);
    }
  }
  
  public override Data GetData(){
    Data dat = GetBaseData();
    return dat;
  }
  
  public override void LoadData(Data dat){
    LoadBaseData(dat);
  }
  
  public DoorRecord GetRecord(){
    return new DoorRecord(this);
  }
}
