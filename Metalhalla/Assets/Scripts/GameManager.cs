using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private bool paused = false;
    public GameObject menu;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (menu.activeSelf)
        {
            paused = !paused;
            OnPause();
        }     
	}

    private void OnPause()
    {
        if (!paused)
            Time.timeScale = 1f;         //Game runs at regular speed
        else
            Time.timeScale = 0f;        //Game paused

    }
}
