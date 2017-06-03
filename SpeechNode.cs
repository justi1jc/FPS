/*
  A container for a list of options that represents the contents of 
  one screen of dialogue. 
*/


using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeechNode{
  
  private int id;
  private string prompt;
  public List<Option> options;
  
  public SpeechNode(int id, string prompt, List<Option> options){
    this.id = id;
    this.prompt = prompt;
    this.options = options;
  }
  
  /* Returns true if the option is hidden, or if the option doesn't exist. */
  public bool Hidden(int option){
    if(!ValidOption(option)){ return true; }
    return options[option].hidden;
  }
  
  /* Access method */
  public string Prompt(){
    return prompt;
  }
  
  /* Returns the option text, if it exists, or a blank string. */
  public string OptionText(int option){
    if(!ValidOption(option)){ return ""; }
    if(!options[option].hidden){ return options[option].text; }
    return "";
  }
  
  /* Returns the actions of an option, or an empty list. */
  public List<string> GetActions(int option){
    if(!ValidOption(option)){ return new List<string>(); }
    return options[option].actions;
  }
  
  /* Returns the arguments, if they exists. Otherwise returns null*/
  public int[] ActionArgs(int option, int action){
    if(!ValidOption(option)){ return null; }
    List<int[]> args = options[option].args;
    if(args == null || action < 0 || action >= args.Count){ return null; }
    return args[action];
  }
  
  /* Sets a particular option's hidden status. */
  public void SetHidden(int option, bool val){
    if(!ValidOption(option)){ return; }
    options[option].hidden = val;
  }
  
  /* Changes the specified option's destination. */
  public void ChangeChild(int option, int val){
    if(!ValidOption(option)){ return; }
    options[option].child = val;
  }
  
  /* Returns the relevant child of an option, or this node's id.*/
  public int GetChild(int option){
    if(!ValidOption(option)){ return id; }
    return options[option].child;
  }
  
  /* Returns true if specified option exists. */
  bool ValidOption(int option){
    if(option < 0 || option >= options.Count){ return false; }
    if(options[option] == null){ return false; }
    return true;
  }
  
}
