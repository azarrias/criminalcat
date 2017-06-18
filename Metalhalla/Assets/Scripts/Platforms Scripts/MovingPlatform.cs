using System.Collections;
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
    private float distanceAtoB;

	void Start () {
        pointA = transformA.position;
        pointB = transformB.position;
        directionAtoB = (pointB - pointA).normalized;
        distanceAtoB = Vector3.Distance(pointA, pointB);
    }
	
    
	void Update () {
        UpdatePlatformPosition(); 
	}
    
    private void FixedUpdate()
    {
    //    UpdatePlatformPosition();
    }

    void UpdatePlatformPosition()
    {
        Vector3 pos = transform.position;
        if (goingToB)
        {
            pos += directionAtoB * Time.fixedDeltaTime * speed;
            //if (Vector3.Distance(pos, pointB) <= distanceThreshold)
            if (Vector3.Distance(pos, pointA) >= distanceAtoB)
            {
                pos = pointB;
                goingToB = false; 
            }
        }
        else
        {
            pos -= directionAtoB * Time.fixedDeltaTime * speed;

            //if (Vector3.Distance(pos, pointA) <= distanceThreshold)
            if (Vector3.Distance(pos, pointB) >= distanceAtoB)
            {
                pos = pointA;
                goingToB = true;
            }
        }

        transform.position = pos; 

    }

}
