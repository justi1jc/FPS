/*
    A warp door is Decor that moves the HoloDeck to a new location.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarpDoor : Decor{
  public string destBuilding;
  public string destCell;
  public HoloDeck deck; // Which deck is this door associated with?
  public int doorId; // Should conform to cardinal NSEW direction of room.
  public int dx, dy; // Destination's offset from current coords
  public bool interior; // True if this door leads to an interior
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
    int dest = doorId;
    if(interior){
      int dtx = dx + deck.focalCell.cell.x;
      int dty = dy + deck.focalCell.cell.y;
      Session.session.LoadInterior(destBuilding, destCell, dtx, dty, deck.id, dest);
    }
    else{
      int dtx = deck.focalCell.cell.x;
      int dty = deck.focalCell.cell.y;
      Session.session.LoadExterior(dtx, dty, deck.id, dest);
    }
  }
  
  public override Data GetData(){
    Data dat = GetBaseData();
    dat.strings.Add(destBuilding);
    dat.strings.Add(destCell);
    dat.ints.Add(doorId);
    dat.ints.Add(dx);
    dat.ints.Add(dy);
    return dat;
  }
  
  public override void LoadData(Data dat){
    LoadBaseData(dat);
    destBuilding = dat.strings[0];
    destCell = dat.strings[1];
    doorId = dat.ints[1];
    dx = dat.ints[2];
    dy = dat.ints[3];
    destPos = transform.position + transform.forward * 2;
    destRot = transform.rotation.eulerAngles; 
  }
}
