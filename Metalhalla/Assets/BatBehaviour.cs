using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBehaviour : MonoBehaviour {

    [Header("Sound Effects")]
    public AudioClip[] fxFlapWings;

    public void FlapWings()
    {
        AudioManager.instance.RandomizePlayFx(fxFlapWings);
    }
}
