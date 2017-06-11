using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BossStats))]
[RequireComponent(typeof(Animator))]
public class FSMBoss : MonoBehaviour
{
    public enum State
    {
        START,
        DEAD,
        PATROL,
        CHASE,
        PRE_MELEE_ATTACK,
        MELEE_ATTACK,
        POST_MELEE_ATTACK,
        PRE_BALL_ATTACK,
        BALL_ATTACK,
        POST_BALL_ATTACK,
        PREPARE_CAST,
        CAST_ICE_SPIKES,
        BACK_TO_CENTER,
        STALK,
        INSIDE_TORNADO,
        WAIT
    }
 
    private BossStats bossStats = null;
    public GameObject spikesCastingSpot = null;
    public GameObject spikesReturnSpot = null;
    private GameObject castingArea = null;
    public GameObject meleeAttackIndicator = null;
    public GameObject ballAttackIndicator = null;
    public GameObject fireAura = null;
    private ParticleSystem meleeAttackIndicatorPS = null;
    private ParticleSystem ballAttackIndicatorPS = null;
    private ParticleSystem fireAuraPS = null;
    private FireAuraDamage fireAuraDamageScript = null;

    [Tooltip("Depth of casting point")]
    public float spikesAttackBossDepth = 1.5f;
    public float detectionHeight = 3.0f;

    [Tooltip("Time until body disappears")]
    public float deadTime = 3.0f;

    [Tooltip("PreMelee animation duration")]
    public float preMeleeAttackDuration = 3.0f;
    private float preMeleeCounter = 0.0f;
    
    [Tooltip("Melee animation duration")]
    public float meleeAttackDuration = 3.0f;
    private float meleeCounter = 0.0f;
    
    [Tooltip("PostMelee animation duration")]
    public float postMeleeAttackDuration = 3.0f;
    private float postMeleeCounter = 0.0f;

    [Tooltip("PreBall attack animation duration")]
    public float preBallAttackDuration = 3.0f;
    private float preBallCounter = 0.0f;

    [Tooltip("Ball attack animation duration")]
    public float ballAttackDuration = 3.0f;
    private float ballCounter = 0.0f;

    [Tooltip("Ball attack animation duration")]
    public float postBallAttackDuration = 3.0f;
    private float postBallcounter = 0.0f;

    [Tooltip("Cast spikes animation duration")]
    public float castSpikesDuration = 3.0f;
    private float castSpikesCounter = 0.0f;

    //ice spikes attack animator
    IceSpikesBehaviour iceSpikesScript = null;
    IceSpikesBehaviour3D iceSpikesScript3D = null;

    private State currState = State.START;
    private State prevState = State.START;
    private GameObject thePlayer = null;
    private PlayerStatus thePlayerStatus = null;
    private Animator bossAnimator = null;

    //------------------- VARIABLES THAT CONTROL FSM LOGIC -------------------
    [HideInInspector]
    public bool playerInSight = false;
    [HideInInspector]
    public bool atMeleeRange = false;
    [HideInInspector]
    public bool playerHit = false; //the collider attached to the stick will notify this boolean when colliding with the player
    [HideInInspector]
    public bool atBallRange = false;
    [HideInInspector]
    public bool playerReachable = true;
  
    private bool preMeleeAttackSelected = true; //starting attack
    private bool preMeleeAttackFinished = false;

    private bool meleeAttackFinished =  false;
    private bool postMeleeAttackFinished = false;

    private bool preBallAttackSelected = false;
    private bool preBallAttackFinished = false;

    private bool ballAttackFinished = false;
    private bool postBallAttackFinished = false;

    private bool prepareCast = false;
    private bool castIceSpikes = true;
    private bool backToCenter = true;
    private bool insideTornado = false;
    private float tornadoDuration = 0.0f;
    
    //Attack Damage
    int meleeDamage = 0;

    //orientation of the boss will be modified when entering triggers at the limits of the platform and when player is around
    [HideInInspector]
    public bool facingRight = true;
    private int numSpikeAttacks = 0;

