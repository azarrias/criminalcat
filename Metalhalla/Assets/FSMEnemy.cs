using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineOfSight))]
public class FSMEnemy : MonoBehaviour
{

    [HideInInspector]
    public LineOfSight los;
    [HideInInspector]
    public State state;

    [Tooltip("X world coordinate to be set as a left limit for this enemy")]
    public int leftPatrolLimit;
    [Tooltip("X world coordinate to be set as a right limit for this enemy")]
    public int rightPatrolLimit;
    public float speed = 1.0f;

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
        Debug.Log(name.ToString() + ": I'm in patrol");
        yield return null;
        while (inPatrol)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
            if (stunned || los.playerInSight)
            {
                inPatrol = false;
            }
            yield return null;
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
        Debug.Log(name.ToString() + ": I'm in chase");
        yield return null;
        while (inChase)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed * 2);
            if (stunned || !los.playerInSight)
            {
                inChase = false;
            }
            yield return null;
        }
        if (stunned)
        {
            state = State.Stunned;
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
}
