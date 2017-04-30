using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDeath : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // add score / items to player
            other.gameObject.SendMessage("ApplyDamage", other.gameObject.GetComponent<PlayerStatus>().GetCurrentHealth()); 

        }
    }
}
