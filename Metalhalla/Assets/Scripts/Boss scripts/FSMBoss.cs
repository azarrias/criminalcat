using UnityEngine;


public class FSMBoss {

    private enum State
    {
        START,
        DEAD,
        PATROL,
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
    public bool meleeAttack = true; //starting attack
    public bool playerReachable = true;

    //damaged will be modified by the player
    public bool damaged = false;
    public bool showIceSpikes = false;
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
            case State.PATROL:
                Patrol();
                if (playerInSight)
                {                               
                    currState = State.CHASE;
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
                    if(iceSpikesAttack)
                    {
                        currState = State.ICE_SPIKES;
                        break;
                    }
                    else
                    {
                        currState = State.CHASE;
                        break;
                    }                   
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
                    if (iceSpikesAttack)
                    {
                        currState = State.ICE_SPIKES;
                        break;
                    }
                    else
                    {
                        currState = State.CHASE;
                        break;
                    }
                }
                break;

            case State.ICE_SPIKES:
                IceSpikesAttack();               
                if (!iceSpikesAttack)  //attack has finished
                {
                    SelectAttack();
                    currState = State.CHASE;
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

            case State.DEAD:
                Dead();
                break;
        }
       
    }

     
    public void Damaged()
    {
        Debug.Log("Damaged");
        bossController.GetTheBoss().GetComponent<BossStats>().DamageBoss(10);
        animation = "damaged";
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
    // -------------------------------------- DAMAGE THE BOSS --------------------------------------
    public void DamageBoss(int damage)
    {
        if(currState != State.ICE_SPIKES && currState != State.STALK)
        {
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
    public void FinishSpikesAttackAnimation()
    {
        iceSpikesAttack = false;
    }

    // ------------------------------------- ACTIONS TO PERFORM IN EACH STATE --------------------------------------------
    public void Dead()
    {
        animation = "dead";
        //set animation in animator. Dead animation is executed once.
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

        if (animation != "walk")
        {
            animation = "walk";
            //set animation in animator
        }
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
        if(animation != "chase")
        {
            animation = "chase";
            //set animation in animator
        }
        
    }

	public void MeleeAttack()
	{
        if(animation != "melee attack")
        {
            animation = "melee attack";
            //set animation in animator
        }
        //cuando acabe la animación se tiene que notificar con un animation event
        FinishMeleeAttackAnimation();
	}

	public void BallAttack()
	{
        if(animation != "ball attack")
        {
            animation = "ball attack";
            //set animation in animator
        }
        //cuando acabe la animación se tiene que notificar con un animation event
        FinishBallAttackAnimation();
	}

	public void IceSpikesAttack()
	{
        if (!showIceSpikes)
        {
            Vector3 bossPos = bossController.GetTheBoss().transform.position;
            if (returningFromSpikesCast == false)
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
                    //start animation
                    showIceSpikes = true;
                    returningFromSpikesCast = true;
                    
                }
            }

            if (returningFromSpikesCast == true)
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
                    //cuando acabe la animación se tiene que setear a false
                    FinishSpikesAttackAnimation();
                    returningFromSpikesCast = false;
                }
            }

        }
        if(showIceSpikes)
        {
            //Sacar los pinchos y dejarlos durante un tiempo
            showIceSpikes = false;                     
        } 
    }
    
    void Stalk()
    {
        animation = "stalk";
    }
}
