using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsController : MonoBehaviour {

    public float totalCreditsTime = 20.0f;
    public float staticCreditsTime = 5.0f;
    public float alphaFadeoutTime = 2.5f;
    private float elapsedCreditsTime = 0.0f;

    public GameObject creditsGO;
    public Image criminalCatLogo;

    private float finalcreditsYtranslation;
    private float creditsYtranslationSpeed;

    public AudioClip creditsTrack;
    private AudioSource audioSource;

    public string nextSceneName;
    private SceneLoader loader;



    private void Start () {
        loader = GameObject.FindWithTag("SceneLoader").GetComponent<SceneLoader>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = creditsTrack;
        audioSource.Play();

        finalcreditsYtranslation = Screen.height / 2 + creditsGO.GetComponent<RectTransform>().rect.height;
        creditsYtranslationSpeed = finalcreditsYtranslation/ (totalCreditsTime - staticCreditsTime);
    }
	
	void Update () {
        elapsedCreditsTime += Time.deltaTime;
        if (elapsedCreditsTime < totalCreditsTime - staticCreditsTime)
        {
            creditsGO.transform.Translate(0, creditsYtranslationSpeed * Time.deltaTime, 0);
        }
        if (elapsedCreditsTime >= totalCreditsTime - alphaFadeoutTime)
        {
            ChangeAlphaToImage(criminalCatLogo, (totalCreditsTime - elapsedCreditsTime) / alphaFadeoutTime);
        }
        if (Input.GetButtonDown("DisplayMenu") || elapsedCreditsTime >= totalCreditsTime)
        {
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        loader.GoToNextScene(nextSceneName);
    }


    private void ChangeAlphaToImage( Image img, float newAlpha)
    {
        Color c = img.color;
        c.a = newAlpha;
        img.color = c;
    }

}
