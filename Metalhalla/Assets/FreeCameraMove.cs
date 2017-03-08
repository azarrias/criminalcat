using System.Collections;
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
        if (Input.GetKey(KeyCode.A))
            tmp.x -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            tmp.x += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
            tmp.y += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            tmp.y -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E))
            tmp.z += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q))
            tmp.z -= speed * Time.deltaTime;

        transform.position = tmp;
    }
}