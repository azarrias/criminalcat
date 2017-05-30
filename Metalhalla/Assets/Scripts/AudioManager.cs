using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

    public static AudioManager instance = null;

    [Header("Channels")]
    public AudioSource musicSource;
    public AudioSource fxSource;

    [Header("Music Tracks")]
    public AudioClip introCutscene;
    public AudioClip playingLevel;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
/*	void Start () {
        musicSource.clip = introCutscene;
        musicSource.volume = 0.0f;
        musicSource.Play();
        StartCoroutine("FadeIn", 10.0f);
	}*/

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
     {

        switch (scene.buildIndex)
        {
            case 0:
                PlayMusic(introCutscene);
                break;
            case 2:
                StopMusic();
                PlayMusic(playingLevel);
                break;
        }

    }

    IEnumerator FadeIn(float duration)
    {
        while(musicSource.volume < 1.0f)
        {
            musicSource.volume += Time.deltaTime / duration;
            yield return null;
        }
    }

}
