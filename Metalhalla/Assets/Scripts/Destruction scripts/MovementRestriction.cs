using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementRestriction : MonoBehaviour {

    float constraintDuration = 1.0f;
    float timeCounter = 0.0f;

	// Use this for initialization
	void Start () {

        foreach (Transform fragment in gameObject.GetComponentInChildren<Transform>())
        {
            fragment.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ; //| RigidbodyConstraints.FreezePositionX ;
        }
    }
	
	// Update is called once per frame
	void Update () {

        timeCounter += Time.deltaTime;
        if (timeCounter >= constraintDuration)
        {
            foreach (Transform fragment in gameObject.GetComponentInChildren<Transform>())
            {
                fragment.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
            }
        }
    }
}
