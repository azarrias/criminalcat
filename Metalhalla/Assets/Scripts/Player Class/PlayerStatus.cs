using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour
{
    [Header("Full model elements")]
    public GameObject playerModel;
    Quaternion playerModelDefaultRotation;
    Quaternion playerModelClimbRotation;

    [Header("Attack Elements")]
    public GameObject hammerMesh;
    public BoxCollider attackCollider;
    public GameObject lightningGenerator;
    public Vector3 eagleAttackInstanceOffset = new Vector3(1.0f, -0.7f, 0);
    public Vector3 wildboarAttackInstanceOffset = new Vector3(1.0f, -0.7f, 0);

    [Header("Defense Elements")]
    public GameObject shieldMesh;

    [Header("Sound Effects")]
    public AudioClip fxSwing;
    public AudioClip fxJump;
    public AudioClip fxLand;
    public AudioClip fxTornado;
    public AudioClip[] leftFootsteps;
    public AudioClip[] rightFootsteps;
    //    AudioSource playerAudioSource;

    [HideInInspector]
    public Animator playerAnimator;

    [Header("Health Setup")]
    [Tooltip("Health start value")]
    public int healthAtStart = 100;
    [Tooltip("Health maximum value")]
    public int healthMaximum = 100;
    int health;

    [Header("Stamina Setup")]
    [Tooltip("Starting stamina value")]
    public int staminaAtStart = 4;
    [Tooltip("Stamina maximum value")]
    public int staminaMaximum = 10;
    [Tooltip("Stamina recovery rate per second")]
    public float staminaRecoveryRate = 0.0f;
    int stamina;
    float staminaRecovery;

    [Header("Beer Setup")]
    [Tooltip("Starting beer value")]
    public int beerAtStart = 1;
    [Tooltip("Stamina maximum value")]
    public int beerMaximum = 5;
    [Tooltip("Health recovery on beer consumption")]
    public int beerHealthRecovery = 20;
    int beer;

    [Header("Ghost Jump parameters")]
    [Tooltip("Number of frames in which the player can still jump after losing foot in their last platform")]
    public int framesToJumpInDelay = 5;

    [Header("Moveset Durations")]
    public float attackDuration = 0.5f;
    public float castDuration = 0.5f;
    public float hitDuration = 0.4f;
    public float refillDuration = 1.5f;
    public float drinkDuration = 0.5f;
    public float fallCloudDuration = 0.3f;
    public float deadDuration = 3.0f;
    public float dashDuration = 0.4f;

    [Header("Respawn management")]
    public Vector3 initialPosition;
    [HideInInspector]
    public Vector3 activeRespawnPoint;
    private CameraFade cameraFade;

    // -- State variables (using state pattern) -- //
    public static AttackState attack;
    public static CastState cast;
    public static ClimbState climb;
    public static DashState dash; 
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

    // -- Variables shared between different states -- // 
    [HideInInspector]
    public bool facingRight;
    [HideInInspector]
    public bool jumpAvailable;
    [HideInInspector]
    public bool climbLadderAvailable;
    [HideInInspector]
    public bool beerRefillAvailable;

    // -- Cross component variables -- // 
    [HideInInspector]
    public bool justHit;
    [HideInInspector]
    public int jumpFrames;
    [HideInInspector]
    public int framesInDelayCount;

    // -- Camera shake variables -- // 
    private CameraFollow camFollow;

    // -- Collider adjustment variables -- // 
    private Vector3 colliderSize;

    // -- Debug variables -- // 
    private bool godMode = false;

    void Start()
    {
        playerModelDefaultRotation = playerModel.transform.localRotation;
        playerModelClimbRotation = Quaternion.identity;

        hammerMesh.GetComponent<Renderer>().enabled = false;    // change if the hammer is to be visible always
        attackCollider.enabled = false;
        attackCollider.GetComponent<Renderer>().enabled = false;    // to remove when finished debugging
        lightningGenerator.SetActive(false);    // temp

        shieldMesh.GetComponent<Renderer>().enabled = false;

        playerAnimator = GetComponent<Animator>();

        health = healthAtStart;
        stamina = staminaAtStart;
        staminaRecovery = 0.0f;
        beer = beerAtStart;

        activeRespawnPoint = initialPosition;
        cameraFade = GameObject.Find("PlayerCamera").GetComponent<CameraFade>();

        attack = new AttackState(CalculateFramesFromTime(attackDuration));
        cast = new CastState(CalculateFramesFromTime(castDuration));
        climb = new ClimbState();
        dash = new DashState(CalculateFramesFromTime(dashDuration));
        dead = new DeadState(CalculateFramesFromTime(deadDuration));
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

        justHit = false;
        jumpFrames = 0;
        framesInDelayCount = 0;

        camFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();

        colliderSize = GetComponent<BoxCollider>().size;

        godMode = false;

    }

    void Update()
    {
        // TODO - Remove this shortcuts when other entities and interactions are in place
        if (Input.GetKeyDown(KeyCode.F1) == true)
            ApplyDamage(30);
        if (Input.GetKeyDown(KeyCode.F2) == true)
            RestoreHealth(30);
        if (Input.GetKeyDown(KeyCode.F3) == true)
            ConsumeStamina(7);
        if (Input.GetKeyDown(KeyCode.F4) == true)
            RestoreStamina(1);
        if (Input.GetKeyDown(KeyCode.F5) == true)
            ConsumeBeer(1);
        if (Input.GetKeyDown(KeyCode.F6) == true)
            RefillBeer(5);
        if (Input.GetKeyDown(KeyCode.G) == true)
            godMode = !godMode;

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
        if (newState != attack)
        {
            attackCollider.enabled = false;
            attackCollider.GetComponent<Renderer>().enabled = false;
        }
        else if (newState != defense)
        {
            shieldMesh.GetComponent<Renderer>().enabled = false; 
        }
    }



    // ---- HEALTH functions ---------------------------------------------------------------------------------------------
    public void ApplyDamage(int damage)
    {
        if (!godMode && currentState != dead)
            camFollow.StartShake();
        if (!godMode && currentState != defense && currentState != dead)
        {
            health -= damage;
            SetState(hit);
        }
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

    public int GetCurrentHealth()
    {
        return health;
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    public void SetMaxHealth()
    {
        health = healthMaximum;
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

    public int GetCurrentStamina()
    {
        return stamina;
    }


    // ---- BEER functions ---------------------------------------------------------------------------------------------
    public bool ConsumeBeer(int consumption)
    {
        if (beer == 0)
            return false;

        //UPDATE: allowed to drink even if health is full for the sake of the animation
        /*
         * if (RestoreHealth(beerHealthRecovery) == false)     // no health recovery, no beer drink
             return false;
         */
        RestoreHealth(beerHealthRecovery);

        beer -= consumption;
        if (beer <= 0)
            beer = 0;
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

    public int GetCurrentBeer()
    {
        return beer;
    }


    // ---- UTIL functions ---------------------------------------------------------------------------------------------
    int CalculateFramesFromTime(float time)
    {
        return (int)(time / Time.fixedDeltaTime);
    }

    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 tmp = transform.localScale;
        tmp.x *= -1;
        transform.localScale = tmp;
    }

    public void SetPlayerAtRespawnPoint()
    {
        transform.position = activeRespawnPoint;
    }

    public void StartRespawnCameraFade()
    {
        cameraFade.ActivateFade();
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
    public bool IsDrink() { return currentState == drink; }
    public bool IsClimb() { return currentState == climb; }
    public bool IsHit() { return currentState == hit; }
    public bool IsDash() { return currentState == dash;  }
    public bool IsDead() { return currentState == dead; }

    public bool WasIdle() { return previousState == idle; }
    public bool WasWalk() { return previousState == walk; }

    // ---- SOUND functions ---------------------------------------------------------------------------------------------
    public void PlayFx( string fx)
    {
        if (fx.Equals("swing"))
            AudioManager.instance.PlayFx(fxSwing);
        else if (fx.Equals("jump"))
            AudioManager.instance.PlayFx(fxJump);
        else if (fx.Equals("land"))
            AudioManager.instance.PlayFx(fxLand);
        else if (fx.Equals("tornado"))
            AudioManager.instance.PlayFx(fxTornado);
        else if (fx.Equals("leftFootstep"))
            AudioManager.instance.RandomizePlayFx(leftFootsteps);
        else if (fx.Equals("rightFootstep"))
            AudioManager.instance.RandomizePlayFx(rightFootsteps);
    }

    // ---- MODEL ROTATION functions --------------------------------------------------------------------------------------------
    public void SetClimbStateModelRotation()
    {
        playerModel.transform.localRotation = playerModelClimbRotation;
    }

    public void SetInitialModelRotation()
    {
        playerModel.transform.localRotation = playerModelDefaultRotation;
    }

    // ---- GHOST JUMP functions --------------------------------------------------------------------------------------------------
    public bool IsGhostJumpAvailable()
    {
        return framesInDelayCount <= framesToJumpInDelay;
    }

    // ---- DEAD STATE COLLIDER adjustment functions ------------------------------------------------------------------------------
    public void RestoreColliderSize()
    {
        GetComponent<BoxCollider>().size = colliderSize;
    }

    public void SetColliderYSize( float ySize )
    {
        Vector3 tmpSize = colliderSize;
        tmpSize.y = ySize;
        GetComponent<BoxCollider>().size = tmpSize;
    }


}
