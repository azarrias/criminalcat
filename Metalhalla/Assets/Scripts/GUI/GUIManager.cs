using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

    private int maxHP;

    [SerializeField]
    private RectTransform HPBar;
    [SerializeField]
    private RectTransform HPbackground;

    [SerializeField]
    private Sprite[] hornImages;
    [SerializeField]
    private Image beerHorn;
    private int hornIndx;

    [SerializeField]
    private Image[] stmImages;
    private int stmIndx;

    [Header("GUI Feedback for player input")]
    [Tooltip("HP animation time")]
    public float hpAnimationTime = 0.1f;
    private float hpBarAnimationTime;
    private float hpBackgroundAnimationTime;

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

    // GUI HP animation when winning or losing HP
    private float healthRatioCurrentPlayer = 1.0f;
    private float healthRatioCurrentGUI = 1.0f;
    private float healthRatioTarget = 1.0f;
    private float healthRatioOrigin = 1.0f; 

    private PlayerStatus playerStatus;

    void Start() {
        if (HPBar == null)
            Debug.Log("Error - HP bar not set");

        if (HPbackground == null)
            Debug.Log("Error - HP greyed bar not set");

        if (hornImages.Length == 0)
            Debug.Log("Error - horn images array not set");

        if (stmImages.Length == 0)
            Debug.Log("Error - stamina images array not set");

        playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        if (playerStatus == null)
            Debug.Log("GUI could not retrieve PlayerStatus component from player");

        maxHP = playerStatus.healthMaximum;
        stmIndx = playerStatus.staminaAtStart;

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

        UpdateHPbar();
        UpdateHPbackground(); 

        UpdatePressedButtonFeedback();
    }

    void SetHealth(float healthRatio)
    {
        if (healthRatioTarget == healthRatio)
            return;

        healthRatio = healthRatio < 0.0f ? 0.0f : healthRatio;

        if (healthRatio > healthRatioTarget) // health recovery
        {
            hpBackgroundAnimationTime = 2*Time.fixedDeltaTime;
            hpBarAnimationTime = hpAnimationTime;
        }
        else 
        {
            hpBackgroundAnimationTime = hpAnimationTime;
            hpBarAnimationTime = 2*Time.fixedDeltaTime;
        }
        healthRatioOrigin = healthRatioTarget;
        healthRatioTarget = healthRatio;
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

    private void UpdateHPbar()
    {
        if (healthRatioCurrentPlayer == healthRatioTarget)
            return;

        hpBarAnimationTime -= Time.deltaTime;

        float lambda = hpBarAnimationTime > 0.0f ? hpBarAnimationTime / hpAnimationTime : 0.0f;
        healthRatioCurrentPlayer = healthRatioOrigin * lambda + healthRatioTarget * (1 - lambda);

        HPBar.localScale = new Vector3(healthRatioCurrentPlayer, 1, 1);
    }

    private void UpdateHPbackground()
    {
        if (healthRatioCurrentGUI == healthRatioTarget)
            return;

        hpBackgroundAnimationTime -= Time.deltaTime;
        float lambda = hpBackgroundAnimationTime > 0.0f ? hpBackgroundAnimationTime / hpAnimationTime : 0.0f;
        healthRatioCurrentGUI = healthRatioOrigin * lambda + healthRatioTarget * (1 - lambda); 

        HPbackground.localScale = new Vector3(healthRatioCurrentGUI, 1, 1);
    }

}
