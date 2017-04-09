using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRange : MonoBehaviour {

    private FSMBoss fsmBoss;

    void Awake()
    {
        fsmBoss = FindObjectOfType<FSMBoss>();
        if (fsmBoss == null)
            Debug.LogError("fsmBoss not found");
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
            fsmBoss.atBallRange = true;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
            fsmBoss.atBallRange = false;
    }
}
