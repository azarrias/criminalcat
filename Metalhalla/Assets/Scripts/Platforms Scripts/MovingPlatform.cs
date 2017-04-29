﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public Transform transformA;
    public Transform transformB;
    public float speed = 2.0f;
    public bool goingToB = true;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 directionAtoB;
    private float distanceThreshold;

	void Start () {
        pointA = transformA.position;
        pointB = transformB.position;
        directionAtoB = (pointB - pointA).normalized;
        distanceThreshold = speed * Time.fixedDeltaTime;
        Debug.Log("distance threshold" + distanceThreshold); 
    }
	
	// Update is called once per frame
	void Update () {
        UpdatePlatformPosition(); 
	}

    void UpdatePlatformPosition()
    {
        Vector3 pos = transform.position;
        if (goingToB)
        {
            pos += directionAtoB * Time.deltaTime * speed;
            if (Vector3.Distance(pos, pointB) <= distanceThreshold)
            {
                pos = pointB;
                goingToB = false; 
            }
        }
        else
        {
            pos -= directionAtoB * Time.deltaTime * speed;
            if (Vector3.Distance(pos, pointA) <= distanceThreshold)
            {
                pos = pointA;
                goingToB = true;
            }
        }

        transform.position = pos; 

    }

}
