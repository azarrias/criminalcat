using System.Collections;
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
    public MusicTrack cinematicaInicio;
    //    public MusicLoop menuInicial;
    public MusicTrack menuInicial;
    public MusicLoop tutorial;
    public MusicTrack[] warmUp;
    private int currentWarmUpIndex;
    public MusicLoop koreanMode;
//    public AudioClip hardcoreBattle;
    public MusicTrack[] liftablePlatforms;
//    public AudioClip cinematicaBoss;
    public MusicTrack boss;
    public MusicTrack final;
    //    public AudioClip creditos;
    public Stack<MusicPlay> musicStack;

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
    public class MusicPlay
    {
        public MusicTrack track;
        public AudioSource audioSource;

        public MusicPlay(MusicTrack mt, AudioSource a)
        {
            track = mt;
            audioSource = a;
        }
    }

    [System.Serializable]
    public class MusicTrack
    {
        public AudioClip sourceAudioClip;
        [HideInInspector]
        public AudioClip audioClip;
        [HideInInspector]
        public AudioSource musicAudioSource;
        [HideInInspector]
        public AudioSource otherMusicAudioSource;

        [HideInInspector]
        // Fade out point in PCM samples
        public int fadePoint;

        virtual public void Init()
        {
            audioClip = sourceAudioClip;
            fadePoint = (int)((audioClip.length - MUSIC_TRACK_FADEOUT_LONG) * audioClip.frequency);

            if (!AudioManager.instance.musicChannel1.isPlaying)
            {
                musicAudioSource = AudioManager.instance.musicChannel1;
                otherMusicAudioSource = AudioManager.instance.musicChannel2;
            }
            else if (!AudioManager.instance.musicChannel2.isPlaying)
            {
                musicAudioSource = AudioManager.instance.musicChannel2;
                otherMusicAudioSource = AudioManager.instance.musicChannel1;
            }
            else
            {
/*                FadeAudio fade = AudioManager.instance.musicChannel1.GetComponent<FadeAudio>();
                if (fade != null && fade.fadeType == FadeAudio.FadeType.FadeOut)
                {
                    musicAudioSource = AudioManager.instance.musicChannel1;
                    otherMusicAudioSource = AudioManager.instance.musicChannel2;
                }
                else
                {
                    musicAudioSource = AudioManager.instance.musicChannel2;
                    otherMusicAudioSource = AudioManager.instance.musicChannel1;
                }*/
                Debug.Log("There are no free music channels to initialize " + sourceAudioClip.name);
            }
        }

        public void Release(AudioSource source)
        {
            source.loop = false;
            source.clip = null;
            audioClip = null;
            AudioClip.DestroyImmediate(audioClip, false);
        }
    }

    [System.Serializable]
    public class MusicLoop : MusicTrack
    {
        public float MusicLoopPointStart, MusicLoopPointEnd;
        public bool isLooping, isFinished;
//        public AudioSource musicAudioSource;

        float[] audioData;
        long position;
        int sampleLoopPointStart, sampleLoopPointEnd;
        int start;

        override public void Init()
        {
            base.Init();
            isLooping = true;
            isFinished = false;
            position = 0;

            double multiplier = MusicLoopPointStart / sourceAudioClip.length;
            sampleLoopPointStart = (int)(multiplier * sourceAudioClip.samples * sourceAudioClip.channels);
            multiplier = MusicLoopPointEnd / sourceAudioClip.length;
            sampleLoopPointEnd = (int)(multiplier * sourceAudioClip.samples * sourceAudioClip.channels);
            audioData = new float[sourceAudioClip.samples * sourceAudioClip.channels];

            sourceAudioClip.GetData(audioData, 0);
            audioClip = AudioClip.Create(sourceAudioClip.name + "_Loop", sourceAudioClip.samples, sourceAudioClip.channels,
                sourceAudioClip.frequency, true, OnAudioRead, OnAudioSetPos);
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

/*        public override void Release(AudioSource source)
        {
            base.Release(source);
            AudioClip.DestroyImmediate(audioClip, false);
        }*/
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

            musicStack = new Stack<MusicPlay>();
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
        MusicPlay play;

        switch(currentState)
        {
            case State.WARMUP:
                if (player.transform.position.x > POSX_ENTER_KOREAN_MODE)
                {
                    while (musicStack.Count > 0)
                    {
                        play = musicStack.Pop();
                        FadeAudioSource(play.audioSource, FadeAudio.FadeType.FadeOut, MUSIC_TRACK_FADEOUT_LONG, FADEOUT_TARGET_VOLUME, false);
                        AudioManager.instance.Wait(MUSIC_TRACK_FADEOUT_LONG * 2, () =>
                        {
                            AudioManager.instance.StopMusic(play);
                        });
                    }

                    koreanMode.Init();
                    PlayMusic(koreanMode);
                    koreanMode.musicAudioSource.loop = true;
                    currentState = State.KOREAN_MODE;
                }
                else if (warmUp[currentWarmUpIndex].fadePoint <= warmUp[currentWarmUpIndex].musicAudioSource.timeSamples)
                {
                    currentWarmUpIndex = (currentWarmUpIndex + 1) % 2;
                    while (musicStack.Count > 0)
                    {
                        play = musicStack.Pop();
                        FadeAudioSource(play.audioSource, FadeAudio.FadeType.FadeOut, MUSIC_TRACK_FADEOUT_LONG, FADEOUT_TARGET_VOLUME, false);
                        AudioManager.instance.Wait(MUSIC_TRACK_FADEOUT_LONG * 2, () =>
                        {
                            AudioManager.instance.StopMusic(play);
                        });
                    }

                    warmUp[currentWarmUpIndex].Init();
                    PlayMusic(warmUp[currentWarmUpIndex]);
                    FadeAudioSource(warmUp[currentWarmUpIndex].musicAudioSource, FadeAudio.FadeType.FadeIn, 
                        MUSIC_TRACK_FADEOUT_LONG, 1.0f, false);
                }
                break;

            case State.KOREAN_MODE:
                if (player.transform.position.x > POSX_ENTER_LIFTABLE_PLATFORMS)
                {
                    currentWarmUpIndex = 0;
                    currentState = State.LIFTABLE_PLATFORMS;
                    while (musicStack.Count > 0)
                    {
                        play = musicStack.Pop();
                        FadeAudioSource(play.audioSource, FadeAudio.FadeType.FadeOut, MUSIC_TRACK_FADEOUT_LONG, FADEOUT_TARGET_VOLUME, false);
                        AudioManager.instance.Wait(MUSIC_TRACK_FADEOUT_LONG * 2, () =>
                        {
                            AudioManager.instance.StopMusic(play);
                        });
                    }

                    liftablePlatforms[currentWarmUpIndex].Init();

                    PlayMusic(liftablePlatforms[currentWarmUpIndex]);
                    FadeAudioSource(liftablePlatforms[currentWarmUpIndex].musicAudioSource, FadeAudio.FadeType.FadeIn, 
                        MUSIC_TRACK_FADEOUT_LONG, 1.0f, false);
                }

                break;

            case State.LIFTABLE_PLATFORMS:
                if (liftablePlatforms[currentWarmUpIndex].fadePoint <= liftablePlatforms[currentWarmUpIndex].musicAudioSource.timeSamples)
                {
                    currentWarmUpIndex = (currentWarmUpIndex + 1) % 2;
                    while (musicStack.Count > 0)
                    {
                        play = musicStack.Pop();
                        FadeAudioSource(play.audioSource, FadeAudio.FadeType.FadeOut, MUSIC_TRACK_FADEOUT_LONG, FADEOUT_TARGET_VOLUME, false);
                        AudioManager.instance.Wait(MUSIC_TRACK_FADEOUT_LONG * 2, () =>
                        {
                            AudioManager.instance.StopMusic(play);
                        });
                    }

                    liftablePlatforms[currentWarmUpIndex].Init();
                    PlayMusic(liftablePlatforms[currentWarmUpIndex]);
                    FadeAudioSource(liftablePlatforms[currentWarmUpIndex].musicAudioSource, FadeAudio.FadeType.FadeIn,
                        MUSIC_TRACK_FADEOUT_LONG, 1.0f, false);
                }
                break;

            case State.BOSS:
                if (player.transform.position.x > POSX_ENTER_ENDING)
                {
                    while (musicStack.Count > 0)
                    {
                        play = musicStack.Pop();
                        FadeAudioSource(play.audioSource, FadeAudio.FadeType.FadeOut, MUSIC_TRACK_FADEOUT_SHORT, FADEOUT_TARGET_VOLUME, false);
                        AudioManager.instance.Wait(MUSIC_TRACK_FADEOUT_LONG * 2, () =>
                        {
                            AudioManager.instance.StopMusic(play);
                        });
                    }

                    currentState = State.ENDING;
                    final.Init();
                    PlayMusic(final, ENDING_TRACK_VOLUME);
                }

                break;
        }

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

    public void StopMusic(MusicPlay music)
    {
        music.track.musicAudioSource.Stop();
        music.track.Release(music.audioSource);
    }

    public AudioSource PlayMusic(MusicTrack track, float volume = 1.0f)
    {
        if(track.musicAudioSource)
        {
            track.musicAudioSource.clip = track.audioClip;
            track.musicAudioSource.volume = volume;
            musicStack.Push(new MusicPlay(track, track.musicAudioSource));
            track.musicAudioSource.Play();
        }
        else
        {
            Debug.Log("Cannot play music track " + track.sourceAudioClip.name + " since it has no audio source attached to it");
        }

        return track.musicAudioSource;
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
                cinematicaInicio.Init();
                PlayMusic(cinematicaInicio);
                break; 

            case 1: // Initial menu
                menuInicial.Init();
                while (musicStack.Count > 0)
                {
                    AudioManager.instance.StopMusic(musicStack.Pop());
                    FadeAudioSource(menuInicial.musicAudioSource, FadeAudio.FadeType.FadeIn, 5.0f, 1.0f, false);
                }

                PlayMusic(menuInicial);
                menuInicial.musicAudioSource.loop = true;

                StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix"));
                break;

            case 2: // Dungeon entrance
                player = GameObject.FindGameObjectWithTag("Player");

                while (musicStack.Count > 0)
                {
                    AudioManager.instance.StopMusic(musicStack.Pop());
                }
                tutorial.Init();
                PlayMusic(tutorial);
                FadeAudioSource(tutorial.musicAudioSource, FadeAudio.FadeType.FadeIn, 5.0f, 1.0f, false);
                tutorial.musicAudioSource.loop = true;

                StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix"));
                break;

            case 3: // Dungeon
                player = GameObject.FindGameObjectWithTag("Player");
                currentState = State.WARMUP;
                currentWarmUpIndex = 0;
                while (musicStack.Count > 0)
                {
                    AudioManager.instance.StopMusic(musicStack.Pop());
                }
                warmUp[currentWarmUpIndex].Init();

                PlayMusic(warmUp[currentWarmUpIndex]);
                FadeAudioSource(warmUp[currentWarmUpIndex].musicAudioSource, FadeAudio.FadeType.FadeIn, 5.0f, 1.0f, false);
                StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix", CAVE_ECHO_WETMIX));
                break;

            case 4: // Boss scene
                player = GameObject.FindGameObjectWithTag("Player");
                currentState = State.BOSS;
                while (musicStack.Count > 0)
                {
                    AudioManager.instance.StopMusic(musicStack.Pop());
                }
                boss.Init();
                if (boss.otherMusicAudioSource.isPlaying)
                    AudioManager.instance.FadeAudioSource(boss.otherMusicAudioSource, FadeAudio.FadeType.FadeOut,
                        MUSIC_TRACK_FADEOUT_SHORT, FADEOUT_TARGET_VOLUME, false);
                PlayMusic(boss);
                boss.musicAudioSource.loop = true;

                StartCoroutine(SetMixerParameter("FXDiegeticEchoWetmix", CAVE_ECHO_WETMIX));
                break;
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
