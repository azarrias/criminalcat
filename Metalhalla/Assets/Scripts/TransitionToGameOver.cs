using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionToGameOver : MonoBehaviour {

    public GameObject gameOverUI;
    public GameObject ingameMenu;

    // Use this for initialization
    void Start () {
        gameOverUI.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.K) && !ingameMenu.activeSelf)
        {
            gameOverUI.SetActive(true);
        }
	}
}
