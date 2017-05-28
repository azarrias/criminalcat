﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

    AudioSource audioSource;

    [Header("Music")]
    public AudioClip introCutscene;
    public AudioClip playingLevel;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

	// Use this for initialization
	void Start () {
        audioSource.clip = introCutscene;
        audioSource.Play();
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
            case 2:
                audioSource.Stop();
                audioSource.clip = playingLevel;
                audioSource.Play();
                break;
        }

    }

}
