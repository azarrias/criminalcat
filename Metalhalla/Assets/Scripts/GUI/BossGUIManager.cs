using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossGUIManager : MonoBehaviour
{

    private int maxHP;

    [SerializeField]
    private RectTransform HPBar;
    [SerializeField]
    private RectTransform HPBackground;
    [SerializeField]
    private RectTransform Background;
    [SerializeField]
    private RectTransform Label;

    [Tooltip("HP animation time")]
    public float hpAnimationTime = 0.1f;
    private float hpBarAnimationTime;
    private float hpBackgroundAnimationTime;

    [Header("Layout parameters")]
    public float horizontalMargin = 20.0f;
    public float bottomMargin = 100.0f;

    // GUI HP animation when winning or losing HP
    private float healthRatioCurrentPlayer = 1.0f;
    private float healthRatioCurrentGUI = 1.0f;
    private float healthRatioTarget = 1.0f;
    private float healthRatioOrigin = 1.0f;

    // parameters after initSetup
    private float _healthRatioMaxScale;
    private BossStats bossStats;

    private void Awake()
    {
        // get the values to display
        bossStats = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossStats>();
        if (bossStats == null)
            Debug.Log("BossGUIManager could not retrieve BossStats component from Boss gameObject");

        // set the correct layout
        InitBarLayout();

        healthRatioTarget = 0;  // for animation
    }

    private void Start()
    {
        if (HPBar == null)
            Debug.Log("Error - HP bar not set");

        if (HPBackground == null)
            Debug.Log("Error - HP greyed bar not set");

        maxHP = bossStats.maxHitPoints;
    }

    private void Update()
    {
        SetHealth(bossStats.GetCurrentHealthRatio());

        UpdateHPbar();
        UpdateHPBackground();
    }

    void SetHealth(float healthRatio)
    {
        if (healthRatioTarget == healthRatio)
            return;

        healthRatio = healthRatio < 0.0f ? 0.0f : healthRatio;

        if (healthRatio > healthRatioTarget) // health recovery
        {
            hpBackgroundAnimationTime = 2 * Time.fixedDeltaTime;
            hpBarAnimationTime = hpAnimationTime;
        }
        else
        {
            hpBackgroundAnimationTime = hpAnimationTime;
            hpBarAnimationTime = 2 * Time.fixedDeltaTime;
        }
        healthRatioOrigin = healthRatioTarget;
        healthRatioTarget = healthRatio;
    }

    private void UpdateHPbar()
    {
        if (healthRatioCurrentPlayer == healthRatioTarget)
            return;

        hpBarAnimationTime -= Time.deltaTime;

        float lambda = hpBarAnimationTime > 0.0f ? hpBarAnimationTime / hpAnimationTime : 0.0f;
        healthRatioCurrentPlayer = healthRatioOrigin * lambda + healthRatioTarget * (1 - lambda);

        HPBar.localScale = new Vector3(healthRatioCurrentPlayer * _healthRatioMaxScale, HPBar.localScale.y, 1);
    }

    private void UpdateHPBackground()
    {
        if (healthRatioCurrentGUI == healthRatioTarget)
            return;

        hpBackgroundAnimationTime -= Time.deltaTime;
        float lambda = hpBackgroundAnimationTime > 0.0f ? hpBackgroundAnimationTime / hpAnimationTime : 0.0f;
        healthRatioCurrentGUI = healthRatioOrigin * lambda + healthRatioTarget * (1 - lambda);

        HPBackground.localScale = new Vector3(healthRatioCurrentGUI * _healthRatioMaxScale, HPBackground.localScale.y, 1);
    }

    private void SetHealthRatioPositions()
    {
        float ratio = bossStats.GetCurrentHealthRatio();
        healthRatioOrigin = ratio;
        healthRatioTarget = ratio;
        healthRatioCurrentPlayer = ratio;
        healthRatioCurrentGUI = ratio;
    }


    private void InitBarLayout()
    {
        RectTransform wildboar = GameObject.Find("GUI").transform.FindChild("HUD").transform.FindChild("Wildboar").GetComponent<RectTransform>();
        Canvas canvas = GetComponentInChildren<Canvas>();
        CanvasScaler canvasScaler = GetComponentInChildren<CanvasScaler>();

        float leftLimit = wildboar.position.x + wildboar.rect.width * (1 - wildboar.pivot.x) + horizontalMargin;
        float rightLimit = canvas.pixelRect.width - horizontalMargin;

        float guiRatio = canvasScaler.referenceResolution.x / canvas.pixelRect.width;
        leftLimit *= guiRatio;
        rightLimit *= guiRatio;

        float bottomLimit = bottomMargin;
        Debug.Log("leftLimit: " + leftLimit + ", rightLimit: " + rightLimit);
        Debug.Log("screen width: " + Screen.width + " canvas: " + canvas.pixelRect.width + " canvasScaler: " + canvasScaler.scaleFactor);
        float desiredWidth = rightLimit - leftLimit;
        float newScaleX = desiredWidth / HPBar.rect.width;

        HPBar.localScale = new Vector3(newScaleX, HPBar.localScale.y, 1);
        HPBackground.localScale = new Vector3(newScaleX, HPBackground.localScale.y, 1);
        Background.localScale = new Vector3(newScaleX, Background.localScale.y, 1);

        HPBar.anchoredPosition = new Vector2(leftLimit, bottomLimit);
        HPBackground.anchoredPosition = new Vector2(leftLimit, bottomLimit);
        Background.anchoredPosition = new Vector2(leftLimit, bottomLimit);
        Label.anchoredPosition = new Vector2(leftLimit, bottomLimit + HPBar.rect.height *0.7f);

        _healthRatioMaxScale = newScaleX;
    }
}
