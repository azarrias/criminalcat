using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour {

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
    int stamina;

    // beer variables
    [Header("Beer Setup")]
    [Tooltip("Starting beer value")]
    public int beerAtStart = 1;
    [Tooltip("Stamina maximum value")]
    public int beerMaximum = 5;
    [Tooltip("Health recovery on beer consumption")]
    public int beerHealthRecovery = 20;
    int beer;

    // -- State variables (using state pattern)
    public static AttackState attack;
    public static CastState cast;
    public static ClimbState climb;
    public static DeadState dead;
    public static DefenseState defense;
    public static DrinkState drink;
    public static FallState fall;
    public static HitState hit;
    public static IdleState idle;
    public static JumpState jump;
    public static RefillState refill;
    public static WalkState walk;

    [HideInInspector]
    public PlayerState currentState;
    [HideInInspector]
    public PlayerState previousState;

    int framesToFallThroughCloudPlatforms;
    int framesToFallThroughCloudPlatformsCount;

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
        health = healthAtStart;
        stamina = staminaAtStart;
        beer = beerAtStart;

        attack = new AttackState(CalculateFramesFromTime(GetComponent<PlayerMove>().attackDuration));
        cast = new CastState();
        climb = new ClimbState();
        dead = new DeadState();
        defense = new DefenseState();
        drink = new DrinkState(CalculateFramesFromTime(GetComponent<PlayerMove>().drinkDuration));
        fall = new FallState();
        hit = new HitState();
        idle = new IdleState();
        jump = new JumpState(CalculateFramesFromTime(GetComponent<PlayerMove>().timeToJumpApex));
        refill = new RefillState(CalculateFramesFromTime(GetComponent<PlayerMove>().refillDuration));
        walk = new WalkState();

        SetState(idle);

        facingRight = true;
        jumpAvailable = true;
        climbLadderAvailable = false;
        beerRefillAvailable = false;

    }

    // TODO - Remove this method when finished testing
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) == true)
            ApplyDamage(5);
        if (Input.GetKeyDown(KeyCode.F2) == true)
            RestoreHealth(5);
        if (Input.GetKeyDown(KeyCode.F3) == true || Input.GetButtonDown("Cast") == true)
            ConsumeStamina(7);  // like a cast
        if (Input.GetKeyDown(KeyCode.F4) == true)
            RestoreStamina(1);
        if (Input.GetKeyDown(KeyCode.F5) == true || Input.GetButtonDown("Context") == true)
            ConsumeBeer(1);
        if (Input.GetKeyDown(KeyCode.F6) == true)
            RefillBeer(5);
    }


    // ---- STATE functions ---------------------------------------------------------------------------------------------
    public void statusUpdateAfterInput(PlayerInput input)
    {
        currentState.HandleInput(input, this);
    }

    public void statusUpdateAfterCollisionCheck(PlayerCollider collider)
    {
        currentState.UpdateAfterCollisionCheck(collider, this);
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
        if (health <= 0)
            health = 0; // TODO: improve when making full player FSM
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
        if (currentState == idle || currentState == walk || currentState == cast || currentState == dead || currentState == defense || currentState == drink || currentState == refill || currentState == attack)
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
    public bool IsRefill() { return currentState == refill; }
    public bool IsDrink() { return currentState == drink;  }

}
