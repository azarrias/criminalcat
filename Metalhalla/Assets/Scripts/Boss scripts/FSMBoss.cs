using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BossStats))]
[RequireComponent(typeof(Animator))]
public class FSMBoss : MonoBehaviour
{
    private enum State
    {
        START,
        DEAD,
        PATROL,
        CHASE,
        MELEE_ATTACK,
        BALL_ATTACK,
        PREPARE_CAST,
        CAST_ICE_SPIKES,
        BACK_TO_CENTER,
        STALK
    }
 
    private BossStats bossStats = null;
    public GameObject spikesCastingSpot = null;
    public GameObject spikesReturnSpot = null;
    [Tooltip("Depth of casting point")]
    public float spikesAttackBossDepth = 1.5f;
    public float detectionHeight = 3.0f;

    [Tooltip("Time until body disappears")]
    public float deadTime = 4.0f;
    [Tooltip("Melee animation duration")]
    public float meleeAttackDuration = 0.5f;
    [Tooltip("Ball attack animation duration")]
    public float ballAttackDuration = 1.0f;
    [Tooltip("Cast spikes animation duration")]
    public float castSpikesDuration = 3.0f;

    //ball attack prefab
    public BossFireBall fireBallPrefab = null;
    //ice spikes attack animator
    IceSpikesBehaviour iceSpikesScript = null;

    private State currState = State.START;
    private State prevState = State.START;
    private GameObject thePlayer = null;
    private Animator bossAnimator = null;

    //------------------- VARIABLES THAT CONTROL FSM LOGIC -------------------
    [HideInInspector]
    public bool playerInSight = false;
    [HideInInspector]
    public bool atMeleeRange = false;
    [HideInInspector]
    public bool atBallRange = false;
    [HideInInspector]
    public bool playerReachable = true;
    
    private bool ballAttack = false;
    private bool meleeAttack = true; //starting attack
    //private bool spikesCastFinished = false;
    private bool prepareCast = false;
    private bool castIceSpikes = true;
    private bool backToCenter = true;

    //orientation of the boss will be modified when entering triggers at the limits of the platform and when player is around
    [HideInInspector]
    public bool facingRight = true;
    private int numSpikeAttacks = 0;

    //interpolation factor to go to spikes casting point and to return from it
    private float lerpPosThreshold = 0.2f;
    private float castingPosSpeed = 1.5f;
    //Ball attack allowed at HP <= 75% maxHP
    private float thresholdBallAttack = 0.75f;
    //first ice spikes at 25% maxHP
    private float thresholdFirstSpikes = 0.25f;
    //second ice spikes at 10% maxHP
    private float thresholdSecondSpikes = 0.10f;

    private string currAnimation = "Patrol"; //start animation
    //public bool nextAnimation = false;

    // variable to set rotations properly when returning from spikes casting spot
    private int prevDiff = 0;
    // ------------------------------------------------   COUNTERS TO ALLOW ANIMATION TRANSITIONS ---------------------------------------------------

    public int patrolFrames = 20;
    public int chaseFrames = 20;
    public int meleeFrames = 20;
    public int ballAttackFrames = 20;
    public int prepareCastFrames = 20;
    public int castFrames = 20;
    public int backToCenterFrames = 20;
    public int stalkFrames = 20;

    private int patrolFrameCounter = 0;
    private int chaseFrameCounter = 0;
    private int meleeAttackFrameCounter = 0;
    private int ballAttackFrameCounter = 0;
    private int prepareCastFrameCounter = 0;
    private int castTimeFrameCounter = 0;
    private int backToCenterFrameCounter = 0;
    private int stalkFrameCounter = 0;


    void Awake()
    {   
        currState = State.PATROL;

        thePlayer = GameObject.FindGameObjectWithTag("Player");
        if (thePlayer == null)
            Debug.LogError("Error: player not found.");

        bossAnimator = GetComponent<Animator>();
        bossStats = GetComponent<BossStats>();

        iceSpikesScript = FindObjectOfType<IceSpikesBehaviour>();
        if (iceSpikesScript == null)
            Debug.LogError("Error: iceSpikesScript not found.");
    }

    void Start()
    {
        spikesCastingSpot.transform.position = gameObject.transform.position + Vector3.forward * spikesAttackBossDepth;
        spikesReturnSpot.transform.position = gameObject.transform.position;
    }


