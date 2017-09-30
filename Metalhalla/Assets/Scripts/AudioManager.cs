﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private const float POSX_ENTER_KOREAN_MODE = 82.0f;
    private const float POSX_ENTER_LIFTABLE_PLATFORMS = 140.0f;
    private const float POSX_ENTER_ENDING = 62.0f;

    private const float ENDING_TRACK_VOLUME = 0.95f;
    private const float MUSIC_TRACK_FADEOUT_SHORT = 2.0f;
    private const float MUSIC_TRACK_FADEOUT_LONG = 6.0f;
    private const float FADEOUT_TARGET_VOLUME = 0.0f;

    private const float CAVE_ECHO_WETMIX = 0.15f;
    private const float STARTING_MIXER_GROUP_ATTENUATION = 0.5f;

    public enum State
    {
        TITLE_CINEMATIC,
        INITIAL_MENU,
        TUTORIAL,
        WARMUP,
        KOREAN_MODE,
//        HARDCORE_BATTLE,
        LIFTABLE_PLATFORMS,
        BOSS_CINEMATIC,
        BOSS,
        ENDING,
        CREDITS
    }

    State currentState;

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
    //    public MusicLoop menuInicial;
    public AudioClip menuInicial;
    public MusicLoop tutorial;
    public AudioClip[] warmUp;
    public MusicLoop koreanMode;
//    public AudioClip hardcoreBattle;
    public AudioClip liftablePlatforms;
//    public AudioClip cinematicaBoss;
    public AudioClip boss;
    public AudioClip final;
