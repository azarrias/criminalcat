using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaftormStayTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.transform.parent = transform.parent;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.transform.parent = null; 
        }
    }
}