    public void Update()
    {
        //DEBUG
        //Debug.Log("state:" + currState + "  animation:" + animation + " " + "  facingRight:" + facingRight);

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
                        //nextAnimation = true;
                        break;
                    }
                }
                break;

            case State.CHASE:
                if (bossStats.hitPoints <= 0)
                {
                    currState = State.DEAD;
                    break;
                }
                Chase();                            
                if (meleeAttack && atMeleeRange)
                {
                    chaseFrameCounter++;
                    if (chaseFrameCounter == chaseFrames)
                    {
                        currState = State.MELEE_ATTACK;
                        chaseFrameCounter = 0;
                    }                      
                    break;
                }
                if (ballAttack && atBallRange)
                {
                    chaseFrameCounter++;
                    if (chaseFrameCounter == chaseFrames)
                    {
                        currState = State.BALL_ATTACK;
                        chaseFrameCounter = 0;
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
                        currState = State.PATROL;
                        chaseFrameCounter = 0;
                    }
                    break;
                }
                break;

            case State.MELEE_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    currState = State.DEAD;
                    break;
                }
                MeleeAttack();
                if (!meleeAttack) //attack has finished
                {
                    meleeAttackFrameCounter++;
                    if (meleeAttackFrameCounter == meleeFrames)
                    {
                        SelectAttack();
                        currState = State.CHASE;
                        meleeAttackFrameCounter = 0;
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
                if (!ballAttack) //attack has finished
                {
                    ballAttackFrameCounter++;
                    if (ballAttackFrameCounter == ballAttackFrames)
                    {
                        SelectAttack();
                        currState = State.CHASE;
                        ballAttackFrameCounter = 0;
                        break;
                    }                  
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
        }

    }
    //Method called by the FSM
    private void SelectAttack()
    {
        if (bossStats.hitPoints > thresholdBallAttack * bossStats.maxHitPoints)
        {
            meleeAttack = true;
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
                    meleeAttack = true;
                    ballAttack = false;

                }
                if (num == 1)
                {
                    meleeAttack = false;
                    ballAttack = true;

                }
            }

        }

    }

    
    public void DamageBoss(int damage)
    {
        if (currState == State.CHASE || currState == State.MELEE_ATTACK || currState == State.BALL_ATTACK)
        {
            Debug.Log("Damaged");
            bossStats.DamageBoss(damage);
            //create damage effect
            
        }
    }

    //---------------------------------------METHODS TO NOTIFY THE END OF THE ATTACK ANIMATIONS--------------------------------
    private IEnumerator FinishMeleeAttackAnimation(float seconds)
    {
        Debug.Log("waiting for melee attack to finish");
        yield return new WaitForSeconds(seconds);
        Debug.Log("melee attack animation finished");
        meleeAttack = false;
    }
    private IEnumerator FinishBallAttackAnimation(float seconds)
    {
        Debug.Log("waitings for ball attack to finish");
        yield return new WaitForSeconds(seconds);
        Debug.Log("ball attack animation finished");
        ballAttack = false;
    }
    private IEnumerator FinishCastIceSpikesAnimation(float seconds)
    {
        Debug.Log("waiting for ice spikes cast to finish");
        yield return new WaitForSeconds(seconds);
        Debug.Log("ice spikes cast finished");
        castIceSpikes = false;
        numSpikeAttacks++;
    }
   
    // ------------------------------------- ACTIONS TO PERFORM IN EACH STATE --------------------------------------------
    private void Dead()
    {
        if (currAnimation != "Dead")
        {
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "Dead";
            bossAnimator.SetBool(currAnimation, true);                             
            //Destroy boss                          
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
                Vector3 newPos = gameObject.transform.position;
                int diff = (int)(thePlayer.transform.position.x - newPos.x);


                if (Mathf.Sign(diff) == Mathf.Sign(prevDiff))
                {
                    if (diff > 0)
                    {
                        gameObject.transform.localRotation *= Quaternion.Euler(0, -90, 0);
                    }
                    if (diff < 0)
                    {
                        gameObject.transform.localRotation *= Quaternion.Euler(0, 90, 0);
                    }
                }
                else
                {
                    if (diff > 0)
                    {
                        gameObject.transform.localRotation *= Quaternion.Euler(0, 90, 0);
                    }
                    if (diff < 0)
                    {
                        gameObject.transform.localRotation *= Quaternion.Euler(0, -90, 0);
                    }
                }

                //if (diff > 0)
                //{
                //    bossController.GetTheBossController().transform.localRotation *= Quaternion.Euler(0, -90, 0);
                //}
                //if (diff < 0)
                //{
                //    bossController.GetTheBossController().transform.localRotation *= Quaternion.Euler(0, 90, 0);
                //}
            }

            if (meleeAttack && !atMeleeRange || ballAttack && !atBallRange)
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

    private void MeleeAttack()
    {
        if (currAnimation != "MeleeAttack")
        {          
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "MeleeAttack";
            bossAnimator.SetBool(currAnimation, true);                    
        
            StartCoroutine(FinishMeleeAttackAnimation(meleeAttackDuration));
        }       
    }

    private void BallAttack()
    {
        if (currAnimation != "BallAttack")
        {   
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "BallAttack";
            bossAnimator.SetBool(currAnimation, true);

            BossFireBall ball = Instantiate<BossFireBall>(fireBallPrefab, gameObject.transform.position, Quaternion.identity);          
            if (facingRight)
                ball.SetDirection(Vector3.right);
            if (!facingRight)
                ball.SetDirection(Vector3.left);   

            StartCoroutine(FinishBallAttackAnimation(ballAttackDuration));
        }   
    }

    private void PrepareCast()
    {
        if (currAnimation != "PrepareCast")
        {           
            if (facingRight)
                gameObject.transform.localRotation *= Quaternion.Euler(0, -90, 0);
            else
                gameObject.transform.localRotation *= Quaternion.Euler(0, 90, 0);

            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "PrepareCast";
            bossAnimator.SetBool(currAnimation, true);

            Vector3 newPos = gameObject.transform.position;
             prevDiff = (int)(thePlayer.transform.position.x - newPos.x);
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

            GameObject.Find("OverHeadCollider").GetComponent<BoxCollider>().enabled = false;
            GameObject.Find("BossCollider").GetComponent<BoxCollider>().enabled = false;

            //Sacar los pinchos y dejarlos durante un tiempo               
            iceSpikesScript.ShowIceSpikes();         
            StartCoroutine(FinishCastIceSpikesAnimation(castSpikesDuration));                  
        } 
    }

    private void BackToCenter()
    {
        if (currAnimation != "BackToCenter")
        {                       
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "BackToCenter";
            bossAnimator.SetBool(currAnimation, true);
            iceSpikesScript.HideIceSpikes();                  
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
                GameObject.Find("OverHeadCollider").GetComponent<BoxCollider>().enabled = true;
                GameObject.Find("BossCollider").GetComponent<BoxCollider>().enabled = true;
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

    //Flip the boss
    public void Flip()
    {
        Vector3 scale = gameObject.transform.localScale;
        scale.x *= -1;
        gameObject.transform.localScale = scale;
        facingRight = !facingRight;
    }

}
