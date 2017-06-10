using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

    public static AudioManager instance = null;
    public AudioMixer mixer;

    [Header("Prefabs")]
    public GameObject fXAudioSourcePrefab;
    public int numberOfFXAudioSources = 5;

    [Header("Channels")]
    public AudioSource musicSource;
    public AudioSource fxSource;

    [Header("Music Tracks")]
    public AudioClip introCutscene;
    public AudioClip playingLevel;

    [Header("Randomization")]
    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

    private List<GameObject> fXAudioSources;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {
        /*        musicSource.clip = introCutscene;
                musicSource.volume = 0.0f;
                musicSource.Play();
                StartCoroutine("FadeIn", 10.0f);*/
        fXAudioSources = new List<GameObject>();
        for(int i = 0; i < numberOfFXAudioSources; ++i)
        {
            GameObject obj = (GameObject)Instantiate(fXAudioSourcePrefab);
            obj.SetActive(false);
            fXAudioSources.Add(obj);
        }

    }

    GameObject GetFXAudioSource()
    {
        for(int i = 0; i < fXAudioSources.Count; ++i)
        {
            if(!fXAudioSources[i].activeInHierarchy)
            {
                return fXAudioSources[i];
            }
        }

        GameObject obj = (GameObject)Instantiate(fXAudioSourcePrefab);
        fXAudioSources.Add(obj);
        return obj;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayFx(AudioClip clip, float pitch = 1.0f)
    {
        GameObject obj = GetFXAudioSource();
        obj.SetActive(true);
        AudioSource fxSource = obj.GetComponent<AudioSource>();

        fxSource.clip = clip;
        fxSource.pitch = pitch;
        fxSource.Play();
        StartCoroutine(ReleaseAudioSource(obj, clip.length, Time.timeScale));
    }

    public void RandomizePlayFx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        PlayFx(clips[randomIndex], randomPitch);
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

    public void PauseAllFXs()
    {
        for (int i = 0; i < fXAudioSources.Count; ++i)
        {
            if (fXAudioSources[i].activeInHierarchy)
            {
                fXAudioSources[i].GetComponent<AudioSource>().Pause();
            }
        }
    }

    public void UnPauseAllFXs()
    {
        for (int i = 0; i < fXAudioSources.Count; ++i)
        {
            if (fXAudioSources[i].activeInHierarchy)
            {
                fXAudioSources[i].GetComponent<AudioSource>().UnPause();
            }
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

    IEnumerator ReleaseAudioSource(GameObject obj, float delayTime, float timeScale)
    {
        if (timeScale < 0.005f)
        {
            do
            {
                yield return new WaitForSecondsRealtime(delayTime);
            }
            while (obj.GetComponent<AudioSource>().isPlaying);
        }
        else
        {
            do
            {
                yield return new WaitForSeconds(delayTime);
            }
            while (obj.GetComponent<AudioSource>().isPlaying);
        }

        obj.SetActive(false);
    }

}
