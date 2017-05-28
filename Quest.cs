/*
  This abstract class provides a base for all other classes. 
*/
using UnityEngine;
public abstract class Quest{
  /* Perform any initial file i/o specific to this quest. Some quests.
     Some quests will implement this with a dummy method because they 
     only need a couple arguments.
  */
  public abstract void Init(string fileName);
  public abstract void Update();     // Check for quest events.
  public abstract Data GetData();    // Return serializeable quest record
  public abstract void LoadData(Data dat);   // Loads an existing quest record
  public abstract string Description();  // Returns string describing this quest.
  public abstract string[] Objectives(); // Returns a list of objectives.
  public abstract string Name(); // Returns displayName
  public abstract int Status(); // -1: Not started 0: Started 1: Success 2: Failed
  /* This Factory method exists for convenience of creating new quests.
     with their appropriate files. 
     NOTE: EVERYTHING about this method is hard-coded to rely on 
     project-specific files.  */
  public static Quest Factory(string questName){
    switch(questName){
      case "kill the enemies!":
        KillXQuest q = new KillXQuest();
        q.displayName = questName;
        q.targetName = "Enemy";
        q.targetNumber = 3;
        q.completion = 0;
        return (Quest)q;
        break;
    }
    return null;
  }
  
  public static Quest Factory(Data dat){
    Quest q = Quest.Factory(dat.displayName);
    q.LoadData(dat);
    return q;
  }
}
