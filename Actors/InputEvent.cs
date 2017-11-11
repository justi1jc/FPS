/*
   An InputEvent stores the information about a particular input passed between
   DeviceManager.cs and ActorInputHandler.
   
   A button has an axis value of 0 (InputEvent.NONE), wheras an axis has a
   button value of o(InputEvent.NONE).
   
*/

using System;

public class InputEvent{  

  // Action performed on a button.
  public enum Actions { None, Up, Down, Held };
  
  // Device buttons
  public enum Buttons{
    None,
    A_0, A_1, A_2, A_3, A_4, A_5, A_6, A_7, A_8, A_9, // Alpha-numerics nums
    N_0, N_1, N_2, N_3, N_4, N_5, N_6, N_7, N_8, N_9, // Numpad nums
    Slash, Asterisk, N_Minus, N_Plus, N_Enter, N_Del, // Other numpad keys 
    A, B, C, D, E, F, G, H, I, J, K, L, M, // Alphabet
    N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
    Escape, Tilde, Tab, Caps, L_Shift, L_Ctrl, L_Alt, Space,
    F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, // F keys
    Up, Down, Left, Right,  // Arrow Keys
    Backspace, Enter, Minus, Plus, LeftParen, RightParen, Semicolon, Quote,
    Backslash, Comma, Period, ForwardSlash,
    // Mouse buttons
    M_0, M_1, M_2, M_Up, M_Down,
    
    // Controller buttons
    C_A, C_B, C_X, C_Y, // Alphabetical controller buttons
    LB, RB, LT, RT, DUp, DDown, DLeft, DRight, 
    LStick, RStick, Back, Start
  };
  
  // Device axes
  public enum Axes{ None, Mouse, LStick, RStick };
  
  // Button data
  public Buttons button;
  public Actions action;
  public float downTime; // The time this button has been held down.
  
  // Axis data
  public Axes axis; // 
  public float x, y; // X and Y values for an Axis
  
  /* Constructor for axis event. */
  public InputEvent(Axes axis, float x, float y){
    this.button = Buttons.None;
    this.action = Actions.None;
    this.downTime = 0.0f;
    
    this.axis = axis;
    this.x = x;
    this.y = y;
  }
  
  /* Constructor for button event. */
  public InputEvent(Buttons button, Actions action, float downTime){
    this.axis = Axes.None;
    this.x = this.y = 0.0f;
    
    this.button = button;
    this.action = action;
    this.downTime = downTime;
  }
  
  /* Returns true if this a button event. */
  public bool IsButton(){
    if(button == Buttons.None){ return false; }
    return true;
  }
}
