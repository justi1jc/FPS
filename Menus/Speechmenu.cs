/*
    SpeechMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpeechMenu : Menu{
  
  public SpeechMenu(MenuManager manager) : base(manager){
    syMax = 3;
    syMin = 0;
    sxMax = 0;
    sxMin = 0;
  }
  
  public override void Render(){
    Actor actor = manager.actor;
    if(!actor){ MonoBehaviour.print("Actor missing"); return; }
    if(actor.interlocutor == null){ 
      MonoBehaviour.print("Interlocutor missing"); 
      return;
    }
    if(actor.interlocutor.speechTree == null){ 
      MonoBehaviour.print("tree missing."); 
      return; 
    }
    
    GUI.skin.button.wordWrap = true; // Make sure text wraps in buttons.
    GUI.skin.box.wordWrap = true; // Make sure text wraps in boxes.
    
    Box("", XOffset(), 0, Width(), Height());
    SpeechTree st = actor.interlocutor.speechTree;
    int iw = Width()/6;
    int ih = Height()/15;
    int mid = Height()/2;
    string str = "";
    if(Button("Trade", XOffset(), Height()/2, iw, ih)){ 
      manager.Change("TRADE"); 
    }
    
    // Render Prompt
    str = st.Prompt(); 
    Box(str,XOffset() + iw, 0, 4*iw, 7*ih);
    
    int i = 0;
    while(st.Option(i) != ""){
      str = st.Option(i);
      if(Button(str, XOffset() + iw, (7+i)*ih, 4*iw, 1*ih)){ st.SelectOption(i); }
      i++;
    }
  }
  
  public override void UpdateFocus(){
    Actor actor = manager.actor;
    if(actor == null){ return; }
    SpeechTree st = actor.interlocutor != null ? actor.interlocutor.speechTree : null;
    if(st != null && st.Option(sy) == ""){ sy--; }
  }
  
  public override void Input(int button){
    DefaultExit(button);
    if(manager.actor == null || manager.actor.interlocutor == null){ return; }
    SpeechTree st = manager.actor.interlocutor.speechTree;
    if(button == A){
      if(st.Option(sy) == ""){ return; }
      switch(sy){
        case 0:
          st.SelectOption(0);
          break;
        case 1:
          st.SelectOption(1);
          break;
        case 2:
          st.SelectOption(2);
          break;
        case 3:
          st.SelectOption(3);
          break;
      }
      sy = 0;
    }
    
  }
  
}
