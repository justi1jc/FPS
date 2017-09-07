/* Manages the traversal of a tree/graph of SpeechNodes 

   Dialogue file format:
    
   NODE
   <Node ID>
   [[HIDDEN:][ACTION:<Action name>:[Arg:]END:][CHILD:<child id>]<Option Text>]
   
   END
   
   Note: You can have multiple nodes, options, and actions.
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeechTree{
  List<SpeechNode> nodes; // Nodes in the tree.
  SpeechNode active; // Node currently rendered.
  int lineCount = 0;
  
  public SpeechTree(string fileName){
    nodes = new List<SpeechNode>();
    active = null;
    Init(fileName);
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
        //Session.session.StartQuest(args[0]);
        break;
      case "AWARD_XP":      
        if(args.Length < 1){ return; }
        //Session.session.AwardXP(args[0]);
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
  
  /* Reads data in from a text file. */
  void Init(string fileName){
    nodes = new List<SpeechNode>();
    active = null;
    try{
      string path = Application.dataPath + "/Resources/Speech/" + fileName + ".txt";
      if(!File.Exists(path)){ MonoBehaviour.print("File doesn't exist at " + path); return; }
      using(StreamReader sr = new StreamReader(path)){
        string line = sr.ReadLine();
        lineCount = 1;
        while(line != null && line.ToUpper() != "END"){
          if(line.ToUpper() == "NODE"){
            SpeechNode sn = ParseNode(sr);
            if(sn == null){ MonoBehaviour.print("Node null"); return; }
            nodes.Add(sn);
          }
          line = sr.ReadLine();
          lineCount++;
        }
      }
    }catch(Exception e){ MonoBehaviour.print("Exception: " + e); }
    MonoBehaviour.print(ToString());
    if(nodes.Count > 0){ active = nodes[0]; }
  }
  
  /* Returns a string representation of this speech tree */
  string ToString(){
    string ret = "SpeechTree. Nodes: " + nodes.Count;
    int sum = 0;
    for(int i = 0; i < nodes.Count; i++){
      foreach(Option o in nodes[i].options){ sum++; }
    }
    ret += " Options: " + sum;
    return ret;
  }
  
  /* Returns the next node, or null upon failure. */
  SpeechNode ParseNode(StreamReader sr){
    
    string line = sr.ReadLine();
    lineCount++;
    int id = int.TryParse(line, out id) ? id : -1;
    if(id == -1){ MonoBehaviour.print("Missing ID: " + lineCount); return null;}
    string prompt = sr.ReadLine(); lineCount++;
    line = sr.ReadLine(); lineCount++;
    List<Option> opts = new List<Option>();
    while(line != "" && line.ToUpper() != "END"){
      Option opt = ParseOption(line, id);
      if(opt== null){ break; }
      opts.Add(opt);
      line = sr.ReadLine(); lineCount++;
    }
    return new SpeechNode(id, prompt, opts);
  }
  
  /* Returns the next option, or null upon failure. */
  Option ParseOption(string line, int id){
    if(line == ""){ return null; }
    Option o = new Option();
    int i = 0;
    char[] delimiters = {':'};
    string[] lines = line.Split(delimiters);
    if(lines.Length == 0){ return null; }
    if(lines[i].ToUpper() == "HIDDEN"){ 
      o.hidden = true;
      i++; 
    }
    else{ o.hidden = false; }
    bool done = ParseCommand(ref i, lines, ref o);
    while(!done){ done = ParseCommand(ref i, lines, ref o); }
    if(lines[i].ToUpper() == "CHILD"){ 
      i++;
      int child = int.TryParse(lines[i], out child) ? child : -1;
      if(child != -1){ o.child = child; i++; }
    }
    else{ o.child = id; }
    o.text = lines[i];
    return o;
  }
  
  /* Parses commands, where i points to the word "ACTION"
     return true when there is no action following this one. */
  bool ParseCommand(ref int i, string[] lines, ref Option o){
    if(lines[i].ToUpper() != "ACTION"){ return true; }
    i++;
    string action = lines[i];
    i++;
    List<int> args = new List<int>();
    while(lines[i].ToUpper() != "END"){
      int arg = int.TryParse(lines[i], out arg) ? arg : -1;
      if(arg == -1){ return true; }
      args.Add(arg);
      i++;
    }
    i++;
    o.actions.Add(action);
    o.args.Add(args.ToArray());
    if(lines[i].ToUpper() != "ACTION"){ return true; }
    return false;
  }
}
