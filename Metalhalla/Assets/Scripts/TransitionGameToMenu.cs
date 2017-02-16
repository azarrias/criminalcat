using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionGameToMenu : MonoBehaviour {

    public GameObject ingameMenu;
    public GameObject gameOverUI;
   
	// Use this for initialization
	void Start () {
        ingameMenu.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape) && !ingameMenu.activeSelf && !gameOverUI.activeSelf)
        {
            if (ingameMenu.activeSelf)
            {
                ingameMenu.SetActive(false);
                
                //Game runs at regular speed
                Time.timeScale = 1f; 
            }
            else
            {
                ingameMenu.SetActive(true);
                //Game paused
                Time.timeScale = 0f;
            }
        }

        
	}
}
