using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    void Awake()
    {        
        theBossController = gameObject;
        
        bossStats = theBossController.GetComponent<BossStats>();
        if (bossStats == null)
            Debug.LogError("Error: BossStats script not found.");

        thePlayer = GameObject.FindGameObjectWithTag("Player");
        if (thePlayer == null)
            Debug.LogError("Error: player not found.");

        bossAnimator = theBossController.GetComponent<Animator>();
        if (bossAnimator == null)
            Debug.LogError("Error: animator not found.");
    }
		
	// Use this for initialization
	void Start ()
    {
        fsmBoss = FSMBoss.CreateInstance(this);
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
        fsmBoss.DamageBoss(damage);
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
