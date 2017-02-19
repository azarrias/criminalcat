using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuitMenu : MonoBehaviour {

    private List<GameObject> selectionList;
    private int selectedMenuOption;

	// Use this for initialization
	void Start () {

        selectionList = new List<GameObject>();
        selectedMenuOption = 1;
	}
	
	// Update is called once per frame
	void Update () {

        if(selectionList.Count == 0)
        {
            GameObject YesButton = GameObject.FindGameObjectWithTag("QuitMenuYes");
            GameObject NoButton = GameObject.FindGameObjectWithTag("QuitMenuNo");
            selectionList.Add(YesButton);
            selectionList.Add(NoButton);
        }

        if (Input.GetAxis("HorizontalMenu") < 0f || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedMenuOption == 1)
                selectedMenuOption = 0;        
        }

        if(Input.GetAxis("HorizontalMenu") > 0f || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedMenuOption == 0)
                selectedMenuOption = 1;
        }

        if (selectionList.Count != 0)
            EventSystem.current.SetSelectedGameObject(selectionList[selectedMenuOption]);
    }

    public void SetSelectedMenuOption(int option)
    {
        selectedMenuOption = option;
    }
  


}
