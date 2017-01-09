/*      Author: James Justice
 * Exit
 * This script is intended as a means for the player to exit demo builds by 
 * pressing the escape key until menus are available.
 * 
 * */


using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
				Application.Quit ();
			}
	}
}
