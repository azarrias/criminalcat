using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFade : MonoBehaviour {

    public Texture2D fadeTexture;
    public float fadeTime = 6.0f;

    [HideInInspector]
    private bool isFading = false;
    private float fadeTimeCurrent = 0.0f;

    private Color guiColor;

	void Start () {
        guiColor.a = 0.0f;
    }
	
	void Update () {
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
                guiColor.a = 1 - Mathf.Abs((fadeTime * 0.5f - fadeTimeCurrent) / (fadeTime * 0.5f));
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
        }
    }

    public void ActivateFade()
    {
        isFading = true;
        fadeTimeCurrent = 0.0f;
    }
}
