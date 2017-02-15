using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTransition : MonoBehaviour {

    public GameObject gameOverUI;

	// Use this for initialization
	void Start () {
        gameOverUI.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.K))
        {
            gameOverUI.SetActive(true);
        }
	}
}
