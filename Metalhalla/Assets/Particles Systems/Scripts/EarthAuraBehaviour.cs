using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthAuraBehaviour : MonoBehaviour {

    private enum State
    {
        INACTIVITY,
        MATERIALIZATION,
        ASCENSION,
        LEVITATION,
        ATTACK
    }

    public Transform[] spawnPoints;
    public Transform[] levitationPoints;
    public GameObject rockPrefab;
    private GameObject[] rocks;
    public GameObject player;
    private State state;
    private float materializationCounter = 0.0f;
    public float materializationTime;
    public float ascensionSpeed;
    private float levitationCounter = 0.0f;
    public float levitationTime;
    public float attackMovementSpeed;

    void Awake()
    {
        rocks = new GameObject[4];
        
        for (int i = 0; i < 4; i++)
        {
            rocks[i] = Instantiate(rockPrefab, Vector3.zero, Quaternion.identity);
            rocks[i].SetActive(false);
        }
                 
        state = State.INACTIVITY;       
    }
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        switch(state)
        {
            case State.INACTIVITY:
                //Inactive();
                break;

            case State.MATERIALIZATION:
                Materialize();
                break;

            case State.ASCENSION:
                Ascend();
                break;

            case State.LEVITATION:
                Levitate();
                break;

            case State.ATTACK:
                Attack();
                break;           
        }

	}

    //private void Inactive()
    //{

    //}

    private void Materialize()
    {
        for (int i = 0; i < rocks.Length; i++)
        {           
            Color color = rocks[i].transform.Find("rock").GetComponent<MeshRenderer>().sharedMaterial.color;
            color.a += color.a * 1 / materializationTime;
            if (color.a > 1.0f)
                color.a = 1.0f;
            rocks[i].transform.Find("rock").GetComponent<MeshRenderer>().sharedMaterial.color = color;
        }
        materializationCounter += Time.deltaTime;
        if(materializationCounter >= materializationTime)
        {
            materializationCounter = 0.0f;
            state = State.ASCENSION;
        }
    }

    private void Ascend()
    {
        bool targetReached = true;

        for (int i = 0; i < rocks.Length; i++)
        {
            Vector3 newPosition = Vector3.MoveTowards(rocks[i].transform.position, levitationPoints[i].transform.position, ascensionSpeed * Time.deltaTime);
            rocks[i].transform.position = newPosition;

            if (newPosition != levitationPoints[i].transform.position)
                targetReached = false;
        }

        if (targetReached)
            state = State.LEVITATION;
    }

    private void Levitate()
    {
        levitationCounter += Time.deltaTime;
        if (levitationCounter >= levitationTime)
            state = State.ATTACK;
    }

    private void Attack()
    {
        bool targetReached = true;

        for (int i = 0; i < rocks.Length; i++)
        {
            Vector3 newPosition = Vector3.MoveTowards(rocks[i].transform.position, player.transform.position, attackMovementSpeed * Time.deltaTime);
            rocks[i].transform.position = newPosition;

            if (newPosition != spawnPoints[i].transform.position)
                targetReached = false;
        }

        if (targetReached)
            state = State.INACTIVITY;
    }

    public void StartEarthAttack()
    {
        state = State.MATERIALIZATION;
        for (int i = 0; i < rocks.Length; i++)
        {
            rocks[i].SetActive(true);
            rocks[i].transform.position = spawnPoints[i].transform.position;
        }        
    }

}
