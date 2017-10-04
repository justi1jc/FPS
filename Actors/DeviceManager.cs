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
      DUP, DDOWN, DLEFT, DRIGHT (7th and 8th axes on controller.)
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
  private float[] triggers; // trigger downtimes
  private float sensitivityX, sensitivityY;
  public DeviceManager(string device){
    this.device = device.ToUpper();
    switch( this.device){
      case "KEYBOARD AND MOUSE": InitKBM(); break;
      case "XBOX 360 CONTROLLER": Init360(); break;
    }
  }
  
  /* Populate dictionaries with keyboard keys. */
  private void InitKBM(){
    if(PlayerPrefs.HasKey("mouseSensitivity")){
      sensitivityX = sensitivityY = PlayerPrefs.GetFloat("mouseSensitivity");
    }
    else{
      sensitivityX = sensitivityY = 1.0f;
      PlayerPrefs.SetFloat("mouseSensitivity", 1f);
    }
    Dictionary<string, Value> ret = new Dictionary<string, Value>();
    mouseDownTimes = new float[3];
    mouseDownTimes[0] = -1.0f;
    mouseDownTimes[1] = -1.0f;
    mouseDownTimes[2] = -1.0f;
    
    ret.Add("K_Q", new Value(KeyCode.Q));
    ret.Add("K_W", new Value(KeyCode.W));
    ret.Add("K_E", new Value(KeyCode.E));
    ret.Add("K_R", new Value(KeyCode.R));
    ret.Add("K_T", new Value(KeyCode.T));
    ret.Add("K_Y", new Value(KeyCode.Y));
    ret.Add("K_U", new Value(KeyCode.U));
    ret.Add("K_I", new Value(KeyCode.I));
    ret.Add("K_O", new Value(KeyCode.O));
    ret.Add("K_P", new Value(KeyCode.P));
    
    ret.Add("K_A", new Value(KeyCode.A));
    ret.Add("K_S", new Value(KeyCode.S));
    ret.Add("K_D", new Value(KeyCode.D));
    ret.Add("K_F", new Value(KeyCode.F));
    ret.Add("K_G", new Value(KeyCode.G));
    ret.Add("K_H", new Value(KeyCode.H));
    ret.Add("K_J", new Value(KeyCode.J));
    ret.Add("K_K", new Value(KeyCode.K));
    ret.Add("K_L", new Value(KeyCode.L));
    
    ret.Add("K_Z", new Value(KeyCode.Z));
    ret.Add("K_X", new Value(KeyCode.X));
    ret.Add("K_C", new Value(KeyCode.C));
    ret.Add("K_V", new Value(KeyCode.V));
    ret.Add("K_B", new Value(KeyCode.B));
    ret.Add("K_N", new Value(KeyCode.N));
    ret.Add("K_M", new Value(KeyCode.M));

    ret.Add("K_UP", new Value(KeyCode.UpArrow));
    ret.Add("K_DOWN", new Value(KeyCode.DownArrow));
    ret.Add("K_LEFT", new Value(KeyCode.LeftArrow));
    ret.Add("K_RIGHT", new Value(KeyCode.RightArrow));
    ret.Add("K_TAB", new Value(KeyCode.Tab));
    ret.Add("K_SHIFT", new Value(KeyCode.LeftShift));
    ret.Add("K_CTRL", new Value(KeyCode.LeftControl));
    ret.Add("K_ENTER", new Value(KeyCode.Return));
    ret.Add("K_ESCAPE", new Value(KeyCode.Escape));
    ret.Add("K_BACKSPACE", new Value(KeyCode.Backspace));
    
    // Numbers on top of alphanumeric keyboard
    ret.Add("K_0", new Value(KeyCode.Alpha0));
    ret.Add("K_1", new Value(KeyCode.Alpha1));
    ret.Add("K_2", new Value(KeyCode.Alpha2));
    ret.Add("K_3", new Value(KeyCode.Alpha3));
    ret.Add("K_4", new Value(KeyCode.Alpha4));
    ret.Add("K_5", new Value(KeyCode.Alpha5));
    ret.Add("K_6", new Value(KeyCode.Alpha6));
    ret.Add("K_7", new Value(KeyCode.Alpha7));
    ret.Add("K_8", new Value(KeyCode.Alpha8));
    ret.Add("K_9", new Value(KeyCode.Alpha9));
    
    // Numbers on keypad
    ret.Add("K_K_0", new Value(KeyCode.Alpha0));
    ret.Add("K_K_1", new Value(KeyCode.Alpha1));
    ret.Add("K_K_2", new Value(KeyCode.Alpha2));
    ret.Add("K_K_3", new Value(KeyCode.Alpha3));
    ret.Add("K_K_4", new Value(KeyCode.Alpha4));
    ret.Add("K_K_5", new Value(KeyCode.Alpha5));
    ret.Add("K_K_6", new Value(KeyCode.Alpha6));
    ret.Add("K_K_7", new Value(KeyCode.Alpha7));
    ret.Add("K_K_8", new Value(KeyCode.Alpha8));
    ret.Add("K_K_9", new Value(KeyCode.Alpha9));
    buttons = ret;
  }
  
  /* Populate dictionaries with Xbox 360 buttons. */
  private void Init360(){
    if(PlayerPrefs.HasKey("controllerSensitivity")){
      sensitivityX = sensitivityY = PlayerPrefs.GetFloat("controllerSensitivity");
    }
    else{
      sensitivityX = sensitivityY = 3.0f;
      PlayerPrefs.SetFloat("controllerSensitivity", 1f);
    }
    
    Dictionary<string, Value> ret = new Dictionary<string, Value>();
    string jb = "joystick button ";
    triggers = new float[2];
    triggers[0] = -1.0f;
    triggers[1] = -1.0f;
    
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
  
  /* Returns DOWN, HELD, or UP action from this button, or null */
  private string[] ButtonAction(string name, Value v){
    float dt = v.downTime;
    if(v.keyCode != KeyCode.None){
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
    }
    else{
      if(Input.GetKeyUp(v.button)){
        buttons[name].downTime = -1.0f;
        return Up(name, dt);
      }
      else if(Input.GetKeyDown(v.button)){
        buttons[name].downTime = UnityEngine.Time.time;
        return Down(name);
      }
      else if(Input.GetKey(v.button)){
        return Held(name, v.downTime);
      }
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
    
    x = -Input.GetAxis("Mouse Y") * sensitivityX;
    y = Input.GetAxis("Mouse X") * sensitivityY;
    if(x > 0 || x < 0 || y > 0 || y < 0){ ret.Add(Axis("MOUSE", x, y)); }
    
    for(int i = 0; i < 3; i++){
      string mkey = "M" + i;
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
    
    x = Input.GetAxis("Mouse ScrollWheel");
    if(x > 0){ ret.Add(Down("M_UP")); }
    else if(x < 0){ ret.Add(Down("M_DOWN")); }
    
    return ret;
  }
  
  /* Returns inputs gathered from Xbox 360 controller. */
  private List<string[]> Xbox360Inputs(){
    List<string[]> ret = new List<string[]>();
    ret.AddRange(ButtonActions());
    float x, y;
    
    x = Input.GetAxis("DX");
    if(x > 0){
      if(buttons["DRIGHT"].downTime < 0){
        ret.Add(Down("DRIGHT"));
        buttons["DRIGHT"].downTime = UnityEngine.Time.time;
      }
      else{
        ret.Add(Held("DRIGHT", buttons["DRIGHT"].downTime));
      }
    }
    else if(x < 0){
      if(buttons["DLEFT"].downTime < 0){
        ret.Add(Down("DLEFT"));
        buttons["DLEFT"].downTime = UnityEngine.Time.time;
      }
      else{
        ret.Add(Held("DLEFT", buttons["DLEFT"].downTime));
      }
    }
    if(x >= 0 && buttons["DLEFT"].downTime > 0){
        ret.Add(Up("DLEFT", buttons["DLEFT"].downTime));
        buttons["DLEFT"].downTime = -1.0f;
    }
    if(x <= 0 && buttons["DRIGHT"].downTime > 0){
        ret.Add(Up("DRIGHT", buttons["DRIGHT"].downTime));
        buttons["DRIGHT"].downTime = -1.0f;
    }
    
    y = Input.GetAxis("DY");
    if(y < 0){
      if(buttons["DUP"].downTime < 0){
        ret.Add(Down("DUP"));
        buttons["DUP"].downTime = UnityEngine.Time.time;
      }
      else{
        ret.Add(Held("DUP", buttons["DUP"].downTime));
      }
    }
    else if(y > 0){
      if(buttons["DDOWN"].downTime < 0){
        ret.Add(Down("DDOWN"));
        buttons["DDOWN"].downTime = UnityEngine.Time.time;
      }
      else{
        ret.Add(Held("DDOWN", buttons["DDOWN"].downTime));
      }
    }
    if(y <= 0 && buttons["DDOWN"].downTime > 0){
        ret.Add(Up("DDOWN", buttons["DDOWN"].downTime));
        buttons["DDOWN"].downTime = -1.0f;
    }
    if(y >= 0 && buttons["DUP"].downTime > 0){
        ret.Add(Up("DUP", buttons["DUP"].downTime));
        buttons["DUP"].downTime = -1.0f;
    }
    
    x = Input.GetAxis("LT");
    if(x > 0){
      if(triggers[0] > 0){
        ret.Add(Held("LT", triggers[0]));     
      }
      else{
        ret.Add(Down("LT"));
        triggers[0] = UnityEngine.Time.time;
      }
    }
    else if( triggers[0] > 0){
      ret.Add(Up("LT", triggers[0]));
      triggers[0] = -1.0f;
    }
    
    x = Input.GetAxis("RT");
    if(x > 0){
      if(triggers[1] > 0){
        ret.Add(Held("RT", triggers[1]));     
      }
      else{
        ret.Add(Down("RT"));
        triggers[1] = UnityEngine.Time.time;
      }
    }
    else if( triggers[1] > 0){
      ret.Add(Up("RT", triggers[1]));
      triggers[1] = -1.0f;
    }
    
    
    x = Input.GetAxis("XL");
    y = -Input.GetAxis("YL");
    if( x != 0 || y != 0){ ret.Add(Axis("LEFTSTICK", x, y)); }
    
    x = Input.GetAxis("YR") * sensitivityX;
    y = Input.GetAxis("XR") * sensitivityY;
    if( x != 0 || y != 0){ ret.Add(Axis("RIGHTSTICK", x, y)); }
    
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
    string[] ret = new string[4];
    ret[0] = name;
    ret[1] = "AXIS";
    ret[2] = "" + x;
    ret[3] = "" + y;
    return ret;
  }
  
  /* Button info for the buttons dictionary. */
  private class Value{
    public KeyCode keyCode;
    public string button;
    public float downTime;
    
    public Value(string button){
      this.button = button;
      this.keyCode = KeyCode.None;
      downTime = -1.0f;
    }
    
    public Value(KeyCode keyCode){
      this.keyCode = keyCode;
      this.button = "NONE";
      downTime = -1.0f;
    }
  }
}
