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

    /*[SerializeField]
    private Image[] magicImages;
    private int mgcIndex;
    */
    [Header("GUI Feedback for player input")]
    [Tooltip("Duration of the feedback shown to the player")]
    public float feedbackTime = 0.2f;
    [Tooltip("Scale increase ratio for feedback")]
    public float feedbackSizeRatio = 1.2f;
    [Tooltip("Color for the actions that are not available (i.e. not having stamina would make the magic images gray")]
    public Color disabledImageColorTint = new Color(0.3f, 0.3f, 0.3f, 1);
    [SerializeField]
    private Image YButtonImage;
    [SerializeField]
    private Image tornadoImage;
    [SerializeField]
    private Image earthquakeImage;

    private bool  yButtonFeedback;
    private float yButtonFeedbackTime;
    private Vector3 yButtonOriginalScale;

    private bool  tornadoFeedback;
    private float tornadoFeedbackTime;
    private Vector3 tornadoOriginalScale;

    private bool  earthquakeFeedback;
    private float earthquakeFeedbackTime;
    private Vector3 earthquakeOriginalScale;
    
    private PlayerStatus playerStatus;

    void Start() {
        if (HPBar == null)
            Debug.Log("Error - HP bar not set");

        if (hornImages.Length == 0)
            Debug.Log("Error - horn images array not set");

        if (stmImages.Length == 0)
            Debug.Log("Error - stamina images array not set");

       /* if (magicImages.Length == 0)
            Debug.Log("Error - magic images array not set");
            */

        playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        if (playerStatus == null)
            Debug.Log("GUI could not retrieve PlayerStatus component from player");

        maxHP = playerStatus.healthMaximum;
     //   hornIndx = playerStatus.beerAtStart;
        stmIndx = playerStatus.staminaAtStart;
     //   ResetMagic((int)playerStatus.magicAtStart);

        UpdateStaminaMeter();

        yButtonFeedback = false;
        tornadoFeedback = false;
        earthquakeFeedback = false;

        yButtonOriginalScale = YButtonImage.transform.localScale;
        tornadoOriginalScale = tornadoImage.transform.localScale;
        earthquakeOriginalScale = earthquakeImage.transform.localScale;
    }

    void Update() {

        SetHealth(playerStatus.GetCurrentHealthRatio());
        SetStamina(playerStatus.GetCurrentStamina());
        SetBeer(playerStatus.GetCurrentBeer());
        //   SetMagic( playerStatus.GetCurrentMagic());

        UpdatePressedButtonFeedback();
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

        if (stmIndx == 0)
        {
            tornadoImage.color = disabledImageColorTint;
            earthquakeImage.color = disabledImageColorTint;
        }
        else
        {
            tornadoImage.color = Color.white;
            earthquakeImage.color = Color.white;
        }
    }

    void SetBeer( int remainingBeer )
    {
        if (hornIndx == remainingBeer)
            return;

        hornIndx = remainingBeer; 
        beerHorn.sprite = hornImages[hornIndx];

        if (hornIndx == 0)
            YButtonImage.color = disabledImageColorTint;
        else
            YButtonImage.color = Color.white;
    }

/*    void SetMagic( int newMagic)
    {
        if (mgcIndex == newMagic)
            return;

        Vector3 tmp; 

        magicImages[mgcIndex].color = new Vector4(0.2f, 0.2f, 0.2f, 0.1f);
        tmp = magicImages[mgcIndex].transform.localPosition;
        tmp.z = 0.1f;
        magicImages[mgcIndex].transform.localPosition = tmp;

        magicImages[newMagic].color = new Vector4(0f, 0.7f, 1, 1);
        tmp = magicImages[newMagic].transform.localPosition;
        tmp.z = 0f;
        magicImages[newMagic].transform.localPosition = tmp;

        mgcIndex = newMagic;
    }

    void ResetMagic (int initialMagic)
    {
        Vector3 tmp; 
        for (int i = 0; i < magicImages.Length; i++)
        {
            magicImages[i].color = new Vector4(0.2f, 0.2f, 0.2f, 0.1f);
            tmp = magicImages[i].transform.localPosition;
            tmp.z = 0.1f;
            magicImages[i].transform.localPosition = tmp; 
        }
        magicImages[initialMagic].color = new Vector4(0, 0.7f, 1, 1);
        tmp = magicImages[initialMagic].transform.localPosition;
        tmp.z = 0.0f;
        magicImages[initialMagic].transform.localPosition = tmp;
    }
*/
    
    public void StartFeedback( string element )
    {
        if (element.Equals("YButton"))
        {
            yButtonFeedback = true;
            yButtonFeedbackTime = 0.0f;
            YButtonImage.transform.localScale = yButtonOriginalScale * feedbackSizeRatio;
        }
        else if (element.Equals("Tornado"))
        {
            tornadoFeedback = true;
            tornadoFeedbackTime = 0.0f;
            tornadoImage.transform.localScale = tornadoOriginalScale * feedbackSizeRatio;
        }
        else if (element.Equals("Earthquake"))
        {
            earthquakeFeedback = true;
            earthquakeFeedbackTime = 0.0f;
            earthquakeImage.transform.localScale = earthquakeOriginalScale * feedbackSizeRatio;
        }
    }

    private void UpdatePressedButtonFeedback()
    {
        if (yButtonFeedback )
        {
            yButtonFeedbackTime += Time.deltaTime;

            Color tmp = YButtonImage.color;
            tmp.a = yButtonFeedbackTime / feedbackTime;
            YButtonImage.color = tmp;
            
            if (yButtonFeedbackTime >= feedbackTime)
            {
                yButtonFeedback = false;
                YButtonImage.transform.localScale = yButtonOriginalScale;
            }
        }
        if (tornadoFeedback )
        {
            tornadoFeedbackTime += Time.deltaTime;

            Color tmp = tornadoImage.color;
            tmp.a = tornadoFeedbackTime / feedbackTime;
            tornadoImage.color = tmp;

            if (tornadoFeedbackTime >= feedbackTime)
            {
                tornadoFeedback = false;
                tornadoImage.transform.localScale = tornadoOriginalScale;
            }
        }
        if (earthquakeFeedback)
        {
            earthquakeFeedbackTime += Time.deltaTime;

            Color tmp = earthquakeImage.color;
            tmp.a = earthquakeFeedbackTime / feedbackTime;
            earthquakeImage.color = tmp;

            if (earthquakeFeedbackTime >= feedbackTime)
            {
                earthquakeFeedback = false;
                earthquakeImage.transform.localScale = earthquakeOriginalScale;
            }
        }
    }

}
