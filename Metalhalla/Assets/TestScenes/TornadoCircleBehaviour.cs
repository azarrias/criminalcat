using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoCircleBehaviour : MonoBehaviour {

    public float maxHeight = 2.0f;
    public float speed = 2.0f;
    public bool readyToMove = false;
    

    // Use this for initialization
    void Start()
    {
        transform.localPosition = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
        if (readyToMove)
        {
            Vector3 newPos = new Vector3();
            newPos = transform.position + Vector3.up * speed * Time.deltaTime;
            transform.position = newPos;

            if (transform.localPosition.y >= maxHeight)
            {
                transform.localPosition = Vector3.zero;
                readyToMove = false;            
            }
        }
        
	}
}
