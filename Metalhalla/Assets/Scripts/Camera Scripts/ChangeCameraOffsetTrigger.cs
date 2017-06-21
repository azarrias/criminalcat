using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraOffsetTrigger : MonoBehaviour {

    public string cameraTagToChange = "MainCamera";
    public Vector3 offsetToApply;
    private CameraFollow camFollow; 

    void Start()
    {
        GetComponent<Renderer>().enabled = false;
        camFollow = GameObject.FindWithTag("MainCamera").GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
            camFollow.ChangeCameraOffset(offsetToApply);
            //camFollow.cameraOffset = offsetToApply;
    }
}
