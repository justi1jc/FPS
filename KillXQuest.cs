/*
  This is the obligatory quest in which the player must kill a certain number
  of a certain type of enemy in order to progress. This will not check who
  killed that enemy, only that the certain enemies have indeed died. 
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KillXQuest: Quest{
  List<int> dead;
  public string displayName; // Name of quest.
  public string targetName;// What needs to die?
  public int targetNumber; // How many need to die?
  public int completion = -1;
  
  public KillXQuest(){
    dead = new List<int>();
    targetName = "";
  }
  
  /* Dummy init method. */
  public override void Init(string fileName){}
  
  /* Checks for new dead enemies. */
  public override void Update(){
    if(completion != 0){ return; }
    List<Actor> actors = Session.session.GetActors();
    for(int i = 0; i < actors.Count; i++){
      if(actors[i] != null && actors[i].displayName == targetName){
        bool exists = !UniqueDead(actors[i].id);
        bool isDead = actors[i].health <= 0;
        if(!exists && isDead){ 
          dead.Add(actors[i].id);
          Session.session.Notify(Objectives()[0]);
          if(dead.Count >= targetNumber){
            completion = 1;
            Reward();
            return; 
          } 
        }
      }
    }
  }
  
  /* Administer reward for completion. */
  void Reward(){
    MonoBehaviour.print("You killed things, hooray!");
    Session.session.Notify(displayName + " completed.");
  }
  
  
  /* Checks if dead exists already. */
  bool UniqueDead(int candidate){
    for(int i = 0; i < dead.Count; i++){
      if(dead[i] == candidate){ return false; }
    }
    return true;
  }
  
  /* Returns Serializeable record. */
  public override Data GetData(){
    Data dat = new Data();
    dat.ints.Add(targetNumber);
    dat.ints.Add(completion);
    for(int i = 0; i < dead.Count; i++){ dat.ints.Add(dead[i]); }
    dat.strings.Add(targetName);
    dat.displayName = displayName;
    return dat;
  }
  
  /* Loads from a serializable record. */
  public override void LoadData(Data dat){
    targetName = dat.strings[0];
    int i = 0;
    targetNumber = dat.ints[i]; i++;
    completion = dat.ints[i]; i++;
    for(int j = i; j < dat.ints.Count; j++){
      dead.Add(dat.ints[j]);
    }
    displayName = dat.displayName;
  }
  
  public override string Description(){
    string ret = "You need to kill ";
    ret += targetNumber + " " + targetName + "s.";
    return ret;
  }
  
  public override string[] Objectives(){
    string[] ret = new string[1];
    ret[0] = "Kill " + targetName + ":(" + dead.Count + "/";
    ret[0] += targetNumber + ")";
    return ret;
  }
  
  public override string Name(){
    return displayName;
  }
  
  public override int Status(){
    return completion;
  }
  
}
