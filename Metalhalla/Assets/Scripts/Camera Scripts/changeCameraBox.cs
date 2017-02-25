using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeCameraBox : MonoBehaviour {

    public float left = 0.0f;
    public float right = 0.0f;
    public float top = 0.0f;
    public float bottom = 0.0f;

    public string cameraTagToChange = "Main Camera";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Camera[] cameraList = FindObjectsOfType<Camera>();
            foreach( Camera cam in cameraList)
            {
                if (cam.tag == cameraTagToChange)
                {
                    CameraFollow camFollow = cam.GetComponent<CameraFollow>();
                    if (camFollow != null)
                        camFollow.SetLimits(left, right, top, bottom);
                }
            }
        }
    }
}
