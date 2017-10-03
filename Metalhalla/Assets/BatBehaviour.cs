using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBehaviour : MonoBehaviour {

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
        AudioManager.instance.RandomizePlayFx(gameObject, 1.0f, AudioManager.FX_BAT_FLAPWINGS_VOL, fxFlapWings);
    }
}
