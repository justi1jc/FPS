/*
    This is a record for use in storing the data of a single option
*/

using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Option{
  public string text; // text displayed for this option
  public int child; // Node this option leads to
  public bool hidden; // Whether option is visible or not.
  public List<string> actions; // the actions for this option
  public List<int[]> args; // These are the int args for given actions
}
