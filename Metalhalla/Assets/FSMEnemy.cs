using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineOfSight))]
[RequireComponent(typeof(EnemyStats))]
public class FSMEnemy : MonoBehaviour
{
    public enum State
    {
        STUNNED,
        IDLE,
        PATROL,
        CHASE,
        ATTACK,
        BEING_HIT,
        DEAD
    }

    [HideInInspector]
    public LineOfSight los;
    [HideInInspector]
    public State currentState;
    [HideInInspector]
    public EnemyStats enemyStats;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public PlayerStatus playerStatus;

    Animator animator;

    [Header("Patrol Limits")]
    [Tooltip("X world coordinate to be set as a left limit for this enemy")]
    public float leftPatrolLimit;
    [Tooltip("X world coordinate to be set as a right limit for this enemy")]
    public float rightPatrolLimit;
    public float attackRange;

    private float minPatrolDistance = 0.0f;
    private Vector3 destination = Vector3.zero;
    private float waitingTime;
    private float timeToWait;
    private Material material;
    private BoxCollider[] boxColliders;

    private float patrol2chaseSpeedRatio;
    private CameraFollow camFollow;

    // Recoil variables 
    [HideInInspector]
    public bool facingRight;
    private float hitRecoil;
    private float deadRecoil;

    private void Awake()
    {
        los = GetComponent<LineOfSight>();
        enemyStats = GetComponent<EnemyStats>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerStatus = player.GetComponent<PlayerStatus>();
        material = GetComponentInChildren<Renderer>().material;
        boxColliders = GetComponentsInChildren<BoxCollider>();

        patrol2chaseSpeedRatio = enemyStats.chasingSpeed / enemyStats.normalSpeed;
        hitRecoil = enemyStats.hitRecoil;
        deadRecoil = enemyStats.deadRecoil;

    }

