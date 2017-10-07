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
  Dictionary<int, Value> buttons;
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
    Dictionary<int, Value> ret = new Dictionary<int, Value>();
    mouseDownTimes = new float[3];
    mouseDownTimes[0] = -1.0f;
    mouseDownTimes[1] = -1.0f;
    mouseDownTimes[2] = -1.0f;
    
    ret.Add(InputEvent.K_Q, new Value(KeyCode.Q));
    ret.Add(InputEvent.K_W, new Value(KeyCode.W));
    ret.Add(InputEvent.K_E, new Value(KeyCode.E));
    ret.Add(InputEvent.K_R, new Value(KeyCode.R));
    ret.Add(InputEvent.K_T, new Value(KeyCode.T));
    ret.Add(InputEvent.K_Y, new Value(KeyCode.Y));
    ret.Add(InputEvent.K_U, new Value(KeyCode.U));
    ret.Add(InputEvent.K_I, new Value(KeyCode.I));
    ret.Add(InputEvent.K_O, new Value(KeyCode.O));
    ret.Add(InputEvent.K_P, new Value(KeyCode.P));
    
    ret.Add(InputEvent.K_A, new Value(KeyCode.A));
    ret.Add(InputEvent.K_S, new Value(KeyCode.S));
    ret.Add(InputEvent.K_D, new Value(KeyCode.D));
    ret.Add(InputEvent.K_F, new Value(KeyCode.F));
    ret.Add(InputEvent.K_G, new Value(KeyCode.G));
    ret.Add(InputEvent.K_H, new Value(KeyCode.H));
    ret.Add(InputEvent.K_J, new Value(KeyCode.J));
    ret.Add(InputEvent.K_K, new Value(KeyCode.K));
    ret.Add(InputEvent.K_L, new Value(KeyCode.L));
    
    ret.Add(InputEvent.K_Z, new Value(KeyCode.Z));
    ret.Add(InputEvent.K_X, new Value(KeyCode.X));
    ret.Add(InputEvent.K_C, new Value(KeyCode.C));
    ret.Add(InputEvent.K_V, new Value(KeyCode.V));
    ret.Add(InputEvent.K_B, new Value(KeyCode.B));
    ret.Add(InputEvent.K_N, new Value(KeyCode.N));
    ret.Add(InputEvent.K_M, new Value(KeyCode.M));

    ret.Add(InputEvent.K_UP, new Value(KeyCode.UpArrow));
    ret.Add(InputEvent.K_DOWN, new Value(KeyCode.DownArrow));
    ret.Add(InputEvent.K_LEFT, new Value(KeyCode.LeftArrow));
    ret.Add(InputEvent.K_RIGHT, new Value(KeyCode.RightArrow));
    ret.Add(InputEvent.K_TAB, new Value(KeyCode.Tab));
    ret.Add(InputEvent.K_L_SHIFT, new Value(KeyCode.LeftShift));
    ret.Add(InputEvent.K_L_CTRL, new Value(KeyCode.LeftControl));
    ret.Add(InputEvent.K_ENTER, new Value(KeyCode.Return));
    ret.Add(InputEvent.K_ESC, new Value(KeyCode.Escape));
    ret.Add(InputEvent.K_BACKSPACE, new Value(KeyCode.Backspace));
    
    // Numbers on top of alphanumeric keyboard
    ret.Add(InputEvent.K_0, new Value(KeyCode.Alpha0));
    ret.Add(InputEvent.K_1, new Value(KeyCode.Alpha1));
    ret.Add(InputEvent.K_2, new Value(KeyCode.Alpha2));
    ret.Add(InputEvent.K_3, new Value(KeyCode.Alpha3));
    ret.Add(InputEvent.K_4, new Value(KeyCode.Alpha4));
    ret.Add(InputEvent.K_5, new Value(KeyCode.Alpha5));
    ret.Add(InputEvent.K_6, new Value(KeyCode.Alpha6));
    ret.Add(InputEvent.K_7, new Value(KeyCode.Alpha7));
    ret.Add(InputEvent.K_8, new Value(KeyCode.Alpha8));
    ret.Add(InputEvent.K_9, new Value(KeyCode.Alpha9));
    
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
    
    Dictionary<int, Value> ret = new Dictionary<int, Value>();
    string jb = "joystick button ";
    triggers = new float[2];
    triggers[0] = -1.0f;
    triggers[1] = -1.0f;
    
    ret.Add(InputEvent.X360_A, new Value(jb + "0"));
    ret.Add(InputEvent.X360_B, new Value(jb + "1"));
    ret.Add(InputEvent.X360_X, new Value(jb + "2"));
    ret.Add(InputEvent.X360_Y, new Value(jb + "3"));
    ret.Add(InputEvent.X360_LB, new Value(jb + "4"));
    ret.Add(InputEvent.X360_RB, new Value(jb + "5"));
    ret.Add(InputEvent.X360_BACK, new Value(jb + "6"));
    ret.Add(InputEvent.X360_START, new Value(jb + "7"));
    ret.Add(InputEvent.X360_LSTICKCLICK, new Value(jb + "9"));
    ret.Add(InputEvent.X360_RSTICKCLICK, new Value(jb + "10"));
    ret.Add(InputEvent.X360_DRIGHT, new Value(jb + "11"));
    ret.Add(InputEvent.X360_DLEFT, new Value(jb + "12"));
    ret.Add(InputEvent.X360_DUP, new Value(jb + "13"));
    ret.Add(InputEvent.X360_DDOWN, new Value(jb + "14"));

    buttons = ret;
  }
  
  public List<InputEvent> GetInputs(){
    switch(device){
      case "KEYBOARD AND MOUSE": return KBMInputs(); break;
      case "XBOX 360 CONTROLLER": return Xbox360Inputs(); break;
      default: return new List<InputEvent>();
    }
  }
  
  /* Return input actions from keyboard keys. */
  public List<InputEvent> ButtonActions(){
    List<InputEvent> ret = new List<InputEvent>();
    foreach(KeyValuePair<int, Value> entry in buttons){
      InputEvent action = ButtonAction(entry.Key, entry.Value);
      if(action != null){ ret.Add(action); }
    }
    return ret;
  }
  
  /* Returns DOWN, HELD, or UP action from this button, or null */
  private InputEvent ButtonAction(int name, Value v){
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
  private List<InputEvent> KBMInputs(){
    List<InputEvent> ret = new List<InputEvent>();
    ret.AddRange(ButtonActions());
    float x, y;
    
    x = -Input.GetAxis("Mouse Y") * sensitivityX;
    y = Input.GetAxis("Mouse X") * sensitivityY;
    if(x > 0 || x < 0 || y > 0 || y < 0){ ret.Add(Axis(InputEvent.MOUSE, x, y)); }
    
    for(int i = 0; i < 3; i++){
      int mkey = 0;
      switch(i){
        case 0: mkey = InputEvent.MOUSE_0; break;
        case 1: mkey = InputEvent.MOUSE_0; break;
        case 2: mkey = InputEvent.MOUSE_0; break;
      }
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
    if(x > 0){ ret.Add(Down(InputEvent.MOUSE_UP)); }
    else if(x < 0){ ret.Add(Down(InputEvent.MOUSE_DOWN)); }
    
    return ret;
  }
  
  /* Returns inputs gathered from Xbox 360 controller. */
  private List<InputEvent> Xbox360Inputs(){
    List<InputEvent> ret = new List<InputEvent>();
    ret.AddRange(ButtonActions());
    float x, y;
    
    x = Input.GetAxis("DX");
    if(x > 0){
      if(buttons[InputEvent.X360_DRIGHT].downTime < 0){
        ret.Add(Down(InputEvent.X360_DRIGHT));
        buttons[InputEvent.X360_DRIGHT].downTime = UnityEngine.Time.time;
      }
      else{
        ret.Add(Held(InputEvent.X360_DRIGHT, buttons[InputEvent.X360_DRIGHT].downTime));
      }
    }
    else if(x < 0){
      if(buttons[InputEvent.X360_DLEFT].downTime < 0){
        ret.Add(Down(InputEvent.X360_DLEFT));
        buttons[InputEvent.X360_DLEFT].downTime = UnityEngine.Time.time;
      }
      else{
        ret.Add(Held(InputEvent.X360_DLEFT, buttons[InputEvent.X360_DLEFT].downTime));
      }
    }
    if(x >= 0 && buttons[InputEvent.X360_DLEFT].downTime > 0){
        ret.Add(Up(InputEvent.X360_DLEFT, buttons[InputEvent.X360_DLEFT].downTime));
        buttons[InputEvent.X360_DLEFT].downTime = -1.0f;
    }
    if(x <= 0 && buttons[InputEvent.X360_DRIGHT].downTime > 0){
        ret.Add(Up(InputEvent.X360_DRIGHT, buttons[InputEvent.X360_DRIGHT].downTime));
        buttons[InputEvent.X360_DRIGHT].downTime = -1.0f;
    }
    
    y = Input.GetAxis("DY");
    if(y < 0){
      if(buttons[InputEvent.X360_DUP].downTime < 0){
        ret.Add(Down(InputEvent.X360_DUP));
        buttons[InputEvent.X360_DUP].downTime = UnityEngine.Time.time;
      }
      else{
        ret.Add(Held(InputEvent.X360_DUP, buttons[InputEvent.X360_DUP].downTime));
      }
    }
    else if(y > 0){
      if(buttons[InputEvent.X360_DDOWN].downTime < 0){
        ret.Add(Down(InputEvent.X360_DDOWN));
        buttons[InputEvent.X360_DDOWN].downTime = UnityEngine.Time.time;
      }
      else{
        ret.Add(Held(InputEvent.X360_DDOWN, buttons[InputEvent.X360_DDOWN].downTime));
      }
    }
    if(y <= 0 && buttons[InputEvent.X360_DDOWN].downTime > 0){
        ret.Add(Up(InputEvent.X360_DDOWN, buttons[InputEvent.X360_DDOWN].downTime));
        buttons[InputEvent.X360_DDOWN].downTime = -1.0f;
    }
    if(y >= 0 && buttons[InputEvent.X360_DUP].downTime > 0){
        ret.Add(Up(InputEvent.X360_DUP, buttons[InputEvent.X360_DUP].downTime));
        buttons[InputEvent.X360_DUP].downTime = -1.0f;
    }
    
    x = Input.GetAxis("LT");
    if(x > 0){
      if(triggers[0] > 0){
        ret.Add(Held(InputEvent.X360_LT, triggers[0]));     
      }
      else{
        ret.Add(Down(InputEvent.X360_LT));
        triggers[0] = UnityEngine.Time.time;
      }
    }
    else if( triggers[0] > 0){
      ret.Add(Up(InputEvent.X360_LT, triggers[0]));
      triggers[0] = -1.0f;
    }
    
    x = Input.GetAxis("RT");
    if(x > 0){
      if(triggers[1] > 0){
        ret.Add(Held(InputEvent.X360_RT, triggers[1]));     
      }
      else{
        ret.Add(Down(InputEvent.X360_RT));
        triggers[1] = UnityEngine.Time.time;
      }
    }
    else if( triggers[1] > 0){
      ret.Add(Up(InputEvent.X360_RT, triggers[1]));
      triggers[1] = -1.0f;
    }
    
    x = Input.GetAxis("XL");
    y = -Input.GetAxis("YL");
    if( x != 0 || y != 0){ ret.Add(Axis(InputEvent.X360_LEFTSTICK, x, y)); }
    
    x = Input.GetAxis("YR") * sensitivityX;
    y = Input.GetAxis("XR") * sensitivityY;
    if( x != 0 || y != 0){ ret.Add(Axis(InputEvent.X360_RIGHTSTICK, x, y)); }
    
    return ret;
  }
  
  /* Returns a down InputEvent. */
  private InputEvent Down(int button){
    return new InputEvent(button, InputEvent.DOWN, 0.0f);
  }
  
  /* Returns a held InputEvent. */
  private InputEvent Held(int button, float downTime){
    return new InputEvent(button, InputEvent.HELD, downTime);
  }
  
  /* Returns an up InputEvent. */
  private InputEvent Up(int button, float downTime){
    return new InputEvent(button, InputEvent.UP, downTime);
  }
  
  /* Returns an axis InputEvent with only an x value. */
  private InputEvent Axis(int axis, float x){
    return new InputEvent(axis, x, 0.0f);
  }
  
  /* Returns an axis InputEvent with an x and y value */
  private InputEvent Axis(int axis, float x, float y){
    return new InputEvent(axis, x, y);
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
