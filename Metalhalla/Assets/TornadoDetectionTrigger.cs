using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoDetectionTrigger : MonoBehaviour {

    private LiftablePlatform platform;

    private void Start()
    {
        platform = transform.parent.GetComponent<LiftablePlatform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tornado")
                platform.animatePlatform = true;
    }
}
