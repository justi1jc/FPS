/*
    Decor is an Item that cannot be picked up nor interacted with.
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Decor : Item{
  public override void Interact(Actor a, int mode = -1, string message = ""){}
}