    //interpolation factor to go to spikes casting point and to return from it
    private float lerpPosThreshold = 0.2f;
    private float castingPosSpeed = 1.5f;
    //Ball attack allowed at HP <= 75% maxHP
    private float thresholdBallAttack = 0.75f;
    //first ice spikes at 50% maxHP
    private float thresholdFirstSpikes = 0.50f;
    //second ice spikes at 25% maxHP
    private float thresholdSecondSpikes = 0.25f;

    private string currAnimation = "Patrol"; //start animation


    // ----------------------   COUNTERS TO ALLOW ANIMATION TRANSITIONS ----
    [Tooltip("Minimun number of frames to stay in Patrol state before transition")]
    public int patrolFrames = 20;
    [Tooltip("Minimun number of frames to stay in Chase state before transition")]
    public int chaseFrames = 20;

    [Tooltip("Minimun number of frames to stay in Pre-Melee Attack state before transition")]
    public int preMeleeFrames = 20;
    [Tooltip("Minimun number of frames to stay in Melee Attack state before transition")]
    public int meleeFrames = 20;
    [Tooltip("Minimun number of frames to stay in Post-Melee Attack state before transition")]
    public int postMeleeFrames = 20;

    [Tooltip("Minimun number of frames to stay in Pre-Ball state before transition")]
    public int preBallFrames = 20;
    [Tooltip("Minimun number of frames to stay in Ball Attack state before transition")]
    public int ballAttackFrames = 20;
    [Tooltip("Minimun number of frames to stay in Post-Ball Attack state before transition")]
    public int postBallFrames = 20;

    [Tooltip("Minimun number of frames to stay in Prepare Cast state before transition")]
    public int prepareCastFrames = 20;
    [Tooltip("Minimun number of frames to stay in Cast Ice Spikes state before transition")]
    public int castFrames = 20;
    [Tooltip("Minimun number of frames to stay in Back To Center state before transition")]
    public int backToCenterFrames = 20;

    [Tooltip("Minimun number of frames to stay in Stalk state before transition")]
    public int stalkFrames = 20;
    [Tooltip("Minimun number of frames to stay in Inside Tornado state before transition")]
    public int insideTornadoFrames = 20;

    private int patrolFrameCounter = 0;
    private int chaseFrameCounter = 0;

    private int preMeleeFrameCounter = 0;
    private int meleeAttackFrameCounter = 0;
    private int postMeleeFrameCounter = 0;

    private int preBallFrameCounter = 0;
    private int ballAttackFrameCounter = 0;
    private int postBallAttackFrameCounter = 0;

    private int prepareCastFrameCounter = 0;
    private int castTimeFrameCounter = 0;
    private int backToCenterFrameCounter = 0;
    private int stalkFrameCounter = 0;
    private int insideTornadoFrameCounter = 0;
    
    
    void Awake()
    {   
        currState = State.PATROL;

        thePlayer = GameObject.FindGameObjectWithTag("Player");       
        thePlayerStatus = thePlayer.GetComponent<PlayerStatus>();       
        bossAnimator = GetComponent<Animator>();
        bossStats = GetComponent<BossStats>();
        meleeDamage = bossStats.meleeDamage;
        
        iceSpikesScript = FindObjectOfType<IceSpikesBehaviour>();
        if (iceSpikesScript == null)
        {
            iceSpikesScript3D = FindObjectOfType<IceSpikesBehaviour3D>();
            if(iceSpikesScript3D == null)
                Debug.LogError("Error: iceSpikesScript not found.");
        }

        castingArea = GameObject.FindGameObjectWithTag("CastingArea");


        meleeAttackIndicatorPS = meleeAttackIndicator.GetComponent<ParticleSystem>();
        meleeAttackIndicatorPS.Stop();
        ballAttackIndicatorPS = ballAttackIndicator.GetComponent<ParticleSystem>();
        ballAttackIndicatorPS.Stop();
        fireAuraPS = fireAura.GetComponent<ParticleSystem>();
        fireAuraPS.Stop();
        fireAuraDamageScript = fireAura.GetComponent<FireAuraDamage>();

    }

