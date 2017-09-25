using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBehaviour : MonoBehaviour {

    [Header("Sound Effects")]
    [Range(0.0f, 1.0f)]
    public float flapWingsVolume = 0.1f;
    [Range(0.0f, 10.0f)]
    public float flapWingsPitch = 1.0f;
    public AudioClip[] fxFlapWings;

    Animator anim;
    AnimatorStateInfo state;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        state = anim.GetCurrentAnimatorStateInfo(0);
    }

    private void Start()
    {
        anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
    }

    public void FlapWings()
    {
        AudioManager.instance.RandomizePlayFx(gameObject, flapWingsPitch, flapWingsVolume, fxFlapWings);
    }
}
