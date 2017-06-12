using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinGUI : MonoBehaviour {

    private float rotationSpeed;

    private void Start()
    {
        transform.Rotate(Random.Range(0, 90) * Vector3.up);
        rotationSpeed = 180f;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
