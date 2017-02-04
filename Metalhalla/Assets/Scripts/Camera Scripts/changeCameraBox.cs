using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeCameraBox : MonoBehaviour {

    public float left = 0.0f;
    public float right = 0.0f;
    public float top = 0.0f;
    public float bottom = 0.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        { 
            CameraFollow camFollow = FindObjectOfType<Camera>().GetComponent<CameraFollow>();
            if (camFollow == null)
                Debug.Log("not working");
            camFollow.SetLimits(left, right, top, bottom);
        }
    }
}
