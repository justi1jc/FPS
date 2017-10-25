/*
    A Kit contains a list of items to be stored in an Actor's inventory
    along with at most two items to put in the player's hands and at most
    four Equipment items for the Actor to wear.
    
    Note: The available kits should be defined in /Resources/Kits.txt
    with the following format:
    
    KIT
    <Kit name>
    ARMS
    [Primary]
    [Secondary]
    CLOTHES
    [Head]
    [Torso]
    [Legs]
    [Feet]
    INVENTORY
    [item]
    /KIT
    
    END
    
    
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class Kit{
  public string name;
  public List<string> arms;
  public List<string> clothes;
  public List<string> inventory;
  
  public Kit(){
    arms = new List<string>();
    clothes = new List<string>();
    inventory = new List<string>();
  }
  
  /* Returns all the kits parsed from the */
  public static List<Kit> GetKits(){
    KitParser kp = new KitParser();
    return kp.Parse();
  }
  
  /* Applies a given kit's contents to an actor. */
  public void ApplyKit(ref Actor actor){
    foreach(string item in arms){ ApplyToArms(item, ref actor); }
    foreach(string item in clothes){ ApplyToClothes(item, ref actor); }
    foreach(string item in inventory){ ApplyToInventory(item, ref actor); }
  }
  
  /* Equips an item to an actor's arms, giving it max ammo if relevant. */
  public static void ApplyToArms(string item, ref Actor actor){
    Data dat = Item.GetItem(item);
    if(dat != null){
      if(dat.itemType == Item.RANGED){ Ranged.MaxAmmo(ref dat); }
      actor.Equip(dat);
    }
    else{ MonoBehaviour.print(item + " was null!"); }
  }
  
  /* Applies clothing with optional arguments for a specific color. */
  public static void ApplyToClothes(
    string item,
    ref Actor actor,
    bool color = false,
    float r = 0f,
    float g = 0f,
    float b = 0f,
    float a = 0f
  ){
    Data dat = Item.GetItem(item);
    if(dat != null){
      if(color){ Equipment.SetColor(new Color(r, g, b, a), ref dat); }
      actor.Equip(dat);
    }
    else{ MonoBehaviour.print(item + " was null!"); }
  }
  
  /* Applies an item to inventory with optional */
  public static void ApplyToInventory(
    string item,
    ref Actor actor,
    bool fullStack = true
  ){
    Data dat = Item.GetItem(item);
    if(dat != null){
      if(dat.itemType == Item.ITEM && fullStack){ Item.FullStack(ref dat); }
      actor.StoreItem(dat);
    }
    else{ MonoBehaviour.print(item + " was null!"); }
  }
  
  private void FullStack(ref Data dat){
    dat.stack = dat.stackSize;
  }
  
  public string ToString(){
    string ret = name;
    ret += " arms: " + arms.Count;
    ret += " clothes: " + clothes.Count;
    ret += " inventory: " + inventory.Count;
    return ret;
  }
  
  /*
      KitParser performs the parsing of the /Resources/Kits.txt file into
      a list of kits.
  */
  private class KitParser{
    int lineCount;
    
    public KitParser(){}
    
    /* Parses available kits from the kits.txt file. */
    public List<Kit> Parse(){
      lineCount = 0;
      List<Kit> ret = new List<Kit>();
      try{
        string path = Application.dataPath + "/Resources/Kits.txt";
        if(!File.Exists(path)){
          MonoBehaviour.print("Kits file does not exist at " + path); 
          return ret;
        }
        using(StreamReader sr = new StreamReader(path)){
          string line = sr.ReadLine();
          while(line != null && line.ToUpper() != "END"){
            if(line.ToUpper() == "KIT"){
              Kit k = (ParseKit(sr));
              if(k == null){ 
                MonoBehaviour.print("Kit null");
                return new List<Kit>();
              }
              ret.Add(k);
            }
            line = sr.ReadLine();
            lineCount++;
          }
        }
      }catch(Exception e){ MonoBehaviour.print("Exception:" + e); }
      return ret;
    }
    
    private Kit ParseKit(StreamReader sr){
      Kit ret = new Kit();
      string line = sr.ReadLine();
      lineCount++;
      ret.name = line;
      while(line.ToUpper() != "ARMS"){
        line = sr.ReadLine();
        lineCount++;
      }
      line = sr.ReadLine();
      lineCount++;
      int arms = 0;
      while(line.ToUpper() != "CLOTHES"){
        ret.arms.Add(line);
        line = sr.ReadLine();
        lineCount++;
      }
      line = sr.ReadLine();
      lineCount++;
      while(line.ToUpper() != "INVENTORY"){
        ret.clothes.Add(line);
        line = sr.ReadLine();
        lineCount++;
      }
      line = sr.ReadLine();
      lineCount++;
      while(line.ToUpper() != "/KIT"){
        ret.inventory.Add(line);
        line = sr.ReadLine();
        lineCount++;
      }
      return ret;
    }
  }
}
