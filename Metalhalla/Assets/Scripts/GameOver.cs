using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    public string nextScene;
    public Button mainMenu;

	// Use this for initialization
	void Start () {

        mainMenu = mainMenu.GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MainMenuPressed()
    {
        SceneManager.LoadScene(nextScene);
    }

    


}
