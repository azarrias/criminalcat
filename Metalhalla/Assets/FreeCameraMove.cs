﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraMove : MonoBehaviour
{

    public float speed = 5f;
    Vector3 tmp;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        tmp = transform.position;
        if (Input.GetKey(KeyCode.J))
            tmp.x -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.L))
            tmp.x += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.I))
            tmp.y += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.K))
            tmp.y -= speed * Time.deltaTime;
        transform.position = tmp;
    }
}