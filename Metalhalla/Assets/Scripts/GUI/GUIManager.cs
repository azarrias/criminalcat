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

    [SerializeField]
    private Image[] magicImages;
    private int mgcIndex;

    private PlayerStatus playerStatus;

    void Start() {
        if (HPBar == null)
            Debug.Log("Error - HP bar not set");

        if (hornImages.Length == 0)
            Debug.Log("Error - horn images array not set");

        if (stmImages.Length == 0)
            Debug.Log("Error - stamina images array not set");

        if (magicImages.Length == 0)
            Debug.Log("Error - magic images array not set");

        playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        if (playerStatus == null)
            Debug.Log("GUI could not retrieve PlayerHealth component from player");

        maxHP = playerStatus.healthMaximum;
        hornIndx = playerStatus.beerAtStart;
        stmIndx = playerStatus.staminaAtStart;
        ResetMagic((int)playerStatus.magicAtStart);

        UpdateStaminaMeter(); 
    }

    void Update() {

        SetHealth(playerStatus.GetCurrentHealthRatio());
        SetStamina(playerStatus.GetCurrentStamina());
        SetBeer(playerStatus.GetCurrentBeer());
        SetMagic( playerStatus.GetCurrentMagic());

    }

    void SetHealth(float healthRatio)
    {
        if (healthRatio < 0.0f)
            healthRatio = 0.0f;

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

    void SetMagic( int newMagic)
    {
        if (mgcIndex == newMagic)
            return;

        Debug.Log("new magic: " + newMagic); 
        magicImages[mgcIndex].color = new Vector4(0, 0, 0, 0);
        magicImages[newMagic].color = new Vector4(1, 1, 1, 1);
        mgcIndex = newMagic;
    }

    void ResetMagic (int initialMagic)
    {
        for (int i = 0; i < magicImages.Length; i++)
            magicImages[i].color = new Vector4(0, 0, 0, 0);
        magicImages[initialMagic].color = new Vector4(1, 1, 1, 1);
    }

}
