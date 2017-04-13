using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FSMBoss))]
[RequireComponent(typeof(BossStats))]
[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour {

    private FSMBoss fsmBoss = null;
    private GameObject theBossController = null;
    private BossStats bossStats = null;
    public GameObject spikesCastingSpot = null;
    public GameObject spikesReturnSpot = null;
    private GameObject thePlayer = null;
    private Animator bossAnimator = null;
    [Tooltip("Depth of casting point")]
    public float spikesAttackBossDepth = 1.5f;
    public float detectionHeight = 3.0f;

    //ball attack prefab
    public GameObject fireBallPrefab = null;

    public int patrolFrames = 20;
    public int chaseFrames = 20;
    public int meleeFrames = 20;
    public int ballAttackFrames = 20;
    public int prepareCastFrames = 20;
    public int castFrames = 20;
    public int backToCenterFrames = 20;
    public int stalkFrames = 20;
    [Tooltip("Time until body disappears")]
    public float deadTime = 4.0f;
    [Tooltip("Melee animation duration")]
    public float meleeAttackDuration = 1.0f;
    [Tooltip("Ball attack animation duration")]
    public float ballAttackDuration = 2.0f;

    void Awake()
    {        
        theBossController = gameObject;

        fsmBoss = theBossController.GetComponent<FSMBoss>();       
        bossStats = theBossController.GetComponent<BossStats>();
        bossAnimator = theBossController.GetComponent<Animator>();

        thePlayer = GameObject.FindGameObjectWithTag("Player");
        if (thePlayer == null)
            Debug.LogError("Error: player not found.");
    }
		
	// Use this for initialization
	void Start ()
    {
        //fsmBoss = FSMBoss.CreateInstance(this);
        spikesCastingSpot.transform.position = theBossController.transform.position + Vector3.forward * spikesAttackBossDepth;
        spikesReturnSpot.transform.position = theBossController.transform.position;
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
        fsmBoss.ApplyDamage(damage);
    }

    //------------------------------------ GETTERS ----------------------------------------

    public GameObject GetTheBossController()
    {
        return theBossController;
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
