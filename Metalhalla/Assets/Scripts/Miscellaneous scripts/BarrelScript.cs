using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelScript : MonoBehaviour {

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerStatus>().beerRefillAvailable = true;
            //collision.GetComponent<PlayerStatus>().activeRespawnPoint = transform.position + Vector3.up;
            collision.GetComponent<PlayerStatus>().activeRespawnPoint = transform.position + Vector3.up;
            Vector3 respawnPoint;
            respawnPoint = transform.position + Vector3.up;
            respawnPoint.z = collision.transform.position.z;
            collision.GetComponent<PlayerStatus>().activeRespawnPoint = respawnPoint;
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
