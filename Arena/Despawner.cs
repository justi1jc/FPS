/*
   Destroys unwanted clutter in Arena gamemode.
*/

using UnityEngine;
using System.Collections;

public class Despawner : MonoBehaviour{
  
  public void Despawn(float seconds){
    StartCoroutine(DespawnRoutine(seconds));
  }
  
  IEnumerator DespawnRoutine(float seconds){
    yield return new WaitForSeconds(seconds);
    Destroy(gameObject);
  }
}
