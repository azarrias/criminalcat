using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossController : MonoBehaviour {

    private FSMBoss fsmBoss = null;
    private GameObject theBoss = null;
    private BossStats bossStats = null;
    public GameObject spikesCastingSpot = null;
    public GameObject spikesReturnSpot = null;
    private GameObject thePlayer = null;
    private Animator bossAnimator = null;
    public float spikesAttackBossDepth = 1.5f;
    public float detectionHeight = 3.0f;

    void Awake()
    {
        theBoss = GameObject.FindGameObjectWithTag("Boss");
        if (theBoss == null)
            Debug.LogError("Error: boss game object not found.");

        bossStats = theBoss.GetComponent<BossStats>();
        if (bossStats == null)
            Debug.LogError("Error: BossStats script not found.");

        thePlayer = GameObject.FindGameObjectWithTag("Player");
        if (thePlayer == null)
            Debug.LogError("Error: player not found.");

        bossAnimator = theBoss.GetComponent<Animator>();
        if (bossAnimator == null)
            Debug.LogError("Error: animator not found.");
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
  
    public void DamageBoss(int damage)
    {
        fsmBoss.DamageBoss(damage);
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

    public GameObject GetThePlayer()
    {
        return thePlayer;
    }

    public Animator GetBossAnimator()
    {
        return bossAnimator;
    }

}
