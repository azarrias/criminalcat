using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour {

    private List<GameObject> selectionList;
    private int selectedMenuOption = 0;
    private bool changeOption = true;

	// Use this for initialization
	void Start () {

        selectionList = new List<GameObject>();
              
    }
	
	// Update is called once per frame
	void Update () {

        if(selectionList.Count == 0)
        {
            GameObject musicVolume = GameObject.FindGameObjectWithTag("OptionsMenuMusicVolume");
            GameObject exitOptions = GameObject.FindGameObjectWithTag("OptionsMenuExit");
            GameObject soundEffectsVolume = GameObject.FindGameObjectWithTag("OptionsMenuSoundEffectsVolume");
            
            //Add buttons in the same order of appearance in the GUI
            selectionList.Add(musicVolume);
            selectionList.Add(soundEffectsVolume);
            selectionList.Add(exitOptions);
            
        }
       
        if (changeOption && (Input.GetAxis("VerticalMenu") > 0 || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            if (selectedMenuOption != 0)
                selectedMenuOption--;

            changeOption = false;
                           
        }

        if (changeOption && (Input.GetAxis("VerticalMenu") < 0 || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            if (selectedMenuOption != selectionList.Count - 1)
                selectedMenuOption++;

            changeOption = false;
       
        }

        if (selectionList.Count != 0)
        {
            EventSystem.current.SetSelectedGameObject(selectionList[selectedMenuOption]);
        }

        //This code prevents the menu from wrapping around when player uses the controll pad
        if(!changeOption && (Input.GetAxis("VerticalMenu") == 0))
        {           
            changeOption = true;
        }


    }

    public void SetSelectedMenuOption(int option)
    {
        selectedMenuOption = option;
    }

    
}
