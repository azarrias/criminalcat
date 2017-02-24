using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionToEndGame : MonoBehaviour {

    public GameObject endGameUI;
    public GameObject ingameMenu;

	// Use this for initialization
	void Start () {
        endGameUI.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F) && !ingameMenu.activeSelf)
        {
            endGameUI.SetActive(true);
        }
	}
}
