using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLimits : MonoBehaviour {

    FSMBoss fsmBoss;

    void Awake()
    {
        fsmBoss = FindObjectOfType<FSMBoss>();
        if (fsmBoss == null)
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
            fsmBoss.facingRight = !fsmBoss.facingRight;
            //flip the boss
            Vector3 scale = fsmBoss.transform.localScale;
            scale.x *= -1;
            fsmBoss.transform.localScale = scale;
        }


    }
}
