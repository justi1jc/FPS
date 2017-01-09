using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {
   public int menu = 0;
   public string entry = "Enter name here";
   public Vector2 scrollPosition = Vector2.zero;
   public void Start(){
      
   }
   
   void OnGUI(){
      switch(menu){
         case 0:
            DisplayTitle();
            break;
         case 1:
            DisplayNew();
            break;
         case 2:
            DisplayLoad();
            break;
      }
      
   }
   public void ChangeMenu(int selection){
      menu = selection;
   }
   public void DisplayTitle(){
      int width = Screen.width - 300;
      int height = Screen.height/8;
      if(GUI.Button(new Rect(0, height, width, height), "New")){
         ChangeMenu(1);
      }
      if(GUI.Button(new Rect(0,(2*height), width, height), "Load")){
         ChangeMenu(2);
      }
      if(GUI.Button(new Rect(0,(3*height), width, height), "Return to Desktop")){
         Application.Quit();
         print("Game quit");
      }
   }
   public void DisplayNew(){
      int width = Screen.width - 200;
      int height = Screen.height/6;
      entry = GUI.TextField(new Rect(100, height, width, height), entry, 25);
      if(GUI.Button(new Rect(100, 2*height, width, height), "Start game")){
         print("Starting game");
         GameController.controller.InitializeGame(entry);
      }
      if(GUI.Button(new Rect(100, 3*height, width, height), "Back")){
         ChangeMenu(0);
      }
      
   }
   public void DisplayLoad(){
      //int width = Screen.width/2;
      //int height = Screen.height/3;
      
      
   }
   
}
