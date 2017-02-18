using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransitionGameToMenu : MonoBehaviour {

    public GameObject menu;
    public GameObject gameOverUI;
     
	// Use this for initialization
	void Start () {

        menu.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {

        //Toggle menu
        if (Input.GetKeyDown(KeyCode.Escape) && !menu.activeSelf && !gameOverUI.activeSelf)
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
