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
  public string room;
  public string destName;
  public bool exterior, exteriorFacing;
  public int destId;
  public int id;
  
  public int deck; // Deck this door is loaded in.
  bool warped = false;
  
  public void Start(){
  }
  
  public override void Interact(Actor a, int mode = -1, string message = ""){
    Warp();
  }

  public Vector3 DestPos(){
    return transform.position + transform.forward * 2;
  }
  
  public Vector3 DestRot(){
    return transform.rotation.eulerAngles;
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
    dat.ints.Add(x);
    dat.ints.Add(y);
    dat.ints.Add(destId);
    dat.ints.Add(id);
    
    dat.strings.Add(room);
    dat.strings.Add(destName);
    return dat;
  }
  
  public override void LoadData(Data dat){
    LoadBaseData(dat);
    int i = 1;
    x = dat.ints[i]; i++;
    y = dat.ints[i]; i++;
    destId = dat.ints[i]; i++;
    id = dat.ints[i]; i++;
    
    int s = 0;
    room = dat.strings[s]; s++;
    destName = dat.strings[s]; s++;
  }
  
  public DoorRecord GetRecord(){
    return new DoorRecord(this);
  }
  
  public void LoadRecord(DoorRecord dr){
    building = dr.building;
    x = dr.x;
    y = dr.y;
  }
  
}
