using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftablePlatform : MonoBehaviour {

    public Transform platformInRestTransform;
    public Transform platformLiftedTransform;
    public float liftSpeed = 50f;
    public float fallSpeed = 4f;
    private bool goingToB = true;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 directionAtoB;
    private float distanceAtoB;

    [HideInInspector]
    public bool animatePlatform;

    void Start()
    {
        pointA = platformInRestTransform.position;
        pointB = platformLiftedTransform.position;
        directionAtoB = (pointB - pointA).normalized;
        distanceAtoB = Vector3.Distance(pointA, pointB);

        animatePlatform = false;
    }


    void Update()
    {
        if (animatePlatform)
            UpdatePlatformPosition();
        if (Input.GetKeyDown(KeyCode.P))
            animatePlatform = true;
    }

    void UpdatePlatformPosition()
    {
        Vector3 pos = transform.position;
        if (goingToB)
        {
            pos += directionAtoB * Time.fixedDeltaTime * liftSpeed;
            if (Vector3.Distance(pos, pointA) >= distanceAtoB)
            {
                pos = pointB;
                goingToB = false;
            }
        }
        else
        {
            pos -= directionAtoB * Time.fixedDeltaTime * fallSpeed;

            if (Vector3.Distance(pos, pointB) >= distanceAtoB)
            {
                pos = pointA;
                goingToB = true;
                animatePlatform = false;
            }
        }

        transform.position = pos;

    }

}
