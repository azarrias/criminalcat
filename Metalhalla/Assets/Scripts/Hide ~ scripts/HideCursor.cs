using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCursor : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //Hide mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	
}
