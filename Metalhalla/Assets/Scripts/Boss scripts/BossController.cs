using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossController : MonoBehaviour {

    private FSMBoss fsmBoss = null;
    private GameObject theBoss = null;
	
	
    //---------------------------------- Boss Stats ---------------------------------------------------
    public float normalSpeed = 10;
    public float chasingSpeed = 20;
    public int maxHitPoints = 200;
    private int hitPoints = 200;
    //-------------------------------------------------------------------------------------------------

    void Awake()
    {
        theBoss = GameObject.FindGameObjectWithTag("Boss");
        if (theBoss == null)
            Debug.Log("Error: boss not found.");
	}
		
	// Use this for initialization
	void Start ()
    {
		fsmBoss = new FSMBoss (this);   
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void FixedUpdate()
    {
      fsmBoss.Update();
    }

    //------------------------------------ GETTERS ----------------------------------------

    public GameObject GetTheBoss()
    {
        return theBoss;
    }

    public int GetHitPoints()
    {
        return hitPoints;
    }

    public FSMBoss GetFSMBoss()
    {
        return fsmBoss;
    }

}
