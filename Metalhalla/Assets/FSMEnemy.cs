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
    public State currentState;
    [HideInInspector]
    public EnemyStats enemyStats;

    Animator animator;

    [Tooltip("X world coordinate to be set as a left limit for this enemy")]
    public float leftPatrolLimit;
    [Tooltip("X world coordinate to be set as a right limit for this enemy")]
    public float rightPatrolLimit;
    public float speed = 1.0f;
    public float attackRange;

    private float minPatrolDistance = 0.0f;
    private Vector3 destination = Vector3.zero;
    private float waitingTime;
    private float timeToWait;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        minPatrolDistance = (rightPatrolLimit - leftPatrolLimit) / 3.0f;
        currentState = State.IDLE;
        StateEnter(currentState);
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
                if (Vector3.Distance(transform.position, destination) > 0.1f)
                    transform.position = Vector3.MoveTowards(transform.position, destination, Time.fixedDeltaTime * speed);
                else ChangeState(State.IDLE);
                break;
            case State.BEING_HIT:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("BeingHit") &&
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                    ChangeState(State.IDLE);
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
                break;
            case State.BEING_HIT:
                animator.SetBool("being_hit", true);
                break;
            case State.STUNNED:
                animator.SetBool("idle", true);
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
                break;
            case State.BEING_HIT:
                animator.SetBool("being_hit", false);
                break;
            case State.STUNNED:
                animator.SetBool("idle", false);
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
            return -1;
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
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
        }
    }

    public void Stun()
    {
        Debug.Log(name.ToString() + ": I've been stunned");
        ChangeState(State.STUNNED);
    }

    public void WakeUp()
    {
        if (currentState == State.STUNNED)
            ChangeState(State.IDLE);
    }
}
