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
    public GameObject shield;
    public GameObject shieldMesh;
    public BoxCollider shieldCollider;
    public Transform shieldFrontTransform;
    public Transform shieldUpTransform;

    [Header("Horn Elements")]
    public GameObject hornMesh;

    [Header("Sound Effects")]
    public AudioClip fxSwing;
    public AudioClip fxJump;
    public AudioClip fxLand;
    public AudioClip fxRestoreLife;
    public AudioClip fxRestoreBeer;
    public AudioClip fxTornado;
    public AudioClip fxWildboar;
    public AudioClip[] leftFootsteps;
    public AudioClip[] rightFootsteps;
    public AudioClip[] hurtScream;
    public AudioClip deathScream;

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

    [Header("Coin Setup")]
    [Tooltip("Starting coin count")]
    public int coinsAtStart = 0;
    [Tooltip("How many coins are needed to fill a section of the beer horn?")]
    public int beerSectionCoinAmount = 10;
    int coins;

    [HideInInspector]
    public enum MAGIC { EAGLE = 0, WILDBOAR, numMagics };
    [HideInInspector]
    public MAGIC magic;

    private GUIManager guiManager;

    [Header("Ghost Jump parameters")]
    [Tooltip("Number of frames in which the player can still jump after losing foot in their last platform")]
    public int framesToJumpInDelay = 5;

    [Header("Jump from ladder parameters")]
    [Tooltip("Number of frames at the start of the jump that the player won't be attaching to the ladder in order to be able to jump from it")]
    public int framesToJumpFromLadder = 15;

    [Header("Moveset Durations")]
    public float attackDuration = 0.5f;
    public float castDuration = 0.5f;
    public float hitDuration = 0.4f;
    public float refillDuration = 1.5f;
    public float drinkDuration = 0.5f;
    public float fallCloudDuration = 0.3f;
    public float deadDuration = 3.0f;
    public float dashDuration = 0.4f;

    [Header("Respawn parameters")]
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

        attackCollider.enabled = false;
        attackCollider.GetComponent<Renderer>().enabled = false;    // to remove when finished debugging
        lightningGenerator.SetActive(false);    // temp

        shieldCollider.enabled = false;
        ShowShield(false);

        ShowHorn(false);

        playerAnimator = GetComponent<Animator>();

        GameObject.FindGameObjectWithTag("GameSession").GetComponent<SavePlayerState>().RecoverPlayerStatusValues( this);
        staminaRecovery = 0.0f;

        GameObject guiObject = GameObject.Find("GUI");
        if (guiObject)
            guiManager = guiObject.GetComponent<GUIManager>();

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
        if (stamina == 0)
        {
            staminaRecovery += staminaRecoveryRate * Time.deltaTime;
            if (staminaRecovery >= 1.0f)
            {
                RestoreStamina(1);
                staminaRecovery = 0.0f;
            }
        }
    }

    // ---- RESPAWN functions ------------------------------------------------------------------------------------------
    public void ReSpawn()
    {
        RestoreColliderSize();   // to restore the animation event when sigmund falls on his knees
        if (facingRight == false)
            Flip();
        SetPlayerAtRespawnPoint();
        SetMaxHealth();
        stamina = staminaAtStart;
        beer = beerAtStart;
        SetState(PlayerStatus.idle);

        // add hoc for level elements 
        GameObject movingDoor = GameObject.FindGameObjectWithTag("MovingDoor");
        if (movingDoor)
            movingDoor.GetComponent<CloseOpenDoor>().OpenDoor();

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

        // show hammer always except when climbing, being hit or dead
        if (newState != climb && newState != hit && newState != dead && newState != defense && newState != drink)
            ShowHammer(true);
        else
            ShowHammer(false);

        if (newState != attack)
            attackCollider.enabled = false;
        if (newState != defense)
            ShowShield(false);

        if (newState != drink)
            ShowHorn(false);

        if (IsClimb() && WasClimb() == false)
            SetClimbStateModelRotation();
        else if (WasClimb() && IsClimb() == false)
        {
            SetInitialModelRotation();
            playerAnimator.speed = 1;
        }
    }



    // ---- HEALTH functions ---------------------------------------------------------------------------------------------
    public void ApplyDamage(int damage)
    {
        if (!godMode && currentState != dead)
            camFollow.StartShake();

        if (!godMode && currentState != dead)
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

        PlayFx("restoreHealth");
        return true;
    }

    public float GetCurrentHealthRatio()
    {
        return (float)health / (float)healthMaximum;
    }

    public void SetCurrentHealth(int value)
    {
        health = value;
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

    public void SetCurrentStamina(int value)
    {
        stamina = value;
    }

    // ---- BEER functions ---------------------------------------------------------------------------------------------
    public bool ConsumeBeer(int consumption)
    {
        if (beer == 0)
            return false;

        if (RestoreHealth(beerHealthRecovery) == false)
            return false;

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

        PlayFx("restoreBeer");
        return true;
    }

    public int GetCurrentBeer()
    {
        return beer;
    }

    public void SetCurrentBeer(int value)
    {
        beer = value; 
    }

    //---- COIN functions -------------------------------------------------------------------------------------------
    public void CollectCoins(int amount) {
        coins += amount;
        while (coins >= beerSectionCoinAmount)
        {
            if (RefillBeer(1) == false)
                break;
            coins -= beerSectionCoinAmount;
        }
    }

    public int GetCurrentCoins()
    {
        return coins; 
    }

    public void SetCurrentCoins(int value)
    {
        coins = value; 
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
    public bool IsDash() { return currentState == dash; }
    public bool IsDead() { return currentState == dead; }

    public bool WasIdle() { return previousState == idle; }
    public bool WasWalk() { return previousState == walk; }
    public bool WasJump() { return previousState == jump; }
    public bool WasFall() { return previousState == fall; }
    public bool WasDead() { return previousState == dead; }
    public bool WasClimb() { return previousState == climb; }

    // ---- SOUND functions ---------------------------------------------------------------------------------------------
    public void PlayFx(string fx)
    {
        if (fx.Equals("swing"))
            AudioManager.instance.PlayDiegeticFx(fxSwing);
        else if (fx.Equals("jump"))
            AudioManager.instance.PlayDiegeticFx(fxJump);
        else if (fx.Equals("land"))
            AudioManager.instance.PlayDiegeticFx(fxLand);
        else if (fx.Equals("tornado"))
            AudioManager.instance.PlayDiegeticFx(fxTornado);
        else if (fx.Equals("wildboar"))
            AudioManager.instance.PlayDiegeticFx(fxWildboar);
        else if (fx.Equals("leftFootstep"))
            AudioManager.instance.RandomizePlayFx(leftFootsteps);
        else if (fx.Equals("rightFootstep"))
            AudioManager.instance.RandomizePlayFx(rightFootsteps);
        else if (fx.Equals("hurtScream"))
            AudioManager.instance.RandomizePlayFx(hurtScream);
        else if (fx.Equals("restoreBeer"))
            AudioManager.instance.PlayNonDiegeticFx(fxRestoreBeer);
        else if (fx.Equals("restoreHealth"))
            AudioManager.instance.PlayNonDiegeticFx(fxRestoreLife);
        else if (fx.Equals("death"))
            AudioManager.instance.PlayDiegeticFx(deathScream);
    }

    // --- ANIMATION LAYER MANAGEMENT functions ---------------------------------------------------------------------------------
    public void ResetAnimationLayerWeights()
    {
        playerAnimator.SetLayerWeight(1, 0f);
    }

    public void SetPlayerDefenseAnimationLayerWeight(float weight)
    {
        playerAnimator.SetLayerWeight(1, weight);
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

    public void SetColliderYSize( float ySize = 1.1f)
    {
        Vector3 tmpSize = colliderSize;
        tmpSize.y = ySize;
        GetComponent<BoxCollider>().size = tmpSize;
    }

    // ---- SHIELD transform functions - to be used from player DefenseState
    public void SetShieldTransform( float verticalInput, float horizontalInput)
    {
        if (verticalInput <= 0)
        {
            shield.transform.position = shieldFrontTransform.position;
            shield.transform.rotation = shieldFrontTransform.rotation;
            SetPlayerDefenseAnimationLayerWeight(0f);
            return;
        }
        if (facingRight == true && horizontalInput <= 0)
            horizontalInput = 0;
        else if (facingRight == false && horizontalInput >= 0)
            horizontalInput = 0; 

        float angle = Mathf.Atan(verticalInput / horizontalInput);
        float maxAngle = Mathf.PI / 2f;
        float lambda = Mathf.Abs(angle / maxAngle);

        shield.transform.position = Vector3.Slerp(shieldFrontTransform.position, shieldUpTransform.position, lambda); 
        Quaternion rot; 
        rot.x = shieldFrontTransform.rotation.x * (1 - lambda) + shieldUpTransform.rotation.x * lambda;
        rot.y = shieldFrontTransform.rotation.y * (1 - lambda) + shieldUpTransform.rotation.y * lambda;
        rot.z = shieldFrontTransform.rotation.z * (1 - lambda) + shieldUpTransform.rotation.z * lambda;
        rot.w = shieldFrontTransform.rotation.w * (1 - lambda) + shieldUpTransform.rotation.w * lambda;
        
        shield.transform.rotation = rot;

        SetPlayerDefenseAnimationLayerWeight(lambda);
    }

    // ---- GUI SYNC functions ----------------------------------------------------------------------------------------------------------
    public void StartGUIFeedback( string element )
    {
        if (guiManager)
            guiManager.StartFeedback(element);
    }

    // ---- SHOW MESHES functions -----------------------------------------------------------------------------------------------------
    public void ShowHorn(bool visible)
    {
        hornMesh.GetComponent<Renderer>().enabled = visible;
    }

    public void ShowShield(bool visible)
    {
        shieldMesh.GetComponent<Renderer>().enabled = visible;
    }

    public void ShowHammer(bool visible)
    {
        hammerMesh.GetComponent<Renderer>().enabled = visible;
    }

    // ---- RESPAWN MANAGEMENT functions ------------------------------------------------------------------------------------------------
    public bool SetRespawnPoint(Vector3 newRespawnPoint)
    {
        if (newRespawnPoint == activeRespawnPoint)
            return false;

        activeRespawnPoint = newRespawnPoint;
        return true;
    }
        
}
