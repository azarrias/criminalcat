using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<myPlayerStatus>().climbLadderAvailable = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<myPlayerStatus>().climbLadderAvailable = false;
        }
    }
}
