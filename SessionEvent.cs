/*
    A record for an event sent to the Session.cs instance to be handled by 
    either Session.cs,  Arena.cs, or World.cs.
    
    
    
*/

public class SessionEvent{
  public enum Events{ Death };
  public enum Destinations{ None, Session, Arena, World };
  public Destinations destination;
  public Events code; // The type of event.
  public string message;// The string representation of this message.
  public Data[] args; // Event's data
  
  /**
    * Default constructor 
    */
  public SessionEvent(){ args = null; }
  
  /**
    * Returns an event formatted for an actor's death.
    * @param {Data} victim - the data of the actor killed.
    * @param {Data} killer - the data of the actor that did the killing
    * @param {Data} weapon - the data of the weapon used to kill the victim.
    * @return {SessionEvent} - The formatted SessionEvent;
    */
  public static SessionEvent DeathEvent(Data victim, Data killer, Data weapon){
    SessionEvent se = new SessionEvent();
    se.message = "Someone died.";
    string victimName = victim != null ? victim.displayName : "";
    string killerName = killer != null ? killer.displayName : "";
    string weaponName = weapon != null ? weapon.displayName : "";
    if(killerName != ""){
      se.message = victimName + " was killed by " + killerName;
      if( weaponName != ""){ se.message += " with " + weaponName; }
      se.message += ".";
    }
    else if(victimName != ""){ se.message = victimName + " died."; }
    se.code = Events.Death;
    se.destination = Destinations.None;
    se.args = new Data[3];
    se.args[0] = victim;
    se.args[1] = killer;
    se.args[2] = weapon;
    return se;
  }
  
  /**
    * Converts Actor to Data for use in DeathEvent
    * @param {Actor} a - the Actor whose data will be converted.
    * @param {Data} - The Actor's data. 
    */
  public static Data ActorDeathData(Actor a){
    if(a == null){ return null; }
    Data dat = new Data();
    dat.displayName = a.displayName;
    dat.strings.Add(a.prefabName);
    dat.ints.Add(a.playerNumber);
    dat.ints.Add(a.id);
    dat.ints.Add((int)a.stats.faction);
    return dat;
  }

}
