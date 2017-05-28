using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

    AudioSource audioSource;

    [Header("Music")]
    public AudioClip introCutscene;

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
	
	// Update is called once per frame
	void Update () {
		
	}
}
