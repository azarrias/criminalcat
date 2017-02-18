using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour {

    //We share the same script between starting menu and ingame menu
    public bool isIngameMenu = true;

    public GameObject menu;
    public GameObject quitMenu;
    public GameObject optionsMenu;
    public Button resume;
    public Button newGame;
    public Button options;
    public Button exitGame;
    public Button exitOptions;
    public string nextScene;

	// Use this for initialization
	void Start () {

        quitMenu.SetActive(false);
        optionsMenu.SetActive(false);
        resume = resume.GetComponent<Button>();
        newGame = newGame.GetComponent<Button>();
        options = options.GetComponent<Button>();
        exitGame = exitGame.GetComponent<Button>();
        exitOptions = exitOptions.GetComponent<Button>();

        //Enable or disable Resume button 
        if (isIngameMenu)
            resume.gameObject.SetActive(true);
        else
            resume.gameObject.SetActive(false);

    }

    public void ResumePressed()
    {
        //Game runs at regular speed
        Time.timeScale = 1f;
        menu.SetActive(false);
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
        quitMenu.SetActive(true);
        newGame.enabled = false;
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
        quitMenu.SetActive(false);
        newGame.enabled = true;
        options.enabled = true;
        exitGame.enabled = true;
        resume.enabled = true;
        
        //Set selected button
        if (isIngameMenu)
            EventSystem.current.SetSelectedGameObject(resume.gameObject);
        else
            EventSystem.current.SetSelectedGameObject(newGame.gameObject);
    }

    //Options menu
    public void OptionsPressed()
    {
        optionsMenu.SetActive(true);
        newGame.enabled = false;
        options.enabled = false;
        exitGame.enabled = false;
        resume.enabled = false;
    }


    public void ExitOptionsPressed()
    {
        optionsMenu.SetActive(false);
        newGame.enabled = true;
        options.enabled = true;
        exitGame.enabled = true;
        resume.enabled = true;

        //Set selected button
        if (isIngameMenu)
            EventSystem.current.SetSelectedGameObject(resume.gameObject);
        else
            EventSystem.current.SetSelectedGameObject(newGame.gameObject);

    }
}
