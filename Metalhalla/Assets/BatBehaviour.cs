﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBehaviour : MonoBehaviour {

    [Header("Sound Effects")]
    public AudioClip[] fxFlapWings;

    private void Awake()
    {
        Animator anim = GetComponent<Animator>();
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
    }

    public void FlapWings()
    {
        AudioManager.instance.RandomizePlayFx(gameObject, 1.0f, 0.3f, fxFlapWings);
    }
}
