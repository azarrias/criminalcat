using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaRight : MonoBehaviour {

    [HideInInspector]
    public bool isOnRightSafeArea = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            isOnRightSafeArea = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            isOnRightSafeArea = false;
        }
    }
}
