using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNextSceneOnPlayerDeath : MonoBehaviour {

    public string nextSceneOnDeath = "Credits";


    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerStatus>().SetNewSceneOnDeath(nextSceneOnDeath);
        }
    }
}
