using UnityEngine;
using System.Collections;

public class FSMBoss : ScriptableObject
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

    public BossController bossController = null;
    private State currState = State.START;
    private State prevState = State.START;
    private GameObject player = null;
    private Animator bossAnimator = null;
    //------------------- VARIABLES THAT CONTROL FSM LOGIC -------------------

    public bool playerInSight = false;
    public bool atMeleeRange = false;
    public bool atBallRange = false;
    public bool playerReachable = true;
    private bool prepareCast = false;
    private bool castIceSpikes = false;
    private bool ballAttack = false;
    private bool meleeAttack = true; //starting attack
    private bool spikesCastFinished = false;
    private bool backToCenter = false;

    //damaged will be modified by the player
    public bool damaged = false;
    //orientation of the boss will be modified when entering triggers at the limits of the platform and when player is around
    public bool facingRight = true;
    private int numSpikeAttacks = 0;
    private Vector3 spikesCastingSpot;
    private Vector3 spikesReturnSpot;
    //interpolation factor to go to spikes casting point and to return from it
    private float lerpPosThreshold = 0.2f;
    private float castingPosSpeed = 1.5f;
    //Ball attack allowed at HP <= 75% maxHP
    private float thresholdBallAttack = 0.75f;
    //first ice spikes at 25% maxHP
    private float thresholdFirstSpikes = 0.25f;
    //second ice spikes at 10% maxHP
    private float thresholdSecondSpikes = 0.10f;

    public string animation = "Patrol"; //start animation
    public bool nextAnimation = false;
    private int prevDiff = 0;
    // ------------------------------------------------   COUNTERS TO ALLOW ANIMATION TRANSITIONS ---------------------------------------------------
    private int patrolCounter = 0;
    private int chaseCounter = 0;
    private int meleeAttackCounter = 0;
    private int ballAttackCounter = 0;
    private int prepareCastCounter = 0;
    private int castTimeCounter = 0;
    private int backToCenterCounter = 0;
    private int stalkCounter = 0;   
    
    public static FSMBoss CreateInstance(BossController bossController)
    {
        FSMBoss fsmBoss = ScriptableObject.CreateInstance<FSMBoss>();
        fsmBoss.Init(bossController);
        return fsmBoss;
    }

    private void Init(BossController bossContr)
    {
        bossController = bossContr;
        currState = State.PATROL;
        spikesCastingSpot = bossController.spikesCastingSpot.transform.position;
        spikesReturnSpot = bossController.spikesReturnSpot.transform.position;
        player = bossController.GetThePlayer();
        bossAnimator = bossController.GetBossAnimator();
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
                    patrolCounter++;
                    if (patrolCounter == bossController.patrolFrames)
                    {
                        patrolCounter = 0;
                        currState = State.CHASE;
                        nextAnimation = true;
                        break;
                    }
                }
                break;

            case State.CHASE:
                if (bossController.GetBossStats().hitPoints <= 0)
                {
                    currState = State.DEAD;
                    break;
                }
                Chase();

                if (damaged)
                {
                    Damaged();
                }
                if (meleeAttack && atMeleeRange)
                {
                    chaseCounter++;
                    if (chaseCounter == bossController.chaseFrames)
                    {
                        currState = State.MELEE_ATTACK;
                        chaseCounter = 0;
                    }                      
                    break;
                }
                if (ballAttack && atBallRange)
                {
                    chaseCounter++;
                    if (chaseCounter == bossController.chaseFrames)
                    {
                        currState = State.BALL_ATTACK;
                        chaseCounter = 0;
                    }
                    break;
                }
                if (prepareCast)
                {
                    chaseCounter++;
                    if (chaseCounter == bossController.chaseFrames)
                    {
                        currState = State.PREPARE_CAST;
                        chaseCounter = 0;
                    }
                    break;
                }
                if (!playerReachable)
                {
                    chaseCounter++;
                    if (chaseCounter == bossController.chaseFrames)
                    {
                        currState = State.STALK;
                        chaseCounter = 0;
                    }
                    break;
                }
                if (!playerInSight)
                {
                    chaseCounter++;
                    if (chaseCounter == bossController.chaseFrames)
                    {
                        currState = State.PATROL;
                        chaseCounter = 0;
                    }
                    break;
                }
                break;

            case State.MELEE_ATTACK:
                if (bossController.GetBossStats().hitPoints <= 0)
                {
                    currState = State.DEAD;
                    break;
                }
                MeleeAttack();
                if (damaged)
                {
                    Damaged();
                }
                if (!meleeAttack) //attack has finished
                {
                    meleeAttackCounter++;
                    if (meleeAttackCounter == bossController.meleeFrames)
                    {
                        SelectAttack();
                        currState = State.CHASE;
                        meleeAttackCounter = 0;
                    }
                    break;
                }

                break;

            case State.BALL_ATTACK:
                if (bossController.GetBossStats().hitPoints <= 0)
                {
                    ballAttackCounter++;
                    if (ballAttackCounter == bossController.ballAttackFrames)
                    {
                        currState = State.DEAD;
                        ballAttackCounter = 0;
                        break;
                    }                  
                }
                BallAttack();
                if (damaged)
                {
                    Damaged();
                }
                if (!ballAttack) //attack has finished
                {
                    ballAttackCounter++;
                    if (ballAttackCounter == bossController.ballAttackFrames)
                    {
                        SelectAttack();
                        currState = State.CHASE;
                        ballAttackCounter = 0;
                        break;
                    }                  
                }
                break;

            case State.PREPARE_CAST:
                PrepareCast();
                if (castIceSpikes)
                {
                    prepareCastCounter++;
                    if (prepareCastCounter == bossController.prepareCastFrames)
                    {
                        currState = State.CAST_ICE_SPIKES;
                        prepareCastCounter = 0;
                        break;
                    }
                }
                break;

            case State.CAST_ICE_SPIKES:
                CastIceSpikes();
                if (spikesCastFinished)
                {
                    castTimeCounter++;
                    if (castTimeCounter == bossController.castFrames)
                    {
                        currState = State.BACK_TO_CENTER;
                        castTimeCounter = 0;
                        break;
                    }
                }
                break;

            case State.BACK_TO_CENTER:
                BackToCenter();
                if (backToCenter)
                {
                    backToCenterCounter++;
                    if (backToCenterCounter == bossController.backToCenterFrames)
                    {
                        backToCenterCounter = 0;
                        backToCenter = false;
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
                    stalkCounter++;
                    if (stalkCounter == bossController.stalkFrames)
                    {
                        stalkCounter = 0;
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
        if (bossController.GetBossStats().hitPoints > thresholdBallAttack * bossController.GetBossStats().maxHitPoints)
        {
            meleeAttack = true;
        }
        else
        {
            if (bossController.GetBossStats().hitPoints <= thresholdFirstSpikes * bossController.GetBossStats().maxHitPoints && numSpikeAttacks == 0)
            {
                prepareCast = true;
            }

            else if (bossController.GetBossStats().hitPoints <= thresholdSecondSpikes * bossController.GetBossStats().maxHitPoints && numSpikeAttacks == 1)
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

    //Method called by the boss controller
    public void DamageBoss(int damage)
    {
        if (
            currState != State.PREPARE_CAST && currState != State.CAST_ICE_SPIKES &&
            currState != State.BACK_TO_CENTER && currState != State.STALK
           )
        {

            damaged = true;
        }
    }
    //Method called by the FSM
    private void Damaged()
    {
        Debug.Log("Damaged");
        bossController.GetTheBossController().GetComponent<BossStats>().DamageBoss(10);                
        //create damage effect
        damaged = false;
          
    }

    //---------------------------------------METHODS TO NOTIFY THE END OF THE ATTACK ANIMATIONS--------------------------------
    private void FinishMeleeAttackAnimation()
    {
        meleeAttack = false;
    }
    private void FinishBallAttackAnimation()
    {
        ballAttack = false;
    }
    private void FinishCastIceSpikes()
    {
        castIceSpikes = false;
    }
   
    // ------------------------------------- ACTIONS TO PERFORM IN EACH STATE --------------------------------------------
    private void Dead()
    {
        if (animation != "Dead")
        {
            bossAnimator.SetBool(animation, false);
            animation = "Dead";
            bossAnimator.SetBool(animation, true);                             
            //Destroy boss                          
        }
    }

    private void Patrol()
    {      
        if (animation != "Patrol")
        {       
            bossAnimator.SetBool(animation, false);
            animation = "Patrol";
            bossAnimator.SetBool(animation, true);                         
        }
        else
        {
            if (prevState == State.CHASE)
            {
                Flip();

                prevState = State.PATROL;
            }

            Vector3 newPos = bossController.GetTheBossController().transform.position;

            if (facingRight == true)
                newPos.x += bossController.GetBossStats().normalSpeed * Time.deltaTime;
            else
                newPos.x -= bossController.GetBossStats().normalSpeed * Time.deltaTime;

            bossController.GetTheBossController().transform.position = newPos;
        }       
    }

    private void Chase()
    {
        if (animation != "Chase")
        {      
            bossAnimator.SetBool(animation, false);
            animation = "Chase";
            bossAnimator.SetBool(animation, true);     
        }
        else
        {
            if (prevState == State.BACK_TO_CENTER)
            {
                prevState = State.CHASE;
                Vector3 newPos = bossController.GetTheBossController().transform.position;
                int diff = (int)(player.transform.position.x - newPos.x);


                if (Mathf.Sign(diff) == Mathf.Sign(prevDiff))
                {
                    if (diff > 0)
                    {
                        bossController.GetTheBossController().transform.localRotation *= Quaternion.Euler(0, -90, 0);
                    }
                    if (diff < 0)
                    {
                        bossController.GetTheBossController().transform.localRotation *= Quaternion.Euler(0, 90, 0);
                    }
                }
                else
                {
                    if (diff > 0)
                    {
                        bossController.GetTheBossController().transform.localRotation *= Quaternion.Euler(0, 90, 0);
                    }
                    if (diff < 0)
                    {
                        bossController.GetTheBossController().transform.localRotation *= Quaternion.Euler(0, -90, 0);
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
                Vector3 newPos = bossController.GetTheBossController().transform.position;
                int diff = (int)(player.transform.position.x - newPos.x);

                if (diff > 0)
                    if (!facingRight)
                        Flip();
                if (diff < 0)
                    if (facingRight)
                        Flip();

                if (facingRight == true)
                    newPos.x += bossController.GetBossStats().chasingSpeed * Time.deltaTime;
                else
                    newPos.x -= bossController.GetBossStats().chasingSpeed * Time.deltaTime;

                bossController.GetTheBossController().transform.position = newPos;
            }

        }     
    }

    private void MeleeAttack()
    {
        if (animation != "MeleeAttack")
        {          
            bossAnimator.SetBool(animation, false);
            animation = "MeleeAttack";
            bossAnimator.SetBool(animation, true);                    
        }
        else
        {          
            FinishMeleeAttackAnimation();               
        }       
    }

    private void BallAttack()
    {
        if (animation != "BallAttack")
        {   
            bossAnimator.SetBool(animation, false);
            animation = "BallAttack";
            bossAnimator.SetBool(animation, true);
        }
        else
        {           
           FinishBallAttackAnimation();
        }   
    }

    private void PrepareCast()
    {
        if (animation != "PrepareCast")
        {           
            if (facingRight)
                bossController.GetTheBossController().transform.localRotation *= Quaternion.Euler(0, -90, 0);
            else
                bossController.GetTheBossController().transform.localRotation *= Quaternion.Euler(0, 90, 0);

            bossAnimator.SetBool(animation, false);
            animation = "PrepareCast";
            bossAnimator.SetBool(animation, true);

            Vector3 newPos = bossController.GetTheBossController().transform.position;
             prevDiff = (int)(player.transform.position.x - newPos.x);
        }
        else
        {
            Vector3 pos = bossController.GetTheBossController().transform.position;
            Vector3 newPos = Vector3.Lerp(pos, spikesCastingSpot, Time.deltaTime * castingPosSpeed);
            bossController.GetTheBossController().transform.position = newPos;

            if (Vector3.Distance(bossController.GetTheBossController().transform.position, spikesCastingSpot) <= lerpPosThreshold)
            {
                castIceSpikes = true;
                prepareCast = false;
            }
        }
        
    }

    private void CastIceSpikes()
    { 
        if (animation != "CastIceSpikes")
        {                            
            bossController.GetTheBossController().transform.localRotation *= Quaternion.Euler(0, 180, 0);

            bossAnimator.SetBool(animation, false);
            animation = "CastIceSpikes";
            bossAnimator.SetBool(animation, true);
            numSpikeAttacks++;
        }
        else
        {
           
            //Sacar los pinchos y dejarlos durante un tiempo  
            
            FinishCastIceSpikes();
            spikesCastFinished = true;
        } 
    }

    private void BackToCenter()
    {
        if (animation != "BackToCenter")
        {                       
            bossAnimator.SetBool(animation, false);
            animation = "BackToCenter";
            bossAnimator.SetBool(animation, true);                    
        }
        else
        {
            Vector3 bossPosition = bossController.GetTheBossController().transform.position;
            Vector3 newPos = Vector3.Lerp(bossPosition, spikesReturnSpot, Time.deltaTime * castingPosSpeed);
            bossPosition = newPos;
            bossController.GetTheBossController().transform.position = bossPosition;
           
            if (Vector3.Distance(bossController.GetTheBossController().transform.position, spikesReturnSpot) <= lerpPosThreshold)
            {
                //Return to the exact position
                bossController.GetTheBossController().transform.position = spikesReturnSpot;

                spikesCastFinished = false;
                backToCenter = true;
            }
        }
        
    }

    private void Stalk()
    {
        if (animation != "Stalk")
        {                            
            bossAnimator.SetBool(animation, false);
            animation = "Stalk";
            bossAnimator.SetBool(animation, true);                    
        }
    }

    //Flip the boss
    public void Flip()
    {
        Vector3 scale = bossController.GetTheBossController().transform.localScale;
        scale.x *= -1;
        bossController.GetTheBossController().transform.localScale = scale;
        facingRight = !facingRight;
    }

}
