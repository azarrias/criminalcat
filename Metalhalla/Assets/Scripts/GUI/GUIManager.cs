using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    private int maxHP;

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

    private PlayerHealth playerHealth;

    void Start() {
        if (HPBar == null)
            Debug.Log("Error - HP bar not set");

        if (hornImages.Length == 0)
            Debug.Log("Error - horn images array not set");

        if (stmImages.Length == 0)
            Debug.Log("Error - stamina images array not set");

        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerHealth == null)
            Debug.Log("GUI could not retrieve PlayerHealth component from player");

        maxHP = playerHealth.healthMaximum;
        hornIndx = playerHealth.beerAtStart;
        stmIndx = playerHealth.staminaAtStart;

        UpdateStaminaMeter(); 
    }

    void Update() {

        SetHealth(playerHealth.GetCurrentHealthRatio());
        SetStamina(playerHealth.GetCurrentStamina());
        SetBeer(playerHealth.GetCurrentBeer());

    }

    void SetHealth(float healthRatio)
    {
        HPBar.localScale = new Vector3(healthRatio, 1, 1);
    }

    void UpdateStaminaMeter()
    {
        int i;
        for (i = 0; i < stmIndx; i++)
            stmImages[i].color = new Vector4(1, 1, 1, 1);
        for (i = stmIndx; i < 10; i++)
            stmImages[i].color = new Vector4(0, 0, 0, 0);
    }

    void SetStamina(int remainingStamina)
    {
        if (stmIndx == remainingStamina)
            return;

        stmIndx = remainingStamina;
        UpdateStaminaMeter(); 
    }

    void SetBeer( int remainingBeer )
    {
        beerHorn.sprite = hornImages[remainingBeer];
    }

}
