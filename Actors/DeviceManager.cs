/*
    The DeviceManager is assigned a device from which it gathers input actions.
    NOTE: All axes MUST be set up with Unity's input manager to match the names
    given in this file. Buttons will work without any setup.
    
    Input actions are represented as an array of strings, where the first index
    is the action name and the remaining indices are arguments following the
    format provided below.
    
    ButtonDownAction: <Name>, DOWN
    ButtonHeldAction: <Name>, HELD, <duration as float>
    ButtonReleasedAction: <Name>, RELEASED, <duration as float>
    AXISMOVEACTION: <AXISName>, <X as float>[, <Y as float>]
    
    Supported devices and buttons/axes are listed below.
    
    Keyboard and Mouse-
    Buttons: 
      W,A,S,D, Q. E, R, F, G
      1, 2, 3, 4, 5, 6, 7, 8, 9, 0
      TAB. SHIFT, CTRL, ENTER, ESCAPE
      UP, DOWN, LEFT, RIGHT
      M0, M1, M2
    Axes:
      LOOK(Mouse)
      SCROLL(Mouse scrollwheel)

    XBOX 360 controller
    Buttons:
      A, B, Y, X
      RB, LB, RT, LT
      START, BACK, XBOX
      DUP, DDOWN, DLEFT, DRIGHT
    Axes:
      LEFTSTICK
      RIGHTSTICK
    
*/

using System;
using UnityEngine;
using System.Collections.Generic;

public class DeviceManager{
  private string device;
  Dictionary<string, Value> buttons;
  private float[] mouseDownTimes; // Mouse button downtimes
  
  public DeviceManager(string device){
    this.device = device.ToUpper();
    switch( this.device){
      case "KEYBOARD AND MOUSE": InitKBM(); break;
      case "XBOX 360 CONTROLLER": Init360(); break;
    }
  }
  
  /* Populate dictionaries with keyboard keys. */
  private void InitKBM(){
    Dictionary<string, Value> ret = new Dictionary<string, Value>();
    mouseDownTimes = new float[3];
    mouseDownTimes[0] = -1.0f;
    mouseDownTimes[1] = -1.0f;
    mouseDownTimes[2] = -1.0f;
    
    ret.Add("W", new Value(KeyCode.W));
    ret.Add("A", new Value(KeyCode.A));
    ret.Add("S", new Value(KeyCode.S));
    ret.Add("D", new Value(KeyCode.D));
    ret.Add("Q", new Value(KeyCode.Q));
    ret.Add("E", new Value(KeyCode.E));
    ret.Add("R", new Value(KeyCode.R));
    ret.Add("F", new Value(KeyCode.F));
    ret.Add("G", new Value(KeyCode.G));
    
    ret.Add("TAB", new Value(KeyCode.Tab));
    ret.Add("SHIFT", new Value(KeyCode.LeftShift));
    ret.Add("CTRL", new Value(KeyCode.LeftControl));
    ret.Add("ENTER", new Value(KeyCode.Return));
    ret.Add("ESCAPE", new Value(KeyCode.Escape));
    
    ret.Add("0", new Value(KeyCode.Alpha0));
    ret.Add("1", new Value(KeyCode.Alpha1));
    ret.Add("2", new Value(KeyCode.Alpha2));
    ret.Add("3", new Value(KeyCode.Alpha3));
    ret.Add("4", new Value(KeyCode.Alpha4));
    ret.Add("5", new Value(KeyCode.Alpha5));
    ret.Add("6", new Value(KeyCode.Alpha6));
    ret.Add("7", new Value(KeyCode.Alpha7));
    ret.Add("8", new Value(KeyCode.Alpha8));
    ret.Add("9", new Value(KeyCode.Alpha9));
    buttons = ret;
  }
  
  /* Populate dictionaries with Xbox 360 buttons. */
  private void Init360(){
    Dictionary<string, Value> ret = new Dictionary<string, Value>();
    string jb = "joystick button ";
    
    ret.Add("A", new Value(jb + "0"));
    ret.Add("B", new Value(jb + "1"));
    ret.Add("X", new Value(jb + "2"));
    ret.Add("Y", new Value(jb + "3"));
    ret.Add("LB", new Value(jb + "4"));
    ret.Add("RB", new Value(jb + "5"));
    ret.Add("BACK", new Value(jb + "6"));
    ret.Add("START", new Value(jb + "7"));
    ret.Add("LSTICKCLICK", new Value(jb + "9"));
    ret.Add("RSTICKCLICK", new Value(jb + "10"));
    ret.Add("DRIGHT", new Value(jb + "11"));
    ret.Add("DLEFT", new Value(jb + "12"));
    ret.Add("DUP", new Value(jb + "13"));
    ret.Add("DDOWN", new Value(jb + "14"));
    
    buttons = ret;
  }
  
