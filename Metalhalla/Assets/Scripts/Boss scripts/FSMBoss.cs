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
        MELEE_ATTACK,
        BALL_ATTACK,
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
    [Tooltip("Depth of casting point")]
    public float spikesAttackBossDepth = 1.5f;
    public float detectionHeight = 3.0f;

    [Tooltip("Time until body disappears")]
    public float deadTime = 3.0f;
    [Tooltip("Melee animation duration")]
    public float meleeAttackDuration = 3.0f;
    [Tooltip("Ball attack animation duration")]
    public float ballAttackDuration = 3.0f;
    [Tooltip("Cast spikes animation duration")]
    public float castSpikesDuration = 3.0f;

    //ice spikes attack animator
    IceSpikesBehaviour iceSpikesScript = null;

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
    public bool atBallRange = false;
    [HideInInspector]
    public bool playerReachable = true;
    
    private bool ballAttack = false;
    private bool meleeAttack = true; //starting attack
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
    //first ice spikes at 25% maxHP
    private float thresholdFirstSpikes = 0.50f;
    //second ice spikes at 10% maxHP
    private float thresholdSecondSpikes = 0.10f;

    private string currAnimation = "Patrol"; //start animation
    //public bool nextAnimation = false;

    // ----------------------   COUNTERS TO ALLOW ANIMATION TRANSITIONS (evita que el animator vaya muy lento si no hay animaciones. Posiblemente se quitará )----
    [Tooltip("Minimun number of frames to stay in Patrol state before transition")]
    public int patrolFrames = 20;
    [Tooltip("Minimun number of frames to stay in Chase state before transition")]
    public int chaseFrames = 20;
    [Tooltip("Minimun number of frames to stay in Melee Attack state before transition")]
    public int meleeFrames = 20;
    [Tooltip("Minimun number of frames to stay in Ball Attack state before transition")]
    public int ballAttackFrames = 20;
    [Tooltip("Minimun number of frames to stay in Prepare Cast state before transition")]
    public int prepareCastFrames = 20;
    [Tooltip("Minimun number of frames to stay in Cast Ice Spikes state before transition")]
    public int castFrames = 20;
    [Tooltip("Minimun number of frames to stay in Back To Center state before transition")]
    public int backToCenterFrames = 20;
    [Tooltip("Minimun number of frames to stay in Stalk state before transition")]
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

        thePlayerStatus = thePlayer.GetComponent<PlayerStatus>();
        if(thePlayerStatus == null)       
            Debug.LogError("Error: player status not found.");
        

        bossAnimator = GetComponent<Animator>();
        bossStats = GetComponent<EnemyStats>();
        meleeDamage = bossStats.meleeDamage;
        
        iceSpikesScript = FindObjectOfType<IceSpikesBehaviour>();
        if (iceSpikesScript == null)
            Debug.LogError("Error: iceSpikesScript not found.");

        castingArea = GameObject.FindGameObjectWithTag("CastingArea");
        if (castingArea == null)
            Debug.LogError("Error: castingArea not found.");

       
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
                if (thePlayerStatus.IsAlive())
                {
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
                            prevState = State.CHASE;
                            currState = State.PATROL;
                            chaseFrameCounter = 0;
                        }
                        break;
                    }
                    if (insideTornado)
                    {
                        InsideTornado();
                        currState = State.INSIDE_TORNADO;
                        break;
                    }
                }
                //Player dead
                if (!thePlayerStatus.IsAlive())
                {
                    currState = State.PATROL;
                    playerInSight = false;
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
                if (insideTornado)
                {
                    InsideTornado();
                    currState = State.INSIDE_TORNADO;
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
                if (insideTornado)
                {
                    InsideTornado();
                    currState = State.INSIDE_TORNADO;
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
                    SelectAttack();
                    currState = State.CHASE;
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
    //-------------------------------- DAMAGE THE BOSS -----------------------------------------
    
    public void ApplyDamage(int damage)
    {
        if (currState == State.CHASE || currState == State.MELEE_ATTACK || currState == State.BALL_ATTACK || currState == State.INSIDE_TORNADO)
        {
            Debug.Log("Damaged");
            bossStats.ApplyDamage(damage);
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

            if (atMeleeRange)
                thePlayer.SendMessage("ApplyDamage", meleeDamage, SendMessageOptions.DontRequireReceiver);
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

            Vector3 ballSpawnPosition = transform.FindChild("BallSpawnPoint").transform.position;
            GameObject fireBall = ParticlesManager.SpawnParticle("bossFireBall", ballSpawnPosition, facingRight);
            fireBall.GetComponent<BossFireBallBehaviour>().SetFacingRight(facingRight);

            StartCoroutine(FinishBallAttackAnimation(ballAttackDuration));
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
            iceSpikesScript.SelectIceSafe();
            
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
        if (value == false)
            insideTornado = false;
        if (value == true)
            insideTornado = true;
    }

    public void NotifyRotationDuration(float duration)
    {
        tornadoDuration = duration;
    }

}
