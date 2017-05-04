using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockGeneration : MonoBehaviour {

    RockFall rockFallScript = null;

	// Use this for initialization
	void Start () {
        rockFallScript = transform.parent.GetComponent<RockFall>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
            rockFallScript.generateRocks = true;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
            rockFallScript.generateRocks = false;
    }
}
