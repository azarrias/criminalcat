using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlatform : MonoBehaviour {

    FSMBoss fsmBoss;
	// Use this for initialization

    void Awake()
    {
        fsmBoss = FindObjectOfType<FSMBoss>();
    }

	void Start () {
          
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    
    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            fsmBoss.playerReachable = false;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            fsmBoss.playerReachable = true;
        }
    }
}
