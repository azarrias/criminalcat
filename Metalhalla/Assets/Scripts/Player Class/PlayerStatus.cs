using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour {

    // colliders
    [Header("Colliders")]
    public BoxCollider attackCollider;

    // health variables
    [Header("Health Setup")]
    [Tooltip("Health start value")]
    public int healthAtStart = 100;
    [Tooltip("Health maximum value")]
    public int healthMaximum = 100;
    int health;

    // stamina variables
    [Header("Stamina Setup")]
    [Tooltip("Starting stamina value")]
    public int staminaAtStart = 10;
    [Tooltip("Stamina maximum value")]
    public int staminaMaximum = 10;
    [Tooltip("Stamina recovery rate per second")]
    public float staminaRecoveryRate = 1.0f;
    int stamina;
    float staminaRecovery; 

    // beer variables
    [Header("Beer Setup")]
    [Tooltip("Starting beer value")]
    public int beerAtStart = 1;
    [Tooltip("Stamina maximum value")]
    public int beerMaximum = 5;
    [Tooltip("Health recovery on beer consumption")]
    public int beerHealthRecovery = 20;
    int beer;

    [Header("Moveset Durations")]
    public float attackDuration = 0.400f;
    public float castDuration = 0.5f; 
    public float hitDuration = 0.5f;
    public float refillDuration = 0.9f;
    public float drinkDuration = 0.5f;
    public float fallCloudDuration = 0.3f;
    public float respawnLatency = 3.0f; // time between dead and alive again

    // -- State variables (using state pattern)
    public static AttackState attack;
    public static CastState cast;
    public static ClimbState climb;
    public static DeadState dead;
    public static DefenseState defense;
    public static DrinkState drink;
    public static FallState fall;
    public static FallCloudState fallcloud;
    public static HitState hit;
    public static IdleState idle;
    public static JumpState jump;
    public static RefillState refill;
    public static WalkState walk;

    [HideInInspector]
    public PlayerState currentState;
    [HideInInspector]
    public PlayerState previousState;

    [HideInInspector]
    public bool facingRight;
    [HideInInspector]
    public bool jumpAvailable;
    [HideInInspector]
    public bool climbLadderAvailable;
    [HideInInspector]
    public bool beerRefillAvailable;

    void Start()
    {
        attackCollider.enabled = false;
        attackCollider.GetComponent<Renderer>().enabled = false;    // to remove when finished debugging

        health = healthAtStart;
        stamina = staminaAtStart;
        staminaRecovery = 0.0f;
        beer = beerAtStart;

        attack = new AttackState(CalculateFramesFromTime(attackDuration));
        cast = new CastState(CalculateFramesFromTime(castDuration));
        climb = new ClimbState();
        dead = new DeadState();
        defense = new DefenseState();
        drink = new DrinkState(CalculateFramesFromTime(drinkDuration));
        fall = new FallState();
        fallcloud = new FallCloudState(CalculateFramesFromTime(fallCloudDuration));
        hit = new HitState(CalculateFramesFromTime(hitDuration));
        idle = new IdleState();
        jump = new JumpState(CalculateFramesFromTime(GetComponent<PlayerMove>().timeToJumpApex));
        refill = new RefillState(CalculateFramesFromTime(refillDuration));
        walk = new WalkState();

        SetState(idle);

        facingRight = true;
        jumpAvailable = true;
        climbLadderAvailable = false;
        beerRefillAvailable = false;

    }

    void Update()
    {
        // TODO - Remove this shortcuts when other entities and interactions are in place
        if (Input.GetKeyDown(KeyCode.F1) == true)
            ApplyDamage(5);
        if (Input.GetKeyDown(KeyCode.F2) == true)
            RestoreHealth(5);
        if (Input.GetKeyDown(KeyCode.F3) == true)
            ConsumeStamina(7);  
        if (Input.GetKeyDown(KeyCode.F4) == true)
            RestoreStamina(1);
        if (Input.GetKeyDown(KeyCode.F5) == true)
            ConsumeBeer(1);
        if (Input.GetKeyDown(KeyCode.F6) == true)
            RefillBeer(5);

        // stamina recovery
        if (stamina < staminaMaximum)
        {
            staminaRecovery += staminaRecoveryRate * Time.deltaTime;
            if (staminaRecovery >= 1.0f)
            {
                RestoreStamina(1);
                staminaRecovery = 0.0f;
            }
        }
    }


    // ---- STATE functions ---------------------------------------------------------------------------------------------
    public void statusUpdateAfterInput(PlayerInput input)
    {
        currentState.HandleInput(input, this);
    }

    public void statusUpdateAfterCollisionCheck(PlayerCollider collider, PlayerInput input)
    {
        currentState.UpdateAfterCollisionCheck(collider, this, input);
    }

    public void SetState(PlayerState newState)
    {
        previousState = currentState;
        currentState = newState;
    }



    // ---- HEALTH functions ---------------------------------------------------------------------------------------------
    public void ApplyDamage(int damage)
    {
        health -= damage;
        SetState(hit);
    }

    public bool RestoreHealth(int restore)
    {
        if (health == healthMaximum)    // cannot restore any more health
            return false;

        health += restore;
        if (health >= healthMaximum)
            health = healthMaximum;
        return true;
    }

    public float GetCurrentHealthRatio()
    {
        return (float)health / (float)healthMaximum;
    }

    public bool IsAlive()
    {
        return health > 0; 
    }
    // ---- STAMINA functions ---------------------------------------------------------------------------------------------
    public bool ConsumeStamina(int consumption)
    {
        if (stamina < consumption) // cannot use magic
            return false;
        stamina -= consumption;
        return true;
    }

    public void RestoreStamina(int restore)
    {
        stamina += restore;
        if (stamina >= staminaMaximum)
            stamina = staminaMaximum;
    }

    public int GetCurrentStamina() {
        return stamina;
    }


    // ---- BEER functions ---------------------------------------------------------------------------------------------
    public bool ConsumeBeer(int consumption)
    {
        if (beer == 0)
            return false;

        if (RestoreHealth(beerHealthRecovery) == false)     // no health recovery, no beer drink
            return false;

        beer -= consumption;
        return true;
    }

    public bool RefillBeer(int refill)
    {
        if (beer == beerMaximum)
            return false;
        beer += refill;
        if (beer > beerMaximum)
            beer = beerMaximum;

        return true;
    }

    public int GetCurrentBeer() {
        return beer;
    }


    // ---- UTIL functions ---------------------------------------------------------------------------------------------
    public bool IsGrounded()
    {
        //TODO - improve with collider collisions
        if (currentState == idle || currentState == walk || currentState == cast || currentState == dead || currentState == defense || currentState == drink || currentState == refill )
            return true;
        else
            return false;
    }

    public bool CanMoveHorizontally()
    {
        if (IsGrounded() == false)
            return true;
        if (currentState == idle || currentState == walk)
            return true;
        return false;
    }

    int CalculateFramesFromTime(float time) {
        return (int)(time / Time.fixedDeltaTime);
    }

    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 tmp = transform.localScale;
        tmp.x *= -1;
        transform.localScale = tmp;

    }

    // BOOLEAN STATE FUNCIONS
    public bool IsIdle() { return currentState == idle; }
    public bool IsWalk() { return currentState == walk; }
    public bool IsCast() { return currentState == cast; }
    public bool IsAttack() { return currentState == attack; }
    public bool IsDefense() { return currentState == defense; }
    public bool IsJump() { return currentState == jump; }
    public bool IsFall() { return currentState == fall; }
    public bool IsFallCloud() { return currentState == fallcloud; }
    public bool IsRefill() { return currentState == refill; }
    public bool IsDrink() { return currentState == drink;  }
    public bool IsClimb() { return currentState == climb; }
    public bool IsHit() { return currentState == hit; }
   
    public bool WasIdle() { return previousState == idle; }
    public bool WasWalk() { return previousState == walk; }

}
