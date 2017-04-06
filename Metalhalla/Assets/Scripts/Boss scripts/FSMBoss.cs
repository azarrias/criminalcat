using UnityEngine;
using System.Collections;

public class FSMBoss {

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
    private State prevState =State.START;
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
    private float lerpPosThreshold = 0.1f;
    private float castingPosSpeed = 2.0f;
    //Ball attack allowed at HP <= 75% * maxHP
    private float thresholdBallAttack = 0.75f;
    //first ice spikes at 25% maxHP
    private float thresholdFirstSpikes = 0.25f;
    //second ice spikes at 10% maxHP
    private float thresholdSecondSpikes = 0.10f;

    public string animation = "Patrol"; //start animation
    public bool nextAnimation = false;

    public FSMBoss(BossController bossContr)
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
        Debug.Log("state:" + currState + "  animation:" + animation + " " + "  facingRight:" + facingRight);
                  
        switch(currState)
        {                       
            case State.PATROL:                                                           
                Patrol();
                if (playerInSight)
                {                                                   
                    currState = State.CHASE;
                    nextAnimation = true;
                    break;
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
                if(meleeAttack && atMeleeRange)
                {
                    currState = State.MELEE_ATTACK;
                    break;
                }
                if(ballAttack && atBallRange)
                {
                    currState = State.BALL_ATTACK;
                    break;
                }
                if(prepareCast)
                {
                    currState = State.PREPARE_CAST;
                    break;
                }
                if(!playerReachable)
                {
                    currState = State.STALK;
                    break;
                }
                if (!playerInSight)
                {                    
                    currState = State.PATROL;
                    break;
                }
                break;

            case State.MELEE_ATTACK:
                if(bossController.GetBossStats().hitPoints <= 0)
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
                    SelectAttack();
                    currState = State.CHASE;
                    break;                                      
                }

                break;

            case State.BALL_ATTACK:
                if (bossController.GetBossStats().hitPoints <= 0)
                {
                    currState = State.DEAD;
                    break;
                }
                BallAttack();
                if (damaged)
                {
                    Damaged();                   
                }
                if (!ballAttack) //attack has finished
                {
                    SelectAttack();
                    currState = State.CHASE;
                    break;                   
                }
                break;

            case State.PREPARE_CAST:
                PrepareCast();
                if (castIceSpikes)
                {
                    currState = State.CAST_ICE_SPIKES;
                    break;
                }
                break;

            case State.CAST_ICE_SPIKES:
                CastIceSpikes();
                if (spikesCastFinished)
                {
                    currState = State.BACK_TO_CENTER;
                    break;
                }
                break;

            case State.BACK_TO_CENTER:
                BackToCenter();
                if (backToCenter)
                {
                    backToCenter = false;
                    SelectAttack();
                    currState = State.CHASE;
                    break;
                }
                break;

            case State.STALK:
                Stalk();
                //if(player.transform.position.y <= bossController.detectionHeight)
                if(playerReachable)
                {
                    //playerReachable = true;
                    currState = State.CHASE;
                    break;
                }
                break;

            case State.DEAD:
                Dead();
                break;
        }
       
    }
    // ---------------------------- METHODS NOT RELATED TO STATES
    public void Damaged()
    {
        Debug.Log("Damaged");
        bossController.GetTheBoss().GetComponent<BossStats>().DamageBoss(10);
        //create damage effect
        damaged = false;
    }

    public void SelectAttack()
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

    // -------------------------------------- DAMAGE THE BOSS --------------------------------------
    public void DamageBoss(int damage)
    {

        if (
            currState != State.PREPARE_CAST && currState != State.CAST_ICE_SPIKES &&
            currState != State.BACK_TO_CENTER && currState != State.STALK
           )
        {
            //test delay
            Delay(1f);
            damaged = true;
        }
    }

    //---------------------------------------METHODS TO NOTIFY THE END OF THE ATTACK ANIMATIONS--------------------------------
    public void FinishMeleeAttackAnimation()
    {
        meleeAttack = false;
    }
    public void FinishBallAttackAnimation()
    {
        ballAttack = false;
    }
    //public void FinishIceSpikesAttackAnimation()
    //{
    //    prepareCast = false;
    //}

    // ------------------------------------- ACTIONS TO PERFORM IN EACH STATE --------------------------------------------
    public void Dead()
    {
       if(animation != "Dead")
        {
            bossAnimator.SetBool(animation, false);
            animation = "Dead";
            bossAnimator.SetBool(animation, true);
            //test delay
            Delay(1f);
        }
    }

    public void Patrol()
	{
        if (animation != "Patrol")
        {
            bossAnimator.SetBool(animation, false);
            animation = "Patrol";
            bossAnimator.SetBool(animation, true);

            //test delay
            Delay(1f);
        }
        if (prevState == State.CHASE)
        {
            facingRight = !facingRight;

            //flip the boss
            Vector3 scale = bossController.GetTheBoss().transform.localScale;
            scale.x *= -1;
            bossController.GetTheBoss().transform.localScale = scale;

            prevState = State.PATROL;
        }

        Vector3 newPos = bossController.GetTheBoss().transform.position;

        if (facingRight == true)
            newPos.x += bossController.GetBossStats().normalSpeed * Time.deltaTime;
        else
            newPos.x -= bossController.GetBossStats().normalSpeed * Time.deltaTime;

        bossController.GetTheBoss().transform.position = newPos;    
    }

	public void Chase()
	{
        if (animation != "Chase")
        {
            bossAnimator.SetBool(animation, false);
            animation = "Chase";
            bossAnimator.SetBool(animation, true);

            //test delay
            Delay(1f);
        }

        if (meleeAttack && !atMeleeRange || ballAttack && !atBallRange)
       {
            Vector3 newPos = bossController.GetTheBoss().transform.position;
            int diff = (int)(player.transform.position.x - newPos.x);
         
            if (diff > 0)
                facingRight = true;
            if (diff < 0)
                facingRight = false;
           
            if (facingRight == true)
                newPos.x += bossController.GetBossStats().chasingSpeed * Time.deltaTime;
            else
                newPos.x -= bossController.GetBossStats().chasingSpeed * Time.deltaTime;

            bossController.GetTheBoss().transform.position = newPos;
        }              
    }

	public void MeleeAttack()
	{
        if(animation != "MeleeAttack")
        {
            bossAnimator.SetBool(animation, false);
            animation = "MeleeAttack";
            bossAnimator.SetBool(animation, true);       
        }
        //cuando acabe la animación se tiene que notificar con un animation event
        //test delay
        Delay(4);
        FinishMeleeAttackAnimation();
	}

	public void BallAttack()
	{
        if (animation != "BallAttack")
        {
            bossAnimator.SetBool(animation, false);
            animation = "BallAttack";
            bossAnimator.SetBool(animation, true);           
        }

        //cuando acabe la animación se tiene que notificar con un animation event
        //test delay
        Delay(4f);
        FinishBallAttackAnimation();
	}
    
    public void PrepareCast()
    {
        if (animation != "PrepareCast")
        {
            bossAnimator.SetBool(animation, false);
            animation = "PrepareCast";
            bossAnimator.SetBool(animation, true);

            //test delay
            Delay(1f);
        }

        Vector3 bossPos = bossController.GetTheBoss().transform.position;
        
        float diff = spikesCastingSpot.x - bossPos.x;
        if (diff > 0)
        {
            if (facingRight == false)
            {
                //flip the boss
                Vector3 scale = bossController.GetTheBoss().transform.localScale;
                scale.x *= -1;
                bossController.GetTheBoss().transform.localScale = scale;
                facingRight = true;
            }
        }
        if (diff < 0)
        {
            if (facingRight == true)
            {
                //flip the boss
                Vector3 scale = bossController.GetTheBoss().transform.localScale;
                scale.x *= -1;
                bossController.GetTheBoss().transform.localScale = scale;
                facingRight = false;
            }

        }

        Vector3 newPos = Vector3.Lerp(bossController.GetTheBoss().transform.position, spikesCastingSpot, Time.deltaTime * castingPosSpeed);
        Vector3 temp = bossController.GetTheBoss().transform.position;
        temp = newPos;
        bossController.GetTheBoss().transform.position = temp;

        //DEBUG
        //Debug.Log("Distance = " + Vector3.Distance(bossController.GetTheBoss().transform.position, spikesCastingSpot));

        if (spikesCastingSpot.z - bossController.GetTheBoss().transform.position.z <= lerpPosThreshold)
        {
            castIceSpikes = true;
            prepareCast = false;
        }      
    }

    public void CastIceSpikes()
    {
        numSpikeAttacks++;
        if (animation != "CastIceSpikes")
        {           
            bossAnimator.SetBool(animation, false);
            animation = "CastIceSpikes";
            bossAnimator.SetBool(animation, true);            
        }

        //Sacar los pinchos y dejarlos durante un tiempo
        //test delay
        Delay(4f);
        castIceSpikes = false;
        spikesCastFinished = true;
    }

    public void BackToCenter()
    {
        if (animation != "BackToCenter")
        {
            bossAnimator.SetBool(animation, false);
            animation = "BackToCenter";
            bossAnimator.SetBool(animation, true);
            
            //test delay
            Delay(4f);
        }
        Vector3 bossPosition = bossController.GetTheBoss().transform.position;
        Vector3 newPos = Vector3.Lerp(bossPosition, spikesReturnSpot, Time.deltaTime * castingPosSpeed);
        bossPosition = newPos;
        bossController.GetTheBoss().transform.position = bossPosition;

        //DEBUG
        //Debug.Log("Distance.z = " + (bossController.GetTheBoss().transform.position.z - spikesReturnSpot.z));

        if (bossController.GetTheBoss().transform.position.z - spikesReturnSpot.z <= lerpPosThreshold)
        {
            //Return to exact position
            bossController.GetTheBoss().transform.position = spikesReturnSpot;

            spikesCastFinished = false;
            backToCenter = true;
        }
    }

    void Stalk()
    {
        if (animation != "Stalk")
        {
            bossAnimator.SetBool(animation, false);
            animation = "Stalk";
            bossAnimator.SetBool(animation, true);
           
        }
    }

    //Test
    IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

}