    void Start()
    {
        spikesCastingSpot.transform.position = gameObject.transform.position + Vector3.forward * spikesAttackBossDepth;
        spikesReturnSpot.transform.position = gameObject.transform.position;
        castingArea.transform.position = spikesReturnSpot.transform.position;
    }


    public void Update()
    {
        //DEBUG
        //Debug.Log("state:" + currState + "  animation:" + currAnimation + " " + "  facingRight:" + facingRight + " localRot:" + gameObject.transform.localRotation.eulerAngles);

        switch (currState)
        {
            case State.PATROL:
                Patrol();
                if (playerInSight)
                {
                    patrolFrameCounter++;
                    if (patrolFrameCounter == patrolFrames)
                    {
                        patrolFrameCounter = 0;
                        currState = State.CHASE;                       
                        break;
                    }
                }
                break;

            case State.CHASE:
                if (bossStats.hitPoints <= 0)
                {
                    chaseFrameCounter++;
                    if (chaseFrameCounter == chaseFrames)
                    {
                        chaseFrameCounter = 0;
                        currState = State.DEAD;
                    }

                    break;
                }
                //Player dead
                if (!thePlayerStatus.IsAlive())
                {
                    if (prevState == State.BACK_TO_CENTER)
                    {
                        gameObject.transform.localRotation *= Quaternion.Euler(0, 270, 0);
                        if (!facingRight)
                            facingRight = true;
                    }

                    currState = State.PATROL;
                    playerInSight = false;
                    break;
                }
                Chase();
                if (thePlayerStatus.IsAlive())
                {
                    if (preMeleeAttackSelected && atMeleeRange)
                    {
                        chaseFrameCounter++;
                        if (chaseFrameCounter == chaseFrames)
                        {
                            currState = State.PRE_MELEE_ATTACK;
                            chaseFrameCounter = 0;
                            preMeleeAttackSelected = false; //reset value
                        }
                        break;
                    }
                    if (preBallAttackSelected && atBallRange)
                    {
                        chaseFrameCounter++;
                        if (chaseFrameCounter == chaseFrames)
                        {
                            currState = State.PRE_BALL_ATTACK;
                            chaseFrameCounter = 0;
                            preBallAttackSelected = false; //reset value
                        }
                        break;
                    }
                    if (prepareCast)
                    {
                        chaseFrameCounter++;
                        if (chaseFrameCounter == chaseFrames)
                        {
                            currState = State.PREPARE_CAST;
                            chaseFrameCounter = 0;
                        }
                        break;
                    }
                    if (!playerReachable)
                    {
                        chaseFrameCounter++;
                        if (chaseFrameCounter == chaseFrames)
                        {
                            currState = State.STALK;
                            chaseFrameCounter = 0;
                        }
                        break;
                    }
                    if (!playerInSight)
                    {
                        chaseFrameCounter++;
                        if (chaseFrameCounter == chaseFrames)
                        {
                            prevState = State.CHASE;
                            currState = State.PATROL;
                            chaseFrameCounter = 0;
                        }
                        break;
                    }
                    if (insideTornado)
                    {
                        chaseFrameCounter++;
                        if (chaseFrameCounter == chaseFrames)
                        {
                            currState = State.INSIDE_TORNADO;
                            chaseFrameCounter = 0;
                        }
                        break;
                    }
                }                                         
                break;
            case State.PRE_MELEE_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    preMeleeFrameCounter++;
                    if (preMeleeFrameCounter == preMeleeFrames)
                    {
                        preMeleeFrameCounter = 0;
                        currState = State.DEAD;
                    }

                    break;
                }
                PreMeleeAttack();
                if (preMeleeAttackFinished) 
                {
                    preMeleeFrameCounter++;
                    if (preMeleeFrameCounter == preMeleeFrames)
                    {                        
                        currState = State.MELEE_ATTACK;
                        preMeleeFrameCounter = 0;
                        preMeleeAttackFinished = false; //reset value                        
                    }
                    break;
                }
                if (insideTornado)
                {
                    preMeleeFrameCounter++;
                    if (preMeleeFrameCounter == preMeleeFrames)
                    {
                        currState = State.INSIDE_TORNADO;
                        preMeleeFrameCounter = 0;                        
                    }
                    break;                 
                }
                break;
             
            case State.MELEE_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    meleeAttackFrameCounter++;
                    if (preMeleeFrameCounter == meleeFrames)
                    {
                        meleeAttackFrameCounter = 0;
                        currState = State.DEAD;
                    }

                    break;
                }
                MeleeAttack();
                if (meleeAttackFinished) 
                {
                    meleeAttackFrameCounter++;
                    if (meleeAttackFrameCounter == meleeFrames)
                    {                        
                        currState = State.POST_MELEE_ATTACK;
                        meleeAttackFrameCounter = 0;
                        meleeAttackFinished = false; //reset value
                    }
                    break;
                }
                if (insideTornado)
                {
                    meleeAttackFrameCounter++;
                    if (meleeAttackFrameCounter == meleeFrames)
                    {
                        meleeAttackFrameCounter = 0;
                        currState = State.INSIDE_TORNADO;                       
                    }
                    break;
                }

