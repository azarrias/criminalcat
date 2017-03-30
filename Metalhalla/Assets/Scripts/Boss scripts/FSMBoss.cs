using UnityEngine;


public class FSMBoss {

    private enum State
    {
        DAMAGED,
        DEAD,
        PATROL,
        ATTACK_SELECTION,
        CHASE,
        MELEE_ATTACK,
        BALL_ATTACK,
        ICE_SPIKES
    }

    public BossController bossController = null;
    private State currState;
    private State prevState;

    //------------------- VARIABLES WHICH CONTROL FSM LOGIC -------------------

    public bool playerInSight = false;
    public bool atMeleeRange = false;
    public bool atBallRange = false;
    public bool iceSpikesAttack = false;
    public bool ballAttack = false;   
    public bool meleeAttack = false;

    //damaged will be modified by the player
    public bool damaged = false;

    //orientation of the boss will be modified when entering triggers at the limits of the platform
    public bool facingRight = true;
    public int numSpikeAttacks = 0;

    //Temporalmente hasta que sepa cómo van las animaciones
    public string animation = "patrol";

    public FSMBoss(BossController bossContr)
	{
        bossController = bossContr;
        currState = State.PATROL;
	}

    public void Update()
    {
        switch(currState)
        {
            case State.DAMAGED:
                Damaged();
                if(bossController.GetHitPoints() <= 0)
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
                    currState = State.PATROL;
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
                    currState = State.PATROL;
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
                    currState = State.PATROL;
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
        }
       
    }

    // ------------------------------------- ACTIONS TO PERFORM IN EACH STATE --------------------------------------------
    public void Damaged()
    {
        animation = "damaged";
    }

    public void Dead()
    {
        animation = "dead";
    }

    public void Patrol()
	{
        Vector3 newPos = bossController.GetTheBoss().transform.position;

        if (facingRight == true)
            newPos.x += bossController.normalSpeed * Time.deltaTime;
        else
            newPos.x -= bossController.normalSpeed * Time.deltaTime;

        bossController.GetTheBoss().transform.position = newPos;

        animation = "walk";
    }

	public void Chase()
	{
        Vector3 newPos = bossController.GetTheBoss().transform.position;

        if (facingRight == true)
            newPos.x += bossController.chasingSpeed * Time.deltaTime;
        else
            newPos.x -= bossController.chasingSpeed * Time.deltaTime;

        bossController.GetTheBoss().transform.position = newPos;

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
        numSpikeAttacks++;       
        animation = "spike attack";        
        iceSpikesAttack = false;
    }

    public void SelectAttack()
    {
        if(bossController.GetHitPoints() > 0.75 * bossController.maxHitPoints)
        {
            meleeAttack = true;
        }
        else
        {
            if(bossController.GetHitPoints() <= 0.25 * bossController.maxHitPoints && numSpikeAttacks == 0)
            {
                iceSpikesAttack = true;
                numSpikeAttacks++;
            }

            else if (bossController.GetHitPoints() <= 0.10 * bossController.maxHitPoints && numSpikeAttacks == 1)
            {
                iceSpikesAttack = true;
                numSpikeAttacks++;
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

}
