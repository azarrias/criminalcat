using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetFacingRight(bool facingRight)
    {
        if (facingRight)
        {           
            if (transform.eulerAngles.y != -0.0f)
                transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else
        {          
            if (transform.eulerAngles.y != -180.0f)
                transform.localRotation = Quaternion.Euler(0.0f, -180.0f, 0.0f);
        }
    }
}
