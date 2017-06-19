/*
    ArenaHUDMenu displays notifications related to the Arena gamemode at hand,
    and will display the scores of players at the end of a match.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ArenaHUDMenu : Menu{
  public ArenaHUDMenu(MenuManager manager) : base(manager){}
  
  public override void Render(){}
  
  public override void UpdateFocus(){}
  
  public override void Input(int button){}
}
