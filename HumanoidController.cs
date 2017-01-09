/*
*        Author: James Justice
 * HumanoidController
 * This controller handles two tasks. Firstly, the controller handles input from
 * players. Secondly, it implements the NPC interface to perform lower-level
 * functions for NPCAI.
 * 
 * Start()
 * If player is a value other than 0, AssignPlayer is called with that number.
 * 
 * AssignPlayer(int _player) 
 * starts the appropriate coroutines and adjusts the
 * attached camera according which player was selected. As for the player variable,
 * 0 is uninitialized, 1-4 are the human player slots, and 5 denotes an NPC. 
 *  
 * PlayerInputRoutine() 
 * Takes input from the corresponding source. (Currently keyboard and mouse.) 
 * and manipulates the character accordingly.
 * 
 * NPCRoutine()
 *  Controls the character using AI.
 * 
 * Move(int direction, bool running)
 * If there is space for the player to move in this direction, the player will.
 * Running will simply speed up the movement, whilst crouching will reduce the
 * speed by that much. Fort direction 0 denotes forward, 1 backward, 2 right, 
 * and 3 is left.
 * 
 * Turn(vector3 rotation)
 * Changes the horizontal rotation of both the head and body, whilst
 * only the vertical rotation of the head. The head's vertical rotation
 * is blocked at 70 degrees.
 * 
 * JumpRoutine()
 * If the character is standing on something, the character propels upward for
 * a short jump.
 * 
 * ToggleCrouch()
 * If the character is standing, they will become shorter, reducing their 
 * silhouette and slowing their movement.
 * 
 * ReceiveDamage(int damage)
 * Health will be reduced by this much. If the health is completely depleted,
 * the character will stop receiving input and fall limp. 
 * 
 * Use(int use)
 * The active item will be used. The values below are listed.
 * 0- left mouse
 * 1- right mouse
 * 2- r
 * 
 * SwitchItem(int _item)
 * Select a particular equipped item, instanciates its prefab, and places it in
 * hand. If another item was active, the previously active item's gameobject is
 * destroyed.
 * 
 * NextItem()
 * Selects the next item.
 * 
 * PreviousItem()
 * Selects the previous item.
 * 
 * Interact()
 * The item in reach will be used if it's GetInteractionMode() returns 0, else
 * it will be picked up and the active item dropped;
 *
 * Drop()
 * Places active item into world and sets slot to null.
 * 
 *
 *
 *
 * 
 */
 





