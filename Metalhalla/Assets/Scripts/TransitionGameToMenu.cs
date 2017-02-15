using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionGameToMenu : MonoBehaviour {

    public GameObject ingameMenu;
   
	// Use this for initialization
	void Start () {
        ingameMenu.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ingameMenu.activeSelf)
                ingameMenu.SetActive(false);
            else
                ingameMenu.SetActive(true);
        }
	}
}
