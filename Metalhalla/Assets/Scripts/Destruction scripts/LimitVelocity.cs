using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitVelocity : MonoBehaviour {

    public float maxSpeed = 10.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        foreach (Transform fragment in gameObject.GetComponentInChildren<Transform>())
        {
            Vector3 controlledVelocity = fragment.GetComponent<Rigidbody>().velocity;
            controlledVelocity.y = Mathf.Clamp(fragment.GetComponent<Rigidbody>().velocity.y, -maxSpeed * 10, maxSpeed);
            fragment.GetComponent<Rigidbody>().velocity = controlledVelocity;
        }
    }
}
