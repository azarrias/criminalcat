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
    private float nextLocation = 0;
    [Tooltip("X world coordinate to be set as a left limit for this enemy")]
    public int leftPatrolLimit;
    [Tooltip("X world coordinate to be set as a right limit for this enemy")]
    public int rightPatrolLimit;
    public float speed = 1.0f;

    public enum State
    {
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
        //Debug.Log(name.ToString() + ": I'm in patrol");
        yield return null;
        while (inPatrol)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
            if (los.playerInSight)
            {
                inPatrol = false;
            }
            yield return null;
        }
        state = State.Chase;
    }

    IEnumerator Chase()
    {
        bool inChase = true;
        //Debug.Log(name.ToString() + ": I'm in chase");
        yield return null;
        while (inChase)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed * 2);
            if (!los.playerInSight)
            {
                inChase = false;
            }
            yield return null;
        }
        state = State.Patrol;
    }
}
