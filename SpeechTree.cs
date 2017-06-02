/* Manages the traversal of a tree/graph of SpeechNodes */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeechTree{
  List<SpeechNode> nodes; // Nodes in the tree.
  SpeechNode active; // Node currently rendered.
  
  public SpeechTree(string filename){
    nodes = new List<SpeechNode>();
    active = null;
    TemporaryInit();
  }
  
  public string Prompt(){
    if(active != null){ return active.Prompt(); }
    return "";
  }
  
  public string Option(int option){
    if(active != null){ return active.OptionText(option); }
    return "";
  }
  
  public void SelectOption(int option){
    if(active == null){ return; }
    int dest = active.GetChild(option);
    List<string> actions = active.GetActions(option);
    if(actions != null){
      for(int i = 0; i < actions.Count; i++){
        PerformAction(actions[i], active.ActionArgs(option, i));
      }
    }
    ChangeActive(dest);
  }
  
  /* Performs specified action if arguments are sufficient. */
  void PerformAction(string action, int[] args){   
    if(args == null){ return; }
    switch(action.ToUpper()){
      case "HIDE_NODE":
        if(args.Length < 2){ return; }
        SetHidden(true, args[0], args[1]);
        break;
      case "SHOW_NODE":
        if(args.Length < 2){ return; }
        SetHidden(false, args[0], args[1]);
        break;
      case "CHANGE_CHILD":
        if(args.Length < 3){ return; }
        ChangeChild(args[0], args[1], args[2]);
        break;
      case "START_QUEST":
        if(args.Length < 1){ return; }
        Session.session.StartQuest(args[0]);
        break;
      case "AWARD_XP":      
        if(args.Length < 1){ return; }
        Session.session.AwardXP(args[0]);
        break;
    }
  }
  
  /* Changes the active node, if the selection is valid. */
  void ChangeActive(int node){
    if(node < 0 || node >= nodes.Count){ return; }
    active = nodes[node];
  }
  
  /* Sets a particular node's option to a particular value */
  void SetHidden(bool val, int node, int option){
    if(node < 0 || node >= nodes.Count){ return; }
    nodes[node].SetHidden(option, val);
  }
  
  /* Changes a specific node's destination. */
  void ChangeChild(int val, int node, int option){
    if(node < 0 || node >= nodes.Count){ return; }
    nodes[node].ChangeChild(option, val);
  }
  
  void TemporaryInit(){
    List<Option> ops = new List<Option>();
    for(int i = 0; i < 4; i++){ ops.Add(new Option()); }
    
    ops[0].text = "First option.";
    ops[0].child = 0;
    ops[0].hidden = false;
    ops[0].actions = new List<string>();
    ops[0].actions = null;
    
    ops[1].text = "Second option.";
    ops[1].child = 0;
    ops[1].hidden = false;
    ops[1].actions = new List<string>();
    ops[1].actions.Add("SHOW_NODE");
    ops[1].args = new List<int[]>();
    ops[1].args.Add( new int[2]);
    ops[1].args[0][0] = 0;
    ops[1].args[0][1] = 3;
    
    ops[2].text = "I really want some XP.";
    ops[2].child = 1;
    ops[2].hidden = false;
    ops[2].actions = new List<string>();
    ops[2].actions.Add("CHANGE_CHILD");
    ops[2].args = new List<int[]>();
    ops[2].args.Add( new int[3]);
    ops[2].args[0][0] = 2;
    ops[2].args[0][1] = 0;
    ops[2].args[0][2] = 2;
    
    ops[3].text = "Fourth option.";
    ops[3].child = 0;
    ops[3].hidden = false;
    ops[3].actions = new List<string>();
    ops[3].actions.Add("START_QUEST");
    ops[3].args = new List<int[]>();
    ops[3].args.Add( new int[1]);
    ops[3].args[0][0] = 0;
    ops[3].actions.Add("HIDE_NODE");
    ops[3].args.Add(new int[2]);
    ops[3].args[1][0] = 0;
    ops[3].args[1][1] = 3;
    
    nodes.Add(new SpeechNode(0, "Choose an option.", ops));
    
    ops = new List<Option>();
    ops.Add(new Option());
    ops[0].text = "[Take XP]";
    ops[0].child = 0;
    ops[0].hidden = false;
    ops[0].actions = new List<string>();
    ops[0].actions.Add("AWARD_XP");
    ops[0].args = new List<int[]>();
    ops[0].args.Add( new int[1]);
    ops[0].args[0][0] = 100;
    
    nodes.Add(new SpeechNode(1, "Well, then, have some XP", ops));
    
    ops = new List<Option>();
    ops.Add(new Option());
    ops[0].text = "Oh, ok.";
    ops[0].child = 0;
    ops[0].hidden = false;
    ops[0].actions = null;
    
    nodes.Add(new SpeechNode(2, "No more XP for you.", ops));
    
    active = nodes[0];
  }
}
