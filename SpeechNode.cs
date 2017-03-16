/*
    This object acts as a record for the information needed to render one 
    speech menu.
*/


public class SpeechNode{
  public const int NONE          = 0; // Do nothing.
  public const int SHOW_NODE     = 1; // Show a node's option.
  public const int HIDE_NODE     = 2; // Hide a node's option.
  public const int CHANGE_CHILD  = 3; // Change the destination of a node's option.
  public const int CHANGE_ACTION = 4; 

  public int id;                 // The node's index in the tree.
  public string prompt;          // Text prompting a user's response.
  public string[] options;       // The four options available
  public bool[] hidden;          // Whether option is hidden.
  public int[] children;         // Speech node to go to.
  public int[] actions;          // The action to perform upon selecting an option.
  public int[] targetNodes;      // Speech node to be edited.
  public int[] targetChildren;   // Target Node's 
  public int[] actionIntArgs;    // Int argument used for the action
  public int[] actionStringArgs; // String argument used for the action
  
  public SpeechNode(
    int _id,
    string _prompt = "",
    string[] _options = null,
    int[] _children = null,
    int[] _actions = null,
    int[] _targetNodes = null,
    int[] _targetChildren = null,
    int[] _actionIntArgs = null,
    int[] _actionStringArgs = null
  ){
    id = _id;
    prompt = _prompt;    
    options  = _options ?? new string[] {"", "", "", ""};
    children = _children ?? new int[] {-1, -1, -1, -1};
    actions = _actions ?? new int[] {NONE, NONE, NONE, NONE};
    targetNodes = _targetNodes ?? new int[] {-1, -1, -1, -1};
    targetChildren = _targetChildren ?? new int[] {-1, -1, -1, -1};
    actionIntArgs = _actionIntArgs ?? new int[] {-1, -1, -1, -1};
    actionStringArgs = _actionStringArgs;
  }
  
  public void SetVisible(int option, bool val){
    hidden[option] = val;
  }
  
  public void ChangeChild(int option, int child){
    children[option] = child;
  }
  
  public void ChangeAction(int option, int action){
    actions[option] = action;
  }

}
