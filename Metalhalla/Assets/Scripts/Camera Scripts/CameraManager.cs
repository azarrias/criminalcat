using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class CameraManager : MonoBehaviour {

    [Header("Cameras")]
    [Tooltip("Add here the camera that follows the player")]
    public Camera playerCamera;
    [Tooltip("Camera that will be responsible for rendering the map")]
    public Camera mapCamera;

    [Header("Labels")]
    [Tooltip("Labels to be managed in CameraCanvas - i.e. checkpoint label")]
    public Text checkpointLabel;

    [Header("Timings")]
    public float showCheckpointTime = 2.0f;
    float showCheckpointTimeAccum; 
    bool showCheckpoint = false; 

    void Start ()
    {
        HideCheckpointLabel();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (showCheckpoint)
        {
            showCheckpointTimeAccum += Time.deltaTime;
            if (showCheckpointTimeAccum >= showCheckpointTime)
                HideCheckpointLabel();
        }
    }

    public void ShowCheckpointLabel()
    {
        showCheckpoint = true;
        checkpointLabel.enabled = true;
        showCheckpointTimeAccum = 0.0f;
    }

    void HideCheckpointLabel()
    {
        showCheckpoint = false;
        checkpointLabel.enabled = false;
    }
}
