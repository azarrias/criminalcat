using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGUIManager : MonoBehaviour
{

    private int maxHP;

    [SerializeField]
    private RectTransform HPBar;
    [SerializeField]
    private RectTransform HPBackground;

    [Tooltip("HP animation time")]
    public float hpAnimationTime = 0.1f;
    private float hpBarAnimationTime;
    private float hpBackgroundAnimationTime;

    // GUI HP animation when winning or losing HP
    private float healthRatioCurrentPlayer = 1.0f;
    private float healthRatioCurrentGUI = 1.0f;
    private float healthRatioTarget = 1.0f;
    private float healthRatioOrigin = 1.0f;

    private BossStats bossStats;

    private void Awake()
    {
        bossStats = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossStats>();
        if (bossStats == null)
            Debug.Log("BossGUIManager could not retrieve BossStats component from Boss gameObject");
        //SetHealthRatioPositions();
        healthRatioTarget = 0;
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

        HPBar.localScale = new Vector3(healthRatioCurrentPlayer, HPBar.localScale.y, 1);
    }

    private void UpdateHPBackground()
    {
        if (healthRatioCurrentGUI == healthRatioTarget)
            return;

        hpBackgroundAnimationTime -= Time.deltaTime;
        float lambda = hpBackgroundAnimationTime > 0.0f ? hpBackgroundAnimationTime / hpAnimationTime : 0.0f;
        healthRatioCurrentGUI = healthRatioOrigin * lambda + healthRatioTarget * (1 - lambda);

        HPBackground.localScale = new Vector3(healthRatioCurrentGUI, HPBackground.localScale.y, 1);
    }

    private void SetHealthRatioPositions()
    {
        float ratio = bossStats.GetCurrentHealthRatio();
        healthRatioOrigin = ratio;
        healthRatioTarget = ratio;
        healthRatioCurrentPlayer = ratio;
        healthRatioCurrentGUI = ratio;
    }
}
