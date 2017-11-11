/*
    Menu for editing user-defined kits.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EditKitMenu : Menu{
  private Kit activeKit; //Kit to edit.
  private List<Kit> kits;
  private List<Data> allItems;
  private List<Data> availableItems;
  private Data activeItem; // Item to render.
  private bool editName = false;
  private Slots activeSlot = Slots.None;
  private Item model; // Visual model of active item
  private float zoom = 2f;
  private int kitIndex = -1;
  
  // Data of selected kit.
  private List<Data> activeArms;
  private List<Data> activeInventory;
  private List<Data> activeClothes;
  enum Slots{ None, Right, Left, First, Second, Third, Fourth };
  
  public EditKitMenu(MenuManager m) : base(m){
    activeKit = null;
    allItems = Item.GetKitItems();
    InitKits();
  }
  
  /* Initializes kits with existing or new kits. */
  private void InitKits(){
    kits = new List<Kit>();
    for(int i = 0; i < 6; i++){
      Kit k = Kit.LoadKit(i);
      if(k == null){ k = Kit.NullSlotKit(); }
      kits.Add(k);
    }
  }
  
  public override void Render(){
    RenderNavigation();
    RenderKitSelection();    
    RenderKitDetails();
    RenderItems();
    RenderActiveItem();
    RotateModel();
    UpdateZoom();
  }
  
  /* Renders back button. */
  private void RenderNavigation(){
    string str = "Back";    
    int w = Width()/4;
    int h = Height()/10;
    int x = 0;
    int y = Height()-h;
    if(Button(str, x, y, w, h)){ 
      Kit.SaveKit(activeKit, kitIndex);
      Session.session.GetKits(true);
      if(model != null){
        MonoBehaviour.Destroy(model.gameObject);
      }
      manager.Change("ARENALOBBY"); 
    }
  }
  
  /* Renders buttons to switch active kits. */
  private void RenderKitSelection(){
    string str = "Custom Kit";
    int w = Width()/8;
    int h = Height()/10;
    int x = 0;
    int y = 0;
    for(int i = 0; i < 6; i++){
      str = kits[i].name;
      y = i * h;
      if(Button(str, x, y, w, h)){ UpdateActiveKit(i); }
    }
  }
  
  /* selects a particular kit for editing. */
  private void UpdateActiveKit(int kit){
    Kit.SaveKit(activeKit, kitIndex);
    activeKit = kits[kit];
    kitIndex = kit;
    editName = false;
    activeArms = new List<Data>();
    activeArms.Add(Item.GetItem(activeKit.arms[0]));
    activeArms.Add(Item.GetItem(activeKit.arms[1]));
    activeClothes = new List<Data>();
    activeClothes.Add(Item.GetItem(activeKit.clothes[0]));
    activeClothes.Add(Item.GetItem(activeKit.clothes[1]));
    activeClothes.Add(Item.GetItem(activeKit.clothes[2]));
    activeClothes.Add(Item.GetItem(activeKit.clothes[3]));
    activeInventory = new List<Data>();
    activeInventory.Add(Item.GetItem(activeKit.inventory[0]));
    activeInventory.Add(Item.GetItem(activeKit.inventory[1]));
    activeInventory.Add(Item.GetItem(activeKit.inventory[2]));
    activeInventory.Add(Item.GetItem(activeKit.inventory[3]));
  }
  
  /* Renders the buttons to modify the selected kit. */
  private void RenderKitDetails(){
    if(activeKit == null){ return; }
    string str = activeKit.name;
    int w = Width()/8;
    int h = Height()/20;
    int x = 2*w;
    int y = 0;
    
    if(!editName){
      if(Button(str, x, y, w, h)){ editName = !editName; }
    }
    else{
      w = (Width()/32)*3;
      Rect r = new Rect(x, y, w, h);
      str = activeKit.name;
      activeKit.name = GUI.TextField(r, str);
      str = "Enter";
      x += w;
      w = Width()/32;
      if(Button(str, x, y, w, h)){ editName = !editName; }
    }
    
    w = Width()/8;
    str = "Right hand: ";
    Data dat = GetKitItem(Slots.Right);
    str += dat != null ? dat.displayName : "Empty";
    x = Width()/4;
    y = h;
    if(Button(str, x, y, w, h)){ ChangeActiveSlot(Slots.Right); }
    str = "Left hand: ";
    dat = GetKitItem(Slots.Left);
    str += dat != null ? dat.displayName : "Empty";
    y = 2*h;
    if(activeArms[0] == null || Item.OneHanded(activeArms[0])){
      if(Button(str, x, y, w, h)){ ChangeActiveSlot(Slots.Left); }
    }
    str = "Item 1: ";
    dat = GetKitItem(Slots.First);
    str += dat != null ? dat.displayName : "Empty";
    y = 3*h;
    if(Button(str, x, y, w, h)){ ChangeActiveSlot(Slots.First); }
    str = "Item 2: ";
    dat = GetKitItem(Slots.Second);
    str += dat != null ? dat.displayName : "Empty";
    y = 4*h;
    if(Button(str, x, y, w, h)){ ChangeActiveSlot(Slots.Second); }
    str = "Item 3: ";
    dat = GetKitItem(Slots.Third);
    str += dat != null ? dat.displayName : "Empty";
    y = 5*h;
    if(Button(str, x, y, w, h)){ ChangeActiveSlot(Slots.Third); }
    str = "Item 4: ";
    dat = GetKitItem(Slots.Fourth);
    str += dat != null ? dat.displayName : "Empty";
    y = 6*h;
    if(Button(str, x, y, w, h)){ ChangeActiveSlot(Slots.Fourth); }
  }
  
  /* Updates available items according to slot. */
  private void ChangeActiveSlot(Slots slot){
    activeSlot = slot;
    if(slot == Slots.None){}
    else if(slot == Slots.Right){ availableItems = GetWeapons(); }
    else if(slot == Slots.Left){ availableItems = GetOneHandedWeapons(); }
    else{ availableItems = new List<Data>(allItems); }
    availableItems.Insert(0, null);
    UpdateModel(GetKitItem(slot));
  }
  
  /* Returns the active kit's item in specified slot, or null */
  private Data GetKitItem(Slots slot){
    if(activeKit == null){ return null; }
    Data ret = null;
    switch(slot){
      case Slots.Right: ret = activeArms[0]; break;
      case Slots.Left: ret =activeArms[1]; break;
      case Slots.First: ret = activeInventory[0]; break;
      case Slots.Second: ret = activeInventory[1]; break;
      case Slots.Third: ret = activeInventory[2]; break;
      case Slots.Fourth: ret = activeInventory[3]; break;
    }
    return ret;
  }
  
  /* Returns weapons from allItems. */
  private List<Data> GetWeapons(){
    List<Data> ret = new List<Data>();
    foreach(Data dat in allItems){
      if(Item.IsWeapon(dat)){ ret.Add(new Data(dat)); }
    }
    return ret;
  }
  
  /* Returns one-handed weapons from allItems. */
  private List<Data> GetOneHandedWeapons(){
    List<Data> ret = new List<Data>();
    foreach(Data dat in allItems){
      bool oneHanded = Item.OneHanded(dat);
      bool isWeapon = Item.IsWeapon(dat);
      if(oneHanded && isWeapon){ ret.Add(new Data(dat)); }
    }
    return ret;
  }
  
  /* string representation of given slot. */
  private string SlotText(Slots slot){
    switch(slot){
      case Slots.None: return "None"; break;
      case Slots.Right: return "Primary Weapon"; break;
      case Slots.Left: return "Secondary Weapon(Dual-wielded)"; break;
      case Slots.First: return "Inventory slot 1"; break;
      case Slots.Second: return "Inventory slot 2"; break;
      case Slots.Third: return "Inventory slot 3"; break;
      case Slots.Fourth: return "Inventory slot 4"; break;
    }
    return "";
  }
  
  /* Renders items available to selected slot. */
  private void RenderItems(){
    if(activeSlot == Slots.None){ return;}
    string str = SlotText(activeSlot);
    int w = Width()/4;
    int h = Height()/20;
    int x = Width()-w;
    int y = 0;
    Box(str, x, y, w, h);
    if(availableItems == null){ return; }
    for(int i = 0; i < availableItems.Count; i++){
      y = (i+1)*h;
      bool aim = availableItems[i] == null;
      str = aim ? "None" : availableItems[i].displayName;
      if(Button(str, x, y, w, h)){ 
        activeItem = availableItems[i];
        UpdateModel(activeItem);
        if(activeItem == null){ Equip(""); }
      }
    }
  }
  
  /* Destroys existing model creates a new one. */
  private void UpdateActiveItem(Data dat){
    activeItem = dat;
    UpdateModel(dat);
  }
  
  /* Destroys current model and replaces it with new model, if possible. */
  private void UpdateModel(Data dat){
    if(model != null){ MonoBehaviour.Destroy(model.gameObject); }
    if(dat == null){ return; }
    model = Item.GetItem(dat);
    if(model == null){ return; }
    Rigidbody rb = model.gameObject.GetComponent<Rigidbody>();
    if(rb != null){
      rb.useGravity = false;
      rb.velocity = new Vector3();
    }
    Vector3 pos = manager.transform.position;
    pos += manager.transform.forward * zoom;
    model.gameObject.transform.position = pos;
  }
  
  /* Renders the item currently selected, if one is.*/
  private void RenderActiveItem(){
    if(activeItem == null){ return; }
    string str = activeItem.displayName;
    int w = Width()/4;
    int h = Height()/20;
    int x = w;
    int y = Height() - (7*h);
    Box(str, x, y, w, h);
    if(model == null){ return; }
    for(int i = 0; i < 6; i++){
      str = model.GetUseInfo(i);
      y = (Height() - 7*h) + ((i+1)*h);
      if(str != ""){ Box(str, x, y, w, h); }
    }
    str = model.itemDesc;
    x = 2*w;
    y = Height() - (7*h);
    h *= 4;
    Box(str, x, y, w, h);
    str = "Select";
    h = Height()/20;
    y = Height() - (3*h);
    if(Button(str, x, y, w, h)){ Equip(activeItem.prefabName); }
  }
  
  /* Equips active item to active slot */
  private void Equip(string name){
    if(activeSlot == Slots.None){ return; }
    switch(activeSlot){
      case Slots.Right:
        activeKit.arms[0] = name;
        activeArms[0] = Item.GetItem(name);
        if(activeArms[0] != null && !Item.OneHanded(activeArms[0])){
          activeKit.arms[1] = ""; 
          activeArms[1] = null;
        }
        break;
      case Slots.Left: 
        activeKit.arms[1] = name;
        activeArms[1] = Item.GetItem(name);
        break;
      case Slots.First: 
        activeKit.inventory[0] = name;
        activeInventory[0] = Item.GetItem(name);
        break;
      case Slots.Second:
        activeKit.inventory[1] = name;
        activeInventory[1] = Item.GetItem(name);
        break;
      case Slots.Third:
        activeKit.inventory[2] = name;
        activeInventory[2] = Item.GetItem(name);
        break;
      case Slots.Fourth:
        activeKit.inventory[3] = name;
        activeInventory[3] = Item.GetItem(name);
        break;
    }
  }
  
  /* Changes the zoom according to mouse wheel input. */
  private void UpdateZoom(){
    float delta = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
    if(delta == 0f){ return; }
    zoom += delta;
    if(zoom < 1f){ zoom = 1f; }
    Vector3 pos = manager.transform.position;
    pos += manager.transform.forward * zoom;
    model.gameObject.transform.position = pos;   
  }
  
  /* Rotates the model on the y axis. */
  private void RotateModel(){
    if(model == null){ return; }
    Vector3 ea = model.gameObject.transform.rotation.eulerAngles;
    ea = new Vector3(ea.x, ea.y+ 0.25f, ea.z);
    model.gameObject.transform.rotation = Quaternion.Euler(ea.x, ea.y, ea.z);
  }
  
}
