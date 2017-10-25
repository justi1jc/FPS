/*
    A Floater attempts to maintain a certain height by adjusting its upward
    force. 
    
    Must have a ConstantForce component attached.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Floater : Decor{
  public float desiredHeight;
  public float upwardForce = 9.81f;
  public bool toggleAble = true;
  public float correctionForce = 0.1f;
  bool toggled = true;
  int health = 100;
  
  void Start(){
    desiredHeight = transform.position.y;
    if(toggled){
      StartCoroutine(MaintainHeight());
    }
  }
  
  public override void Interact(Actor a, int mode = -1, string message = ""){
    if(toggleAble){
      toggled = !toggled;
      if(toggled){
        StartCoroutine(MaintainHeight());
      }
      else{
        StopCoroutine(MaintainHeight());
      }
    }
  }
  
  public override void ReceiveDamage(Damage dam){
    health -= dam.health;
    if(health <= 0){
      toggleAble = false;
      toggled = false;
      gameObject.GetComponent<ConstantForce>().force = new Vector3();
      StopCoroutine(MaintainHeight());
    }
  }
  
  public IEnumerator MaintainHeight(){
    ConstantForce cf = gameObject.GetComponent<ConstantForce>();
    Rigidbody rb = gameObject.GetComponent<Rigidbody>();
    while(toggled){
      if(transform.position.y < desiredHeight -1f){
        upwardForce = 9.81f + correctionForce;
        if(rb.velocity.y > 0.01){ upwardForce = 9.81f; }
      }
      else if(transform.position.y > desiredHeight +1f){
        upwardForce = 9.81f - correctionForce;
        if(rb.velocity.y < -0.01f){ upwardForce = 9.81f; }
      }
      else{
        upwardForce = 9.81f;
      }
      cf.force = new Vector3(0f, upwardForce, 0f);
      yield return new WaitForSeconds(0.25f);
    }
    yield return new WaitForSeconds(0f);
  }
  
  
}
