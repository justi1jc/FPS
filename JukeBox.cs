/*
  The JukeBox is a class that handles music being played.
  The music folder should be located in the resources folder.
  Within the music folder should be three additional folders, 
  Ambient - Wandering music
  Combat  - Music for fighting
  Menu    - Music for the Main menu
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class JukeBox{
  List<AudioClip> ambient;
  List<AudioClip> combat;
  List<AudioClip> menu;
  MonoBehaviour parent;
  GameObject gameObject;
  AudioSource audioSource;
  
  public string playList;
  
  public JukeBox(MonoBehaviour parent, GameObject gameObject){
    playList = "Ambient";
    this.parent = parent;
    this.gameObject = gameObject;
  }
  
  public void Play(string playList = "Ambient"){
    playList = playList;
    if(audioSource == null){
      audioSource = gameObject.AddComponent<AudioSource>();
    }
    parent.StopCoroutine("PlayFolder");
    parent.StartCoroutine(PlayFolder());
    MonoBehaviour.print("Playing");
  }
  
  public void Stop(){
    parent.StopCoroutine("PlayFolder");
    GameObject.Destroy(audioSource);
  }
  
  IEnumerator PlayFolder(){
    List<AudioClip> songs = new List<AudioClip>();
    switch(playList){
      case "Ambient":
        if(ambient == null){ ambient = GetSongs("Ambient"); }
        songs = ambient;
        break;
      case "Combat":
        if(combat == null){ combat = GetSongs("Combat"); }
        songs = combat;
        break;
      case "Menu":
        if(menu == null){ menu = GetSongs("Menu"); }
        songs = menu;
        break;
    }
    while(audioSource != null && songs.Count > 0){
      int song = UnityEngine.Random.Range(0, songs.Count);
      audioSource.clip = songs[song];
      MonoBehaviour.print("Playing " + songs[song]);
      audioSource.volume = 1f;
      if(PlayerPrefs.HasKey("masterVolume")){
        audioSource.volume = PlayerPrefs.GetFloat("masterVolume");
      }
      audioSource.Play();
      while(audioSource != null && audioSource.isPlaying){
        yield return new WaitForSeconds(1f);
      }
      yield return new WaitForSeconds(1f);
    }
    yield return new WaitForSeconds(0f);  
  }
  
  
  
  public List<AudioClip> GetSongs(string folder){
    
    List<AudioClip> clips = new List<AudioClip>();
    string path = Application.dataPath + "/Resources/Music/" + folder;
    if(!Directory.Exists(path)){ MonoBehaviour.print(path + " Doesn't exist"); return clips; }
    DirectoryInfo dir = new DirectoryInfo(path);
    FileInfo[] info = dir.GetFiles("*.*");
    foreach(FileInfo f in info){
      if(f.Extension == ".mp3" || f.Extension == ".ogg" || f.Extension == ".wav"){
        string file = "Music/" + folder + "/" + f.Name;
        file = file.Remove(file.IndexOf("."));
        AudioClip clip = Resources.Load(file, typeof(AudioClip)) as AudioClip;
        if(clip == null){ MonoBehaviour.print("Null clip:" + file); }
        clips.Add(clip);  
      }
    }
    return clips;
  }
  
}
