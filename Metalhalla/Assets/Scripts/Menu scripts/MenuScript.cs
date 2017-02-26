using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour {

    //We share the same script between starting menu and ingame menu
    public bool isIngameMenu;

    public GameObject menu;
    public GameObject quitMenu;
    public GameObject optionsMenu;
    public GameObject helpMenu;
    public GameObject creditsMenu;
    public Button resumeButton;
    public Button newGameButton;
    public Button optionsButton;
    public Button helpButton;
    public Button creditsButton;
    public Button exitGamebutton;
    public string nextScene;
    private Button lastOptionSelected;   
    private SaveMenuState saveMenuStateScript;
    public Sprite restartSpriteIdle;
    public Sprite restartSpriteHover;
    private SpriteState spriteState;
    
	// Use this for initialization
	void Start () {
        
        quitMenu.SetActive(false);
        optionsMenu.SetActive(false);
        helpMenu.SetActive(false);
        creditsMenu.SetActive(false);
        resumeButton = resumeButton.GetComponent<Button>();
        newGameButton = newGameButton.GetComponent<Button>();
        optionsButton = optionsButton.GetComponent<Button>();
        helpButton = helpButton.GetComponent<Button>();
        creditsButton = creditsButton.GetComponent<Button>();
        exitGamebutton = exitGamebutton.GetComponent<Button>();
        saveMenuStateScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SaveMenuState>();


        if (isIngameMenu)
        {
            //Enable or disable Resume button 
            resumeButton.gameObject.SetActive(true);
            //change button image NEW GAME -> RESTART            
            newGameButton.GetComponent<Image>().sprite = restartSpriteIdle;
            spriteState = new SpriteState();
            spriteState.highlightedSprite = restartSpriteHover;
            newGameButton.spriteState = spriteState;

        }
        else
        {
            resumeButton.gameObject.SetActive(false);
        }
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

	//Activate quit menu
	public void ExitGamePressed()
    {
        lastOptionSelected = exitGamebutton;
        quitMenu.SetActive(true);
        quitMenu.GetComponent<QuitMenu>().SetSelectedMenuOption(1);
        resumeButton.enabled = false;
        newGameButton.enabled = false;
        optionsButton.enabled = false;
        helpButton.enabled = false;
        creditsButton.enabled = false;
        exitGamebutton.enabled = false;
        
    }

    public void YesPressed()
    {
        Application.Quit();
    }

    //Deactivate quit menu
    public void NoPressed()
    {
        quitMenu.SetActive(false);
        resumeButton.enabled = true;
        newGameButton.enabled = true;
        optionsButton.enabled = true;
        helpButton.enabled = true;
        creditsButton.enabled = true;
        exitGamebutton.enabled = true;

        //Set selected button
        EventSystem.current.SetSelectedGameObject(lastOptionSelected.gameObject);

    }

    //Activate options menu
    public void OptionsPressed()
    {
        lastOptionSelected = optionsButton;
        optionsMenu.SetActive(true);

        //set slider values
        Slider musicSlider = GameObject.FindGameObjectWithTag("OptionsMenuMusicVolume").GetComponent<Slider>();
        musicSlider.value = saveMenuStateScript.GetMusicSliderValue();
        Slider fxSlider = GameObject.FindGameObjectWithTag("OptionsMenuSoundEffectsVolume").GetComponent<Slider>();
        fxSlider.value = saveMenuStateScript.GetFxSoundSliderValue();
             
        optionsMenu.GetComponent<OptionsMenu>().SetSelectedMenuOption(0);
        resumeButton.enabled = false;
        newGameButton.enabled = false;
        optionsButton.enabled = false;
        helpButton.enabled = false;
        creditsButton.enabled = false;
        exitGamebutton.enabled = false;
               
    }

    //Deactivate options menu
    public void ExitOptionsPressed()
    {
        //Save sliders value
        Slider musicSlider = GameObject.FindGameObjectWithTag("OptionsMenuMusicVolume").GetComponent<Slider>();
        saveMenuStateScript.SetMusicSliderValue(musicSlider.value);
        Slider fxSlider = GameObject.FindGameObjectWithTag("OptionsMenuSoundEffectsVolume").GetComponent<Slider>();
        saveMenuStateScript.SetFxSoundSliderValue(fxSlider.value);

        optionsMenu.SetActive(false);
        resumeButton.enabled = true;
        newGameButton.enabled = true;
        optionsButton.enabled = true;
        helpButton.enabled = true;
        creditsButton.enabled = true;
        exitGamebutton.enabled = true;

        //Set selected button
        EventSystem.current.SetSelectedGameObject(lastOptionSelected.gameObject);

    }

    //Activate Help menu
    public void HelpPressed()
    {
        lastOptionSelected = helpButton;
        helpMenu.SetActive(true);
        helpMenu.GetComponent<HelpMenu>().SetSelectedMenuOption(0);
        resumeButton.enabled = false;
        newGameButton.enabled = false;
        optionsButton.enabled = false;
        helpButton.enabled = false;
        creditsButton.enabled = false;
        exitGamebutton.enabled = false;
         
    }

    //Deactivate help menu
    public void ExitHelpPressed()
    {
        helpMenu.SetActive(false);
        resumeButton.enabled = true;
        newGameButton.enabled = true;
        optionsButton.enabled = true;
        helpButton.enabled = true;
        creditsButton.enabled = true;
        exitGamebutton.enabled = true;

        //Set selected button        
        EventSystem.current.SetSelectedGameObject(lastOptionSelected.gameObject);

    }

    //Activate Credits menu
    public void CreditsPressed()
    {
        lastOptionSelected = creditsButton;
        creditsMenu.SetActive(true);
        creditsMenu.GetComponent<CreditsMenu>().SetSelectedMenuOption(0);
        resumeButton.enabled = false;
        newGameButton.enabled = false;
        optionsButton.enabled = false;
        helpButton.enabled = false;
        creditsButton.enabled = false;
        exitGamebutton.enabled = false;
        
    }

    //Deactivate Credits menu
    public void ExitCreditsPressed()
    {
        creditsMenu.SetActive(false);
        resumeButton.enabled = true;
        newGameButton.enabled = true;
        optionsButton.enabled = true;
        helpButton.enabled = true;
        creditsButton.enabled = true;
        exitGamebutton.enabled = true;

        //Set selected button
        EventSystem.current.SetSelectedGameObject(lastOptionSelected.gameObject);

    }
}
