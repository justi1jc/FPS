/*
    The DeviceManager is assigned a device from which it gathers input actions.
    NOTE: All axes MUST be set up with Unity's input manager to match the names
    given in this file. Buttons will work without any setup.
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
    
    ret.Add((int)InputEvent.Buttons.Q, new Value(KeyCode.Q));
    ret.Add((int)InputEvent.Buttons.W, new Value(KeyCode.W));
    ret.Add((int)InputEvent.Buttons.E, new Value(KeyCode.E));
    ret.Add((int)InputEvent.Buttons.R, new Value(KeyCode.R));
    ret.Add((int)InputEvent.Buttons.T, new Value(KeyCode.T));
    ret.Add((int)InputEvent.Buttons.Y, new Value(KeyCode.Y));
    ret.Add((int)InputEvent.Buttons.U, new Value(KeyCode.U));
    ret.Add((int)InputEvent.Buttons.I, new Value(KeyCode.I));
    ret.Add((int)InputEvent.Buttons.O, new Value(KeyCode.O));
    ret.Add((int)InputEvent.Buttons.P, new Value(KeyCode.P));
    
    ret.Add((int)InputEvent.Buttons.A, new Value(KeyCode.A));
    ret.Add((int)InputEvent.Buttons.S, new Value(KeyCode.S));
    ret.Add((int)InputEvent.Buttons.D, new Value(KeyCode.D));
    ret.Add((int)InputEvent.Buttons.F, new Value(KeyCode.F));
    ret.Add((int)InputEvent.Buttons.G, new Value(KeyCode.G));
    ret.Add((int)InputEvent.Buttons.H, new Value(KeyCode.H));
    ret.Add((int)InputEvent.Buttons.J, new Value(KeyCode.J));
    ret.Add((int)InputEvent.Buttons.K, new Value(KeyCode.K));
    ret.Add((int)InputEvent.Buttons.L, new Value(KeyCode.L));
    
    ret.Add((int)InputEvent.Buttons.Z, new Value(KeyCode.Z));
    ret.Add((int)InputEvent.Buttons.X, new Value(KeyCode.X));
    ret.Add((int)InputEvent.Buttons.C, new Value(KeyCode.C));
    ret.Add((int)InputEvent.Buttons.V, new Value(KeyCode.V));
    ret.Add((int)InputEvent.Buttons.B, new Value(KeyCode.B));
    ret.Add((int)InputEvent.Buttons.N, new Value(KeyCode.N));
    ret.Add((int)InputEvent.Buttons.M, new Value(KeyCode.M));

    ret.Add((int)InputEvent.Buttons.Up, new Value(KeyCode.UpArrow));
    ret.Add((int)InputEvent.Buttons.Down, new Value(KeyCode.DownArrow));
    ret.Add((int)InputEvent.Buttons.Left, new Value(KeyCode.LeftArrow));
    ret.Add((int)InputEvent.Buttons.Right, new Value(KeyCode.RightArrow));
    ret.Add((int)InputEvent.Buttons.Tab, new Value(KeyCode.Tab));
    ret.Add((int)InputEvent.Buttons.L_Shift, new Value(KeyCode.LeftShift));
    ret.Add((int)InputEvent.Buttons.L_Ctrl, new Value(KeyCode.LeftControl));
    ret.Add((int)InputEvent.Buttons.Enter, new Value(KeyCode.Return));
    ret.Add((int)InputEvent.Buttons.Escape, new Value(KeyCode.Escape));
    ret.Add((int)InputEvent.Buttons.Backspace, new Value(KeyCode.Backspace));
    ret.Add((int)InputEvent.Buttons.Space, new Value(KeyCode.Space));
    
    // Numbers on top of alphanumeric keyboard
    ret.Add((int)InputEvent.Buttons.A_0, new Value(KeyCode.Alpha0));
    ret.Add((int)InputEvent.Buttons.A_1, new Value(KeyCode.Alpha1));
    ret.Add((int)InputEvent.Buttons.A_2, new Value(KeyCode.Alpha2));
    ret.Add((int)InputEvent.Buttons.A_3, new Value(KeyCode.Alpha3));
    ret.Add((int)InputEvent.Buttons.A_4, new Value(KeyCode.Alpha4));
    ret.Add((int)InputEvent.Buttons.A_5, new Value(KeyCode.Alpha5));
    ret.Add((int)InputEvent.Buttons.A_6, new Value(KeyCode.Alpha6));
    ret.Add((int)InputEvent.Buttons.A_7, new Value(KeyCode.Alpha7));
    ret.Add((int)InputEvent.Buttons.A_8, new Value(KeyCode.Alpha8));
    ret.Add((int)InputEvent.Buttons.A_9, new Value(KeyCode.Alpha9));
    
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
    
    ret.Add((int)InputEvent.Buttons.C_A, new Value(jb + "0"));
    ret.Add((int)InputEvent.Buttons.C_B, new Value(jb + "1"));
    ret.Add((int)InputEvent.Buttons.C_X, new Value(jb + "2"));
    ret.Add((int)InputEvent.Buttons.C_Y, new Value(jb + "3"));
    ret.Add((int)InputEvent.Buttons.LB, new Value(jb + "4"));
    ret.Add((int)InputEvent.Buttons.RB, new Value(jb + "5"));
    ret.Add((int)InputEvent.Buttons.Back, new Value(jb + "6"));
    ret.Add((int)InputEvent.Buttons.Start, new Value(jb + "7"));
    ret.Add((int)InputEvent.Buttons.LStick, new Value(jb + "9"));
    ret.Add((int)InputEvent.Buttons.RStick, new Value(jb + "10"));
    ret.Add((int)InputEvent.Buttons.DRight, new Value(jb + "11"));
    ret.Add((int)InputEvent.Buttons.DLeft, new Value(jb + "12"));
    ret.Add((int)InputEvent.Buttons.DUp, new Value(jb + "13"));
    ret.Add((int)InputEvent.Buttons.DDown, new Value(jb + "14"));

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
        return Up((InputEvent.Buttons)name, dt);
      }
      else if(Input.GetKeyDown(v.keyCode)){
        buttons[name].downTime = UnityEngine.Time.time;
        return Down((InputEvent.Buttons)name);
      }
      else if(Input.GetKey(v.keyCode)){
        return Held((InputEvent.Buttons)name, v.downTime);
      }
    }
    else{
      if(Input.GetKeyUp(v.button)){
        buttons[name].downTime = -1.0f;
        return Up((InputEvent.Buttons)name, dt);
      }
      else if(Input.GetKeyDown(v.button)){
        buttons[name].downTime = UnityEngine.Time.time;
        return Down((InputEvent.Buttons)name);
      }
      else if(Input.GetKey(v.button)){
        return Held((InputEvent.Buttons)name, v.downTime);
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
    if(x > 0 || x < 0 || y > 0 || y < 0){ 
      ret.Add(Axis(InputEvent.Axes.Mouse, x, y)); 
    }
    
    for(int i = 0; i < 3; i++){ ret.AddRange(MouseButton(i)); }
    
    x = Input.GetAxis("Mouse ScrollWheel");
    if(x > 0){ ret.Add(Down(InputEvent.Buttons.M_Up)); }
    else if(x < 0){ ret.Add(Down(InputEvent.Buttons.M_Down)); }
    
    return ret;
  }
  
  /* Returns events corresponding to the provided mouse button */
  private List<InputEvent> MouseButton(int button){
    InputEvent.Buttons btn = InputEvent.Buttons.None;
    List<InputEvent> ret = new List<InputEvent>();
    switch(button){
      case 0: btn = InputEvent.Buttons.M_0; break;
      case 1: btn = InputEvent.Buttons.M_1; break;
      case 2: btn = InputEvent.Buttons.M_2; break; 
    }
    if(btn == InputEvent.Buttons.None){ return ret; }
    if(Input.GetMouseButtonUp(button)){
      ret.Add(Up(btn, mouseDownTimes[button]));
      mouseDownTimes[button] = -1.0f;
    }
    else if(Input.GetMouseButtonDown(button)){
      ret.Add(Down(btn));
      mouseDownTimes[button] = UnityEngine.Time.time;
    }
    else if(Input.GetMouseButton(button)){
      ret.Add(Held(btn, mouseDownTimes[button]));
    }
    return ret;
  } 
  
  /* Returns inputs gathered from Xbox 360 controller. */
  private List<InputEvent> Xbox360Inputs(){
    List<InputEvent> ret = new List<InputEvent>();
    ret.AddRange(ButtonActions());
    float x, y;
    
    x = Input.GetAxis("DX");
    if(x > 0){
      if(buttons[(int)InputEvent.Buttons.DRight].downTime < 0){
        ret.Add(Down(InputEvent.Buttons.DRight));
        buttons[(int)InputEvent.Buttons.DRight].downTime = UnityEngine.Time.time;
      }
      else{
        ret.Add(
          Held(
            InputEvent.Buttons.DRight, 
            buttons[(int)InputEvent.Buttons.DRight].downTime
          )
        );
      }
    }
    else if(x < 0){
      if(buttons[(int)InputEvent.Buttons.DLeft].downTime < 0){
        ret.Add(Down(InputEvent.Buttons.DLeft));
        buttons[(int)InputEvent.Buttons.DLeft].downTime = UnityEngine.Time.time;
      }
      else{
        ret.Add(
          Held(
            InputEvent.Buttons.DLeft, 
            buttons[(int)InputEvent.Buttons.DLeft].downTime
          )
        );
      }
    }
    if(x >= 0 && buttons[(int)InputEvent.Buttons.DLeft].downTime > 0){
        ret.Add(
          Up(InputEvent.Buttons.DLeft,buttons[(int)InputEvent.Buttons.DLeft].downTime)
        );
        buttons[(int)InputEvent.Buttons.DLeft].downTime = -1.0f;
    }
    if(x <= 0 && buttons[(int)InputEvent.Buttons.DRight].downTime > 0){
        ret.Add(
          Up(
            InputEvent.Buttons.DRight, 
            buttons[(int)InputEvent.Buttons.DRight].downTime
          )
        );
        buttons[(int)InputEvent.Buttons.DRight].downTime = -1.0f;
    }
    
    y = Input.GetAxis("DY");
    if(y < 0){
      if(buttons[(int)InputEvent.Buttons.DUp].downTime < 0){
        ret.Add(Down(InputEvent.Buttons.DUp));
        buttons[(int)InputEvent.Buttons.DUp].downTime = UnityEngine.Time.time;
      }
      else{
        ret.Add(
          Held(
            InputEvent.Buttons.DUp, 
            buttons[(int)InputEvent.Buttons.DUp].downTime
          )
        );
      }
    }
    else if(y > 0){
      if(buttons[(int)InputEvent.Buttons.DDown].downTime < 0){
        ret.Add(Down(InputEvent.Buttons.DDown));
        buttons[(int)InputEvent.Buttons.DDown].downTime = UnityEngine.Time.time;
      }
      else{
        ret.Add(
          Held(
            InputEvent.Buttons.DDown, 
            buttons[(int)InputEvent.Buttons.DDown].downTime
          )
        );
      }
    }
    if(y <= 0 && buttons[(int)InputEvent.Buttons.DDown].downTime > 0){
        ret.Add(
          Up(
            InputEvent.Buttons.DDown, 
            buttons[(int)InputEvent.Buttons.DDown].downTime
          )
        );
        buttons[(int)InputEvent.Buttons.DDown].downTime = -1.0f;
    }
    if(y >= 0 && buttons[(int)InputEvent.Buttons.DUp].downTime > 0){
        ret.Add(
          Up(InputEvent.Buttons.DUp,
          buttons[(int)InputEvent.Buttons.DUp].downTime
        )
      );
        buttons[(int)InputEvent.Buttons.DUp].downTime = -1.0f;
    }
    
    x = Input.GetAxis("LT");
    if(x > 0){
      if(triggers[0] > 0){
        ret.Add(Held(InputEvent.Buttons.LT, triggers[0]));     
      }
      else{
        ret.Add(Down(InputEvent.Buttons.LT));
        triggers[0] = UnityEngine.Time.time;
      }
    }
    else if( triggers[0] > 0){
      ret.Add(Up(InputEvent.Buttons.LT, triggers[0]));
      triggers[0] = -1.0f;
    }
    
    x = Input.GetAxis("RT");
    if(x > 0){
      if(triggers[1] > 0){
        ret.Add(Held(InputEvent.Buttons.RT, triggers[1]));     
      }
      else{
        ret.Add(Down(InputEvent.Buttons.RT));
        triggers[1] = UnityEngine.Time.time;
      }
    }
    else if( triggers[1] > 0){
      ret.Add(Up(InputEvent.Buttons.RT, triggers[1]));
      triggers[1] = -1.0f;
    }
    
    x = Input.GetAxis("XL");
    y = -Input.GetAxis("YL");
    if( x != 0 || y != 0){ ret.Add(Axis(InputEvent.Axes.LStick, x, y)); }
    
    x = Input.GetAxis("YR") * sensitivityX;
    y = Input.GetAxis("XR") * sensitivityY;
    if( x != 0 || y != 0){ ret.Add(Axis(InputEvent.Axes.LStick, x, y)); }
    
    return ret;
  }
  
  /* Returns a down InputEvent. */
  private InputEvent Down(InputEvent.Buttons button){
    return new InputEvent(button, InputEvent.Actions.Down, 0.0f);
  }
  
  /* Returns a held InputEvent. */
  private InputEvent Held(InputEvent.Buttons button, float downTime){
    return new InputEvent(button, InputEvent.Actions.Held, downTime);
  }
  
  /* Returns an up InputEvent. */
  private InputEvent Up(InputEvent.Buttons button, float downTime){
    return new InputEvent(button, InputEvent.Actions.Up, downTime);
  }
  
  /* Returns an axis InputEvent with only an x value. */
  private InputEvent Axis(InputEvent.Axes axis, float x){
    return new InputEvent(axis, x, 0.0f);
  }
  
  /* Returns an axis InputEvent with an x and y value */
  private InputEvent Axis(InputEvent.Axes axis, float x, float y){
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
