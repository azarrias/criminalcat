using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionTitleMenu : MonoBehaviour
{

    public float waitingTime = 3.0f;
    public string nextSceneName;
    public Image image;
    public float fadeSpeed = 1.5f;
    private bool allowFading = false;
    private float colorThreshold = 0.01f;

    // Use this for initialization
    void Start()
    {
        StartCoroutine("WaitForFade");
    }

    void Update()
    {
        if(allowFading)
            image.color = Color.Lerp(image.color, Color.black, fadeSpeed * Time.deltaTime);

        Debug.Log("Color: " + image.color.r + " , " + image.color.g + " , " + image.color.b + " , " + image.color.a);

        if(image.color.r < colorThreshold)
        {
            SceneManager.LoadScene(nextSceneName);
            allowFading = false;
        }
          
    }

    private IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(waitingTime);
        allowFading = true;    
    }
}
