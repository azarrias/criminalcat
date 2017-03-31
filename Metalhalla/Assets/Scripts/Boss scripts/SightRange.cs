using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightRange : MonoBehaviour {

    BossController bossController;

    void Awake()
    {    
        bossController = FindObjectOfType<BossController>();
        if (bossController == null)
            Debug.Log("bossController not found");
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
            Vector3 bossPos = bossController.GetTheBoss().transform.position;

            float diff = playerPos.x - bossPos.x;
            if (diff > 0)
            {
                if (bossController.GetFSMBoss().facingRight == false)
                {
                    bossController.GetFSMBoss().facingRight = true;
                    //flip the boss
                    Vector3 scale = bossController.GetTheBoss().transform.localScale;
                    scale.x *= -1;
                    bossController.GetTheBoss().transform.localScale = scale;
                }

            }
            if (diff < 0)
            {
                if (bossController.GetFSMBoss().facingRight == true)
                {
                    bossController.GetFSMBoss().facingRight = false;
                    //flip the boss
                    Vector3 scale = bossController.GetTheBoss().transform.localScale;
                    scale.x *= -1;
                    bossController.GetTheBoss().transform.localScale = scale;
                }
            }


        }

    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
            bossController.GetFSMBoss().playerInSight = false;
    }
}
