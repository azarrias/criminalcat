using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour {

    private List<GameObject> selectionList;
    public int selectedMenuOption = 0;
    

	// Use this for initialization
	void Start () {

        selectionList = new List<GameObject>();
        selectedMenuOption = 0;
        
    }
	
	// Update is called once per frame
	void Update () {

        if(selectionList.Count == 0)
        {
            GameObject musicVolume = GameObject.FindGameObjectWithTag("OptionsMenuMusicVolume");
            GameObject exitOptions = GameObject.FindGameObjectWithTag("OptionsMenuExit");
            selectionList.Add(musicVolume);
            selectionList.Add(exitOptions);
        }
       
        if (Input.GetAxis("VerticalMenu") > 0 || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedMenuOption != 0)
                selectedMenuOption--;                     
        }

        if (Input.GetAxis("VerticalMenu") < 0 || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedMenuOption != selectionList.Count - 1)
                selectedMenuOption++;                        
        }
       
        if (selectionList.Count != 0)
            EventSystem.current.SetSelectedGameObject(selectionList[selectedMenuOption]);

    }

    public void SetSelectedMenuOption(int option)
    {
        selectedMenuOption = option;
    }
}
