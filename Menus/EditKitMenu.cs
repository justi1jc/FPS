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
  private int activeSlot = NONE;
  private Item model; // Visual model of active item
  private float zoom = 2f;
  
  // Kit Slot constants.
  const int NONE = 0;
  const int RIGHT = 1;
  const int LEFT = 2;
  const int FIRST = 3;
  const int SECOND = 4;
  const int THIRD = 5;
  const int FOURTH = 6;
  
  public EditKitMenu(MenuManager m) : base(m){
    activeKit = null;
    allItems = Item.GetKitItems();
    InitKits();
  }
  
  /* Initializes kits with existing or new kits. */
  private void InitKits(){
    kits = new List<Kit>();
    for(int i = 0; i < 6; i++){
      kits.Add(Kit.NullSlotKit());
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
    if(Button(str, x, y, w, h)){ manager.Change("ARENALOBBY"); }
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
    activeKit = kits[kit];
    editName = false;
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
    str = "Right hand.";
    x = Width()/4;
    y = h;
    if(Button(str, x, y, w, h)){ ChangeActiveSlot(RIGHT); }
    str = "Left hand.";
    y = 2*h;
    if(Button(str, x, y, w, h)){ ChangeActiveSlot(LEFT); }
    str = "Item 1";
    y = 3*h;
    if(Button(str, x, y, w, h)){ ChangeActiveSlot(FIRST); }
    str = "Item 2";
    y = 4*h;
    if(Button(str, x, y, w, h)){ ChangeActiveSlot(SECOND); }
    str = "Item 3";
    y = 5*h;
    if(Button(str, x, y, w, h)){ ChangeActiveSlot(THIRD); }
    str = "Item 4";
    y = 6*h;
    if(Button(str, x, y, w, h)){ ChangeActiveSlot(FOURTH); }
  }
  
  
  
  /* Updates available items according to slot. */
  private void ChangeActiveSlot(int slot){
    activeSlot = slot;
    if(slot == NONE){}
    else if(slot == RIGHT){ availableItems = GetWeapons(); }
    else if(slot == LEFT){ availableItems = GetOneHandedWeapons(); }
    else{ availableItems = new List<Data>(allItems); }
    UpdateModel(GetKitItem(slot));
  }
  
  /* Returns the active kit's item in specified slot, or null */
  private Data GetKitItem(int slot){
    if(activeKit == null){ return null; }
    switch(slot){
      case RIGHT: break;
      case LEFT: break;
      case FIRST: break;
      case SECOND: break;
      case THIRD: break;
      case FOURTH: break;
    }
    return null;
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
  private string SlotText(int slot){
    switch(slot){
      case NONE: return "None"; break;
      case RIGHT: return "Primary Weapon"; break;
      case LEFT: return "Secondary Weapon(Dual-wielded)"; break;
      case FIRST: return "Inventory slot 1"; break;
      case SECOND: return "Inventory slot 2"; break;
      case THIRD: return "Inventory slot 3"; break;
      case FOURTH: return "Inventory slot 4"; break;
    }
    return "";
  }
  
  /* Renders items available to selected slot. */
  private void RenderItems(){
    if(activeSlot == NONE){ return;}
    string str = SlotText(activeSlot);
    int w = Width()/4;
    int h = Height()/20;
    int x = Width()-w;
    int y = 0;
    Box(str, x, y, w, h);
    if(availableItems == null){ return; }
    for(int i = 0; i < availableItems.Count; i++){
      y = (i+1)*h;
      str = availableItems[i].displayName;
      if(Button(str, x, y, w, h)){ 
        UpdateActiveItem(new Data(availableItems[i]));
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
    int y = Height()/2;
    Box(str, x, y, w, h);
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
