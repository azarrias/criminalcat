using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightRange : MonoBehaviour {

    FSMBoss fsmBoss;

    void Awake()
    {
        fsmBoss = FindObjectOfType<FSMBoss>();
        if (fsmBoss == null)
            Debug.LogError("fsmBoss not found");
    }

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
            fsmBoss.playerInSight = true;

            Vector3 playerPos = collider.gameObject.transform.position;
            Vector3 bossPos = fsmBoss.transform.position;

            float diff = playerPos.x - bossPos.x;
            if (diff > 0)
            {
                if (fsmBoss.facingRight == false)
                {
                    fsmBoss.Flip();
                }
            }
            if (diff < 0)
            {
                if (fsmBoss.facingRight == true)
                {
                    fsmBoss.Flip();
                }
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
            fsmBoss.playerInSight = false;

        if (collider.CompareTag("Boss"))
        {
            fsmBoss.Flip();
        }
    }  
}