//    public AudioClip creditos;

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

    private GameObject player;

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
            audioLoop = AudioClip.Create(audioClip.name + "_Loop", audioClip.samples, audioClip.channels, 
                audioClip.frequency, true, OnAudioRead, OnAudioSetPos);
        }

        void OnAudioRead(float[] data)
        {
            if (isFinished)
                return;

            if (start <= 16)
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
    void Start()
    {
        AudioManager.instance.mixer.SetFloat("MusicVolume", 20.0f * Mathf.Log10(STARTING_MIXER_GROUP_ATTENUATION));
        AudioManager.instance.mixer.SetFloat("FXDiegeticVolume", 20.0f * Mathf.Log10(STARTING_MIXER_GROUP_ATTENUATION));
        AudioManager.instance.mixer.SetFloat("FXNonDiegeticVolume", 20.0f * Mathf.Log10(STARTING_MIXER_GROUP_ATTENUATION));
    }

    void Update()
    {
        switch(currentState)
        {
            case State.WARMUP:
                {
                    if (!musicChannel2.isPlaying)
                        PlayMusic(warmUp[Random.Range(0, warmUp.Length)], musicChannel2);

                    if (player.transform.position.x > POSX_ENTER_KOREAN_MODE)
                    {
                        koreanMode.Init(musicChannel1);
                        if (AudioManager.instance.musicChannel2)
                            AudioManager.instance.FadeAudioSource(AudioManager.instance.musicChannel2, FadeAudio.FadeType.FadeOut,
                                MUSIC_TRACK_FADEOUT_LONG, FADEOUT_TARGET_VOLUME, false);
                        PlayMusic(koreanMode.audioLoop, musicChannel1);
                        currentState = State.KOREAN_MODE;
                    }

                    break;
                }
            case State.KOREAN_MODE:
                {
                    if (player.transform.position.x > POSX_ENTER_LIFTABLE_PLATFORMS)
                    {
                        if (AudioManager.instance.musicChannel1)
                            AudioManager.instance.FadeAudioSource(AudioManager.instance.musicChannel1, FadeAudio.FadeType.FadeOut,
                                MUSIC_TRACK_FADEOUT_LONG, FADEOUT_TARGET_VOLUME, false);
                        PlayMusic(liftablePlatforms, musicChannel2);
                        musicChannel2.loop = true;
                        currentState = State.LIFTABLE_PLATFORMS;
                    }
                    break;
                }
            case State.BOSS:
                {
                    if (player.transform.position.x > POSX_ENTER_ENDING)
                    {
                        if (AudioManager.instance.musicChannel1)
                            AudioManager.instance.FadeAudioSource(AudioManager.instance.musicChannel1, FadeAudio.FadeType.FadeOut,
                                MUSIC_TRACK_FADEOUT_SHORT, FADEOUT_TARGET_VOLUME, false);
                        PlayMusic(final, musicChannel2, ENDING_TRACK_VOLUME);
                        musicChannel2.loop = false;
                        currentState = State.ENDING;
                    }
                    break;
                }
        }

//        if (menuInicial.isFinished)
//            menuInicial.Release();
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

    public AudioSource PlayMusic(AudioClip clip, AudioSource musicSource, float volume = 1.0f)
    {
        //        obj.SetActive(true);
        //        AudioSource musicSource = obj.GetComponent<AudioSource>();

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();

        //       StartCoroutine(ReleaseAudioSource(obj, clip.length, Time.timeScale));
        return musicSource;
    }

    public AudioSource PlayDiegeticFx(GameObject sourceGO, AudioClip clip, bool loop = false, float pitch = 1.0f, float volume = 1.0f)
    {
        // Play diegetic sound fx only if they are produced by the player
        // or if their source GO position is within player camera boundaries 
        GameObject obj;
        if (sourceGO.tag.Equals("Player") || sourceGO.layer == LayerMask.NameToLayer("totem attack")
            || cameraManager.Is3DPositionOnScreen(sourceGO.transform.position))
        {
            obj = GetAudioSource(fXDiegeticAudioSourcePrefab, ref fXDiegeticAudioSources);
            return PlayFx(obj, clip, loop, pitch, volume);
        }
        else return null;
    }

    public AudioSource PlayNonDiegeticFx(AudioClip clip, bool loop = false, float pitch = 1.0f, float volume = 1.0f)
    {
        GameObject obj = GetAudioSource(fXNonDiegeticAudioSourcePrefab, ref fXNonDiegeticAudioSources);
        return PlayFx(obj, clip, loop, pitch, volume);
    }

    public AudioSource PlayFx(GameObject obj, AudioClip clip, bool loop = false, float pitch = 1.0f, float volume = 1.0f)
    {
        obj.SetActive(true);
        AudioSource fxSource = obj.GetComponent<AudioSource>();

        fxSource.clip = clip;
        fxSource.pitch = pitch;
        fxSource.volume = volume;
        if (loop)
            fxSource.loop = true;

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
        PlayDiegeticFx(sourceGO, clips[randomIndex], false, randomPitch, randomVolume);
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

            case 0: // Title
                PlayMusic(cinematicaInicio, musicChannel1);
                break; 
            case 1: // Initial menu
                {
                    if (AudioManager.instance.musicChannel1)
                        AudioManager.instance.FadeAudioSource(AudioManager.instance.musicChannel1, FadeAudio.FadeType.FadeOut, 
                            MUSIC_TRACK_FADEOUT_SHORT, FADEOUT_TARGET_VOLUME, false);

//                    menuInicial.Init(musicChannel2);
                    PlayMusic(menuInicial, musicChannel2);
                    FadeAudioSource(musicChannel2, FadeAudio.FadeType.FadeIn, 2.0f, 1.0f, false);
                    musicChannel2.loop = true;

                    StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix"));
                    break;
                }
            case 2: // Dungeon entrance
                {
                    player = GameObject.FindGameObjectWithTag("Player");

                    if (AudioManager.instance.musicChannel2)
                        AudioManager.instance.FadeAudioSource(AudioManager.instance.musicChannel2, FadeAudio.FadeType.FadeOut,
                            MUSIC_TRACK_FADEOUT_SHORT, FADEOUT_TARGET_VOLUME, false);

                    tutorial.Init(musicChannel1);
                    PlayMusic(tutorial.audioLoop, musicChannel1);
                    musicChannel1.loop = true;

                    StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix"));
                    break;
                }
            case 3: // Dungeon
                {
                    player = GameObject.FindGameObjectWithTag("Player");
                    currentState = State.WARMUP;

//                    if (AudioManager.instance.musicChannel2)
//                        menuInicial.Release();

                    if (AudioManager.instance.musicChannel1)
                        AudioManager.instance.FadeAudioSource(AudioManager.instance.musicChannel1, FadeAudio.FadeType.FadeOut,
                            MUSIC_TRACK_FADEOUT_LONG, FADEOUT_TARGET_VOLUME, false);

                    StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix", CAVE_ECHO_WETMIX));
                    break;
                }
            case 4: // Boss scene
                {
                    player = GameObject.FindGameObjectWithTag("Player");
                    currentState = State.BOSS;

                    if (AudioManager.instance.musicChannel2)
                        AudioManager.instance.FadeAudioSource(AudioManager.instance.musicChannel2, FadeAudio.FadeType.FadeOut,
                            MUSIC_TRACK_FADEOUT_SHORT, FADEOUT_TARGET_VOLUME, false);

                    PlayMusic(boss, musicChannel1);

                    StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix", CAVE_ECHO_WETMIX));
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

    public void FadeAudioSource(AudioSource audioSource, FadeAudio.FadeType type, float duration, float targetVolume, bool pause = true)
    {
        FadeAudio fadeAudioComponent = gameObject.AddComponent<FadeAudio>() as FadeAudio;
        fadeAudioComponent.fadeType = type;
        fadeAudioComponent.audioSource = audioSource;
        fadeAudioComponent.fadeDuration = duration;
        fadeAudioComponent.targetVolume = targetVolume;
        fadeAudioComponent.canBePaused = pause;
    }

    IEnumerator SetMixerParameter(string parameter, float value = 0.0f)
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

    public void Wait(float seconds, System.Action action)
    {
        StartCoroutine(_wait(seconds, action));
    }

    IEnumerator _wait(float time, System.Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }
}
