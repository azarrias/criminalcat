using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBehaviour : MonoBehaviour {

    [Header("Sound Effects")]
    public AudioClip[] fxFlapWings;
    private bool onScreen = false;
    private CameraManager cameraManager;
    public Camera playerCamera;

    private void Awake()
    {
        GameObject camGO = GameObject.FindGameObjectWithTag("CameraManager");
        cameraManager = camGO.GetComponent<CameraManager>();
        playerCamera = cameraManager.playerCamera;
        Animator anim = GetComponent<Animator>();
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
    }

    private void Update()
    {
        Vector3 viewPos = playerCamera.WorldToViewportPoint(transform.position);
        if (viewPos.x > 0.0f && viewPos.x < 1.0f && viewPos.y > 0.0f && viewPos.y < 1.0f
            && viewPos.z > 0.0f)
            onScreen = true;
        else onScreen = false;
    }

    public void FlapWings()
    {
        if (onScreen)
            AudioManager.instance.RandomizePlayFx(fxFlapWings);
    }
}
