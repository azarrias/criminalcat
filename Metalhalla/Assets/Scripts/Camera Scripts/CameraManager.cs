using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class CameraManager : MonoBehaviour {

    [Header("Cameras to manage")]
    [Tooltip("Add here all the cameras that will be managed from this GameObject")]
    public Camera[] cameras;

    [Tooltip("Label that will show when the i-th camera is active")]
    public string[] cameraLabels;

    [Tooltip("Index of the camera that will be loaded in the scene the first.")]
    public int defaultCameraIndex = 0;
    private int currentCameraIndex;
    private BlurOptimized blur = null;

    private Text label;
//    private Text
    // Use this for initialization
    void Start () {
        label = GetComponentInChildren<Text>();
      //  label.material.color = Color.red;
        currentCameraIndex = defaultCameraIndex;
        ShiftCameras();
        blur = cameras[currentCameraIndex].GetComponent<BlurOptimized>() as BlurOptimized;
        blur.enabled = false;
        cameras[8].gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C) == true || Input.GetButtonDown("ShiftCameras"))
        {
            ShiftCameraIndex();
            ShiftCameras();
        }
        else if (Input.GetKeyDown(KeyCode.M) == true)
        {
            blur.enabled = true;
            cameras[8].gameObject.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.M) == true)
        {
            blur.enabled = false;
            cameras[8].gameObject.SetActive(false);
        }

       // label.material.color = Color.red;
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
        label.text = cameraLabels[currentCameraIndex];
    }

}
