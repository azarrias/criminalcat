﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineOfSight))]
[RequireComponent(typeof(EnemyStats))]
public class FSMEnemy : MonoBehaviour
{

    [HideInInspector]
    public LineOfSight los;
    [HideInInspector]
    public PlayerStatus playerStatus;
    [HideInInspector]
    public State state;
    [HideInInspector]
    public EnemyStats enemyStats;

    Animator animator;

    [Tooltip("X world coordinate to be set as a left limit for this enemy")]
    public float leftPatrolLimit;
    [Tooltip("X world coordinate to be set as a right limit for this enemy")]
    public float rightPatrolLimit;
    public float speed = 1.0f;
    public float attackRange;

    public bool stunned = false;
    public bool hit = false;
    private float nextLocation = 0;
    private string[] animatorConditions = { "idle", "walk", "being_hit", "dead", "attack" };
    Material material;

    public enum State
    {
        Stunned,
        Patrol,
        Chase,
        Attack,
        BeingHit,
        Dead
    }

    private void Awake()
    {
        los = GetComponent<LineOfSight>();
        enemyStats = GetComponent<EnemyStats>();
        state = State.Patrol;
        nextLocation = transform.position.x;
        animator = GetComponent<Animator>();
        EnableAnimatorCondition("idle");

        material = GetComponentInChildren<Renderer>().material;
 //       materialCopy = Material.Instantiate(materialRender);

    }

    private void Start()
    {
        playerStatus = los.player.GetComponent<PlayerStatus>();
        if (!playerStatus)
        {
            Debug.LogError("The Player GameObj has no PlayerStatus script attached to it!");
        }

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
        Vector3 destination = Vector3.zero;
        Debug.Log(name.ToString() + ": I'm in patrol");
        float new_distance_threshold = (rightPatrolLimit - leftPatrolLimit) / 3.0f;
        yield return null;

        while (state == State.Patrol)
        {
            // sets a new random destination within bounds and above a given minimum distance
            do
            {
                destination.Set(Random.Range(leftPatrolLimit, rightPatrolLimit), transform.position.y, transform.position.z);
            } while (Mathf.Abs(destination.x - transform.position.x) < new_distance_threshold);

            faceXCoordinate(destination.x);
            EnableAnimatorCondition("walk");

            while (Vector3.Distance(transform.position, destination) > 0.1f)
            {
                if (stunned)
                {
                    state = State.Stunned;
                    break;
                }
                else if (hit)
                {
                    state = State.BeingHit;
                    break;
                }
                else if (los.playerInSight)
                {
                    state = State.Chase;
                    break;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, destination, Time.fixedDeltaTime * speed);
                }
                yield return null;
            }
            EnableAnimatorCondition("idle");
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        }
    }

    IEnumerator Chase()
    {
        Vector3 destination = Vector3.zero;
        Debug.Log(name.ToString() + ": I'm in chase");
        yield return null;

        while (state == State.Chase)
        {
            destination.Set(faceXCoordinate(los.player.transform.position.x) * attackRange + los.player.transform.position.x,
                transform.position.y, transform.position.z);
            EnableAnimatorCondition("walk");

            while (Vector3.Distance(transform.position, destination) > 0.1f)
            {
                if (stunned)
                {
                    state = State.Stunned;
                    break;
                }
                else if (hit)
                {
                    state = State.BeingHit;
                    break;
                }
                else if (PlayerAtRange())
                {
                    state = State.Attack;
                    break;
                }
                else if (!InBounds())
                {
                    state = State.Patrol;
                    break;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, destination, Time.fixedDeltaTime * speed * 2);
                }
                yield return null;
            }
            EnableAnimatorCondition("idle");
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        }
    }

    IEnumerator Stunned()
    {
        Debug.Log(name.ToString() + ": I'm stunned");
        EnableAnimatorCondition("idle");
        yield return null;
        while (stunned)
        {
            if (hit)
            {
                state = State.BeingHit;
            }
            yield return new WaitForSeconds(2.0f);
        }
        state = State.Patrol;
    }

    IEnumerator BeingHit()
    {
        Debug.Log(name.ToString() + ": I'm hit");
        yield return null;

        EnableAnimatorCondition("being_hit");
        yield return new WaitForSeconds(1.208f);

        faceXCoordinate(los.player.transform.position.x);
        hit = false;

        if (enemyStats.hitPoints > 0)
            state = State.Chase;
        else
            state = State.Dead;
    }

    IEnumerator Attack()
    {
        Debug.Log(name.ToString() + ": I'm attacking");
        EnableAnimatorCondition("attack");
        yield return null;

        if (stunned)
        {
            state = State.Stunned;
        }
        else if (hit)
        {
            state = State.BeingHit;
        }
        else if (!playerStatus.IsAlive())
        {
            state = State.Patrol;
        }
        else
        {
            los.player.SendMessage("ApplyDamage", enemyStats.meleeDamage, SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(2.458f);
        }
        state = State.Chase;
    }

    IEnumerator Dead()
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        float timeToFade = 5.0f;
        Debug.Log(name.ToString() + ": I'm dying");
        EnableAnimatorCondition("dead");
        yield return new WaitForSeconds(2.458f);

        while (material.color.a > 0)
        {
            Color newColor = material.color;
            newColor.a -= Time.deltaTime / timeToFade;
            material.color = newColor;
            yield return null;
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
        if (Mathf.Abs(los.player.transform.position.y - transform.position.y) > 1.0f)
            return false;
        if (los.player.transform.position.x > transform.position.x)
            return los.player.transform.position.x - attackRange - transform.position.x <= 0.2f;
        else
            return transform.position.x - attackRange - los.player.transform.position.x <= 0.2f;
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
        Debug.Log(name.ToString() + ": I've been hit");
        hit = true;
        //        enemyStats.ApplyDamage(damage);
    }

    public void EnableAnimatorCondition(string condition)
    {
        foreach (string cond in animatorConditions)
        {
            if (cond.Equals(condition))
                animator.SetBool(cond, true);
            else
                animator.SetBool(cond, false);
        }
    }
}
