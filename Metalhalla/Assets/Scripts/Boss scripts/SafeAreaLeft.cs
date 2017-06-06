using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaLeft : MonoBehaviour {

    [HideInInspector]
    public bool isOnLeftSafeArea = false;

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
            isOnLeftSafeArea = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            isOnLeftSafeArea = false;
        }
    }
}
