using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineOfSight))]
[RequireComponent(typeof(EnemyStats))]
public class FSMDarkElf : MonoBehaviour
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

    [Header("Sound FXs")]
    public AudioClip fireBall;
    public AudioClip[] hurtScream;

    private float minPatrolDistance = 0.0f;
    private Vector3 destination = Vector3.zero;
    private float waitingTime;
    private float timeToWait;
    private Material material;
    private BoxCollider[] boxColliders;

    //variables for camera shake
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
 /*       foreach (BoxCollider b in boxColliders)
        {
            if (b.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
            {
                b.enabled = false;
            }
        }
        */

        // get camera component to create Shake effect when being hit
        camFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();
        if (camFollow == null)
            Debug.Log("FSM Dark Elf: could not find the component CameraFollow in the MainCamera of the scene");
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
                else
                    ChangeState(State.IDLE);
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
                /*
                if (waitingTime > timeToWait)
                {
                    if (enemyStats.hitPoints > 0)
                        ChangeState(State.CHASE);
                    else ChangeState(State.DEAD);
                }
                else
                    waitingTime += Time.fixedDeltaTime;
                    */
                break;
            case State.CHASE:
                if (!los.playerInSight)
                    ChangeState(State.PATROL);
                else if (PlayerAtRange())
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
                /*waitingTime = 0.0f;
                timeToWait = 1.0f;*/
                faceXCoordinate(player.transform.position.x);
                break;
            case State.STUNNED:
                animator.SetBool("idle", true);
                break;
            case State.CHASE:
                animator.SetBool("walk", true);
                destination.Set(player.transform.position.x, transform.position.y, transform.position.z);
                faceXCoordinate(destination.x);
                los.enabled = true;
                break;
            case State.ATTACK:
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
                los.enabled = false;
                break;
            case State.ATTACK:
                animator.SetBool("attack", false);
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
            Debug.Log(name.ToString() + ": I've been hit");
            ChangeState(State.BEING_HIT);
            AudioManager.instance.RandomizePlayFx(hurtScream);
            // camera shake when starting being hit state
            camFollow.StartShake();
            GameObject blood = ParticlesManager.SpawnParticle("blood", transform.position + 2 * Vector3.back, facingRight);  // blood positioning has to be improved
            //blood.transform.parent = transform;
            blood.transform.SetParent(transform);
        }
    }

    public void Stunt()
    {
        Debug.Log(name.ToString() + ": I've been stunned");
        ChangeState(State.STUNNED);
    }

    public void WakeUp()
    {
        if (currentState == State.STUNNED)
            ChangeState(State.IDLE);
    }

    public bool PlayerAtRange()
    {
/*        if (Mathf.Abs(los.player.transform.position.y - transform.position.y) > 1.0f)
            return false;*/
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

    private void ThrowFireBall()
    {
        float fbOffsetX = 0.6f;
        float fbOffsetY = 0.3f;
        Vector3 iniPos;

        if (transform.localEulerAngles.y == 180)
        {
            iniPos = new Vector3(transform.position.x + fbOffsetX, transform.position.y - fbOffsetY, transform.position.z);
        }
        else
        {
            iniPos = new Vector3(transform.position.x - fbOffsetX, transform.position.y - fbOffsetY, transform.position.z);
        }

        AudioManager.instance.PlayFx(fireBall);
        Vector3 direction;
        if ((facingRight == true && player.transform.position.x >= iniPos.x) || (facingRight == false && player.transform.position.x <= iniPos.x))
            direction = player.transform.position - iniPos;
        else
            direction = facingRight ? Vector3.right : Vector3.left;
        ParticlesManager.SpawnElfFireBall(iniPos, direction);
        //ParticlesManager.SpawnElfFireBall(iniPos, player.transform.position - iniPos);
    } 

    IEnumerator WaitForSeconds(float s)
    {
        yield return new WaitForSeconds(s);
    }

    // ---- DEAD STATE COLLIDER adjustment functions ------------------------------------------------------------------------------
    public void SetGameObjectXTranslation(float x)
    {
        Vector3 tmp = transform.position;
        float sign = facingRight ? -1 : 1;
        tmp.x += sign * x;
        transform.position = tmp;
    }

    public void SetGameObjectYTranslation( float y)
    {
        Vector3 tmp = transform.position;
        tmp.y += y;
        transform.position = tmp; 
    }

    public void SetColliderXTranslation(float x)
    {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        Vector3 tmp = capsule.center;
        tmp.x = x;
        capsule.center = tmp;
    }

    public void SetColliderYTranslation(float y)
    {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        Vector3 tmp = capsule.center;
        tmp.y = y;
        capsule.center = tmp;

    }
    public void SetColliderRadius(float r)
    {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        capsule.radius = r;
    }

    public void SetColliderHeight(float h)
    {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        capsule.height = h;
    }

    public void SetColliderDirection( int direction)
    {
        // direction (0,1,2) = (X, Y, Z) respectively
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        capsule.direction = direction;
    }

}
