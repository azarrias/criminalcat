using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafePointsTrigger : MonoBehaviour {

    private IceSpikesBehaviour iceSpikesScript = null;
    private GameObject thePlayer = null;
    private bool isOnRightCollider = false;
    private bool isOnLeftCollider = false;

    // Use this for initialization
    void Start()
    {
        iceSpikesScript = transform.parent.GetComponent<IceSpikesBehaviour>();
        if (iceSpikesScript == null)
            Debug.LogError("Error: iceSpikesScript not found."); 
    }

    // Update is called once per frame
    void Update () {
       
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (gameObject.name == "SafePointRight")
                isOnRightCollider = true;

            if (gameObject.name == "SafePointLeft")
                isOnLeftCollider = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (gameObject.name == "SafePointRight")
                isOnRightCollider = false;

            if (gameObject.name == "SafePointLeft")
                isOnLeftCollider = false;
        }
    }
}