using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class HumanoidController : MonoBehaviour, NPC {
	public int player;
	public string displayName;
	public string prefabName;
	public NPCAI ai;
	public Camera cam;
	public GameObject head;
	public GameObject hand;
	public GameObject body;
	//Turning
	public float sensitivityX =1f;
	public float sensitivityY =1f;
	public Transform torso;
	float rotxMax = 60f;
	float headRotx = 0f;
	float headRoty= 0f;
	float bodyRoty = 0f;
	public SpeechController sc;
	public int faction;
	public int health = 100;
	public float speed;
	bool walking = false;
	public bool jumpCoolDown = true;
	public bool falling;
	public Vector3 fallOrigin;
	public Animator anim;
	int aliveHash;
	int crouchedHash;
	int walkingHash;
	int holdRifleHash;
	int aimRifleHash;
	//Inventory info
	public GameObject defaultItem;
	public GameObject activeItem;
	public GameObject itemInReach;
	public string itemInReachName;
	public List<InventorySlot> inventory = new List<InventorySlot>();
	public float carryWeight;
	public float carryWeightMax;
	Region region;
	
	
	public HUDController hController;
	void Awake(){
	   List<Transform> transforms = GetAllChildren(transform.parent);
	   transforms.Add(transform.parent);
	   foreach(Transform t in transforms){
	      HitBox hb = t.GetComponent<HitBox>() as HitBox;
	      if(hb != null)
	         hb.npc = (NPC)this;
	      Hand h = t.GetComponent<Hand>() as Hand;
	      if(h != null){
	         h.npc = (NPC)this;
	         h.active = true;
	      }
	      SpeechController speech = 
	         t.GetComponent<SpeechController>() as SpeechController;
	      if(speech != null){
	         sc = speech;
	         speech.npc = (NPC)this;
	         
	      }
	         
	   }
	}
	List<Transform> GetAllChildren(Transform tran){
	   List<Transform> transforms = new List<Transform>();
	   foreach(Transform t in tran){
	      transforms.Add(t);
	      transforms.AddRange(GetAllChildren(t));
	   }
	   return transforms;
	}
	void Start () {
	   anim = transform.parent.GetComponent<Animator>();
	   aliveHash = Animator.StringToHash("Alive");
	   crouchedHash = Animator.StringToHash("Crouched");
	   walkingHash = Animator.StringToHash("Walking");
	   holdRifleHash = Animator.StringToHash("HoldingRifle");
	   aimRifleHash = Animator.StringToHash("Aiming");
		if(activeItem == null)
		   Drop();
		if(player > 0){
			AssignPlayer(player);
		}
		
	}
	void LateUpdate(){	
	   if(torso != null)   
	      torso.rotation = 
	         Quaternion.Euler(new Vector3(headRotx, headRoty, torso.rotation.z));
	}
	void AssignPlayer(int _player){
		player = _player;
      ai = head.GetComponent(typeof(NPCAI)) as NPCAI;
		if(player <5 && player > 0){
		   GameController.controller.AddPlayer(this);
			StartCoroutine("PlayerInputRoutine");
	   }
		if(player == 5){
		   if(ai != null){
		      region = GameController.controller.GetRegion(
			      transform.position);
			   ai.Begin(this);
			   sc = body.transform.GetComponent<SpeechController>();
			   
			}
			else
			   print("ai not found");
		}
	}
	IEnumerator PlayerInputRoutine(){
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		while(true){
		   Rigidbody rb = body.transform.GetComponent<Rigidbody>();
		   if(!falling && rb != null && rb.velocity.y < 0f){
		      falling =true;
		      fallOrigin = transform.position;
		   }
		   if(!GameController.controller.paused){
		      if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
	            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)){
	         if(!walking){
	            walking = true;
	            anim.SetBool(walkingHash, true);
	         }
		      if(Input.GetKey(KeyCode.W)) 
			      Move(0,Input.GetKey(KeyCode.LeftShift));
		      if(Input.GetKey(KeyCode.S))
			      Move(1,Input.GetKey(KeyCode.LeftShift));
		      if(Input.GetKey(KeyCode.D))
			      Move(2,Input.GetKey(KeyCode.LeftShift));
		      if(Input.GetKey(KeyCode.A))
			      Move(3,Input.GetKey(KeyCode.LeftShift));
		   }
			   else if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) ||
			         Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D)){
		         if(walking){
		            walking = false;
		            anim.SetBool(walkingHash, false);
		         }
			   }
			   if(Input.GetKeyDown(KeyCode.Space))
				   StartCoroutine("JumpRoutine");
			   if(Input.GetKeyDown(KeyCode.LeftControl))
				   ToggleCrouch();
			   if(Input.GetKeyUp(KeyCode.LeftControl))
				   ToggleCrouch();
			   if(Input.GetMouseButtonDown(0))
			      Use(0);
			   if(Input.GetMouseButtonDown(1))
			      Use(1);
			   if(Input.GetKeyDown(KeyCode.R))
			      Use(2);
			   if(Input.GetKeyDown(KeyCode.Q))
			      Drop();
			   if(Input.GetKeyDown(KeyCode.E))
			      Interact();
			   if(Input.GetKeyDown(KeyCode.LeftArrow))
			      Interact(1);
			   if(Input.GetKeyDown(KeyCode.UpArrow))
			      Interact(2);
			   if(Input.GetKeyDown(KeyCode.RightArrow))
			      Interact(3);
			   if(Input.GetKeyDown(KeyCode.DownArrow))
			      Interact(4);
			   if(Input.GetKeyDown(KeyCode.Backspace))
			      Interact(5);
			   float rotx = -Input.GetAxis("Mouse Y") * sensitivityX;
			   float roty = Input.GetAxis("Mouse X") * sensitivityY;
			   Turn(new Vector3(rotx, roty, 0f));
		   }
		   else{
		      if(Input.GetKeyDown(KeyCode.LeftArrow))
			      hController.MoveFocus(-1, 0);
			   if(Input.GetKeyDown(KeyCode.RightArrow))
			      hController.MoveFocus(1, 0);
			   if(Input.GetKeyDown(KeyCode.UpArrow))
			      hController.MoveFocus(0, -1);
			   if(Input.GetKeyDown(KeyCode.DownArrow))
			      hController.MoveFocus(0,1);
			   if(Input.GetKeyDown(KeyCode.Return))
			      hController.Select();
			   if(Input.GetKeyDown(KeyCode.RightShift))
			      hController.Back();
		   }
		   
			yield return new WaitForSeconds(0.01f);
		}
	}

	public void Move(int direction, bool running){
	   Rigidbody rb = body.transform.GetComponent<Rigidbody>();
	   if(rb != null){
		   float pace = speed;
		   Vector3 pos = body.transform.position;
		   Vector3 dest =new Vector3();
		   Vector3 dir = new Vector3();
		   if(running){
			   pace *= 1.25f;
		   }
		   
		   if(direction == 0){
		      dest = pos+ body.transform.forward * pace;
			   dir = body.transform.forward;
		   }
		   else if(direction == 1){
	   	   dest = pos + body.transform.forward * -pace;
			   dir = -body.transform.forward;
		   }
		   else if(direction == 2){
			   dest = pos + body.transform.right * pace;
			   dir = body.transform.right;
		   }
		   else{
			   dest = pos + body.transform.right * -pace;
			   dir = -body.transform.right;
		   }
		   int layerMask = 1 << 8;
		   layerMask = ~layerMask;
		   RaycastHit[] hits =
		      Physics.RaycastAll(pos + new Vector3(0,1,0),dir,3*pace, layerMask);
		   if(hits.Length == 0)
		       rb.MovePosition(dest);
		}
	}
	void Turn(Vector3 direction){
		headRotx += direction.x;
		headRoty += direction.y;
		bodyRoty += direction.y;
		if(headRotx > rotxMax)
			headRotx = rotxMax;
		if(headRotx < -rotxMax)
			headRotx = -rotxMax;
			
		head.transform.rotation = Quaternion.Euler(headRotx, headRoty, 0f);
		transform.parent.rotation = Quaternion.Euler(0f,bodyRoty, 0f);
	}
	IEnumerator JumpRoutine(){
	   if(jumpCoolDown){
	      jumpCoolDown = false;
		   Rigidbody rb = body.GetComponent<Rigidbody>();
		   int lifts = 5;
		   int jumpforce = 150;
		   while(lifts > 0){
			   lifts--;
			   rb.AddForce(body.transform.up* jumpforce);
			   yield return new WaitForSeconds(0.001f);
		   }
		}
		yield return new WaitForSeconds(0f);
	}
	
	void ToggleCrouch(){
		anim.SetBool(crouchedHash, !anim.GetBool(crouchedHash));
	}
	public void ReceiveDamage(int damage, GameObject weapon){
	   if(health > 0 && weapon != activeItem){
		   health -= damage;
		   if(health<1){
		      health = 0;
			   StopAllCoroutines();
			   anim.SetBool(aliveHash, false);
			   SkinnedMeshRenderer mesh = 
			      transform.GetComponent<SkinnedMeshRenderer>();
			   Destroy(body.transform.GetComponent<Rigidbody>());
			   Destroy(body.transform.GetComponent<BoxCollider>());
			   
			   if(mesh != null){
			      foreach(Transform t in mesh.bones){
			         t.gameObject.AddComponent(typeof(Rigidbody));
			         t.gameObject.AddComponent(typeof(CharacterJoint));
			      }
			      foreach(Transform t in mesh.bones){
			         CharacterJoint cj = t.GetComponent<CharacterJoint>();
			         if(cj != null)
			            cj.connectedBody = t.parent.GetComponent<Rigidbody>();
			      }
			   }
			   
			   if(player == 1){
			      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			   }
		   }
	   }
	   
	}
	public void Use(int use){
	   if(activeItem != null){
		   FPSItem item =  activeItem.GetComponent(typeof(FPSItem)) as FPSItem;
         if(item == null){
            foreach(Transform child in activeItem.transform){
               item = child.gameObject.GetComponent(typeof(FPSItem)) as FPSItem;
               if(item != null)
                  break;
            }
         }
         if(item == null && activeItem.transform.parent != null){
            GameObject parent = activeItem.transform.parent.gameObject;
            item = parent.GetComponent(typeof(FPSItem)) as FPSItem;
         }  
		   if(item != null){
            if(item.GetInteractionMode() == 1)
			      item.Use(use);
			   else if (item.GetInteractionMode() == 2)
			      item.Use(this);
		   }
		   else{
		      print("FPSItem was null.");
         }
		}
		else if (GameController.controller.debug == 1){
		   print("Character's item was null.");
		}
	}
	
	public void Equip(int itemIndex){
	   
	   if(itemIndex > -1 && itemIndex < inventory.Count){
	      anim.SetBool(aimRifleHash,false);
	      ItemData itemDat = inventory[itemIndex].GetData();
	      GameObject prefab = Resources.Load(itemDat.prefabName) as GameObject;
	      if(prefab != null){
	         GameObject itemGO = (GameObject)GameObject.Instantiate(prefab, 
			      transform.position, Quaternion.identity);
	         if(itemGO != null){
	            FPSItem item = GetFPSItem(itemGO);
	            if(item != null){
	               
	               if(item.IsParent())
	                  item.GetGameObject().transform.parent = hand.transform;
	               else
	                  item.GetGameObject().transform.parent.parent =
	                     hand.transform;
	               
	               item.AssumeHeldPosition(this);
		            activeItem = item.GetGameObject();
		            inventory.Remove(inventory[itemIndex]);
		            if(item.ConsumesAmmo())
		               LoadAmmo(item);
		               
		            
		         }
		      }
		   }
		}
	}
	public void LoadAmmo(FPSItem item){
	   FPSItem ammoType = item.AmmoType();
	   if(ammoType != null){
	      int ammoCount = 0;
	      for(int i = 0; i< inventory.Count; i++){
	         if(inventory[i].displayName.Equals(ammoType.GetDisplayName())){
	            ammoCount += inventory[i].stack;
	            inventory.Remove(inventory[i]);
	            i--;
	         }
	      }
	      item.LoadAmmo(ammoCount);
	   }
	   Destroy(ammoType.GetGameObject());
	   
	}
	public void UnloadAmmo(FPSItem item){
	   int ammoCount = item.UnloadAmmo();
	   FPSItem ammoType = item.AmmoType();
	   ItemData ammoDat = ammoType.SaveData();
	   if(ammoType != null){
	      while(ammoCount > 0){
	         if(ammoCount > ammoType.StackMax()){
	            ammoCount-= ammoType.StackMax();
               ammoDat.stack = ammoType.StackMax();
               ammoType.LoadData(ammoDat);
	            StoreItem(ammoType);
	         }
	         else{
	            ammoDat.stack = ammoCount;
	            ammoType.LoadData(ammoDat);
	            StoreItem(ammoType);
	            ammoCount = 0;
	         }
	      }
	      Destroy(ammoType.GetGameObject());
	   }
	}
	void StoreItem(FPSItem item){
	    ItemData dat = item.SaveData();
	    if(item.ConsumesAmmo())
          UnloadAmmo(item);
	    if(dat != null){
	       bool stackFound = false;
	       foreach(InventorySlot ldat in inventory){
	         if(ldat.displayName.Equals(dat.displayName)){
	            if(ldat.stack+ dat.stack <= ldat.stackSize){
	               ldat.stack += dat.stack;
	               dat.stack = 0;
	               stackFound = true;
	               break;
	            }
	            else{
	               if(ldat.stack < ldat.stackSize){
	                  dat.stack -= ldat.stackSize -ldat.stack;
	                  ldat.stack = ldat.stackSize;
	               }
	            }
	         }
	       }
	       if(!stackFound || dat.stack > 0)
	         inventory.Add(new InventorySlot(dat));
	       Destroy(item.GetGameObject());
	    }
	    else if(GameController.controller.debug == 1)
         print("item data null");
	}
	public void DiscardItem(int itemIndex){
	   if(itemIndex > -1 && itemIndex < inventory.Count){
	      inventory[itemIndex].stack--;
	      GameObject prefab = 
	         Resources.Load(inventory[itemIndex].prefabName) as GameObject;
	      if(prefab != null){
	         GameObject itemGO = (GameObject)GameObject.Instantiate(prefab,
				hand.transform.position,Quaternion.identity);
				if(itemGO != null){
				   FPSItem fpsitem = GetFPSItem(itemGO);
   				if(fpsitem != null){
                  ItemData dat = inventory[itemIndex].data;
                  dat.stack = 1;
   				   fpsitem.LoadData(dat);
   				   fpsitem.GetGameObject().transform.position = 
   				      hand.transform.position;
   				   if(fpsitem.ConsumesAmmo())
   				      fpsitem.UnloadAmmo();
   				   
   				   if(GameController.controller.debug == 1)
	                  print("Item discarded successfully");
				   }
				   else if (GameController.controller.debug == 1)
				      print("FPSItem not found");
				}
				else if (GameController.controller.debug == 1)
               print("item gameObject not found");
	      }
	      else if (GameController.controller.debug == 1)
	         print("Prefab not found");
	      if(inventory[itemIndex].stack < 1){
	         inventory.RemoveAt(itemIndex);
         }
         
	   }
	   
   }
	public void Interact(){
		if(itemInReach != null){
			FPSItem fpsitem= GetFPSItem(itemInReach);
			if(fpsitem != null){
			   if(fpsitem.GetInteractionMode() == 0){
			      fpsitem.Use(0);
			      SpeechController speechController = 
			         itemInReach.GetComponent<SpeechController>();
			      if(hController != null && speechController != null)
			         hController.sc = speechController;
			   }
			   else{
			      if(!fpsitem.IsParent() 
			            || fpsitem.GetGameObject().transform.parent == null)
				   StoreItem(fpsitem);
				}
				
			}
			else if(GameController.controller.debug == 1)
			   print("fpsitem is null");
			
		}
		else if(GameController.controller.debug == 1)
		   print("itemInReach is null");
	}
	public void Interact(int i){
	   if(itemInReach != null){
	      FPSItem fpsitem = GetFPSItem(itemInReach);
	      if(fpsitem !=null && fpsitem.GetInteractionMode() == 0){
	         fpsitem.Use(i);
	      }  
	   }
	   if(hController != null && hController.sc != null){
	         if(i < 5 && i > 0)
	            hController.sc.Use(i);
	         if(i==5)
	            hController.sc = null;
      }
	}
	public void Drop(){
	   FPSItem item = GetFPSItem(activeItem);
	   if(activeItem != defaultItem){
	      if(item != null){
	         if(item != null){
	            if(item.ConsumesAmmo())
	               UnloadAmmo(item);
	            item.Drop();
	            anim.SetBool(holdRifleHash,false);
	            anim.SetBool(aimRifleHash,false);
	         }
	         if(item.IsChild()){
	             activeItem.transform.parent.parent = null;
	         }
	         else{
	            activeItem.transform.parent = null;
	         }
	      }
		   if(defaultItem != null){
	         activeItem = defaultItem;
	         item = GetFPSItem(activeItem);
	         item.AssumeHeldPosition(this);
	         
	      }
	      else{
	         activeItem = null;
	         if(GameController.controller.debug == 1)
	            print("Default item null");
	      }
	   }
	   else{
	      item = GetFPSItem(activeItem);
	      
	   }
	   
		
	}
	public FPSItem GetFPSItem(GameObject gitem){
	   if(gitem != null){
	      FPSItem item = gitem.GetComponent(typeof(FPSItem)) as FPSItem;
	      if(item == null){
            foreach(Transform child in gitem.transform){
               item = child.gameObject.GetComponent(typeof(FPSItem)) as FPSItem;
               if(item != null)
                  break;
            }
         }
         if(item == null && gitem.transform.parent != null){
            GameObject parent = gitem.transform.parent.gameObject;
            item = parent.GetComponent(typeof(FPSItem)) as FPSItem;
         }
         return item;
      }
      if(GameController.controller.debug == 1)
         print("gitem null");
      
      return null;
   }
	public void ItemInReach(GameObject gitem){
	   FPSItem item =  GetFPSItem(gitem);
	   if(item != null && item.GetGameObject() != activeItem 
	         && !item.GetDisplayName().Equals("")){
			itemInReach = gitem;
			itemInReachName = item.GetDisplayName();
		}
		else if(gitem != null){
		   HitBox hb = gitem.GetComponent<HitBox>() as HitBox;
		   if(hb != null && hb.npc != null &&
		          hb.npc.GetSpeechController() != null){
		      SpeechController speech = hb.npc.GetSpeechController();
		      itemInReach = speech.gameObject;
		      itemInReachName = speech.GetDisplayName();
		   }
		   else{
		      itemInReach = null;
		      itemInReachName = "";
		   }
      }
      else{
         itemInReach = null;
		   itemInReachName = "";
      }
      
		
	}
	public ItemData GetData(){
	   ItemData dat = new ItemData();
	   dat.displayName = displayName;
	   dat.prefabName = prefabName;
	   dat.ints.Add(health);
	   dat.floats.Add(body.transform.position.x);
	   dat.floats.Add(body.transform.position.y);
	   dat.floats.Add(body.transform.position.z);
      dat.floats.Add(body.transform.eulerAngles.x);
      dat.floats.Add(body.transform.eulerAngles.y);
      dat.floats.Add(body.transform.eulerAngles.z);
      dat.bools.Add(activeItem != null && activeItem != defaultItem);
      if(ai != null){
	      dat.itemData.Add(ai.GetData());
	   }
	   else{
	      ItemData d = new ItemData();
	      d.displayName = "Null ai";
	      dat.itemData.Add(d);
	   }
	   if(sc != null)
	      dat.itemData.Add(sc.GetData());
	   else{
	      ItemData d = new ItemData();
	      d.displayName = "Null speech controller";
	      dat.itemData.Add(d);
	   }
	   dat.ints.Add(inventory.Count);
	   //print("Inventory count:" + inventory.Count);
	   foreach(InventorySlot item in inventory){
	      dat.itemData.Add(item.GetData());
	   }
	   if(dat.bools[0]){
	      FPSItem i = GetFPSItem(activeItem);
	      dat.itemData.Add(i.SaveData());
	   }
	   return dat;
	}
	public void LoadData(ItemData dat){
	   displayName = dat.displayName;
	   prefabName = dat.prefabName;
	   health = dat.ints[0];
	   if(ai != null)
	      ai.LoadData(dat.itemData[0]);
	   if(sc != null)
   	   sc.LoadSpeechData(dat.itemData[1]);
	   for(int i = 2; i<dat.ints[1]+2; i++){
	      inventory.Add(new InventorySlot(dat.itemData[i]));
	   }
	   body.transform.position = new Vector3(dat.floats[0],
	      dat.floats[1],dat.floats[2]);
	   body.transform.rotation = Quaternion.Euler(dat.floats[3],
	      dat.floats[4],dat.floats[5]);
	      bodyRoty += dat.floats[4];
	      headRoty += dat.floats[4];
	   if(dat.bools[0]){
	      int index = dat.itemData.Count -1;
	      inventory.Add(new InventorySlot(dat.itemData[index]));
	      Equip(inventory.Count -1);
	   }
	}
	void Move(int x, int z){
	   bool running = true;
	   if(x > 0){
         Move(0, running);
      }
      else if(x < 0){
         Move(1, running);
      }
      if(z > 0){
         Move(2, running);
      }
      else if(z < 0){
         Move(3, running);
      }
	}
	IEnumerator MoveToNode(NavNode node){
      yield return StartCoroutine("MoveToRoutine", node.position);
   }
   IEnumerator MoveToNode(int node){
      yield return StartCoroutine("MoveToRoutine",
         region.nodes[node].position);
   }
   IEnumerator TakePath(List<int> path){
      foreach(int node in path){
         yield return StartCoroutine("MoveToNode",node);
      }
   }
   public List<int> GetPath(int start, int finish){
      List<int> path = new List<int>();
      NavNode[] nodes = new NavNode[region.nodes.Count];
      Stack<NavNode> unexplored = new Stack<NavNode>();
      for(int i = 0; i< nodes.Length; i++){
         nodes[i] = new NavNode(region.nodes[i].GetData());
         nodes[i].distance = -1;
      }
      nodes[start].distance = 0;
      unexplored.Push(nodes[start]);
      bool found = false;
      while(!found && unexplored.Count > 0){
         NavNode node = unexplored.Pop();
         foreach(int edge in node.edges){
            if(nodes[edge].distance == -1){
               nodes[edge].distance = node.distance+1;
               nodes[edge].parent = node.id;
               unexplored.Push(nodes[edge]);
               if(edge == finish)
                  found = true;
            }
         }
      }
      NavNode currentNode = nodes[finish];
      while(currentNode.distance > 0){
         path.Add(currentNode.parent);
         currentNode = nodes[currentNode.parent];
      }
      path.Add(start);
      path.Reverse();
      return path;
   }
   
	public void MoveTo(Vector3 dest){
	   StartCoroutine("MoveToRoutine", dest);
	}
	public IEnumerator MoveToRoutine(Vector3 dest){
	   float navMargin = 5f;
	   while(System.Math.Abs(transform.position.x - dest.x)>navMargin ||
            System.Math.Abs(transform.position.z - dest.z)>navMargin){
         Vector3 oneStep = body.transform.forward + body.transform.position;
         float xMag = oneStep.x-body.transform.position.x;
         float zMag = oneStep.y-body.transform.position.z;
         int dirX = 0;
         int dirZ = 0;
         if(xMag > 0 && dest.x > body.transform.position.x ||
               xMag <0 && dest.x < body.transform.position.x)
            dirX = 1;
         else if(xMag != 0)
            dirX = -1;
         if(zMag > 0 && dest.z > body.transform.position.z ||
               zMag <0 && dest.z < body.transform.position.z)
            dirZ = 1;
         else if(zMag != 0)
            dirZ = -1;
         Move(dirX,dirZ);
         yield return new WaitForSeconds(0.05f);
      }
      yield return new WaitForSeconds(0f);
	}
	NavNode NearestNode(){
      if(region != null && region.nodes.Count != 0){
         NavNode nearest = region.nodes[0];
         float tempDist;
         float minDist = Vector3.Distance(
               body.gameObject.transform.position,nearest.position);
         foreach(NavNode node in region.nodes){
            tempDist = Vector3.Distance( node.position,
                  body.gameObject.transform.position);
            if(minDist > tempDist){
               minDist = tempDist;
               nearest = node;
            }
         }
         return nearest;
      }
      return null;
   }
	
	public void Aim(Vector3 eulers){
	   StartCoroutine("AimRoutine", eulers);
	}
	IEnumerator AimRoutine(Vector3 eulers){
	   Vector3 rotation = hand.transform.rotation.eulerAngles;
	   float margin = 5f;
	   float turnRate = 0.1f;
	   while(Vector3.Distance(rotation, eulers) > margin){
	      float x = rotation.x - eulers.x;
	      float y = rotation.x - eulers.y;
	      Turn(new Vector3(x,y,0f));
	      rotation = hand.transform.rotation.eulerAngles;
	      yield return new WaitForSeconds(turnRate);
	   }
	   yield return new WaitForSeconds(0f);
	}
	public Vector3 CurrentAim(){
	   return hand.transform.rotation.eulerAngles;
   }
	public ItemData[] GetInventory(){
	   List<ItemData> items = new List<ItemData>();
	   return items.ToArray();
	}
	public int Health(){
	   return health;
	}
	public void SetItemInReach(GameObject item){
	   ItemInReach(item);
	}
	public GameObject GetItemInReach(){
	   return itemInReach;
	}
	public GameObject GetGameObject(){
	   return gameObject;
   }
   public void SetAnimationTrigger(int hash){
      anim.SetTrigger(hash);
   }
   public void SetAnimationBool(int hash, bool val){
      anim.SetBool(hash, val);
   }
   public bool GetAnimationBool(int hash){
      return anim.GetBool(hash);
   }
   public FPSItem GetActiveItem(){
      return activeItem.GetComponent<FPSItem>() as FPSItem;
   }
   public bool IsFalling(){
      return falling;
   }
   public Vector3 FallOrigin(){
      return fallOrigin;
   }
   public void Land(){
      falling = false;
      jumpCoolDown = true;
   }
   public string DisplayName(){
      return displayName;
   }
   public SpeechController GetSpeechController(){
      return sc;
   }
   public void TalkTo(NPC other){  
      if(hController != null){
         hController.sc = other.GetSpeechController(); 
         hController.sc.Use(0);    
      }
   }
   public void TalkTo(int option){
      sc.Use(option);
   }
   public int GetFaction(){
      return faction;
   }
}
