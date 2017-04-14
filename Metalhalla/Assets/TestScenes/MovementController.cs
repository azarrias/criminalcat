using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {

   
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.O))
        {
            transform.Translate(Vector3.left * Time.deltaTime * 5);         
        }

        if(Input.GetKey(KeyCode.P))
        {
            transform.Translate(Vector3.right * Time.deltaTime * 5);
        }
	}
}
