using UnityEngine;
using System.Collections;

public class Blade : MonoBehaviour {
   public MeleeWeapon weapon;
	void OnTriggerEnter(Collider col){
	   if(weapon.cooled)
	      weapon.BladeImpact(col);
	}
	void OnTriggerStay(Collider col){
	   if(weapon.cooled)
	      weapon.BladeImpact(col);
	}
	void OnCollisionEnter(Collision col){
	   if(weapon.cooled)
	      weapon.BladeImpact(col.collider);
	}
	void OnCollisionStay(Collision col){
	   if(weapon.cooled)
	      weapon.BladeImpact(col.collider);
	}

}
