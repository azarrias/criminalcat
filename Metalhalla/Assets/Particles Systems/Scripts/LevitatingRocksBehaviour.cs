using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitatingRocksBehaviour : MonoBehaviour {

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
    public float ascensionSpeed;
    private float levitationCounter = 0.0f;
    public float levitationTime;
    public float forceMagnitude;
    private Vector3 targetPosition;
    public float scaleSpeed = 1.0f;
    public float maxScale = 1.0f;

    private GameObject boss;

    void Awake()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");

        rocks = new GameObject[4];
        
        for (int i = 0; i < rocks.Length; i++)
        {
            rocks[i] = Instantiate(rockPrefab, Vector3.zero, Quaternion.identity);
            rocks[i].transform.Find("rock").transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
            rocks[i].GetComponent<EarthAttackRockBehaviour>().parentGO = boss;
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
            Vector3 scale = rocks[i].transform.Find("rock").transform.localScale;
            scale.x += scaleSpeed * Time.deltaTime;
            scale.y += scaleSpeed * Time.deltaTime;
            scale.z += scaleSpeed * Time.deltaTime;
            rocks[i].transform.Find("rock").transform.localScale = scale;

            //Detach rocks from boss
            rocks[i].transform.parent = boss.transform.parent;
            if (i == rocks.Length-1 && scale.z >= maxScale)
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
        {
            state = State.ATTACK;
            targetPosition = player.transform.position;            
        }
    }

    private void Attack()
    {       
        for (int i = 0; i < rocks.Length; i++)
        {
            Vector3 forceDir = (targetPosition - rocks[i].transform.position).normalized;
            rocks[i].GetComponent<Rigidbody>().AddForce(forceMagnitude * forceDir, ForceMode.Impulse);            
        }

        state = State.INACTIVITY;
    }

    public void StartRocksAttack()
    {
        state = State.MATERIALIZATION;
        for (int i = 0; i < rocks.Length; i++)
        {
            rocks[i].SetActive(true);
            rocks[i].transform.position = spawnPoints[i].transform.position;
        }        
    }

}
