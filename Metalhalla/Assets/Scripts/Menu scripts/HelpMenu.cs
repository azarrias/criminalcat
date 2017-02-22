using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HelpMenu : MonoBehaviour
{

    private List<GameObject> selectionList;
    private int selectedMenuOption;

    // Use this for initialization
    void Start()
    {
        selectionList = new List<GameObject>();
        selectedMenuOption = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (selectionList.Count == 0)
        {
            GameObject exitButton = GameObject.FindGameObjectWithTag("HelpMenuExit");      
            selectionList.Add(exitButton);           
        }

        if (selectionList.Count != 0)
            EventSystem.current.SetSelectedGameObject(selectionList[selectedMenuOption]);
    }

    public void SetSelectedMenuOption(int option)
    {
        selectedMenuOption = option;
    }



}
