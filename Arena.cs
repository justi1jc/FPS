/*
*   The arena module will spawn waves of enemies growing in size for the purpose
*   of demonstrating existing combat features. This script will be
*   removed with the addition of functioning interiors.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Arena : MonoBehaviour {
  public int wave = 0;
  public List<Actor> enemies;
  public Session s;
  public int playerNumber = -1;
  public string[] items = {"Brick", "Bullet", "Rifle", "Sword", "Food"};
  void Start(){
    s = Session.session;
    StartCoroutine(StartMenu());
  }
  
  IEnumerator StartMenu(){
    while(playerNumber == -1){
      yield return new WaitForSeconds(0.01f);
    }
    if(playerNumber == 1){ s.Spawn("Player1", new Vector3());}
    else{ s.Spawn("Player2", new Vector3()); }
    Camera cam = gameObject.GetComponent<Camera>();
    if(cam){ Destroy(cam); }
    InitiateWave();
  }
  
  void OnGUI(){
    if(playerNumber != -1){ return; }
    GUI.skin.button.wordWrap = true; // Make sure text wraps in buttons.
    GUI.skin.box.wordWrap = true; // Make sure text wraps in boxes.
    int iw = Screen.width/2;
    int ih = Screen.height/3;
    GUI.Box(
      new Rect(iw/2, 0, iw, ih),
      "Choose your controls."
    );
    
    
    if(GUI.Button(
      new Rect(0, ih, iw, ih),
        "Mouse + Keyboard"
    )){
      playerNumber = 1;
    }
      
    if(GUI.Button(
      new Rect(iw, ih, iw, ih),
        "Controller"
    )){
      playerNumber = 2;
    }
  }
  
  /* Creates a new wave of enemies. */
  void InitiateWave(){
    wave++;
    enemies = new List<Actor>();
    for(int i = 0; i < wave*wave; i++){
      float x = Random.Range(-50f, 50f);
      float z = Random.Range(-50f, 50f);
      int item = Random.Range(0, (items.Length -1));
      enemies.Add(s.Spawn("Enemy", new Vector3(x, 0f, z)).GetComponent<Actor>());
      s.Spawn(items[item], new Vector3(z, 0f, x));
    }
    print("Starting wave " + wave);
    StartCoroutine(MonitorWave());
  }
  
  /* Waits until an existing wave is done. */
  IEnumerator MonitorWave(){
    yield return new WaitForSeconds(1f);
    while(WaveActive()){
      yield return new WaitForSeconds(1f);
    }
    for(int i = 0; i < enemies.Count; i++){
      Destroy(enemies[i].gameObject);
    }
    InitiateWave();
  }
  
  bool WaveActive(){
    for(int i = 0; i < enemies.Count; i++){
      if(enemies[i].health > 0){ return true; }
    }
    return false;
  }
  
}
