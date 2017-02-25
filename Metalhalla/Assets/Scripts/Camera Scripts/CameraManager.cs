using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    [Header("Cameras to manage")]
    [Tooltip("Add here all the cameras that will be managed from this GameObject")]
    public Camera[] cameras;

    [Tooltip("Index of the camera that will be loaded in the scene the first.")]
    public int defaultCameraIndex = 0;
    private int currentCameraIndex; 

	// Use this for initialization
	void Start () {
        currentCameraIndex = defaultCameraIndex;
        ShiftCameras();

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C) == true)
        {
            ShiftCameraIndex();
            ShiftCameras();
        }
	}


    // get next gamera in array and set it active
    void ShiftCameraIndex()
    {
        currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
    }

    void ShiftCameras()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (i == currentCameraIndex)
                cameras[i].gameObject.SetActive(true);
            else
                cameras[i].gameObject.SetActive(false);
        }
    }

}
