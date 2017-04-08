using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightRange : MonoBehaviour {

    BossController bossController;

    void Awake()
    {    
        bossController = FindObjectOfType<BossController>();
        if (bossController == null)
            Debug.LogError("bossController not found");
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
            bossController.GetFSMBoss().playerInSight = true;

            Vector3 playerPos = collider.gameObject.transform.position;
            Vector3 bossPos = bossController.GetTheBossController().transform.position;

            float diff = playerPos.x - bossPos.x;
            if (diff > 0)
            {
                if (bossController.GetFSMBoss().facingRight == false)
                {                   
                    bossController.GetFSMBoss().Flip();
                }
            }
            if (diff < 0)
            {
                if (bossController.GetFSMBoss().facingRight == true)
                {
                    bossController.GetFSMBoss().Flip();
                }
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
            bossController.GetFSMBoss().playerInSight = false;

        if (collider.CompareTag("BossCollider"))
        {
            bossController.GetFSMBoss().Flip();

        }
    }  
}
