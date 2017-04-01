﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMovement : MonoBehaviour {

    public float rotSpeed = 3.0f;
    public float attackRange = 15.0f;
    private Vector3 maxRange;
    private GameObject maxRangePoint;
    public LayerMask whatToHit;

    public bool withCollider2D = true;
  
	// Use this for initialization
	void Start ()
    {
        maxRange.x = transform.position.x + attackRange;
        maxRange.y = transform.position.y;
        maxRange.z = transform.position.z;

        maxRangePoint = GameObject.FindWithTag("MaxRangePoint");
        maxRangePoint.transform.position = maxRange;
    }

    void Update()
    { 
        float v = Input.GetAxis("DragonHeadMovementVertical");
        transform.Rotate(Vector3.forward * v * rotSpeed);
        Debug.DrawLine(transform.position, maxRangePoint.transform.position, Color.red);
    }

    void FixedUpdate()
    {
        if (withCollider2D == false)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, maxRangePoint.transform.position - transform.position, out hitInfo, attackRange, whatToHit))
            {
                DestroyCube destructionScript = hitInfo.collider.GetComponent<DestroyCube>();
                destructionScript.Destroy();
            }
        }
        else
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, maxRangePoint.transform.position - transform.position, attackRange, whatToHit);
            if (hitInfo)
            {
                DestroyCube destructionScript = hitInfo.collider.GetComponent<DestroyCube>();
                destructionScript.Destroy();
            }


        }
    }
	
}