  public List<string[]> GetInputs(){
    switch(device){
      case "KEYBOARD AND MOUSE": return KBMInputs(); break;
      case "XBOX 360 CONTROLLER": return Xbox360Inputs(); break;
      default: return new List<string[]>();
    }
  }
  
  /* Return input actions from keyboard keys. */
  public List<string[]> ButtonActions(){
    List<string[]> ret = new List<string[]>();
    foreach(KeyValuePair<string, Value> entry in buttons){
      string[] action = ButtonAction(entry.Key, entry.Value);
      if(action != null){ ret.Add(action); }
    }
    return ret;
  }
  
  /* Returns DOWN, HELD, or UP action from this key, or null */
  private string[] ButtonAction(string name, Value v){
    float dt = v.downTime;
    if(Input.GetKeyUp(v.keyCode)){
      buttons[name].downTime = -1.0f;
      return Up(name, dt);
    }
    else if(Input.GetKeyDown(v.keyCode)){
      buttons[name].downTime = UnityEngine.Time.time;
      return Down(name);
    }
    else if(Input.GetKey(v.keyCode)){
      return Held(name, v.downTime);
    }
    return null;
  }
  
  /* Returns inputs from keyboard and mouse. 
     Mouse buttons have to be handled specially.
  */
  private List<string[]> KBMInputs(){
    List<string[]> ret = new List<string[]>();
    ret.AddRange(ButtonActions());
    float x, y;
    
    x = -Input.GetAxis("Mouse Y");
    y = Input.GetAxis("Mouse X");
    if(x > 0 || x < 0 || y > 0 || y < 0){ ret.Add(Axis("MOUSE", x, y)); }
    
    for(int i = 0; i < 3; i++){
      string mkey = "m" + i;
      if(Input.GetMouseButtonUp(i)){
        ret.Add(Up(mkey, mouseDownTimes[i]));
        mouseDownTimes[i] = -1.0f;
      }
      else if(Input.GetMouseButtonDown(i)){
        ret.Add(Down(mkey));
        mouseDownTimes[i] = UnityEngine.Time.time;
      }
      else if(Input.GetMouseButton(i)){
        ret.Add(Held(mkey, mouseDownTimes[i]));
      }
    }
    return ret;
  }
  
  /* Returns inputs gathered from Xbox 360 controller. */
  private List<string[]> Xbox360Inputs(){
    List<string[]> ret = new List<string[]>();
    ret.AddRange(ButtonActions());
    float x, y;
    
    x = Input.GetAxis("DX");
    if(x > 0){
      //if(keyboard["DRIGHT"]){}
    }
    else if(x < 0){
    
    }
    else{
    
    }
    return ret;
  }
  
  /* Returns a down input action. */
  private string[] Down(string name){
    string[] ret = new string[2];
    ret[0] = name;
    ret[1] = "DOWN";
    return ret;
  }
  
  /* Returns a held input action. */
  private string[] Held(string name, float time){
    string[] ret = new string[3];
    ret[0] = name;
    ret[1] = "HELD";
    ret[2] = "" + (UnityEngine.Time.time - time);
    return ret;
  }
  
  /* Returns an up input action. */
  private string[] Up(string name, float time){
    string[] ret = new string[3];
    ret[0] = name;
    ret[1] = "UP";
    ret[2] = "" + (UnityEngine.Time.time - time);
    return ret;
  }
  
  /* Returns an axis input action with only an x value. */
  private string[] Axis(string name, float x){
    string[] ret = new string[2];
    ret[0] = name;
    ret[1] = "" + x;
    return ret;
  }
  
  /* Returns an axis input action with an x and y value */
  private string[] Axis(string name, float x, float y){
    string[] ret = new string[2];
    ret[0] = name;
    ret[1] = "" + x;
    return ret;
  }
  
  /* A three-field value to populate dictionaries with. */
  private class Value{
    public KeyCode keyCode;
    public string button;
    public float downTime;
    
    public Value(string button){
      this.button = button;
      downTime = -1.0f;  
    }
    
    public Value(KeyCode keyCode){
      this.keyCode = keyCode;
      downTime = -1.0f;  
    }
  }
}
