/*
  The speech tree is responsible for parsing a text file into a tree of
  SpeechNode objects.


  SPEECH NODE TEXT FILE FORMAT:
  <ARGUMENTS>:[<ARGUMENT>:<VALUE>]

*/
using System.Collections.Generic;

public class SpeechTree{
  public string fileName;
  public int activeNode;
  public List<SpeechNode> nodes;
  
  public SpeechTree(){
    
  }
  
  /* Returns active node. */
  public SpeechNode ActiveNode(){
    return nodes[activeNode];
  }
  
  /* Selects an option and performs its actions. */
  public void SelectOption(int option){
    SpeechNode node = ActiveNode();
    switch(node.actions[option]){
      case SpeechNode.NONE:
        break;
      case SpeechNode.SHOW_NODE:
        break;
      case SpeechNode.HIDE_NODE:
        break;
      case SpeechNode.CHANGE_CHILD:
        break;
      case SpeechNode.CHANGE_ACTION:
        break;
    }
  }
  
  
  /* Reads data in from file. */
  public void ParseFile(){
  }
  
  /* Creates a node.*/
  public void ParseNode(){
  }
}
