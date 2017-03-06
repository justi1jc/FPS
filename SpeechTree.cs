/*
*  The speech tree is responsible for storing the text found in
*  one text file as a series of SpeechNodes. Each prompt has at
*  most four responses. It is assumed the first node in the list is the root.
*/
using System.Collections.Generic;

public class SpeechTree{
  public string file;
  List<SpeechNode> nodes;
  public int root, active;
  
  /* Constructor */
  public SpeechTree(){
    root = active = 0;
  }
  
   
  
  
  /* Reads data in from file. */
  public void ParseFile(){
  }
}

/*
private class SpeechNode(){
  string parentOption, prompt;// 
  int one, two, three, four;  // Child nodes
  
  public SpeechNode(
                   _parentOption,
                   _prompt,
                   _one = -1,
                   _two = -1,
                   _three = -1,
                   _four = -1
                   ){
   this.

  }

}
*/
