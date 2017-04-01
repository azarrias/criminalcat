using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLimits : MonoBehaviour {

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
        if (collider.CompareTag("Boss"))
        {
            bossController.GetFSMBoss().facingRight = !bossController.GetFSMBoss().facingRight;
            //flip the boss
            Vector3 scale = bossController.GetTheBoss().transform.localScale;
            scale.x *= -1;
            bossController.GetTheBoss().transform.localScale = scale;

        }


    }
}
