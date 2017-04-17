using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineOfSight))]
public class FSMEnemy : MonoBehaviour
{

    [HideInInspector]
    public LineOfSight los;
    [HideInInspector]
    public PlayerStatus playerStatus;
    [HideInInspector]
    public State state;

    [Tooltip("X world coordinate to be set as a left limit for this enemy")]
    public float leftPatrolLimit;
    [Tooltip("X world coordinate to be set as a right limit for this enemy")]
    public float rightPatrolLimit;
    public float speed = 1.0f;
    public float attackRange;

    public bool stunned = false;
    private float nextLocation = 0;

    public enum State
    {
        Stunned,
        Patrol,
        Chase,
        Attack
    }

    private void Awake()
    {
        los = GetComponent<LineOfSight>();
        playerStatus = los.player.GetComponent<PlayerStatus>();
        if (!playerStatus)
        {
            Debug.LogError("The Player GameObj has no PlayerStatus script attached to it!");
        }
        state = State.Patrol;
        nextLocation = transform.position.x;
    }

    private void Start()
    {
        StartCoroutine(FSM());
    }

    IEnumerator FSM()
    {
        while (true)
        {
            yield return StartCoroutine(state.ToString());
        }
    }

    IEnumerator Patrol()
    {
        bool inPatrol = true;
        Vector3 destination = Vector3.zero;
        Debug.Log(name.ToString() + ": I'm in patrol");
        yield return null;

        while (inPatrol)
        {
            do
            {
                destination.Set(Random.Range(leftPatrolLimit, rightPatrolLimit), transform.position.y, transform.position.z);
            } while (Mathf.Abs(destination.x - transform.position.x) < 3.0f);

            if (destination.x > transform.position.x)
            {
                transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }

            while (Vector3.Distance(transform.position, destination) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, Time.fixedDeltaTime * speed);
                if (stunned || los.playerInSight)
                {
                    inPatrol = false;
                    break;
                }
                yield return null;
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        }
        if (stunned)
        {
            state = State.Stunned;
        }
        else
        {
            state = State.Chase;
        }
    }

    IEnumerator Chase()
    {
        bool inChase = true;
        Vector3 destination = Vector3.zero;
        Debug.Log(name.ToString() + ": I'm in chase");

        if (los.player.transform.position.x > transform.position.x)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
            destination.Set(los.player.transform.position.x - attackRange, transform.position.y, transform.position.z);
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
            destination.Set(los.player.transform.position.x + attackRange, transform.position.y, transform.position.z);
        }
        yield return null;
        while (inChase)
        {
            if (!stunned && !PlayerAtRange() && InBounds())
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, Time.fixedDeltaTime * speed * 2);
            }
            else
            {
                inChase = false;
            }
            yield return null;
        }
        if (stunned)
        {
            state = State.Stunned;
        }
        else if (PlayerAtRange())
        {
            state = State.Attack;
        }
        else
        {
            state = State.Patrol;
        }
    }

    IEnumerator Stunned()
    {
        Debug.Log(name.ToString() + ": I'm stunned");
        yield return null;
        while (stunned)
        {
            yield return new WaitForSeconds(2.0f);
        }
        state = State.Patrol;
    }

    IEnumerator Attack()
    {
        bool inAttack = true;
        Debug.Log(name.ToString() + ": I'm attacking");
        yield return null;
        while (inAttack)
        {
            if (stunned || !playerStatus.IsAlive() || !PlayerAtRange())
            {
                inAttack = false;
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        }
        if (stunned)
        {
            state = State.Stunned;
        }
        else if (!PlayerAtRange())
        {
            state = State.Chase;
        }
        else
        {
            state = State.Patrol;
        }
    }

    public void Stun()
    {
        stunned = true;
        los.Stop();
    }

    public void WakeUp()
    {
        stunned = false;
        los.Start();
    }

    public bool PlayerAtRange()
    {
        if (los.player.transform.position.x > transform.position.x)
            return los.player.transform.position.x - attackRange - transform.position.x <= 0.5f;
        else
            return transform.position.x - attackRange - los.player.transform.position.x <= 0.5f;
    }

    public bool InBounds()
    {
        return transform.position.x > leftPatrolLimit && transform.position.x < rightPatrolLimit;
    }
}
