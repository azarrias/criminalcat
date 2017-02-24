using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class TransitionGameToMenu : MonoBehaviour {

    public GameObject menu;
    public GameObject gameOverUI;
    public GameObject endGameUI;
     
	// Use this for initialization
	void Start () {

        menu.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {

        //Toggle menu
        if (Input.GetButton("DisplayMenu") && !menu.activeSelf && !gameOverUI.activeSelf && !endGameUI.activeSelf)
        {
            if (menu.activeSelf)
            {
                menu.SetActive(false);
                
                //Game runs at regular speed
                Time.timeScale = 1f; 
            }
            else
            {
                menu.SetActive(true);

                //Game paused
                Time.timeScale = 0f;
                //Re-select the button in order to highlight it again
                EventSystem.current.SetSelectedGameObject(null);
                GameObject resume = GameObject.FindGameObjectWithTag("GameMenuResume");
                EventSystem.current.SetSelectedGameObject(resume);
                
            }
        }   
	}
}
