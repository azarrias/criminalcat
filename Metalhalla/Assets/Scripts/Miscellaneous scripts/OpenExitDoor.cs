using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenExitDoor : MonoBehaviour {

    public GameObject exitDoor;
    private CloseOpenDoor closeOpenDoor;

    void Awake()
    {
        exitDoor = GameObject.Find("MovingDoorExit");
        closeOpenDoor = exitDoor.GetComponent<CloseOpenDoor>();
    }

   // Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenDoor()
    {
        closeOpenDoor.openExitDoor = true;
        closeOpenDoor.doorSoundSource = AudioManager.instance.PlayDiegeticFx(gameObject, closeOpenDoor.doorSound, 
            false, 1.0f, AudioManager.FX_DOOR_VOL);
    }
}
