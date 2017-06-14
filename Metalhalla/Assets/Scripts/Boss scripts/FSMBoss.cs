﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyStats))]
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
 
    private EnemyStats bossStats = null;
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

    [Space(10)]
    [Tooltip("Stalk state transition duration")]
    public float stalkDuration = 0.0f;
    private float stalkCounter = 0.0f;

    [Tooltip("Patrol state transition duration")]
    public float patrolDuration = 0.0f;
    private float patrolCounter = 0.0f;

    [Tooltip("Chase state transition duration")]
    public float chaseDuration = 0.0f;
    private float chaseCounter = 0.0f;

    [Header("Short Range Attack")]
    [Tooltip("PreMelee state transition duration")]
    public float preMeleeAttackDuration = 0.0f;
    private float preMeleeCounter = 0.0f;
    
    [Tooltip("Melee state transition duration")]
    public float meleeAttackDuration = 0.0f;
    private float meleeCounter = 0.0f;
    
    [Tooltip("PostMelee state transition duration")]
    public float postMeleeAttackDuration = 0.0f;
    private float postMeleeCounter = 0.0f;

    [Header("Long Range Attack")]
    [Tooltip("PreBall state transition duration")]
    public float preBallAttackDuration = 0.0f;
    private float preBallCounter = 0.0f;

    [Tooltip("Ball state transition duration")]
    public float ballAttackDuration = 0.0f;
    private float ballCounter = 0.0f;

    [Tooltip("PostBall state transition duration")]
    public float postBallAttackDuration = 0.0f;
    private float postBallcounter = 0.0f;

    [Header("Spikes Attack")]
    [Tooltip("Prepare Cast spikes state transition duration")]
    public float prepareCastDuration = 0.0f;
    private float prepareCastCounter = 0.0f;

    [Tooltip("Cast spikes state transition duration")]
    public float castSpikesDuration = 0.0f;
    private float castSpikesCounter = 0.0f;

    [Tooltip("Back To Center state transition duration")]
    public float backDuration = 0.0f;
    private float backCounter = 0.0f;

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


    void Awake()
    {   
        currState = State.PATROL;

        thePlayer = GameObject.FindGameObjectWithTag("Player");       
        thePlayerStatus = thePlayer.GetComponent<PlayerStatus>();       
        bossAnimator = GetComponent<Animator>();
        bossStats = GetComponent<EnemyStats>();
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
                    patrolCounter += Time.deltaTime;
                    if (patrolCounter >= patrolDuration)
                    {
                        patrolCounter = 0.0f;
                        currState = State.CHASE;
                        break;
                    }                                    
                }
                break;

            case State.CHASE:
                if (bossStats.hitPoints <= 0)
                {
                    chaseCounter += Time.deltaTime;
                    if (chaseCounter >= chaseDuration)
                    {
                        chaseCounter = 0.0f;                       
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
                    chaseCounter += Time.deltaTime;
                    if (chaseCounter >= chaseDuration)
                    {
                        chaseCounter = 0.0f;
                        currState = State.PATROL;
                        playerInSight = false;
                        break;
                    }
                }
                Chase();
                if (thePlayerStatus.IsAlive())
                {
                    if (preMeleeAttackSelected && atMeleeRange)
                    {
                        chaseCounter += Time.deltaTime;
                        if (chaseCounter >= chaseDuration)
                        {
                            chaseCounter = 0.0f;
                            currState = State.PRE_MELEE_ATTACK;
                            preMeleeAttackSelected = false; //reset value                        
                            break;
                        }
                    }
                    if (preBallAttackSelected && atBallRange)
                    {
                        chaseCounter += Time.deltaTime;
                        if (chaseCounter >= chaseDuration)
                        {
                            chaseCounter = 0.0f;
                            preBallCounter = 0.0f;
                            currState = State.PRE_BALL_ATTACK;
                            preBallAttackSelected = false; //reset value                      
                            break;
                        }
                    }
                    if (prepareCast)
                    {
                        chaseCounter += Time.deltaTime;
                        if (chaseCounter >= chaseDuration)
                        {
                            chaseCounter = 0.0f;
                            currState = State.PREPARE_CAST;
                            break;
                        }
                    }
                    if (!playerReachable)
                    {
                        chaseCounter += Time.deltaTime;
                        if (chaseCounter >= chaseDuration)
                        {
                            chaseCounter = 0.0f;
                            currState = State.STALK;
                            break;
                        }
                    }
                    if (!playerInSight)
                    {
                        chaseCounter += Time.deltaTime;
                        if (chaseCounter >= chaseDuration)
                        {
                            chaseCounter = 0.0f;
                            prevState = State.CHASE;
                            currState = State.PATROL;
                            break;
                        }
                    }
                    if (insideTornado)
                    {                                                    
                        currState = State.INSIDE_TORNADO;
                        break;                       
                    }
                }                                         
                break;

            case State.PRE_MELEE_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    preMeleeCounter += Time.deltaTime;
                    if (preMeleeCounter >= preMeleeAttackDuration)
                    {
                        preMeleeCounter = 0.0f;
                        currState = State.DEAD;
                        break;
                    }
                }
                PreMeleeAttack();
                if (preMeleeAttackFinished) 
                {
                    preMeleeCounter += Time.deltaTime;
                    if (preMeleeCounter >= preMeleeAttackDuration)
                    {
                        preMeleeCounter = 0.0f;
                        currState = State.MELEE_ATTACK;
                        preMeleeAttackFinished = false; //reset value                                            
                        break;
                    }
                }
                if (insideTornado)
                {                                           
                    currState = State.INSIDE_TORNADO;
                    break;                                
                }
                break;
             
            case State.MELEE_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    meleeCounter += Time.deltaTime;
                    if (meleeCounter >= meleeAttackDuration)
                    {
                        meleeCounter = 0.0f;
                        currState = State.DEAD;
                        break;
                    }
                }
                MeleeAttack();
                if (meleeAttackFinished) 
                {
                    meleeCounter += Time.deltaTime;
                    if (meleeCounter >= meleeAttackDuration)
                    {
                        meleeCounter = 0.0f;
                        currState = State.POST_MELEE_ATTACK;
                        meleeAttackFinished = false; //reset value                   
                        break;
                    }
                }
                if (insideTornado)
                {                                           
                    currState = State.INSIDE_TORNADO;
                    break;                   
                }
                break;

            case State.POST_MELEE_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    postMeleeCounter += Time.deltaTime;
                    if (postMeleeCounter >= postMeleeAttackDuration)
                    {
                        postMeleeCounter = 0.0f;
                        currState = State.DEAD;
                        break;
                    }
                }
                PostMeleeAttack();
                if (postMeleeAttackFinished) //attack has finished
                {
                    postMeleeCounter += Time.deltaTime;
                    if (postMeleeCounter >= postMeleeAttackDuration)
                    {
                        postMeleeCounter = 0.0f;
                        SelectAttack();
                        currState = State.CHASE;
                        postMeleeAttackFinished = false; //reset value                    
                        break;
                    }
                }
                if (insideTornado)
                {                                          
                    currState = State.INSIDE_TORNADO;
                    break;                   
                }
                break;

            case State.PRE_BALL_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    preBallCounter += Time.deltaTime;
                    if (preBallCounter >= preBallAttackDuration)
                    {
                        preBallCounter = 0.0f;
                        currState = State.DEAD;
                        break;
                    }                   
                }
                PreBallAttack();
                if (preBallAttackFinished) 
                {
                    preBallCounter += Time.deltaTime;
                    if (preBallCounter >= preBallAttackDuration)
                    {
                        preBallCounter = 0.0f;
                        currState = State.BALL_ATTACK;
                        preBallAttackFinished = false; //reset value
                        break;
                    }                  
                }
                if (insideTornado)
                {                    
                    currState = State.INSIDE_TORNADO;
                    break;                   
                }
                break;

            case State.BALL_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    ballCounter += Time.deltaTime;
                    if (ballCounter >= ballAttackDuration)
                    {
                        ballCounter = 0.0f;
                        currState = State.DEAD;
                        break;
                    }                                     
                }
                BallAttack();               
                if (ballAttackFinished) //attack has finished
                {
                    ballCounter += Time.deltaTime;
                    if (ballCounter >= ballAttackDuration)
                    {
                        ballCounter = 0.0f;
                        currState = State.POST_BALL_ATTACK;
                        ballAttackFinished = false; //reset value
                        break;
                    }                                      
                }
                if (insideTornado)
                {
                    currState = State.INSIDE_TORNADO;
                    break;                   
                }
                break;

            case State.POST_BALL_ATTACK:
                if (bossStats.hitPoints <= 0)
                {
                    postBallcounter += Time.deltaTime;
                    if (postBallcounter >= postBallAttackDuration)
                    {
                        postBallcounter = 0.0f;
                        currState = State.DEAD;
                        break;
                    }                 
                }
                PostBallAttack();
                if (postBallAttackFinished) //attack has finished
                {
                    postBallcounter += Time.deltaTime;
                    if (postBallcounter >= postBallAttackDuration)
                    {
                        postBallcounter = 0.0f;
                        SelectAttack();
                        currState = State.CHASE;
                        postBallAttackFinished = false; //reset value
                        break;
                    }
                }
                if (insideTornado)
                {                                          
                    currState = State.INSIDE_TORNADO;
                    break;                    
                }
                break;

            case State.PREPARE_CAST:
                PrepareCast();
                if (!prepareCast)  //prepared finished
                {
                    prepareCastCounter += Time.deltaTime;
                    if (prepareCastCounter >= prepareCastDuration)
                    {
                        prepareCastCounter = 0.0f;
                        currState = State.CAST_ICE_SPIKES;
                        break;
                    }                  
                }
                break;

            case State.CAST_ICE_SPIKES:
                CastIceSpikes();
                if (!castIceSpikes)  //cast finished
                {
                    castSpikesCounter += Time.deltaTime;
                    if (castSpikesCounter >= castSpikesDuration)
                    {
                        castSpikesCounter = 0.0f;
                        currState = State.BACK_TO_CENTER;
                        castIceSpikes = true; //reset value
                        break;
                    }                    
                }
                break;

            case State.BACK_TO_CENTER:
                BackToCenter();
                if (!backToCenter)   //back finished
                {
                    backCounter += Time.deltaTime;
                    if (backCounter >= backDuration)
                    {
                        backCounter = 0.0f;
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
                    stalkCounter += Time.deltaTime;
                    if (stalkCounter >= stalkDuration)
                    {
                        stalkCounter = 0.0f;
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
            //Debug.Log("Damaged");
            bossStats.ApplyDamage(damage);
            //create damage effect
            
        }
    }

    //---------------------------------------METHODS TO NOTIFY THE END OF THE ATTACK ANIMATIONS--------------------------------
    //private void FinishPreMeleeAttackAnimation()
    //{
    //    //Debug.Log("waiting for pre-melee attack to finish");
    //    preMeleeCounter += Time.deltaTime;
    //    if (preMeleeCounter >= preMeleeAttackDuration)
    //    {
    //        preMeleeCounter = 0.0f;
    //        //Debug.Log("pre-melee attack animation finished");
    //        preMeleeAttackFinished = true;

    //    }
    //}

    //private void FinishMeleeAttackAnimation()
    //{
    //    //Debug.Log("waiting for melee attack to finish");
    //    meleeCounter += Time.deltaTime;
    //    if (meleeCounter >= meleeAttackDuration)
    //    {
    //        meleeCounter = 0.0f;
    //        //Debug.Log("melee attack animation finished");
    //        meleeAttackFinished = true;
    //    }
    //}

    //private void FinishPostMeleeAttackAnimation()
    //{
    //    //Debug.Log("waiting for post-melee attack to finish");
    //    postMeleeCounter += Time.deltaTime;
    //    if (postMeleeCounter >= postMeleeAttackDuration)
    //    {
    //        postMeleeCounter = 0.0f;
    //        //Debug.Log("post-melee attack animation finished");
    //        postMeleeAttackFinished = true;
    //    }
    //}

    //private void FinishPreBallAttackAnimation()
    //{
    //    //Debug.Log("waiting for pre-ball attack to finish");
    //    preBallCounter += Time.deltaTime;
    //    if (preBallCounter >= preBallAttackDuration)
    //    {
    //        preBallCounter = 0.0f;
    //        //Debug.Log("pre-ball attack animation finished");
    //        preBallAttackFinished = true;
    //    }
    //}

    //private void FinishBallAttackAnimation()
    //{
    //    //Debug.Log("waiting for ball attack to finish");
    //    ballCounter += Time.deltaTime;
    //    if (ballCounter >= ballAttackDuration)
    //    {
    //        ballCounter = 0.0f;
    //        //Debug.Log("ball attack animation finished");
    //        ballAttackFinished = true;
    //    }
    //}

    //private void FinishPostBallAttackAnimation()
    //{
    //    //Debug.Log("waiting for post-ball attack to finish");
    //    postBallcounter += Time.deltaTime;
    //    if (postBallcounter >= postBallAttackDuration)
    //    {
    //        postBallcounter = 0.0f;
    //        //Debug.Log("post-ball attack animation finished");
    //        postBallAttackFinished = true;
    //    }
    //}

    //private void FinishCastIceSpikesAnimation()
    //{
    //    //Debug.Log("waiting for ice spikes cast to finish");
    //    castSpikesCounter += Time.deltaTime;
    //    if (castSpikesCounter >= castSpikesDuration)
    //    {
    //        castSpikesCounter = 0.0f;
    //        //Debug.Log("ice spikes cast finished");
    //        castIceSpikes = false;
    //        numSpikeAttacks++;
    //    }
    //}

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
            //if (currState == State.INSIDE_TORNADO)
            //    deadTime += (tornadoDuration + 3.0f);
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
            
            meleeAttackIndicatorPS.Play();        
        }
       
        if (bossAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            preMeleeAttackFinished = true;
        }
        
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

        if(bossAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            meleeAttackFinished = true;
        }
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

        if (bossAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            postMeleeAttackFinished = true;
        }
    }

    private void PreBallAttack()
    {
        if (currAnimation != "PreBallAttack")
        {
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "PreBallAttack";
            bossAnimator.SetBool(currAnimation, true);
            
            ballAttackIndicatorPS.Play();
            fireAuraPS.Play();
            fireAuraDamageScript.preBallAttack = true;           
        }

        if (bossAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            preBallAttackFinished = true;
        }
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

        if (bossAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            ballAttackFinished = true;
            ballAttackIndicatorPS.Stop();
            fireAuraPS.Stop();
        }
    }

    private void PostBallAttack()
    {
        if (currAnimation != "PostBallAttack")
        {
            bossAnimator.SetBool(currAnimation, false);
            currAnimation = "PostBallAttack";
            bossAnimator.SetBool(currAnimation, true);
            
        }

        if (bossAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            postBallAttackFinished = true;           
        }
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

        if (bossAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            castIceSpikes = false;
            numSpikeAttacks++;
        }
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
            fireAuraPS.Stop();
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

    public void Stunt()
    {
        insideTornado = true;
    }

    public void WakeUp()
    {
        insideTornado = false;
    }

    //public void NotifyRotationDuration(float duration)
    //{
    //    tornadoDuration = duration;
    //}

}
