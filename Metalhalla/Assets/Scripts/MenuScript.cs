using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    //We share the same script between starting menu and ingame menu
    public bool ingameMenu = true;

    public GameObject menu;
    public Canvas quitMenu;
    public Canvas optionsMenu;
    public Button resume;
    public Button play;
    public Button options;
    public Button exitGame;
    public Button exitOptions;
    public string nextScene;

	// Use this for initialization
	void Start () {

        quitMenu = quitMenu.GetComponent<Canvas>();
        quitMenu.enabled = false;
        optionsMenu = optionsMenu.GetComponent<Canvas>();
        optionsMenu.enabled = false;
        resume = resume.GetComponent<Button>();
        play = play.GetComponent<Button>();
        options = options.GetComponent<Button>();
        exitGame = exitGame.GetComponent<Button>();
        exitOptions = exitOptions.GetComponent<Button>();

        //Enable or disable Resume button 
        if (ingameMenu)
            resume.gameObject.SetActive(true);
        else
            resume.gameObject.SetActive(false);

	}

    public void ResumePressed()
    {
        menu.SetActive(false);
        
        //Game runs at regular speed
        Time.timeScale = 1f;
    }

    public void NewGamePressed()
    {
        //Game runs at regular speed
        Time.timeScale = 1f;

        SceneManager.LoadScene(nextScene);

    }

	//Quit menu
	public void ExitGamePressed()
    {
        quitMenu.enabled = true;
        play.enabled = false;
        options.enabled = false;
        exitGame.enabled = false;
        resume.enabled = false;
    }

    public void YesPressed()
    {
        Application.Quit();
    }

    public void NoPressed()
    {
        quitMenu.enabled = false;
        play.enabled = true;
        options.enabled = true;
        exitGame.enabled = true;
        resume.enabled = true;

    }

    //Options menu
    public void OptionsPressed()
    {
        optionsMenu.enabled = true;
        play.enabled = false;
        options.enabled = false;
        exitGame.enabled = false;
        resume.enabled = false;
    }


    public void ExitOptionsPressed()
    {
        optionsMenu.enabled = false;
        play.enabled = true;
        options.enabled = true;
        exitGame.enabled = true;
        resume.enabled = true;

    }
}
