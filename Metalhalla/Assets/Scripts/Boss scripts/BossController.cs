using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossController : MonoBehaviour {

    private FSMBoss fsmBoss = null;
    private GameObject theBoss = null;
    private BossStats bossStats = null;
    public GameObject spikesCastingSpot = null;
    public GameObject spikesReturnSpot = null;
    public float spikesAttackBossDepth = 1.5f;

    void Awake()
    {
        theBoss = GameObject.FindGameObjectWithTag("Boss");
        if (theBoss == null)
            Debug.Log("Error: boss game object not found.");

        bossStats = theBoss.GetComponent<BossStats>();
        if (bossStats == null)
            Debug.Log("Error: BossStats script not found.");
    }
		
	// Use this for initialization
	void Start ()
    {
		fsmBoss = new FSMBoss (this);
        spikesCastingSpot.transform.position = theBoss.transform.position + Vector3.forward * spikesAttackBossDepth;
        spikesReturnSpot.transform.position = theBoss.transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void FixedUpdate()
    {
      fsmBoss.Update();
    }

    //------------------------------------ GETTERS ----------------------------------------

    public GameObject GetTheBoss()
    {
        return theBoss;
    }

    public FSMBoss GetFSMBoss()
    {
        return fsmBoss;
    }

    public BossStats GetBossStats()
    {
        return bossStats;
    }

}
