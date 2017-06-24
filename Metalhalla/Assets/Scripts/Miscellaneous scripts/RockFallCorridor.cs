using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFallCorridor : MonoBehaviour {

    public GameObject rockGenerator1;
    private RockFall rockFallScript1;
    private bool allowFall = true;
    public float allowFallTime = 3.0f;
    public float disabledFallTime = 1.0f;
    private float counter = 0.0f;
    

	// Use this for initialization
	void Start () {
        rockFallScript1 = rockGenerator1.GetComponent<RockFall>();
	}
	
	// Update is called once per frame
	void Update () {
		if(rockGenerator1.GetComponent<RockFall>().generateRocks)
        {
            if (allowFall)
                ActiveTick();
            else
                InactiveTick();
        }
	}

    private void ActiveTick()
    {
        counter += Time.deltaTime;
        if(counter >= allowFallTime)
        {
            counter = 0.0f;
            allowFall = false;
            rockFallScript1.enabled = false;
            
        }
    }

    private void InactiveTick()
    {
        counter += Time.deltaTime;
        if (counter >= disabledFallTime)
        {
            counter = 0.0f;
            allowFall = true;
            rockFallScript1.enabled = true;
            
        }
    }
}
