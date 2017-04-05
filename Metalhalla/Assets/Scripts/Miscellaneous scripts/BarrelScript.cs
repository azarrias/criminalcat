using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelScript : MonoBehaviour {

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerStatus>().beerRefillAvailable  = true;
            // TODO: add code to save this position as a spawning point
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerStatus>().beerRefillAvailable = false;
        }
    }
}
