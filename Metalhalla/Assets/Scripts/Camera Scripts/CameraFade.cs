using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFade : MonoBehaviour {

    public GameObject fadeTexture;
    public float fadeTime = 2.0f;

    private bool isFading = false;
    private float fadeTimeCurrent = 0.0f;
    private Color textureColor;
    private SpriteRenderer textureRenderer;


	// Use this for initialization
	void Start () {
        textureRenderer = fadeTexture.GetComponent<SpriteRenderer>();
        textureColor = textureRenderer.color;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R) == true && isFading == false)
        {
            isFading = true;
            fadeTimeCurrent = 0.0f;
        }
        if ( isFading == true )
        {
            if (fadeTimeCurrent >= fadeTime)
            {
                textureColor.a = 0.0f;
                isFading = false;

            }
            else
            {
                textureColor.a = 1 - Mathf.Abs((fadeTime * 0.5f - fadeTimeCurrent) / (fadeTime * 0.5f));
                fadeTimeCurrent += Time.deltaTime;
            }

            textureRenderer.color = textureColor;
        }

	}
}
