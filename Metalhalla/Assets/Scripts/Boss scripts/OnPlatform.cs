using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlatform : MonoBehaviour {

    BossController bossController;
	// Use this for initialization

    void Awake()
    {
        bossController = FindObjectOfType<BossController>();
    }

	void Start () {
          
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            bossController.GetFSMBoss().playerReachable = false;
        }
    }
}
