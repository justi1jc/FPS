﻿/*
      Author: James Justice
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechNode : MonoBehaviour {
   public string[] optionsText = new string[4];
   public int[] branches = new int[4];
   public List<string> actions = new List<string>();
   public bool visible = true;
   public string messageText;
   public string audioClipName;
   public ItemData GetData(){
      ItemData dat = new ItemData();
      foreach(int i in branches){
         dat.ints.Add(i);
      }
      dat.strings.Add(messageText);
      dat.strings.Add(audioClipName);
      foreach(string s in optionsText)
         dat.strings.Add(s);
      foreach(string s in actions)
         dat.strings.Add(s);
      return dat;
   }
   public void LoadData(ItemData dat){
      for(int i = 0; i<4; i++)
         branches[i] = dat.ints[i];
      messageText = dat.strings[0];
      audioClipName = dat.strings[1];
      for(int i = 2; i<6; i++)
         optionsText[i-2] = dat.strings[i];
      for(int i = 6; i<dat.strings.Count; i++)
         actions.Add(dat.strings[i]);
   }
}
