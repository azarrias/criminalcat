using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateRockGenerator : MonoBehaviour {

    RockFall rockGeneratorScript = null;

    // Use this for initialization
    void Start () {
        rockGeneratorScript = transform.parent.GetComponent<RockFall>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
            rockGeneratorScript.active = false;
    }
}
