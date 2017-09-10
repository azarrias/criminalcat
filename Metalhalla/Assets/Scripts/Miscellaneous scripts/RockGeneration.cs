using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockGeneration : MonoBehaviour {

    RockFall rockFallScript = null;
    RockFallEndGame rockFallEndGameScript = null;

	// Use this for initialization
	void Start () {
        rockFallScript = transform.parent.GetComponent<RockFall>();
        if(rockFallScript == null)
            rockFallEndGameScript = transform.parent.GetComponent<RockFallEndGame>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (rockFallScript != null)
                rockFallScript.generateRocks = true;
            else if (rockFallEndGameScript != null)
                rockFallEndGameScript.StartRockFall();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (rockFallScript != null)
                rockFallScript.generateRocks = false;
        }
    }
}
