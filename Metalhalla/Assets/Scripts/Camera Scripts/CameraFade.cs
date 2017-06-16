using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFade : MonoBehaviour {

    public Texture2D fadeTexture;
    public Texture2D deadImage;
    public float fadeToBlackTime = 3.0f;
    public float showImageStartTime = 1.0f;
    public float stayInBlackTime = 2.0f;
    public float fadeToSceneTime = 1.0f;

    float fadeTime;

    [HideInInspector]
    private bool isFading = false;
    private bool showMessage = false; 
    private float fadeTimeCurrent = 0.0f;

    private Color guiColor;

	void Start () {
        guiColor = Color.white;
        guiColor.a = 0.0f;
        fadeTime = fadeToBlackTime + stayInBlackTime+ fadeToSceneTime;
    }
	
	void Update () {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.R) == true && isFading == false)
            ActivateFade();

        if ( isFading == true )
        {
            if (fadeTimeCurrent >= fadeTime)
            {
                guiColor.a = 0.0f;
                isFading = false;
            }
            else
            {
                if ( fadeTimeCurrent <= fadeToBlackTime)
                {
                    if (fadeTimeCurrent >= showImageStartTime)
                    {
                        showMessage = true; 
                    }
                    guiColor.a = 1 - (fadeToBlackTime - fadeTimeCurrent) / fadeToBlackTime;
                }
                else if (fadeTimeCurrent >= fadeToBlackTime + stayInBlackTime)
                {
                    showMessage = false;
                    guiColor.a = (fadeToSceneTime - fadeTimeCurrent + fadeToBlackTime + stayInBlackTime) / fadeToSceneTime;
                }
                fadeTimeCurrent += Time.deltaTime;
            }
        }

	}

    private void OnGUI()
    {
        if (isFading == true)
        {
            GUI.color = guiColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
            if (showMessage == true)
                GUI.DrawTexture(new Rect(0, Screen.height / 4, Screen.width, Screen.height / 4), deadImage);
        }
        
    }

    public void ActivateFade()
    {
        isFading = true;
        fadeTimeCurrent = 0.0f;
    }
}
