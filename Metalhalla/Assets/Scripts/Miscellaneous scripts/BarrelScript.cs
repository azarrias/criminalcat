using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelScript : MonoBehaviour {

    private Vector3 respawnPoint;
    private CameraManager cameraManagerScript;

    private void Start()
    {
        respawnPoint = transform.position + Vector3.up;
        cameraManagerScript = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            respawnPoint.z = collision.transform.position.z;
            if (collision.GetComponent<PlayerStatus>().SetRespawnPoint(respawnPoint) == true)
                cameraManagerScript.ShowCheckpointLabel();

        }
    }
}
