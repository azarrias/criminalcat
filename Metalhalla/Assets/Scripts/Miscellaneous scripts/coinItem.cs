using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinItem : MonoBehaviour {

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            // add score / items to player
            Destroy(this.gameObject);
        }
    }
}
