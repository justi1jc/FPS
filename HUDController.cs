/*
        Author: James Justice


        This script controls the heads-up display for a player. 
        The NPC interface is cast aside due to the fact that the
        player only controls a humanoid character.



*/




﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDController : MonoBehaviour {
   public HumanoidController player;
   public SpeechController sc;
	public int menu = 0;
   public bool pressed = false;
	public AudioClip menuBing;
	public int focusX =0;
	public int focusY =0;
	public Vector2 scrollPosition = Vector2.zero;
	public int selectedItem = -1;
   public bool selectPressed;
   public bool backPressed;
   public bool displayMessage;
   public string message;
   public AudioClip messageSound;
	public void MoveFocus(int x, int y){
	   focusX += x;
	   focusY += y;
	}
	public void Select(){
	   if(!pressed){
	      selectPressed = true;
	      StartCoroutine("PressCoolDown");
	   }
	}
	public void Back(){
	   if(!pressed){
	      backPressed = true;
	      StartCoroutine("PressCoolDown");
	   }
	}
   void OnGUI(){
      if(Input.GetKeyDown(KeyCode.Escape) && !pressed){
         
         StartCoroutine("PressCoolDown");
         if(menu == 0){
            ChangeMenu(1);
         }
         else{
            ChangeMenu(0);
         }
            
      }
      else if(Input.GetKeyDown(KeyCode.Tab) && !pressed){
         StartCoroutine("PressCoolDown");
         if(menu == 5){
            ChangeMenu(0);
         }
         else{
            ChangeMenu(5);
         }
      }
         
      switch(menu){
         case 0:
            if(player != null)
               DisplayHUD();
            break;
         case 1:
            DisplayPauseMenu();
            break;
         case 2:
            DisplayOptionsMenu();
            break;
         case 3:
            DisplayControlsMenu();
            break;
         case 4:
            DisplayAudioMenu();
            break;
         case 5:
            DisplayInventory();
            break;
         case 6:
            DisplayLoadMenu();
            break;
      }
   
      
      
   }
   
   public IEnumerator PressCoolDown(){
      pressed = true;
      yield return new WaitForSeconds(0.01f);
      pressed = false;
   }
   public void ChangeMenu(int selection){
      focusX = 0;
      focusY = 0;
      selectPressed = false;
      backPressed = false;
      if(menu == 0 && selection > 0){
         Cursor.visible = true;
		   Cursor.lockState = CursorLockMode.None;
		   GameController.controller.paused = true;
      }
      else if(menu > 0 && selection == 0){
         Cursor.visible = false;
		   Cursor.lockState = CursorLockMode.Locked;
		   GameController.controller.paused = false;
      }
      menu = selection;
   }
   public void DisplayHUD(){
         int width = Screen.width/5;
         int height = Screen.height/25;
         GUI.Box(new Rect(0, Screen.height-(3 * height), width , height),
                "health:" + player.health);
         //GUI.Box(new Rect(0, Screen.height- (2 * height), width,
         //       height), "Fatigue Bar");
         //GUI.Box(new Rect(0, Screen.height- height, width,
         //       height), "Mana Bar");
         FPSItem item = player.activeItem.GetComponent(typeof(FPSItem)) as FPSItem;
         if(item != null)
            GUI.Box(new Rect(Screen.width - width, Screen.height- height,
                width, height), item.GetItemInfo());
         if(player.itemInReach != null)
            GUI.Box(new Rect((2 * width), Screen.height- height,
                width, height), "" + player.itemInReachName);
         if(sc != null){
            GUI.Box(new Rect((Screen.width-width)/2, Screen.height- (3*height),
                width, 2*height), sc.visibleMessage);
            if(sc.visibleOptions.Length > 0 && !sc.visibleOptions[0].Equals("")){
               GUI.Box(new Rect(Screen.width - (width), Screen.height- (6f*height),
                width, height), sc.visibleOptions[0]);
            }
            if(sc.visibleOptions.Length > 1 && !sc.visibleOptions[1].Equals("")){
               GUI.Box(new Rect(Screen.width - 1.5f*(width), Screen.height- (7*height),
                width, height), sc.visibleOptions[1]);
            }
            if(sc.visibleOptions.Length > 2 && !sc.visibleOptions[2].Equals("")){
               GUI.Box(new Rect(Screen.width - 2*width, Screen.height- (6f*height),
                width, height), sc.visibleOptions[2]);
            }
            if(sc.visibleOptions.Length > 3 && !sc.visibleOptions[3].Equals("")){
               GUI.Box(new Rect(Screen.width - 1.5f*(width), Screen.height- (5*height),
                width, height), sc.visibleOptions[3]);
            }
            
         }
         if(displayMessage){
            print("Displaying!");
            GUI.Box(new Rect(0, (3 * height), width , height),
             message);
         }
   }
   public void DisplayPauseMenu(){
      int width = Screen.width - 200;
      int height = Screen.height/5;
      GUI.Box(new Rect(0, 0, Screen.width, Screen.height),"");
      if(GUI.Button(new Rect(100, Screen.height -height, width, height), "Return") && !pressed){
         StartCoroutine("PressCoolDown");
         ChangeMenu(0);
         if(menuBing){
				AudioSource.PlayClipAtPoint (menuBing, transform.position,
				   GameController.controller.masterVolume * GameController.controller.effectsVolume);
			}
      }
      if(GUI.Button(new Rect(100, Screen.height -(2 * height), width, height), "Options") && !pressed){
         StartCoroutine("PressCoolDown");
         ChangeMenu(2);
         if(menuBing){
				AudioSource.PlayClipAtPoint (menuBing, transform.position,
				   GameController.controller.masterVolume * GameController.controller.effectsVolume);
			}
      }
      if(GUI.Button(new Rect(100, Screen.height -(3 * height), width, height), "QuitGame") && !pressed){
         Application.Quit();
      }
      if(GUI.Button(new Rect(100, Screen.height-(4*height), width, height), "Save Game") && !pressed){
         StartCoroutine("PressCoolDown");
         ChangeMenu(0);
         GameFile data = GameController.controller.GetData();
         if(data != null)
            GameController.controller.SaveGame(data);
      }
      if(GUI.Button(new Rect(100, Screen.height-(5* height), width, height), "LoadGame") && !pressed){
         StartCoroutine("PressCoolDown");
         ChangeMenu(6);
      }
         
   }
   public void DisplayOptionsMenu(){
      int width = Screen.width - 200;
      int height = Screen.height/5;
      GUI.Box(new Rect(0, 0, Screen.width, Screen.height),"");
      if(GUI.Button(new Rect(100, Screen.height -height, width, height), "Back") && !pressed){
         StartCoroutine("PressCoolDown");
         ChangeMenu(1);
         if(menuBing){
				AudioSource.PlayClipAtPoint (menuBing, transform.position,
				   GameController.controller.masterVolume * GameController.controller.effectsVolume);
			}
      }
      if(GUI.Button(new Rect(100, Screen.height -(2 * height), width, height), "Controls") && !pressed){
         StartCoroutine("PressCoolDown");
         ChangeMenu(3);
         if(menuBing){
				AudioSource.PlayClipAtPoint (menuBing, transform.position,
				   GameController.controller.masterVolume * GameController.controller.effectsVolume);
			}
      }
      if(GUI.Button(new Rect(100, Screen.height -(3 * height), width, height), "Audio") && !pressed){
         StartCoroutine("PressCoolDown");
         ChangeMenu(4);
         if(menuBing){
				AudioSource.PlayClipAtPoint (menuBing, transform.position,
				   GameController.controller.masterVolume * GameController.controller.effectsVolume);
			}
      }
   }
   public void DisplayControlsMenu(){
      int width = Screen.width - 200;
      int height = Screen.height/5;
      GUI.Box(new Rect(0, 0, Screen.width, Screen.height),"");
      if(GUI.Button(new Rect(100, Screen.height -height, width, height), "Back") && !pressed){
         StartCoroutine("PressCoolDown");
         ChangeMenu(2);
         if(menuBing){
				AudioSource.PlayClipAtPoint (menuBing, transform.position,
				   GameController.controller.masterVolume * GameController.controller.effectsVolume);
			}
      }
      
      player.sensitivityY = GUI.HorizontalSlider(new Rect(100, Screen.height- (1.5f * height),
          width, height/2), player.sensitivityY, 1.0F, 4.0F);
      GUI.Box(new Rect(200, Screen.height-(2 * height),
          width, height/2),"Horizontal sensitivity.");
      player.sensitivityX = GUI.HorizontalSlider(new Rect(100, Screen.height- (2.5f * height),
          width, height/2), player.sensitivityX, 1.0F, 4.0F);
      GUI.Box(new Rect(200, Screen.height-(3 * height),
          width, height/2),"Vertical sensitivity.");
   }
   public void DisplayAudioMenu(){
      int width = Screen.width - 200;
      int height = Screen.height/5;
      GUI.Box(new Rect(0, 0, Screen.width, Screen.height),"");
      if(GUI.Button(new Rect(100, Screen.height -height, width, height), "Back") && !pressed){
         StartCoroutine("PressCoolDown");
         ChangeMenu(2);
         if(menuBing){
				AudioSource.PlayClipAtPoint (menuBing, transform.position,
				   GameController.controller.masterVolume * GameController.controller.effectsVolume);
			}
      }
      
      GameController.controller.effectsVolume = GUI.HorizontalSlider(new Rect(100, Screen.height- (1.5f * height),
          width, height/2), GameController.controller.effectsVolume, 0.0F, 1.0F);
      GUI.Box(new Rect(200, Screen.height-(2 * height),
          width, height/2),"Effects Volume");
      GameController.controller.musicVolume = GUI.HorizontalSlider(new Rect(100, Screen.height- (2.5f * height),
          width, height/2), GameController.controller.musicVolume, 0.0F, 1.0F);
      GUI.Box(new Rect(200, Screen.height-(3 * height),
          width, height/2),"Music Volume");
      GameController.controller.masterVolume = GUI.HorizontalSlider(new Rect(100, Screen.height- (3.5f * height),
          width, height/2), GameController.controller.masterVolume, 0.0F, 1.0F);
      GUI.Box(new Rect(200, Screen.height-(4 * height),
          width, height/2),"Master Volume");
   }
   public void DisplayInventory(){
      int scrollWidth = Screen.width/4;
      int boxHeight = Screen.height/ 15;
      GUIStyle normalStyle = new GUIStyle(GUI.skin.box);
      GUIStyle redStyle = new GUIStyle(normalStyle);
      GUIStyle blueStyle = new GUIStyle(normalStyle);
      redStyle.normal.textColor = Color.red;
      blueStyle.normal.textColor= Color.blue;
      if(focusX > 1 || focusX < 0)
         focusX = 0;
      else if(focusX == 1 && focusY > 2)
         focusY = 2;
      else if(focusX == 0 && focusY > player.inventory.Count || focusX < 0)
         focusX = 0;
      if(selectPressed){
         if(focusX == 0)
            selectedItem =focusY;
         else if(focusX == 1){
            if(focusY == 0)
               print("Dropped");
            else if(focusY == 1)
               print("Used");
         }
         
      }
      if(focusX == 0)
         scrollPosition = new Vector2(scrollPosition.y, focusY*boxHeight);
      
      scrollPosition = GUI.BeginScrollView(new Rect(scrollWidth,
            Screen.height/2, scrollWidth, Screen.height/2), scrollPosition,
            new Rect(0, 0, scrollWidth-20,boxHeight* player.inventory.Count),
            false, true);
      print("Inventory count:" +  player.inventory.Count);
      for(int i = 0; i< player.inventory.Count; i++){
         string displayName = player.inventory[i].displayName;
         print(displayName);
         if(player.inventory[i].stack > 1)
            displayName += "(" + player.inventory[i].stack + ")";
         if(focusY == i && focusX == 0){
            if(GUI.Button(new Rect(0,i * boxHeight, scrollWidth, boxHeight ),
                  displayName,blueStyle)){
                selectedItem = i;
            }
         }
         else{
            if(GUI.Button(new Rect(0,i * boxHeight, scrollWidth, boxHeight ),
                  displayName,
                  selectedItem==i ? redStyle : normalStyle)&& !pressed){
                
                selectedItem = i;
            }
         }
           
      }
      GUI.EndScrollView();
      if(selectedItem > -1 && selectedItem < player.inventory.Count){
         GUI.Box(new Rect(Screen.width/2, Screen.height/2, scrollWidth, boxHeight), player.inventory[selectedItem].displayName);
         if(GUI.Button(new Rect(Screen.width/2, Screen.height/2+boxHeight, scrollWidth, boxHeight), "Drop Item") && !pressed){
            StartCoroutine("PressCoolDown");
            if(player.inventory[selectedItem].stack == 1){
               StartCoroutine("PressCoolDown");
               player.DiscardItem(selectedItem);
               selectedItem = -1;
               focusY--;
            }
            else
               player.DiscardItem(selectedItem);
            
         }
         if(GUI.Button(new Rect(Screen.width/2, Screen.height/2+ 2*boxHeight, scrollWidth, boxHeight), "Equip Item") && !pressed){
            if(player.inventory[selectedItem].stack == 1){
               
               player.Equip(selectedItem);
               selectedItem = -1;
               focusY--;
            }
            else
               player.Equip(selectedItem);
            StartCoroutine("PressCoolDown");
         }
      } 
   }
   public void DisplayLoadMenu(){
      List<GameFile> files = GameController.controller.GetFiles();
      int scrollWidth = Screen.width/2;
      int boxHeight = Screen.height/ 15;
      int width = Screen.width - 200;
      int height = Screen.height/5;
      scrollPosition = GUI.BeginScrollView(new Rect(scrollWidth,
            Screen.height/2, scrollWidth, Screen.height/2), scrollPosition,
            new Rect(0, 0, scrollWidth-20,boxHeight* files.Count),
            false, true);
      for(int i = 0; i< files.Count; i++){
         
         if(GUI.Button(new Rect(0,i * boxHeight, scrollWidth, boxHeight ),
               files[i].gameName) && !pressed){
            StartCoroutine("PressCoolDown");
            ChangeMenu(0);
            GameController.controller.LoadData(files[i]);
         }
      }
      GUI.EndScrollView();
      if(GUI.Button(new Rect(100, Screen.height -height, width, height),
            "Back") && !pressed){
         StartCoroutine("PressCoolDown");
         ChangeMenu(1);
         if(menuBing){
				AudioSource.PlayClipAtPoint (menuBing, transform.position,
				   GameController.controller.masterVolume *
				   GameController.controller.effectsVolume);
			}
      }
   }
   public void Display(string message){
      print(player.displayName);
      string[] args = message.Split(' ');
      if(args[0].Equals("sound")){
         messageSound = Resources.Load(args[1]) as AudioClip;
         this.message = message.Substring(args[0].Length+args[1].Length + 2);
      }
      else{
         messageSound = null;
         this.message = message;
      }
         
      StartCoroutine("DisplayMessage");
   }
   IEnumerator DisplayMessage(){
      displayMessage = true;
      print("Start display");
      if(messageSound != null)
         AudioSource.PlayClipAtPoint (messageSound, transform.position,
				   GameController.controller.masterVolume * 
				   GameController.controller.effectsVolume);
      yield return new WaitForSeconds(4f);
      displayMessage = false;
      print("End display");
      
   }
}
