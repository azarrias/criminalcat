using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveTest : MonoBehaviour {

    public float speed = 3.0f;

    private Vector3 tmp;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        tmp = transform.position;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            tmp.y += speed * Time.deltaTime;
            transform.position = tmp;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            tmp.y -= speed * Time.deltaTime;
            transform.position = tmp;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            tmp.x -= speed * Time.deltaTime;
            transform.position = tmp;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            tmp.x += speed * Time.deltaTime;
            transform.position = tmp;
        }

    }
}
