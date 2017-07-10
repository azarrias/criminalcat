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
    public GameObject fXDiegeticAudioSourcePrefab;
    public int numberOfFXDiegeticAudioSources = 5;
    public GameObject fXNonDiegeticAudioSourcePrefab;
    public int numberOfFXNonDiegeticAudioSources = 5;

        [Header("Channels")]
        public AudioSource musicSource;
    /*    public AudioSource diegeticFxSource;
        public AudioSource nondiegeticFxSource;*/

    [Header("Music Tracks")]
    public AudioClip introCutscene;
    public AudioClip playingLevel;

    [Header("Randomization")]
    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

    private List<GameObject> fXDiegeticAudioSources;
    private List<GameObject> fXNonDiegeticAudioSources;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            fXDiegeticAudioSources = new List<GameObject>();
            for (int i = 0; i < numberOfFXDiegeticAudioSources; ++i)
            {
                GameObject obj = (GameObject)Instantiate(fXDiegeticAudioSourcePrefab);
                obj.SetActive(false);
                fXDiegeticAudioSources.Add(obj);
                DontDestroyOnLoad(obj);
            }

            fXNonDiegeticAudioSources = new List<GameObject>();
            for (int i = 0; i < numberOfFXDiegeticAudioSources; ++i)
            {
                GameObject obj = (GameObject)Instantiate(fXDiegeticAudioSourcePrefab);
                obj.SetActive(false);
                fXDiegeticAudioSources.Add(obj);
                DontDestroyOnLoad(obj);
            }
        }
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {

    }

    GameObject GetDiegeticFXAudioSource()
    {
        for(int i = 0; i < fXDiegeticAudioSources.Count; ++i)
        {
            if(!fXDiegeticAudioSources[i].activeInHierarchy)
            {
                return fXDiegeticAudioSources[i];
            }
        }

        GameObject obj = (GameObject)Instantiate(fXDiegeticAudioSourcePrefab);
        fXDiegeticAudioSources.Add(obj);
        DontDestroyOnLoad(obj);
        return obj;
    }

    GameObject GetNonDiegeticFXAudioSource()
    {
        for (int i = 0; i < fXNonDiegeticAudioSources.Count; ++i)
        {
            if (!fXNonDiegeticAudioSources[i].activeInHierarchy)
            {
                return fXNonDiegeticAudioSources[i];
            }
        }

        GameObject obj = (GameObject)Instantiate(fXNonDiegeticAudioSourcePrefab);
        fXNonDiegeticAudioSources.Add(obj);
        DontDestroyOnLoad(obj);
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

    public AudioSource PlayDiegeticFx(AudioClip clip, float pitch = 1.0f)
    {
        GameObject obj = GetDiegeticFXAudioSource();
        return PlayFx(obj, clip, pitch);
    }

    public AudioSource PlayNonDiegeticFx(AudioClip clip, float pitch = 1.0f)
    {
        GameObject obj = GetNonDiegeticFXAudioSource();
        return PlayFx(obj, clip, pitch);
    }

    public AudioSource PlayFx(GameObject obj, AudioClip clip, float pitch = 1.0f)
    {
        obj.SetActive(true);
        AudioSource fxSource = obj.GetComponent<AudioSource>();

        fxSource.clip = clip;
        fxSource.pitch = pitch;
        fxSource.Play();
        StartCoroutine(ReleaseAudioSource(obj, clip.length, Time.timeScale));

        return fxSource;
    }

    public void RandomizePlayFx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        PlayDiegeticFx(clips[randomIndex], randomPitch);
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
            case 1: //initial menu
            {
                PlayMusic(introCutscene);
                break;
            }
            case 2: // Dungeon entrance
            { 
                StopMusic();
                break;
            }
            case 3: // Dungeon
            {
                StopMusic();
                PlayMusic(playingLevel);
                break;
            }
            case 4: // Boss scene
            {
                StopMusic(); 
                break;
            }
        }

    }

    public void PauseAllFXs()
    {
        for (int i = 0; i < fXDiegeticAudioSources.Count; ++i)
        {
            if (fXDiegeticAudioSources[i].activeInHierarchy)
            {
                fXDiegeticAudioSources[i].GetComponent<AudioSource>().Pause();
            }
        }
    }

    public void UnPauseAllFXs()
    {
        for (int i = 0; i < fXDiegeticAudioSources.Count; ++i)
        {
            if (fXDiegeticAudioSources[i].activeInHierarchy)
            {
                fXDiegeticAudioSources[i].GetComponent<AudioSource>().UnPause();
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

    public void FadeAudioSource(AudioSource audioSource, FadeAudio.FadeType type, float duration, float targetVolume)
    {
        FadeAudio fadeAudioComponent = gameObject.AddComponent<FadeAudio>() as FadeAudio;
        fadeAudioComponent.fadeType = type;
        fadeAudioComponent.audioSource = audioSource;
        fadeAudioComponent.fadeDuration = duration;
        fadeAudioComponent.targetVolume = targetVolume;
    }

}
