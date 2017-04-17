using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafePointsTrigger : MonoBehaviour {

    private IceSpikesBehaviour iceSpikesScript = null;
    private GameObject thePlayer = null;

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

    void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (iceSpikesScript.rightSafe == true && gameObject.name == "SafePointRight")        
                iceSpikesScript.isPlayerSafe = true;
                        
            if (iceSpikesScript.leftSafe == true && gameObject.name == "SafePointLeft")
                iceSpikesScript.isPlayerSafe = true;
        }
    }

}
