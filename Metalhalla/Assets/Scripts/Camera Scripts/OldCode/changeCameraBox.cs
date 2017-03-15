using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeCameraBox : MonoBehaviour {

    public float left = 0.0f;
    public float right = 0.0f;
    public float top = 0.0f;
    public float bottom = 0.0f;
    [Range(0.0f, 100.0f)]
    public float newPlayerToScreenHeightRatio = 15f;

    public string cameraTagToChange = "MainCamera";

    public void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
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
                    {
                        camFollow.SetPlayerToScreenHeightRatio(newPlayerToScreenHeightRatio);
                        camFollow.SetLimits(left, right, top, bottom);
                    }
                    
                }
            }
        }
    }
}
