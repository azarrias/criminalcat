using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransitionToEndGame : MonoBehaviour {

    public GameObject endGameUI;
    public GameObject ingameMenu;

	// Use this for initialization
	void Start () {
        endGameUI.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

        //Code for testing pursposes
        /*
		if(Input.GetKeyDown(KeyCode.F) && !ingameMenu.activeSelf)
        {
            endGameUI.SetActive(true);
            GameObject goToMenu = GameObject.FindGameObjectWithTag("EndGameGoToMenu");
            EventSystem.current.SetSelectedGameObject(goToMenu);
        }
        */
	}

    public void GoToEndGame()
    {
        endGameUI.SetActive(true);
        GameObject goToMenu = GameObject.FindGameObjectWithTag("EndGameGoToMenu");
        EventSystem.current.SetSelectedGameObject(goToMenu);
    }
}
