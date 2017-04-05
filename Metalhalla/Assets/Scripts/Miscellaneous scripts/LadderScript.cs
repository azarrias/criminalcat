using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour {

	private void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerStatus>().climbLadderAvailable = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerStatus>().climbLadderAvailable = false;
        }
    }
}
