using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafePointLeft : MonoBehaviour {

    private IceSpikesBehaviour iceSpikesScript = null;

    // Use this for initialization
    void Start()
    {
        iceSpikesScript = FindObjectOfType<IceSpikesBehaviour>();
        if (iceSpikesScript == null)
            Debug.LogError("Error: iceSpikesScript not found.");
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("player"))
            iceSpikesScript.playerOnLeft = true;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("player"))
            iceSpikesScript.playerOnLeft = false;
    }
}
