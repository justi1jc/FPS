/*
    DeathmatchArena handles the particulars of the 
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DeathmatchArena : Arena{

  /* Override to prevent this script from replacing itself. */
  public override void Start(){}
  
  public override void Begin(){ StartCoroutine(InitializeRound()); }
  
  /* Begin a new round. */
  private IEnumerator InitializeRound(){
    yield return new WaitForSeconds(0.5f);
    InitPlayers();
    GameModeAnnouncement();
    StartCoroutine(UpdateRoutine());
  }
  
  /* Main loop that updates hud and respawns dead players. */
  protected IEnumerator UpdateRoutine(){
    while(time > 0){
      UpdateHUD();
      if(respawns){ HandleDeaths(); }
      yield return new WaitForSeconds(1f);
      if(time < 1){ EndGame(); }
    }
  }
  
  protected override string Message(){
    return "Deathmatch: " + Time();
  }
  
  /* Respawns dead actors. */
  void HandleDeaths(){
    for(int i = 0; i < players.Count; i++){
      Actor actor = players[i];
      if(!actor.Alive()){
        players.Remove(actor);
        i--;
        StartCoroutine(RespawnPlayer(actor));
      }
    }
  }
  
  /* Stop gameplay and display end game message. */
  void EndGame(){
    ArenaHUDMenu HUD = (ArenaHUDMenu)menu.active;
    HUD.subMenu = 1;
    for(int i = 0; i < players.Count; i++){
      players[i].SetMenuOpen(true);
      Destroy(players[i].gameObject);
    }
  }
  
  /* Loads data encoded for this gamemode. */
  public override void LoadDataFromSession(){
    if(!Session.Active() || Session.session.arenaData == null){ return; }
    if(ActiveGameMode() != GameModes.Deathmatch){ return; }
    Data dat = Session.session.arenaData;
    gameMode = (GameModes)dat.ints[0];
    time = startingTime = (dat.ints[1] * 60);
    bots = dat.ints[2];
    respawns = dat.bools[0];
    teams = dat.bools[1];
    p1red = dat.bools[2];
    p2red = dat.bools[3];
    spawnWeapons = dat.bools[4];
    kit = dat.strings[0];
  }
  
  /* Handles a SessionEvent according to the game mode. */
  public override void HandleEvent(SessionEvent evt){
    switch(evt.code){
      case SessionEvent.Events.Death:
        if( gameMode == GameModes.Deathmatch ){ 
          RecordKill(evt);
          if(!respawns && (players.Count <= 1 || OneTeamLeft())){ EndGame(); }
        }
        break;
    }
  }
  
}
