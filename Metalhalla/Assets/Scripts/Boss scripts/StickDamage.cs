using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickDamage : MonoBehaviour {

    private FSMBoss fsmBoss;

    void Awake()
    {
        fsmBoss = GameObject.FindGameObjectWithTag("Boss").GetComponent<FSMBoss>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.name == "Player")
        {
            fsmBoss.playerHit = true;
        }
    }
}
