/*
    An AIEvent is used to inform an AI about a particular event that occurs.
*/
public class AIEvent{
  public enum Events{ 
    Damage // The AI's actor has been damaged.
  };
  public Events evt; // What event has occurred.
  public Data data; // The data pertaining to this event.
  
  /**
    * No-args constructor
    */
  public AIEvent(){}
  
  /**
    * Returns an AIEvent formatted for Events.Damage.
    * @param {Actor} attacker - The source of this attack.
    * @param {int} damage - The health damage dealt by this attack.
    * @param {Data} weapon - Data of the attacker's weapon.
    * @return {AIEvent} - the damage event. 
    */
  public static AIEvent Damage(Actor attacker, int damage, Data weapon){
    AIEvent evt = new AIEvent();
    Data d = new Data();
    d.strings.Add(attacker.displayName);
    d.strings.Add(weapon.displayName);
    d.ints.Add(damage);
    d.ints.Add(attacker.id);
    evt.data = d;
    evt.evt = Events.Damage;
    return evt;
  } 
}
