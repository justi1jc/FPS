/*
  The speech tree is responsible for parsing a text file into a tree of
  SpeechNode objects.


  SPEECH NODE TEXT FILE FORMAT:
  NODE
  <Node id>
  <ARGUMENT COUNT>:<Child>:<Hidden?>[:<Action>:<TargetNode>:<TargetOption>:<Arg>]:<Option text>
  <ARGUMENT COUNT>:<Child>:<Hidden?>[:<Action>:<TargetNode>:<TargetOption>:<Arg>]:<Option text>
  <ARGUMENT COUNT>:<Child>:<Hidden?>[:<Action>:<TargetNode>:<TargetOption>:<Arg>]:<Option text>
  <ARGUMENT COUNT>:<Child>:<Hidden?>[:<Action>:<TargetNode>:<TargetOption>:<Arg>]:<Option text>
  
  //Repeat for however many nodes you will include
  END// File ends with END on last line
*/
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class SpeechTree{
  public string fileName;
  public int activeNode;
  public List<SpeechNode> nodes;
  public int lineCount;
  
  public SpeechTree(string _fileName, int _activeNode = 0){
    lineCount = 0;
    fileName = _fileName;
    activeNode = _activeNode;
    nodes = new List<SpeechNode>();
    ParseFile();
  }
  
  /* Returns active node. */
  public SpeechNode ActiveNode(){
    return nodes[activeNode];
  }
  
  /* Selects an option and performs its actions. */
  public void SelectOption(int option){
    SpeechNode node = ActiveNode();
    SpeechNode targetNode = null;
    int child = node.children[option];
    if(child != -1 && child < nodes.Count){ activeNode = nodes[child].id; }
    switch(node.actions[option]){
      case SpeechNode.NONE:
        break;
      case SpeechNode.SHOW_NODE:
        targetNode = nodes[node.targetNodes[option]];
        targetNode.hidden[node.targetOptions[option]] = false;
        break;
      case SpeechNode.HIDE_NODE:
        targetNode = nodes[node.targetNodes[option]];
        targetNode.hidden[node.targetOptions[option]] = true;
        break;
      case SpeechNode.CHANGE_CHILD:
        targetNode = nodes[node.targetNodes[option]];
        targetNode.children[node.targetOptions[option]] = node.actionIntArgs[option];
        break;
      case SpeechNode.CHANGE_ACTION:
        targetNode = nodes[node.targetNodes[option]];
        targetNode.actions[node.targetOptions[option]] = node.actionIntArgs[option];
        break;
    }
  }
  
  /* Prints out some debug ingo */
  void DebugPrint(){
    for(int i = 0; i < nodes.Count; i++){
      MonoBehaviour.print("Node " + i +nodes[i].prompt);
      for(int j = 0; j < 4; j++){
        MonoBehaviour.print("Node " + i + " option "+ j + nodes[i].options[j]);
      }
    }
  }
  
  
  /* Reads data in from file. */
  void ParseFile(){
    
    try{
        string location  = Application.dataPath + "/Resources/"; 
        using (StreamReader sr = new StreamReader(location+fileName)){
            String line = sr.ReadLine(); lineCount++;
            while(line != null && line.ToUpper() != "END"){
              if(line.ToUpper() == "NODE"){
                if(!ParseNode(sr)){ return; } 
              }
              line = sr.ReadLine(); lineCount++;
            }
        }
    }
    catch (Exception e){ MonoBehaviour.print("Exception:" + e); }
  }
  
  /* Creates a node.*/
  bool ParseNode(StreamReader sr){

    string line = sr.ReadLine(); lineCount++;
    int id = int.TryParse(line, out id) ? id : -1;
    if(id == -1){ MonoBehaviour.print("Missing ID. Line " + lineCount); return false;}
    
    //Parse prompt
    string prompt = sr.ReadLine(); lineCount++;
    
    //Set up option vars.
    char[] delimiters = {':'}; 
    string[] lines;
    int val, argCount;
    int arg = 0;
    string[] options    = {"", "", "", ""};
    bool[] hidden       = {false, false, false, false};
    int[] children      = {-1, -1, -1, -1};
    int[] actions       = {-1, -1, -1, -1};
    int[] targetNodes   = {-1, -1, -1, -1};
    int[] targetOptions = {-1, -1, -1, -1};
    int[] actionIntArgs = {-1, -1, -1, -1};
    
    //Parse each option.
    for(int i = 0; i<4; i++){
      arg = 0;
      line = sr.ReadLine(); lineCount++;
      lines = line.Split(delimiters);
      
      // Parse number of args before text
      val = int.TryParse(lines[arg], out val) ? val : -1; arg++;
      if(val == -1){ MonoBehaviour.print("Missing argCount. Line " + lineCount); return false; }
      argCount = val;
      if(argCount > lines.Length){
        MonoBehaviour.print("Missing arg number. Line " + lineCount); 
        return false;
      }
      
      // Parse child
      if(argCount > 0){
        val = int.TryParse(lines[arg], out val) ? val : -1; arg++;
        if(val == -1){ 
          MonoBehaviour.print("Missing child number. Line " + lineCount);
          return false;
        }
        children[i] = val;
      }
      
      // Parse hidden
      if(argCount > 1){
        val = int.TryParse(lines[arg], out val) ? val : -1; arg++;
        if(val == -1){ 
          MonoBehaviour.print("Missing child number. Line " + lineCount);
          return false;
        }
        hidden[i] = val==1 ? true : false;
      }
      
      
      // Parse action, if defined
      if(argCount > 2){
        val = int.TryParse(lines[arg], out val) ? val : -1; arg++;
        if(val == -1){ MonoBehaviour.print("Missing action. Line " + lineCount); return false; }
        children[i] = val;
      }
      
      // Parse targetNode, if defined
      if(argCount > 3){
        val = int.TryParse(lines[arg], out val) ? val : -1; arg++;
        if(val == -1){
          MonoBehaviour.print("Missing targetNode. Line " + lineCount); 
          return false; 
        }
        targetNodes[i] = val;
      }
      
      // Parse targetOption, if defined
      if(argCount > 4){
        val = int.TryParse(lines[arg], out val) ? val : -1; arg++;
        if(val == -1){
          MonoBehaviour.print("Missing targetNode. Line " + lineCount); 
          return false; 
        }
        targetOptions[i] = val;
      }
      
      // Parse actionIntArg, if defined
      if(argCount > 5){
        val = int.TryParse(lines[arg], out val) ? val : -1; arg++;
        if(val == -1){
          MonoBehaviour.print("Missing actionIntArg. Line " + lineCount); 
          return false; 
        }
        actionIntArgs[i] = val;
      }
      options[i] = lines[arg];
    }
    SpeechNode node = new SpeechNode(
      id = id,
      prompt = prompt,
      hidden = hidden,
      options = options,
      children = children,
      actions = actions,
      targetNodes = targetNodes,
      targetOptions = targetOptions,
      actionIntArgs = actionIntArgs
    );
    nodes.Add(node);
    return true; 
  }
}
