using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRange : MonoBehaviour {

    BossController bossController;
    
    void Awake()
    {
        bossController = FindObjectOfType<BossController>();
        if (bossController == null)
            Debug.LogError("bossController not found");
  
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
            bossController.GetFSMBoss().atMeleeRange = true;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
            bossController.GetFSMBoss().atMeleeRange = false;
    }
}
