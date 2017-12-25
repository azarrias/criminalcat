using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAudio : MonoBehaviour {

    public enum FadeType { FadeIn, FadeOut };
    public AudioSource audioSource;
    public float fadeDuration = 1f;
    public FadeType fadeType;

    float startingVolume;
    public float targetVolume;
    float normalizedTime;
    float step;
    public bool canBePaused;

    void Start()
    {
        startingVolume = audioSource.volume;
        normalizedTime = 0.0f;
        step = 1.0f / fadeDuration;
    }

    void Update () {
        if (normalizedTime <= 1.0f)
        {
            if (canBePaused)
                normalizedTime += step * Time.deltaTime;
            else
                normalizedTime += step * Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(fadeType == FadeType.FadeIn ? 0.0f : startingVolume, targetVolume, normalizedTime);
        }
        else
        {
            if (fadeType == FadeType.FadeOut)
            {
                audioSource.Stop();
            }
            audioSource.volume = startingVolume;

            Destroy(this);
        }
    }
}
