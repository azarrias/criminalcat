using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLimits : MonoBehaviour {

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

    void OnTriggerEnter(Collider col)
    {
        bossController.GetFSMBoss().facingRight = !bossController.GetFSMBoss().facingRight;
    }
}
