using UnityEngine;


public class FSMBoss {

    private enum State
    {
        START,
        DAMAGED,
        DEAD,
        PATROL,
        ATTACK_SELECTION,
        CHASE,
        MELEE_ATTACK,
        BALL_ATTACK,
        ICE_SPIKES,
        STALK
    }

    public BossController bossController = null;
    private State currState = State.START;
    private State prevState =State.START;
    private GameObject player = null;
    //------------------- VARIABLES THAT CONTROL FSM LOGIC -------------------

    public bool playerInSight = false;
    public bool atMeleeRange = false;
    public bool atBallRange = false;
    public bool iceSpikesAttack = false;
    public bool ballAttack = false;   
    public bool meleeAttack = false;
    public bool playerReachable = true;

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
    private bool returningFromSpikesCast = false;
    
    
    //Temporalmente hasta que sepa cómo van las animaciones
    public string animation = "patrol";

    public FSMBoss(BossController bossContr)
	{
        bossController = bossContr;
        currState = State.PATROL;
        spikesCastingSpot = bossController.spikesCastingSpot.transform.position;
        spikesReturnSpot = bossController.spikesReturnSpot.transform.position;
        player = bossController.GetThePlayer();
    }

    public void Update()
    {
        //DEBUG
        Debug.Log("state:" + currState + "  animation:" + animation + " " + "  facingRight:" + facingRight);
                  
        switch(currState)
        {
            case State.DAMAGED:
                Damaged();
                if(bossController.GetBossStats().hitPoints <= 0)
                {
                    currState = State.DEAD;
                    break;
                } 
                if(!damaged)              
                    currState = prevState;
                break;

            case State.DEAD:
                Dead();
                break;

            case State.PATROL:
                Patrol();
                if (playerInSight)
                {                   
                    currState = State.ATTACK_SELECTION;
                    break;
                }                                  
                break;

            case State.ATTACK_SELECTION:
                SelectAttack();                            
                if(iceSpikesAttack)
                {
                    currState = State.ICE_SPIKES;
                    break;
                }
                if(!iceSpikesAttack)
                {
                    currState = State.CHASE;
                    break;
                }
                break;

            case State.CHASE:                
                Chase();
                if(!playerInSight)
                {
                    prevState = currState;
                    currState = State.PATROL;
                    break;
                }
                if (damaged)
                {
                    prevState = currState;
                    currState = State.DAMAGED;
                    break;
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
                if(!playerReachable)
                {
                    currState = State.STALK;
                    break;
                }
                break;

            case State.MELEE_ATTACK:
                MeleeAttack();
                if (damaged)
                {
                    prevState = currState;
                    currState = State.DAMAGED;
                    break;
                }
                if (!meleeAttack)
                {
                    currState = State.ATTACK_SELECTION;
                    break;
                }
                break;

            case State.BALL_ATTACK:
                BallAttack();
                if (damaged)
                {
                    prevState = currState;
                    currState = State.DAMAGED;
                    break;
                }
                if (!ballAttack)
                {
                    currState = State.ATTACK_SELECTION;
                    break;
                }
                break;

            case State.ICE_SPIKES:
                IceSpikesAttack();
                if (damaged)
                {
                    prevState = currState;
                    currState = State.DAMAGED;
                    break;
                }
                if (!iceSpikesAttack)
                {
                    currState = State.PATROL;
                    break;
                }
                break;

            case State.STALK:
                Stalk();
                if(player.transform.position.y <= bossController.detectionHeight)
                {
                    playerReachable = true;
                    currState = State.CHASE;
                    break;
                }
                break;
        }
       
    }

    // ------------------------------------- ACTIONS TO PERFORM IN EACH STATE --------------------------------------------
    public void Damaged()
    {
        animation = "damaged";
        damaged = false;
    }

    public void Dead()
    {
        animation = "dead";
    }

    public void Patrol()
	{
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

        animation = "walk";
    }

	public void Chase()
	{
        if (meleeAttack && !atMeleeRange || ballAttack && !atBallRange)
        {
            Vector3 newPos = bossController.GetTheBoss().transform.position;
            int diff = (int)(player.transform.position.x - newPos.x);

            Debug.Log("diff: " + diff);

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
        animation = "chase";
    }

	public void MeleeAttack()
	{
        animation = "melee attack";
        meleeAttack = false;
	}

	public void BallAttack()
	{
        animation = "ball attack";
        ballAttack = false;
	}

	public void IceSpikesAttack()
	{
        Vector3 bossPos = bossController.GetTheBoss().transform.position;

        if(returningFromSpikesCast == false)
        {
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
                numSpikeAttacks++;
                animation = "spike attack";                
                returningFromSpikesCast = true;
            }
        }

        if(returningFromSpikesCast == true)
        {
            Vector3 newPos = Vector3.Lerp(bossController.GetTheBoss().transform.position, spikesReturnSpot, Time.deltaTime * castingPosSpeed);
            Vector3 temp = bossController.GetTheBoss().transform.position;
            temp = newPos;
            bossController.GetTheBoss().transform.position = temp;

            //DEBUG
            Debug.Log("Distance.z = " + (bossController.GetTheBoss().transform.position.z - spikesReturnSpot.z));

            if (bossController.GetTheBoss().transform.position.z - spikesReturnSpot.z <= lerpPosThreshold)
            {
                //Return to exact position
                bossController.GetTheBoss().transform.position = spikesReturnSpot;
               
                animation = "patrol";
                iceSpikesAttack = false;
                returningFromSpikesCast = false;
            }
        }
        
        
    }

    public void SelectAttack()
    {
        if(bossController.GetBossStats().hitPoints > thresholdBallAttack * bossController.GetBossStats().maxHitPoints)
        {
            meleeAttack = true;
        }
        else
        {
            if(bossController.GetBossStats().hitPoints <= thresholdFirstSpikes * bossController.GetBossStats().maxHitPoints && numSpikeAttacks == 0)
            {
                iceSpikesAttack = true;             
            }

            else if (bossController.GetBossStats().hitPoints <= thresholdSecondSpikes * bossController.GetBossStats().maxHitPoints && numSpikeAttacks == 1)
            {
                iceSpikesAttack = true;                
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

    void Stalk()
    {
        animation = "stalk";
    }

}
