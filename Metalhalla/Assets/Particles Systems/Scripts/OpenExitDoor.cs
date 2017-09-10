using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenExitDoor : MonoBehaviour {

    public GameObject exitDoor;

    void Awake()
    {
        exitDoor = GameObject.Find("MovingDoorExit");
    }

   // Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenDoor()
    {
        exitDoor.GetComponent<CloseOpenDoor>().openExitDoor = true;
    }
}
