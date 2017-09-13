using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {

    public enum State
    {
        TITLE_CINEMATIC,
        INITIAL_MENU,
        TUTORIAL,
        WARMUP,
        KOREAN_MODE,
        HARDCORE_BATTLE,
        LIFTABLE_PLATFORMS,
        BOSS_CINEMATIC,
        BOSS,
        COMICAL_ENDING,
        CREDITS
    }

    State currentState;
    State previousState;

    public static AudioManager instance = null;
    public AudioMixer mixer;

    [Header("Prefabs")]
    public GameObject fXDiegeticAudioSourcePrefab;
    public GameObject fXNonDiegeticAudioSourcePrefab;
    public GameObject musicAudioSourcePrefab;
    private int numberOfFXDiegeticAudioSources = 5;
    private int numberOfFXNonDiegeticAudioSources = 5;
//    private int numberOfMusicAudioSources = 2;

    [Header("Music Tracks")]
    public AudioClip cinematicaInicio;
    public MusicLoop menuInicial;
    public AudioClip tutorial;
    public AudioClip[] warmUp;
    public AudioClip koreanMode;
    public AudioClip hardcoreBattle;
    public AudioClip liftablePlatforms;
    public AudioClip cinematicaBoss;
    public AudioClip boss;
    public AudioClip finalComico;
    public AudioClip creditos;

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
//    private List<GameObject> musicAudioSources;

    public AudioSource musicChannel1;
    public AudioSource musicChannel2;

    [System.Serializable]
    public class MusicLoop
    {
        public AudioClip audioClip;
        public AudioClip audioLoop;
        public float MusicLoopPointStart, MusicLoopPointEnd;
        public bool isLooping, isFinished;
        public AudioSource musicAudioSource;

        float[] audioData;
        long position;
        int sampleLoopPointStart, sampleLoopPointEnd;
        int start;

        public void Init(AudioSource audioSource)
        {
            isLooping = true;
            isFinished = false;
            musicAudioSource = audioSource;

            double multiplier = MusicLoopPointStart / audioClip.length;
            sampleLoopPointStart = (int)(multiplier * audioClip.samples * audioClip.channels);
            multiplier = MusicLoopPointEnd / audioClip.length;
            sampleLoopPointEnd = (int)(multiplier * audioClip.samples * audioClip.channels);
            audioData = new float[audioClip.samples * audioClip.channels];

            audioClip.GetData(audioData, 0);
            audioLoop = AudioClip.Create(audioClip.name + "_Loop", audioClip.samples, audioClip.channels, audioClip.frequency, true, OnAudioRead, OnAudioSetPos);
        }

        void OnAudioRead(float[] data)
        {
            if (isFinished)
            {
                Debug.Log("shit has finished");
                return;
            }

            if (start < 64)
            {
                start++;
                position = 0;
                return;
            }

            int count = 0;
            while (count < data.Length)
            {
                data[count] = audioData[position];

                if (position < audioData.Length - 1)
                {
                    position++;
                    count++;
                }
                else if (!isLooping)
                {
                    isFinished = true;
                    return;
                }

                if (position >= sampleLoopPointEnd && isLooping == true)
                {
                    position = sampleLoopPointStart;
                }
            }
        }

        void OnAudioSetPos(int newPos)
        {

        }

        public void Release()
        {
            musicAudioSource.loop = false;
            musicAudioSource.clip = null;
            AudioManager.instance.StopMusic(musicAudioSource);
            audioLoop = null;
            AudioClip.DestroyImmediate(audioLoop, false);
        }
    }

    void Awake()
    {
        GameObject obj;

        if (instance == null)
        {
            instance = this;

            // Create Sound FX pools of audio source objects
            fXDiegeticAudioSources = InitializeAudioSources(fXDiegeticAudioSourcePrefab, numberOfFXDiegeticAudioSources);
            fXNonDiegeticAudioSources = InitializeAudioSources(fXNonDiegeticAudioSourcePrefab, numberOfFXNonDiegeticAudioSources);

            // Create Music audio source objects
            obj = (GameObject)Instantiate(musicAudioSourcePrefab);
            musicChannel1 = obj.GetComponent<AudioSource>();
            DontDestroyOnLoad(obj);
            obj = (GameObject)Instantiate(musicAudioSourcePrefab);
            musicChannel2 = obj.GetComponent<AudioSource>();
            DontDestroyOnLoad(obj);

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

    void Update()
    {
        if (menuInicial.isFinished)
            menuInicial.Release();
    }

    GameObject GetAudioSource(GameObject audioSourcePrefab, ref List<GameObject> audioSourceList)
    {
        for (int i = 0; i < audioSourceList.Count; ++i)
        {
            if (!audioSourceList[i].activeInHierarchy)
            {
                return audioSourceList[i];
            }
        }

        GameObject obj = (GameObject)Instantiate(audioSourcePrefab);
        audioSourceList.Add(obj);
        DontDestroyOnLoad(obj);
        return obj;
    }

    public void StopMusic(AudioSource musicSource)
    {
        musicSource.Stop();
    }

    public AudioSource PlayMusic(AudioClip clip, AudioSource musicSource, float loopStart = 0.0f, float loopEnd = 0.0f)
    {
//        obj.SetActive(true);
//        AudioSource musicSource = obj.GetComponent<AudioSource>();

        musicSource.clip = clip;
        musicSource.Play();

 //       StartCoroutine(ReleaseAudioSource(obj, clip.length, Time.timeScale));
        return musicSource;
    }

    public AudioSource PlayDiegeticFx(GameObject sourceGO, AudioClip clip, float pitch = 1.0f, float volume = 1.0f)
    {
        // Play diegetic sound fx only if they are produced by the player
        // or if their source GO position is within player camera boundaries 
        GameObject obj;
        if (sourceGO.tag.Equals("Player") || sourceGO.layer == LayerMask.NameToLayer("totem attack") 
            || cameraManager.Is3DPositionOnScreen(sourceGO.transform.position))
        {
            obj = GetAudioSource(fXDiegeticAudioSourcePrefab, ref fXDiegeticAudioSources);
            return PlayFx(obj, clip, pitch, volume);
        }
        else return null;
    }

    public AudioSource PlayNonDiegeticFx(AudioClip clip, float pitch = 1.0f, float volume = 1.0f)
    {
        GameObject obj = GetAudioSource(fXNonDiegeticAudioSourcePrefab, ref fXNonDiegeticAudioSources);
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
            case 0:
                PlayMusic(cinematicaInicio, musicChannel1);
                break; // Title
            case 1: // Initial menu
            {
                menuInicial.Init(musicChannel2);
                if (musicChannel1)
                    FadeAudioSource(musicChannel1, FadeAudio.FadeType.FadeOut, 2.0f, 0.0f);
                PlayMusic(menuInicial.audioLoop, musicChannel2);
                musicChannel2.loop = true;
//                PlayMusic(introCutscene);
                break;
            }
            case 2: // Dungeon entrance
            {
                StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix", 0.0f));
//                StopMusic();
                break;
            }
            case 3: // Dungeon
            {
                StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix", 0.15f));
//                StopMusic();
//                PlayMusic(playingLevel);
                break;
            }
            case 4: // Boss scene
            {
                StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix", 0.15f));
//                StopMusic(); 
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
//        while(musicSource.volume < 1.0f)
 //       {
 //           musicSource.volume += Time.deltaTime / duration;
            yield return null;
 //       }
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

    private List<GameObject> InitializeAudioSources(GameObject audioSourcePrefab, int numberOfAudioSources)
    {
        List<GameObject> audioSourceList = new List<GameObject>();
        for (int i = 0; i < numberOfAudioSources; ++i)
        {
            GameObject obj = (GameObject)Instantiate(audioSourcePrefab);
            obj.SetActive(false);
            audioSourceList.Add(obj);
            DontDestroyOnLoad(obj);
        }

        return audioSourceList;
    }
}
