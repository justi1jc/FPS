/*
    The DeviceManager is assigned a device from which it gathers input actions.
    
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
      RB, LB
      START, BACK, XBOX
      DUP, DRIGHT, DLEFT, DDown
    Axes:
      LEFTSTICK
      RIGHTSTICK
      RT
      LT
    
*/

using System;
using System.Collections.Generic;

public class DeviceManager{
  string device;
  int id; // id of device, if multiple exist.
  
  public DeviceManager(string device, int id = 0){
    this.device = device.ToUpper();
    this.id = 0;
  }
  
  public List<string[]> GetInputs(){
    switch(device){
      case "KEYBOARD AND MOUSE": return KBMInputs(); break;
      case "XBOX 360 CONTROLLER": return Xbox360Inputs(); break;
      default: return new List<string[]>();
    }
  }
  
  /* Returns inputs from keyboard and mouse */
  private List<string[]> KBMInputs(){
    return new List<string[]>();
  }
  
  /* Returns inputs from Xbox 360 controller. */
  private List<string[]> Xbox360Inputs(){
    return new List<string[]>();
  }
}
