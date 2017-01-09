using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour {
   public NPC npc;
   public bool active = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider col){
	   if(active)
	      npc.SetItemInReach(col.gameObject);
   }
   void OnTriggerExit(Collider col){
      if(active && col.gameObject == npc.GetItemInReach()){
         npc.SetItemInReach(null);
      }
   }
}
