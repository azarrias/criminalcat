using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour {

    public Quaternion initialRotation = Quaternion.identity;
    public float initialHeight = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        //Save local rotation to return to it when the tornado vanishes
        if(collider.gameObject.CompareTag("Tornado"))
        {
            initialRotation = transform.localRotation;
            initialHeight = transform.position.y;
        }
    }
}
