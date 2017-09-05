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
    [Tooltip("Pitch Offset (relative to the base pitch)")]
    [Range(0.0f, 0.5f)]
    public float pitchRelativeOffset = 0.1f;
    [Tooltip("Volume Offset (relative to the base volume)")]
    [Range(0.0f, 0.5f)]
    public float volumeRelativeOffset = 0.1f;

    private GameObject cameraManagerGO;
    private CameraManager cameraManager;
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

            cameraManagerGO = GameObject.FindGameObjectWithTag("CameraManager");
            if (cameraManagerGO)
            {
                cameraManager = cameraManagerGO.GetComponent<CameraManager>();
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

    public AudioSource PlayDiegeticFx(GameObject sourceGO, AudioClip clip, float pitch = 1.0f, float volume = 1.0f)
    {
        // Play diegetic sound fx only if they are produced by the player
        // or if their source GO position is within player camera boundaries 
        GameObject obj;
        if (sourceGO.tag.Equals("Player") || sourceGO.layer == LayerMask.NameToLayer("totem attack") 
            || cameraManager.Is3DPositionOnScreen(sourceGO.transform.position))
        {
            obj = GetDiegeticFXAudioSource();
            return PlayFx(obj, clip, pitch, volume);
        }
        else return null;
    }

    public AudioSource PlayNonDiegeticFx(AudioClip clip, float pitch = 1.0f, float volume = 1.0f)
    {
        GameObject obj = GetNonDiegeticFXAudioSource();
        return PlayFx(obj, clip, pitch, volume);
    }

    public AudioSource PlayFx(GameObject obj, AudioClip clip, float pitch = 1.0f, float volume = 1.0f)
    {
        obj.SetActive(true);
        AudioSource fxSource = obj.GetComponent<AudioSource>();

        fxSource.clip = clip;
        fxSource.pitch = pitch;
        fxSource.volume = volume;
        fxSource.Play();
        StartCoroutine(ReleaseAudioSource(obj, clip.length, Time.timeScale));

        return fxSource;
    }

    public void RandomizePlayFx(GameObject sourceGO, float basePitch = 1.0f, float baseVolume = 1.0f, params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(basePitch - basePitch * pitchRelativeOffset, 
            basePitch + basePitch * pitchRelativeOffset);
        float randomVolume = Random.Range(baseVolume - 2 * baseVolume * volumeRelativeOffset, baseVolume);
        PlayDiegeticFx(sourceGO, clips[randomIndex], randomPitch, randomVolume);
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
        cameraManagerGO = GameObject.FindGameObjectWithTag("CameraManager");
        if (cameraManagerGO)
        {
            cameraManager = cameraManagerGO.GetComponent<CameraManager>();
        }

        switch (scene.buildIndex)
        {
            // case 0: break; // Title
            case 1: // Initial menu
            {
                PlayMusic(introCutscene);
                break;
            }
            case 2: // Dungeon entrance
            {
                StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix", 0.0f));
                StopMusic();
                break;
            }
            case 3: // Dungeon
            {
                StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix", 0.15f));
                StopMusic();
                PlayMusic(playingLevel);
                break;
            }
            case 4: // Boss scene
            {
                StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix", 0.15f));
                StopMusic(); 
                break;
            }
            // case 5: break; // End
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

    IEnumerator SetMixerParameter(string parameter, float value)
    {
        // I know this looks silly, but it is a workaround to a unity bug
        yield return new WaitForEndOfFrame();
        AudioManager.instance.mixer.SetFloat(parameter, value);
    }

}
