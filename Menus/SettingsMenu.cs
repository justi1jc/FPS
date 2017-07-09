/*
    OptionsMenu
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SettingsMenu : Menu{
  float masterVolume, mouseSensitivity, controllerSensitivity;

  public SettingsMenu(MenuManager manager) : base(manager){
    if(PlayerPrefs.HasKey("masterVolume")){
      masterVolume = PlayerPrefs.GetFloat("masterVolume");
    }
    else{ masterVolume = 1f; }
    if(PlayerPrefs.HasKey("mouseSensitivity")){
      mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity");
    }
    else{
      mouseSensitivity = 1f;
    }
  }
  
  public override void Render(){
    int iw = Width()/3;
    int off = XOffset();
    int ih = Height()/20;
    string str;
    
    str = "Settings";
    Box(str, off, 0, ih, ih);
    
    str = "Master Volume";
    Box(str, off, 2*ih, iw, ih);
    
    masterVolume = Slider(off, 3*ih, iw, ih, masterVolume, 0f, 1f);
    
    str = "Mouse Sensitivity";
    Box(str, off, 4*ih, iw, ih);
    
    mouseSensitivity = Slider(off, 5*ih, iw, ih, mouseSensitivity, 0f, 3f);
    
    str = "Controller Sensitivity";
    Box(str, off, 6*ih, iw, ih);
    
    controllerSensitivity = Slider(off, 7*ih, iw, ih, controllerSensitivity, 0f, 3f);
    
    str = "Apply settings";
    if(Button(str, off, 8*ih, iw, ih)){ Save(); }
    
    str = "Back";
    if(Button(str, off, 12*ih, iw, ih)){ manager.Change("OPTIONS"); }
  }
  
  /* Saves the selected settings. */
  public void Save(){
    PlayerPrefs.SetFloat("masterVolume", masterVolume);
    PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivity);
    PlayerPrefs.SetFloat("controllerSensitivity", controllerSensitivity);
  }
  
  
}
