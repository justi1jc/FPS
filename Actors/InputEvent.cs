/*
   An InputEvent stores the information about a particular input passed between
   DeviceManager.cs and ActorInputHandler.
   
   A button has an axis value of 0 (InputEvent.NONE), wheras an axis has a
   button value of o(InputEvent.NONE).
   
*/

using System;

public class InputEvent{
  public const int NONE = 0;
  
  // Held types
  public const int UP = 1;
  public const int HELD = 2;
  public const int DOWN = 3;
  
  // Keyboard buttons
  public const int K_0 = 1;
  public const int K_1 = 2;
  public const int K_2 = 3;
  public const int K_3 = 4;
  public const int K_4 = 5;
  public const int K_5 = 6;
  public const int K_6 = 7;
  public const int K_7 = 8;
  public const int K_8 = 9;
  public const int K_9 = 10;
  public const int K_A = 11;
  public const int K_B = 12;
  public const int K_C = 13;
  public const int K_D = 14;
  public const int K_E = 15;
  public const int K_F = 16;
  public const int K_G = 17;
  public const int K_H = 18;
  public const int K_I = 19;
  public const int K_J = 20;
  public const int K_K = 21;
  public const int K_L = 22;
  public const int K_M = 23;
  public const int K_N = 24;
  public const int K_O = 25;
  public const int K_P = 26;
  public const int K_Q = 27;
  public const int K_R = 28;
  public const int K_S = 29;
  public const int K_T = 30;
  public const int K_U = 31;
  public const int K_V = 32;
  public const int K_W = 33;
  public const int K_X = 34;
  public const int K_Y = 35;
  public const int K_Z = 36;
  public const int K_ESC = 37;
  public const int K_Tilde = 38;
  public const int K_F1 = 39;
  public const int K_F2 = 40;
  public const int K_F3 = 41;
  public const int K_F4 = 42;
  public const int K_F5 = 43;
  public const int K_F6 = 44;
  public const int K_F7 = 45;
  public const int K_F8 = 46;
  public const int K_F9 = 47;
  public const int K_F10 = 48;
  public const int K_F11 = 49;
  public const int K_F12 = 50;
  public const int K_TAB = 51;
  public const int K_CAPS = 52;
  public const int K_L_SHIFT = 53;
  public const int K_L_CTRL = 54;
  public const int K_L_ALT = 55;
  public const int K_SPACE = 56;
  public const int K_R_ALT = 57;
  public const int K_R_CTRL = 58;
  public const int K_UP = 59;
  public const int K_DOWN = 60;
  public const int K_LEFT = 61;
  public const int K_RIGHT = 62;
  public const int K_BACKSPACE = 63;
  public const int K_ENTER = 64;
  public const int K_MINUS = 65;
  public const int K_PLUS = 66;
  public const int K_LEFTPAREN = 67;
  public const int K_RIGHTPAREN = 68;
  public const int K_SEMICOLON = 69;
  public const int K_QUOTE = 70;
  public const int K_BACKSLASH = 71;
  public const int K_COMMA = 72;
  public const int K_PERIOD = 73;
  public const int K_FORWRDSLASH = 74;
  public const int K_N_SLASH = 75;
  public const int K_N_ASTERISK = 76;
  public const int K_N_MINUS = 77;
  public const int K_N_PLUS = 78;
  public const int K_N_0 = 79;
  public const int K_N_1 = 80;
  public const int K_N_2 = 81;
  public const int K_N_3 = 82;
  public const int K_N_4 = 83;
  public const int K_N_5 = 84;
  public const int K_N_6 = 85;
  public const int K_N_7 = 86;
  public const int K_N_8 = 87;
  public const int K_N_9 = 88;
  public const int K_N_ENTER = 89;
  public const int K_N_DEL = 90;
  
  // XBOX 360 Buttons
  public const int X360_A = 91;
  public const int X360_B = 92;
  public const int X360_X = 93;
  public const int X360_Y = 94;
  public const int X360_LB = 95;
  public const int X360_RB = 96;
  public const int X360_LT = 97;
  public const int X360_RT = 98;
  public const int X360_DUP = 99;
  public const int X360_DRIGHT = 100;
  public const int X360_DLEFT = 101;
  public const int X360_DDOWN = 102;
  public const int X360_LSTICKCLICK = 103;
  public const int X360_RSTICKCLICK = 104;
  public const int X360_BACK = 105;
  public const int X360_START = 106;
  
  // Mouse Buttons
  public const int MOUSE_0 = 107;
  public const int MOUSE_1 = 108;
  public const int MOUSE_2 = 109;
  public const int MOUSE_UP = 110;
  public const int MOUSE_DOWN = 111;
  
  // Mouse Axes
  public const int MOUSE = 1;
  
  // Xbox 360 axes
  public const int X360_LEFTSTICK = 3;
  public const int X360_RIGHTSTICK = 4;
  
  // Button data
  public int button;
  public int pressType;
  public float downTime; // The time this button has been held down.
  
  // Axis data
  public int axis; // 
  public float x, y; // X and Y values for an Axis
  
  public InputEvent(int axis, float x, float y){
    this.button = NONE;
    this.pressType = NONE;
    this.downTime = 0.0f;
    
    this.axis = axis;
    this.x = x;
    this.y = x;
  }
  
  public InputEvent(int button, int pressType, float downTime){
    this.axis = NONE;
    this.x = this.y = 0.0f;
    
    this.button = button;
    this.pressType = pressType;
    this.downTime = downTime;
  }
  
  public bool IsButton(){
    if(button == NONE){ return false; }
    return true;
  }
}
