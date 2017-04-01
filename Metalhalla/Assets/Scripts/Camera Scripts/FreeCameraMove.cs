using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraMove : MonoBehaviour
{

    public float speed = 5f;
    public float rotationSpeed = 30f;
    Vector3 tmp;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        tmp = transform.position;

        if (Input.GetKey(KeyCode.A) || Input.GetAxis("FreeCameraHorizontal") < 0)
            tmp.x -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D) || Input.GetAxis("FreeCameraHorizontal") > 0)
            tmp.x += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W) || Input.GetAxis("FreeCameraVertical") > 0)
            tmp.y += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S) || Input.GetAxis("FreeCameraVertical") < 0)
            tmp.y -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E) || Input.GetAxis("FreeCameraZoom") > 0)
            tmp.z += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q) || Input.GetAxis("FreeCameraZoom") < 0)
            tmp.z -= speed * Time.deltaTime;

        transform.position = tmp;

        if (Input.GetAxis("FreeCameraHorizontalRotation") > 0)
            transform.Rotate(transform.up, rotationSpeed * Time.deltaTime);
        if (Input.GetAxis("FreeCameraHorizontalRotation") < 0)
            transform.Rotate(transform.up, -rotationSpeed * Time.deltaTime);

    }
}