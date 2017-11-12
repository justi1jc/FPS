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
  List<AudioClip> ambient, combat, menu; // Playlists to choose from.
  MonoBehaviour parent;
  public GameObject gameObject;
  AudioSource audioSource;
  public string playList;
  
  /**
    Default constructor.
    @param {MonoBehaviour} parent - parent of this JukeBox
  */
  public JukeBox(MonoBehaviour parent){
    playList = "Ambient";
    this.parent = parent;
    this.gameObject = new GameObject();
  }
  
  /**
    * Begins playing music from a playlist.
    * @param {string} playlist - The name of the playlist that will be played from.
    */
  public void Play(string playList = "Ambient"){
    playList = playList;
    if(gameObject == null){
      gameObject = new GameObject();
    }
    if(audioSource == null){
      audioSource = gameObject.AddComponent<AudioSource>();
    }
    parent.StopCoroutine("PlayFolder");
    parent.StartCoroutine(PlayFolder());
  }
  
  /**
    * Ceases playing music and destroys the gameObject associated with this
    * JukeBox.
    */
  public void Stop(){
    parent.StopAllCoroutines();
    if(audioSource != null){
      audioSource.Pause();
      GameObject.Destroy(gameObject);
    }
  }
  
  /**
    * Plays music from the currently selected playlist.
    */
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

  /**
    * Returns the AudioClips in a particular folder.
    * @param {string} folder - The name of the foler to check in.
    * @return {List<AudioClip>} the clips found in that folder. 
    */  
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