                break;

            case State.POST_MELEE_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    postMeleeFrameCounter++;
                    if (postMeleeFrameCounter == postMeleeFrames)
                    {
                        postMeleeFrameCounter = 0;
                        currState = State.DEAD;
                    }

                    break;
                }
                PostMeleeAttack();
                if (postMeleeAttackFinished) //attack has finished
                {
                    postMeleeFrameCounter++;
                    if (postMeleeFrameCounter == postMeleeFrames)
                    {
                        SelectAttack();
                        currState = State.CHASE;
                        postMeleeFrameCounter = 0;
                        postMeleeAttackFinished = false; //reset value
                    }
                    break;
                }
                if (insideTornado)
                {
                    postMeleeFrameCounter++;
                    if (postMeleeFrameCounter == postMeleeFrames)
                    {
                        postMeleeFrameCounter = 0;
                        currState = State.INSIDE_TORNADO;
                    }
                    break;
                }
                break;

            case State.PRE_BALL_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    preBallFrameCounter++;
                    if (preBallFrameCounter == preBallFrames)
                    {
                        currState = State.DEAD;
                        preBallFrameCounter = 0;
                        break;
                    }
                }
                PreBallAttack();
                if (preBallAttackFinished) 
                {
                    preBallFrameCounter++;
                    if (preBallFrameCounter == preBallFrames)
                    {                       
                        currState = State.BALL_ATTACK;
                        preBallFrameCounter = 0;
                        preBallAttackFinished = false; //reset value
                        break;
                    }
                }
                if (insideTornado)
                {
                    preBallFrameCounter++;
                    if (preBallFrameCounter == preBallFrames)
                    {
                        preBallFrameCounter = 0;
                        currState = State.INSIDE_TORNADO;
                    }
                    break;
                }
                break;

            case State.BALL_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    ballAttackFrameCounter++;
                    if (ballAttackFrameCounter == ballAttackFrames)
                    {
                        currState = State.DEAD;
                        ballAttackFrameCounter = 0;
                        break;
                    }                  
                }
                BallAttack();               
                if (ballAttackFinished) //attack has finished
                {
                    ballAttackFrameCounter++;
                    if (ballAttackFrameCounter == ballAttackFrames)
                    {                      
                        currState = State.POST_BALL_ATTACK;
                        ballAttackFrameCounter = 0;
                        ballAttackFinished = false; //reset value
                        break;
                    }                  
                }
                if (insideTornado)
                {
                    ballAttackFrameCounter++;
                    if (ballAttackFrameCounter == ballAttackFrames)
                    {
                        ballAttackFrameCounter = 0;
                        currState = State.INSIDE_TORNADO;
                    }
                    break;
                }
                break;

            case State.POST_BALL_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    postBallAttackFrameCounter++;
                    if (postBallAttackFrameCounter == postBallFrames)
                    {
                        currState = State.DEAD;
                        postBallAttackFrameCounter = 0;
                        break;
                    }
                }
                PostBallAttack();
                if (postBallAttackFinished) //attack has finished
                {
                    postBallAttackFrameCounter++;
                    if (postBallAttackFrameCounter == postBallFrames)
                    {
                        SelectAttack();
                        currState = State.CHASE;
                        postBallAttackFrameCounter = 0;
                        postBallAttackFinished = false; //reset value
                        break;
                    }
                }
                if (insideTornado)
                {
                    postBallAttackFrameCounter++;
                    if (postBallAttackFrameCounter == postBallFrames)
                    {
                        postBallAttackFrameCounter = 0;
                        currState = State.INSIDE_TORNADO;
                    }
                    break;
                }
                break;

            case State.PREPARE_CAST:
                PrepareCast();
                if (!prepareCast)  //prepared finished
                {
                    prepareCastFrameCounter++;
                    if (prepareCastFrameCounter == prepareCastFrames)
                    {
                        currState = State.CAST_ICE_SPIKES;
                        prepareCastFrameCounter = 0;
                        break;
                    }
                }
                break;

            case State.CAST_ICE_SPIKES:
                CastIceSpikes();
                if (!castIceSpikes)  //cast finished
                {
                    castTimeFrameCounter++;
                    if (castTimeFrameCounter == castFrames)
                    {
                        currState = State.BACK_TO_CENTER;
                        castTimeFrameCounter = 0;
                        castIceSpikes = true; //reset value
                        break;
                    }
                }
                break;

            case State.BACK_TO_CENTER:
                BackToCenter();
                if (!backToCenter)   //back finished
                {
                    backToCenterFrameCounter++;
                    if (backToCenterFrameCounter == backToCenterFrames)
                    {
                        backToCenterFrameCounter = 0;
                        backToCenter = true;  //reset value
                        SelectAttack();
                        prevState = State.BACK_TO_CENTER;
                        currState = State.CHASE;
                        break;
                    }
                }
                break;

            case State.STALK:
                Stalk();
                if (playerReachable)
                {
                    stalkFrameCounter++;
                    if (stalkFrameCounter == stalkFrames)
                    {
                        stalkFrameCounter = 0;
                        currState = State.CHASE;
                        break;
                    }
                }
                break;

            case State.DEAD:
                Dead();
                break;

            case State.INSIDE_TORNADO:               
                InsideTornado();
                if(!insideTornado)
                {
                    insideTornadoFrameCounter++;
                    if (insideTornadoFrameCounter == insideTornadoFrames)
                    {
                        insideTornadoFrameCounter = 0;
                        preMeleeAttackSelected = false; //reset value
                        preBallAttackSelected = false; //reset value
                        if (bossStats.hitPoints <= 0)
                        {
                            currState = State.DEAD;
                        }
                        else
                        {
                            SelectAttack();
                            currState = State.CHASE;
                        }
                        break;
                    }
                }
                break;
        }

    }
    //Method called by the FSM
    private void SelectAttack()
    {
        if (bossStats.hitPoints > thresholdBallAttack * bossStats.maxHitPoints)
        {
            preMeleeAttackSelected = true;
        }
        else
        {
            if (bossStats.hitPoints <= thresholdFirstSpikes * bossStats.maxHitPoints && numSpikeAttacks == 0)
            {
                prepareCast = true;
            }

            else if (bossStats.hitPoints <= thresholdSecondSpikes * bossStats.maxHitPoints && numSpikeAttacks == 1)
            {
                prepareCast = true;
            }

            else
            {
                System.Random rand = new System.Random();
                int num = rand.Next(0, 2);
                if (num == 0)
                {
                    preMeleeAttackSelected = true;                   
                }
                if (num == 1)
                {                   
                    preBallAttackSelected = true;
                }
            }

        }

    }
    //-------------------------------- DAMAGE THE BOSS -----------------------------------------
    
    public void ApplyDamage(int damage)
    {
        if (currState == State.CHASE ||
            currState == State.PRE_MELEE_ATTACK ||
            currState == State.MELEE_ATTACK ||
            currState == State.POST_MELEE_ATTACK ||
            currState == State.PRE_BALL_ATTACK ||
            currState == State.BALL_ATTACK ||
            currState == State.POST_BALL_ATTACK || 
            currState == State.INSIDE_TORNADO)
        {
            Debug.Log("Damaged");
            bossStats.BossApplyDamage(damage);
            //create damage effect
            
        }
    }

    //---------------------------------------METHODS TO NOTIFY THE END OF THE ATTACK ANIMATIONS--------------------------------
    private void FinishPreMeleeAttackAnimation()
    {
        //Debug.Log("waiting for pre-melee attack to finish");
        preMeleeCounter += Time.deltaTime;
        if (preMeleeCounter >= preMeleeAttackDuration)
        {
            preMeleeCounter = 0.0f;
            //Debug.Log("pre-melee attack animation finished");
            preMeleeAttackFinished = true;
            
        }
    }

    private void FinishMeleeAttackAnimation()
    {
        //Debug.Log("waiting for melee attack to finish");
        meleeCounter += Time.deltaTime;
        if (meleeCounter >= meleeAttackDuration)
        {
            meleeCounter = 0.0f;
            //Debug.Log("melee attack animation finished");
            meleeAttackFinished = true;         
        }
    }

    private void FinishPostMeleeAttackAnimation()
    {
        //Debug.Log("waiting for post-melee attack to finish");
        postMeleeCounter += Time.deltaTime;
        if (postMeleeCounter >= postMeleeAttackDuration)
        {
            postMeleeCounter = 0.0f;
            //Debug.Log("post-melee attack animation finished");
            postMeleeAttackFinished = true;
        }
    }

    private void FinishPreBallAttackAnimation()
    {
        //Debug.Log("waiting for pre-ball attack to finish");
        preBallCounter += Time.deltaTime;
        if (preBallCounter >= preBallAttackDuration)
        {
            preBallCounter = 0.0f;
            //Debug.Log("pre-ball attack animation finished");
            preBallAttackFinished = true;
        }
    }

    private void FinishBallAttackAnimation()
    {
        //Debug.Log("waiting for ball attack to finish");
        ballCounter += Time.deltaTime;
        if (ballCounter >= ballAttackDuration)
        {
            ballCounter = 0.0f;
            //Debug.Log("ball attack animation finished");
            ballAttackFinished = true;
        }
    }

    private void FinishPostBallAttackAnimation()
    {
        //Debug.Log("waiting for post-ball attack to finish");
        postBallcounter += Time.deltaTime;
        if (postBallcounter >= postBallAttackDuration)
        {
            postBallcounter = 0.0f;
            //Debug.Log("post-ball attack animation finished");
            postBallAttackFinished = true;
        }
    }

    private void FinishCastIceSpikesAnimation()
    {
        //Debug.Log("waiting for ice spikes cast to finish");
        castSpikesCounter += Time.deltaTime;
        if (castSpikesCounter >= castSpikesDuration)
        {
            castSpikesCounter = 0.0f;
            //Debug.Log("ice spikes cast finished");
            castIceSpikes = false;
            numSpikeAttacks++;
        }
    }
   
    // ------------------------------------- ACTIONS TO PERFORM IN EACH STATE --------------------------------------------
    private void Dead()
    {

        //Apagar los efectos de aviso del bastón y después morir

        if (currAnimation != "Dead")
        {
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "Dead";
            bossAnimator.SetBool(currAnimation, true);
            //Destroy boss
            if (currState == State.INSIDE_TORNADO)
                deadTime += (tornadoDuration + 3.0f);
            Destroy(gameObject, deadTime);                          
        }
    }

    private void Patrol()
    {      
        if (currAnimation != "Patrol")
        {       
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "Patrol";
            bossAnimator.SetBool(currAnimation, true);                         
        }
        else
        {
            if (prevState == State.CHASE)
            {
                Flip();

                prevState = State.PATROL;
            }

            Vector3 newPos = gameObject.transform.position;

            if (facingRight == true)
                newPos.x += bossStats.normalSpeed * Time.deltaTime;
            else
                newPos.x -= bossStats.normalSpeed * Time.deltaTime;

            gameObject.transform.position = newPos;
        }       
    }

    private void Chase()
    {
        if (currAnimation != "Chase")
        {      
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "Chase";
            bossAnimator.SetBool(currAnimation, true);     
        }
        else
        {
            if (prevState == State.BACK_TO_CENTER)
            {
                prevState = State.CHASE;
                Vector3 bossPos = gameObject.transform.position;
                int diff = (int)(thePlayer.transform.position.x - bossPos.x);
              
                if (diff > 0)
                {
                    gameObject.transform.localRotation *= Quaternion.Euler(0, 270, 0);
                    if (!facingRight)
                        facingRight = true;
                }
                if (diff < 0)
                {
                    gameObject.transform.localRotation *= Quaternion.Euler(0, 90, 0);
                    if (facingRight)
                        facingRight = false;
                }
            }

            if (preMeleeAttackSelected && !atMeleeRange || preBallAttackSelected && !atBallRange)
            {
                Vector3 newPos = gameObject.transform.position;
                int diff = (int)(thePlayer.transform.position.x - newPos.x);

                if (diff > 0)
                    if (!facingRight)
                        Flip();
                if (diff < 0)
                    if (facingRight)
                        Flip();

                if (facingRight == true)
                    newPos.x += bossStats.chasingSpeed * Time.deltaTime;
                else
                    newPos.x -= bossStats.chasingSpeed * Time.deltaTime;

                gameObject.transform.position = newPos;
            }

        }     
    }

    private void PreMeleeAttack()
    {
        if(currAnimation != "PreMeleeAttack")
        {
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "PreMeleeAttack";
            bossAnimator.SetBool(currAnimation, true);

            //meleeAttackIndicator.SetActive(true);
            meleeAttackIndicatorPS.Play();        
        }

        FinishPreMeleeAttackAnimation();
    }

    private void MeleeAttack()
    {
        if (currAnimation != "MeleeAttack")
        {
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "MeleeAttack";
            bossAnimator.SetBool(currAnimation, true);          
        }
                        
        if (playerHit)
        {
            thePlayer.SendMessage("ApplyDamage", meleeDamage, SendMessageOptions.DontRequireReceiver);
            playerHit = false;
        }

        FinishMeleeAttackAnimation();
    }

    private void PostMeleeAttack()
    {
        if (currAnimation != "PostMeleeAttack")
        {
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "PostMeleeAttack";
            bossAnimator.SetBool(currAnimation, true);

            //meleeAttackIndicator.SetActive(false);   
            meleeAttackIndicatorPS.Stop();
        }

        FinishPostMeleeAttackAnimation();
    }

    private void PreBallAttack()
    {
        if (currAnimation != "PreBallAttack")
        {
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "PreBallAttack";
            bossAnimator.SetBool(currAnimation, true);

            //ballAttackIndicator.SetActive(true);
            ballAttackIndicatorPS.Play();
            fireAuraPS.Play();
            fireAuraDamageScript.preBallAttack = true;           
        }

        FinishPreBallAttackAnimation();
    }

    private void BallAttack()
    {
        if (currAnimation != "BallAttack")
        {   
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "BallAttack";
            bossAnimator.SetBool(currAnimation, true);

            Vector3 ballSpawnPosition = transform.FindChild("BallSpawnPoint").transform.position;
            GameObject fireBall = ParticlesManager.SpawnParticle("bossFireBall", ballSpawnPosition, facingRight);
            fireBall.GetComponent<BossFireBallBehaviour>().SetFacingRight(facingRight);
            fireAuraDamageScript.preBallAttack = false;        
        }

        FinishBallAttackAnimation();
    }

    private void PostBallAttack()
    {
        if (currAnimation != "PostBallAttack")
        {
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "PostBallAttack";
            bossAnimator.SetBool(currAnimation, true);

            //ballAttackIndicator.SetActive(false);
            ballAttackIndicatorPS.Stop();
            fireAuraPS.Stop();          
        }

        FinishPostBallAttackAnimation();
    }

    private void PrepareCast()
    {
        if (currAnimation != "PrepareCast")
        {
            if (facingRight)
                gameObject.transform.localRotation *= Quaternion.Euler(0, 270, 0);
            else
                gameObject.transform.localRotation *= Quaternion.Euler(0, 90, 0);

            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "PrepareCast";
            bossAnimator.SetBool(currAnimation, true);
            
            //Select the safe side to go when spikes are on
            if(iceSpikesScript != null)
                iceSpikesScript.SelectIceSafe();

            if (iceSpikesScript3D != null)
                iceSpikesScript3D.SelectIceSafe();
        }
        else
        {
            Vector3 pos = gameObject.transform.position;
            Vector3 newPos = Vector3.Lerp(pos, spikesCastingSpot.transform.position, Time.deltaTime * castingPosSpeed);
            gameObject.transform.position = newPos;

            if (Vector3.Distance(gameObject.transform.position, spikesCastingSpot.transform.position) <= lerpPosThreshold)
            {
                castIceSpikes = true;
                prepareCast = false;                       
            }
        }
        
    }

    private void CastIceSpikes()
    { 
        if (currAnimation != "CastIceSpikes")
        {                            
            gameObject.transform.localRotation *= Quaternion.Euler(0, 180, 0);

            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "CastIceSpikes";
            bossAnimator.SetBool(currAnimation, true);

            //Sacar los pinchos y dejarlos durante un tiempo
            if (iceSpikesScript != null)
            {
                iceSpikesScript.ShowIceSpikes();               
            }

            if (iceSpikesScript3D != null)
            {
                iceSpikesScript3D.ShowIceSpikes();               
            }
        }

        FinishCastIceSpikesAnimation();
    }

    private void BackToCenter()
    {
        if (currAnimation != "BackToCenter")
        {                       
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "BackToCenter";
            bossAnimator.SetBool(currAnimation, true);

            if(iceSpikesScript != null)
                iceSpikesScript.HideIceSpikes(); 
             
            if(iceSpikesScript3D != null)
                iceSpikesScript3D.HideIceSpikes();
        }
        else
        {
            Vector3 bossPosition = gameObject.transform.position;
            Vector3 newPos = Vector3.Lerp(bossPosition, spikesReturnSpot.transform.position, Time.deltaTime * castingPosSpeed);
            bossPosition = newPos;
            gameObject.transform.position = bossPosition;
           
            if (Vector3.Distance(gameObject.transform.position, spikesReturnSpot.transform.position) <= lerpPosThreshold)
            {
                //Return to the exact position
                gameObject.transform.position = spikesReturnSpot.transform.position;         
                backToCenter = false;           
               
            }
        }
        
    }

    private void Stalk()
    {
        if (currAnimation != "Stalk")
        {                            
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "Stalk";
            bossAnimator.SetBool(currAnimation, true);                    
        }
    }

    private void InsideTornado()
    {
        //Poner efecto de daño de tornado 
        if (currAnimation != "InsideTornado")
        {
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "InsideTornado";
            bossAnimator.SetBool(currAnimation, true);

            ballAttackIndicatorPS.Stop();
            meleeAttackIndicatorPS.Stop();
        }
    }

    //Flip the boss
    public void Flip()
    {
        gameObject.transform.localRotation *= Quaternion.Euler(0.0f, 180.0f, 0.0f);
        facingRight = !facingRight;
    }

    public State GetCurrentState()
    {
        return currState;
    }

    public void IsInsideTornado(bool value)
    {
        insideTornado = value;
    }

    public void NotifyRotationDuration(float duration)
    {
        tornadoDuration = duration;
    }

}
