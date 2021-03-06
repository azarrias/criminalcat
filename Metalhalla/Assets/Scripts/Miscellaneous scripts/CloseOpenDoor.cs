﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseOpenDoor : MonoBehaviour {

    private  GameObject movingDoor = null;
    public Vector3 localInitialPosition = new Vector3(87.71f, 27.31f, 0.0f);
    private bool closed = false;   
    private bool playerInside = false;
    public float speed = 8.0f;
    public bool openExitDoor = false; //boss has died, door opens again

    [Header("Sound FXs")]
    public AudioClip doorSound;
    public AudioSource doorSoundSource;

    void Awake()
    {
        movingDoor = gameObject;
        if (movingDoor == null)
            Debug.Log("movingDoor not found.");   
    }

    void Start () {

        movingDoor.transform.localPosition = localInitialPosition;
    }
	
	// Update is called once per frame
	void Update () {
        if (!closed && playerInside)
            CloseDoor();

        if(openExitDoor)
        {
            if (gameObject.name == "MovingDoorExit")
                OpenDoorSlowly();

            if (movingDoor.transform.localPosition.y >= localInitialPosition.y)
            {
                openExitDoor = false;
                //if (doorSoundSource.isPlaying)
                //    AudioManager.instance.FadeAudioSource(doorSoundSource, FadeAudio.FadeType.FadeOut, 1.0f, 0.0f);
            }
        }
	}

    public void CloseDoor()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    //To be called when player respawns
    public void OpenDoor()
    {
        gameObject.transform.localPosition = localInitialPosition;
        closed = false;
        playerInside = false; 
    }

    public void OpenDoorSlowly()
    {
         transform.position += Vector3.up * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "BossFightAreaFloor" || LayerMask.LayerToName(collider.gameObject.layer) == "ground")
        {
            closed = true;
            //AudioManager.instance.FadeAudioSource(doorSoundSource, FadeAudio.FadeType.FadeOut, 1.0f, 0.0f);
        }
    }

    public void PlayerInside()
    {
        playerInside = true;
        doorSoundSource = AudioManager.instance.PlayDiegeticFx(gameObject, doorSound, false, 1.0f, AudioManager.FX_DOOR_VOL);
    }

    public void PlayerOutside()
    {
        playerInside = false;
    }
}