    private void Start()
    {
        los.enabled = false;
        minPatrolDistance = (rightPatrolLimit - leftPatrolLimit) / 3.0f;
        currentState = State.IDLE;
        StateEnter(currentState);

        // disable attack colliders
        foreach (BoxCollider b in boxColliders)
        {
            if (b.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
            {
                b.enabled = false;
            }
        }

        // get camera component to create Shake effect when being hit
        camFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();
        if (camFollow == null)
            Debug.Log("FSM Enemy: could not find the component CameraFollow in the MainCamera of the scene");
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.IDLE:
                if (waitingTime > timeToWait)
                    ChangeState(State.PATROL);
                else
                    waitingTime += Time.fixedDeltaTime;
                break;
            case State.PATROL:
                if (los.playerInSight)
                    ChangeState(State.CHASE);
                else if (Vector3.Distance(transform.position, destination) > 0.1f)
                    transform.position = Vector3.MoveTowards(transform.position, destination, Time.fixedDeltaTime * enemyStats.normalSpeed);
                else ChangeState(State.IDLE);
                break;
            case State.BEING_HIT:
                if (enemyStats.hitPoints <= 0)
                    ChangeState(State.DEAD);
                else if (animator.GetCurrentAnimatorStateInfo(0).IsName("BeingHit") &&
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                {
                    ChangeState(State.CHASE);
                }
                else
                {
                    int direction = facingRight ? -1 : 1;
                    transform.position += new Vector3(direction * Time.fixedDeltaTime * hitRecoil, 0, 0);
                }
                    /*if (animator.GetCurrentAnimatorStateInfo(0).IsName("BeingHit") &&
                            animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                    {
                        if (enemyStats.hitPoints > 0)
                            ChangeState(State.CHASE);
                        else ChangeState(State.DEAD);
                    }*/
            break;

            case State.CHASE:
                if (PlayerAtRange())
                    ChangeState(State.ATTACK);
                else if (!InBounds())
                {
                    if ((player.transform.position.x - transform.position.x) > 0.0f)
                    {
                        faceXCoordinate(transform.position.x - 3.0f);
                    }
                    else faceXCoordinate(transform.position.x + 3.0f); 
                    ChangeState(State.IDLE);
                }
                else if (Vector3.Distance(transform.position, destination) > 0.1f)
                    transform.position = Vector3.MoveTowards(transform.position, destination, Time.fixedDeltaTime * enemyStats.chasingSpeed);
                else ChangeState(State.IDLE);
                break;
            case State.ATTACK:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                {
                    if (!playerStatus.IsAlive())
                        ChangeState(State.PATROL);
                    else ChangeState(State.CHASE);
                }
                break;
            case State.DEAD:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dead") &&
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                {
                    StateExit(State.DEAD);
                } 
                break;
        }
    }

    private void ChangeState(State newState)
    {
        if (newState != currentState)
        {
            StateExit(currentState);
            currentState = newState;
            StateEnter(newState);
        }
    }

    private void StateEnter(State state)
    {
        animator.speed = 1; 
        switch (state)
        {
            case State.IDLE:
                animator.SetBool("idle", true);
                waitingTime = 0.0f;
                timeToWait = Random.Range(0.5f, 1.5f);
            break;

            case State.PATROL:
                animator.SetBool("walk", true);
                destination.Set(NextPatrolLocation(), transform.position.y, transform.position.z);
                faceXCoordinate(destination.x);
                los.enabled = true;
            break;

            case State.BEING_HIT:
                animator.SetBool("being_hit", true);
                faceXCoordinate(player.transform.position.x);
            break;

            case State.STUNNED:
                animator.SetBool("idle", true);
            break;

            case State.CHASE:
                animator.SetBool("walk", true);
                //  animator.speed = enemyStats.chasingSpeed;
                animator.speed = patrol2chaseSpeedRatio;
                destination.Set(player.transform.position.x, transform.position.y, transform.position.z);
                faceXCoordinate(destination.x);
                los.enabled = true;
            break;

            case State.ATTACK:
                foreach (BoxCollider b in boxColliders)
                {
                    if (b.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
                    {
                        b.enabled = true;
                    }
                }
                animator.SetBool("attack", true);
            break;

            case State.DEAD:
                animator.SetBool("dead", true);
                int direction = facingRight ? -1 : 1;
                transform.position += new Vector3(direction * deadRecoil, 0, 0);
                break;
        }
    }

    private void StateExit(State state)
    {
        switch (state)
        {
            case State.IDLE:
                animator.SetBool("idle", false);
                break;
            case State.PATROL:
                animator.SetBool("walk", false);
                los.enabled = false;
                break;
            case State.BEING_HIT:
                animator.SetBool("being_hit", false);
                break;
            case State.STUNNED:
                animator.SetBool("idle", false);
                break;
            case State.CHASE:
                animator.SetBool("walk", false);
                animator.speed = enemyStats.normalSpeed;
                los.enabled = false;
                break;
            case State.ATTACK:
                animator.SetBool("attack", false);
                foreach (BoxCollider b in boxColliders)
                {
                    if (b.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
                    {
                        b.enabled = false;
                    }
                }
                break;
            case State.DEAD:
                animator.SetBool("dead", false);
                StartCoroutine(FadeOut());
                break;
        }
    }

    private float NextPatrolLocation()
    {
        if ((transform.position.x + minPatrolDistance) > rightPatrolLimit)
            return Random.Range(leftPatrolLimit, transform.position.x - minPatrolDistance);
        else if ((transform.position.x - minPatrolDistance) < leftPatrolLimit)
            return Random.Range(transform.position.x + minPatrolDistance, rightPatrolLimit);
        else
            return Random.Range(0, 2) == 0 ? Random.Range(leftPatrolLimit, transform.position.x - minPatrolDistance)
                                        : Random.Range(transform.position.x + minPatrolDistance, rightPatrolLimit);
    }

    private int faceXCoordinate(float xcoord)
    {
        if (xcoord > transform.position.x)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
            facingRight = true;
            return -1;
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
            facingRight = false; 
            return 1;
        }
    }

    public bool InBounds()
    {
        return transform.position.x > leftPatrolLimit && transform.position.x < rightPatrolLimit;
    }

    public void ApplyDamage(int damage)
    {
        if (currentState != State.STUNNED)
        {
          //  Debug.Log(name.ToString() + ": I've been hit");
            ChangeState(State.BEING_HIT);
            // camera shake when starting being hit state
            camFollow.StartShake();
            GameObject blood = ParticlesManager.SpawnParticle("blood", transform.position + 2*Vector3.back, facingRight);  // blood positioning has to be improved
            //blood.transform.parent = transform;
            blood.transform.SetParent(transform);
        }
    }

    public void Stun()
    {
      //  Debug.Log(name.ToString() + ": I've been stunned");
        ChangeState(State.STUNNED);
    }

    public void WakeUp()
    {
        if (currentState == State.STUNNED)
            ChangeState(State.IDLE);
    }

    public bool PlayerAtRange()
    {
        if (Mathf.Abs(los.player.transform.position.y - transform.position.y) > 1.0f)
            return false;
        if (los.player.transform.position.x > transform.position.x)
            return los.player.transform.position.x - attackRange - transform.position.x <= 0.2f;
        else
            return transform.position.x - attackRange - los.player.transform.position.x <= 0.2f;
    }

    IEnumerator FadeOut()
    {
        float timeToFade = 5.0f;

        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        foreach (BoxCollider b in boxColliders)
        {
            b.enabled = false;
        }

        while (material.color.a > 0)
        {
            Color newColor = material.color;
            newColor.a -= Time.deltaTime / timeToFade;
            material.color = newColor;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    // ---- DEAD STATE COLLIDER adjustment functions ------------------------------------------------------------------------------
    public void SetColliderXTranslation(float x) {
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 tmp = box.center;
        tmp.x = x;
        box.center = tmp;
    }

    public void SetColliderYTranslation(float y) {
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 tmp = box.center;
        tmp.y = y;
        box.center = tmp;

    }
    public void SetColliderXSize(float xsize) {
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 tmp = box.size;
        tmp.x = xsize;
        box.size = tmp;
    }

    public void SetColliderYSize(float ysize) {
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 tmp = box.size;
        tmp.y = ysize;
        box.size = tmp;
    }

}
