********************************************************************************
____________  _____  ______                
|  ___| ___ \/  ___| | ___ \               
| |_  | |_/ /\ `--.  | |_/ / __ _ ___  ___ 
|  _| |  __/  `--. \ | ___ \/ _` / __|/ _ \
| |   | |    /\__/ / | |_/ / (_| \__ \  __/
\_|   \_|    \____/  \____/ \__,_|___/\___|
(Text art Source: http://patorjk.com/software/taag/)
********************************************************************************

This is boiler-plate code for a first-person shooter game.
These scripts work with Unity3D editor 2017.1.1xf1 Ubuntu links below.
https://forum.unity3d.com/threads/unity-on-linux-release-notes-and-known-issues.350256/
http://beta.unity3d.com/download/f4fc8fd4067d/public_download.html

# Hiatus/project dormancy:
My original vision for this project was to recreate the gameplay found in the
3D Fallout series and provide a code base others could fork. I was finishing up
my degree(and subsequently hunting for a job) at that time. I've found a job
and thus my time budget for this project is about to dry up. This repository
will undergo some pruning and improvements to documentation to prepare it for
dormancy. It is unlikely that I will resume work with Unity3D following this
hiatus, and so this project will become the first entry in my portfolio.

********************************************************************************
______               _                                  _   
|  _  \             | |                                | |  
| | | |_____   _____| | ___  _ __  _ __ ___   ___ _ __ | |_ 
| | | / _ \ \ / / _ \ |/ _ \| '_ \| '_ ` _ \ / _ \ '_ \| __|
| |/ /  __/\ V /  __/ | (_) | |_) | | | | | |  __/ | | | |_ 
|___/ \___| \_/ \___|_|\___/| .__/|_| |_| |_|\___|_| |_|\__|
                            | |                             
                            |_|                             
********************************************************************************

# Current version: 0.6.0

## Roadmap:
0.7 - Project dormancy.

## Backlog

## Current Objectives
-Prepare for project dormancy.
  -Prune non-functioning code.
  -Provide documentation for remaining code.

********************************************************************************
 _____              __ _       
/  __ \            / _(_)      
| /  \/ ___  _ __ | |_ _  __ _ 
| |    / _ \| '_ \|  _| |/ _` |
| \__/\ (_) | | | | | | | (_| |
 \____/\___/|_| |_|_| |_|\__, |
                          __/ |
                         |___/ 
********************************************************************************

## Resources/kits.txt
Contains default kits used in Arena
## Resources/maps.txt
Contains all maps used in Arena
## Resources/items.txt
Contains all items that can be used in user-defined kits.


********************************************************************************
  ___           _     _ _            _                  
 / _ \         | |   (_) |          | |                 
/ /_\ \_ __ ___| |__  _| |_ ___  ___| |_ _   _ _ __ ___ 
|  _  | '__/ __| '_ \| | __/ _ \/ __| __| | | | '__/ _ \
| | | | | | (__| | | | | ||  __/ (__| |_| |_| | | |  __/
\_| |_/_|  \___|_| |_|_|\__\___|\___|\__|\__,_|_|  \___|
                                                        
********************************************************************************

# Architecture at a glance:

## Session.cs- Singleton that routes method calls between classes and contains
global state.
SessionEvent.cs- A record for an event to be handled by either the session,
arena, or world.

JukeBox.cs- Responsible for loading and playing music from /Resources/Music/

## /Arena/Arena.cs- Manages the "Arena" gamemode.
  Kit.cs - Contains items to equip and store on an Actor. Loads kits defined in
  /Resources/Kits.txt configuration file.

CellSaver.cs- Used to automatically create or update /Resources/world.master
using open Unity scenes.

## World.cs- Manages the "Adventure" mode.
  Cell.cs- Contains the Actors and Items of a specific
  Building.cs- A collection of Cells representing rooms in a building.
  HoloDeck.cs- Creates Holocells to load rooms and contiguous overworld.
    HoloCell.cs- Renders the items, Actors of a portion of the world.
  MapRecord.cs- The state of a map with auto-incrementing IDs.
  Cartographer.cs- Offers a static method that procedurally-generates a new map
  using the contents of /Resources/world.master

/Quests/- Quest classes inheriting from Quest.cs
/Menus/- Menu classes ineriting from Menu.cs
  MenuManger.cs- Changes the active Menu, handling constructors and some state. 
/Items/- Item Classes inheriting from Item.cs


## /Actor/- Components of Actor.cs class.
  EquipSlot.cs- Manages the equipping and use of Actor's weapon slots.
  PaperDoll.cs- Manages the equipping of the Actor's body with clothes.
  HitBox.cs- Routes attacks to actor.
  Inventory.cs- Storage for an Actor's unequipped items. 
  StatHandler.cs- Handles the roleplaying number-crunching of an Actor. 
  SpeechTree.cs- Handles the dialogue state of an NPC Actor.
    SpeechNode.cs - A single dialogue prompt.
    Option.cs- A single dialogue option.
  DeviceManager.cs - Collects input from selected device. 
  ActorInputHandler.cs - Controls Actor according to input from DeviceManager.
  Damage.cs - A record containing the data of a single attack.
