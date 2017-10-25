/*
    A record for an event sent to the Session.cs instance to be handled by 
    either Session.cs,  Arena.cs, or World.cs.
    
    
    
*/

public class SessionEvent{
  // Event code constants
  public const int DEATH = 0;
  
  // Destination constants
  public const int NONE = -1; // This event has multiple potential destinations.
  public const int SESSION = 0;
  public const int ARENA = 1; 
  public const int WORLD = 2;
  
  
  
  public int destination;
  public int code; // The type of event.
  public string message;// The string representation of this message.
  public Data[] args; // Event's data
  
  
  
  /* Default constructor */
  public SessionEvent(){ args = null; }
  
  /* Returns an event formatted for an actor's death. */
  public static SessionEvent DeathEvent(){
    SessionEvent se = new SessionEvent();
    se.message = "Someone died.";
    se.code = DEATH;
    se.destination = NONE;
    return se;
  }

}
