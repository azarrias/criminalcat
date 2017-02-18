using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransitionGameToMenu : MonoBehaviour {

    public GameObject menu;
    public GameObject gameOverUI;
    public GameObject resume;
    public GameObject newGame;

    private bool isIngameMenu;
   
	// Use this for initialization
	void Start () {

        menu.SetActive(false);
        isIngameMenu = menu.GetComponent<MenuScript>().isIngameMenu;

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
                
                //Set the default selected menu option -> hay que usar tags, si no, no los pilla
                if (isIngameMenu)
                    EventSystem.current.SetSelectedGameObject(resume);
                else
                    EventSystem.current.SetSelectedGameObject(newGame);

                //Game paused
                Time.timeScale = 0f;
            }
        }   
	}
}
