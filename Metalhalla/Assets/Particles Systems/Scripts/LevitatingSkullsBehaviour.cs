using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitatingSkullsBehaviour : MonoBehaviour {

    private enum State
    {
        INACTIVITY,
        MATERIALIZATION,
        ASCENSION,
        LEVITATION,
        ATTACK
    }

    public Transform[] spawnPoints;
    public GameObject skullPrefab;
    private GameObject[] skulls;
    public GameObject player;
    private State state;
    public float ascensionSpeed = 2.0f;
    private float ascensionCounter = 0.0f;
    public float ascensionTime = 4.0f;
    private float levitationCounter = 0.0f;
    public float levitationTime = 2.0f;
    public float forceMagnitude;
    private Vector3 targetPosition;
    public float scaleSpeed = 1.0f;
    public float maxScale = 1.0f;
    
    private GameObject boss;

    void Awake()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");

        skulls = new GameObject[4];
        
        for (int i = 0; i < skulls.Length; i++)
        {
            skulls[i] = Instantiate(skullPrefab, Vector3.zero, Quaternion.identity);
            skulls[i].transform.Find("SkullMesh").transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
            skulls[i].GetComponent<SkullsAttackBehaviour>().parentGO = boss;
            skulls[i].SetActive(false);
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
        for (int i = 0; i < skulls.Length; i++)
        {
            Vector3 scale = skulls[i].transform.Find("SkullMesh").transform.localScale;
            scale.x += scaleSpeed * Time.deltaTime;
            scale.y += scaleSpeed * Time.deltaTime;
            scale.z += scaleSpeed * Time.deltaTime;
            skulls[i].transform.Find("SkullMesh").transform.localScale = scale;
           
            if (i == skulls.Length-1 && scale.z >= maxScale)
                state = State.ASCENSION;
        }     
    }

    private void Ascend()
    {
        bool targetReached = false;

        for (int i = 0; i < skulls.Length; i++)
        {          
            skulls[i].transform.position += Vector3.up * Time.deltaTime * ascensionSpeed;
            ascensionCounter += Time.deltaTime;
            if(ascensionCounter >= ascensionTime)
            {
                ascensionCounter = 0.0f;
                targetReached = true;            
            }
        }

        if (targetReached)
            state = State.LEVITATION;
    }

    private void Levitate()
    {
        levitationCounter += Time.deltaTime;
        if (levitationCounter >= levitationTime)
        {
            levitationCounter = 0.0f;
            state = State.ATTACK;
            targetPosition = player.transform.position;
            for (int i = 0; i < skulls.Length; i++)
            {
                skulls[i].transform.Find("SkullMesh").GetComponent<SphereCollider>().enabled = true;
            }
        }
    }

    private void Attack()
    {       
        for (int i = 0; i < skulls.Length; i++)
        {
            Vector3 forceDir = (targetPosition - skulls[i].transform.position).normalized;
            skulls[i].GetComponent<Rigidbody>().AddForce(forceMagnitude * forceDir, ForceMode.Impulse);            
        }

        state = State.INACTIVITY;
    }

    public void StartSkullsAttack()
    {
        state = State.MATERIALIZATION;
        for (int i = 0; i < skulls.Length; i++)
        {
            skulls[i].transform.Find("SkullMesh").GetComponent<SphereCollider>().enabled = false;
            skulls[i].SetActive(true);
            skulls[i].transform.position = spawnPoints[i].transform.position;
        }        
    }

    public void StopSkullsAttack()
    {
        if (skulls[0].activeSelf)
        {
            for (int i = 0; i < skulls.Length; i++)
            {
                skulls[i].GetComponent<SkullsAttackBehaviour>().ShrinkSkulls();
            }
        }
    }

}
