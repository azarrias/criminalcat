using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    [SerializeField]
    private int maxHP;
    private int curHP;
    [SerializeField]
    private RectTransform HPBar;

    [SerializeField]
    private Sprite[] hornImages;
    [SerializeField]
    private Image beerHorn;
    private int hornIndx;

    [SerializeField]
    private Image[] stmImages;
    private int stmIndx;

	// Use this for initialization
	void Start () {
        if (HPBar == null)
            Debug.Log("Error - HP bar not set");

        if (hornImages.Length == 0)
            Debug.Log("Error - horn images array not set");

        if (stmImages.Length == 0)
            Debug.Log("Error - stamina images array not set");

        maxHP = 100;
        curHP = 100;    
        hornIndx = hornImages.Length - 1;
        beerHorn.sprite = hornImages[hornIndx];
        stmIndx = stmImages.Length - 1;
    }
	
	// Update is called once per frame
	void Update () {

        //Increase and decrease HP
        if(Input.GetKeyDown(KeyCode.F2))
        {
            curHP += 10;
            if (curHP >= maxHP)
                curHP = maxHP;

            SetHEalth(maxHP, curHP);
        }
		
        if(Input.GetKeyDown(KeyCode.F1))
        {
            curHP -= 10;
            if (curHP <= 0)
                curHP = 0;

            SetHEalth(maxHP, curHP);
        }

        //Increase and decrease beer
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (hornIndx < hornImages.Length - 1)
                hornIndx++;

            SetBeer();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (hornIndx > 0)
                hornIndx--;

            SetBeer();
        }

        //Increase and decrease stamina
        if(Input.GetKeyDown(KeyCode.F6))
        {
            if(stmIndx < stmImages.Length - 1)
            {
                stmIndx++;
                AddStamina();
            }

        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            if(stmIndx > -1)
            {
                ReduceStamina();
                stmIndx--;
            }
                    
        }

    }

    public void SetHEalth(int hp_max, int hp_curr)
    {
        float value =(float) hp_curr / hp_max;
        HPBar.localScale = new Vector3(value, 1, 1);
    }

    public void SetBeer()
    {
        beerHorn.sprite = hornImages[hornIndx];    
    }

    public void ReduceStamina()
    {
        stmImages[stmIndx].color = new Vector4(0, 0, 0, 0);
    }

    public void AddStamina()
    {
       stmImages[stmIndx].color = new Vector4(1, 1, 1, 1);
    }


    public int HP_Curr {
        get { return curHP; }
        set { curHP = value; }
    }
}
