using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraParametersTrigger : MonoBehaviour {

    public string cameraTagToChange = "MainCamera";
    [SerializeField]
    private Vector3 frameCenter;
    [SerializeField]
    private Vector3 frameExtents;

    void Start () {
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            Camera[] cameraList = FindObjectsOfType<Camera>();
            foreach (Camera cam in cameraList)
            {
                if (cam.tag == cameraTagToChange)
                {
                    CameraParametersConfigurator configurator = cam.GetComponent<CameraParametersConfigurator>();
                    if (configurator != null)
                    {

                        configurator.ConfigureCamera(frameCenter, frameExtents);
                    }

                }
            }
        }
        if (collision.tag == "Framing" )  // only called once because neither move
        {
            frameCenter = collision.GetComponent<Renderer>().bounds.center;
            frameExtents = collision.GetComponent<Renderer>().bounds.extents;
        }
    }
}
