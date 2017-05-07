using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustDirection : MonoBehaviour {
    

    [Range(-20, 20)]
    public int pushForceX = 4;
    [Range(-20, 20)]
    public int pushForceY = 4;
    [Range(-20, 20)]
    public int pushForceZ = 0;

	// Use this for initialization
	void Start () {

		foreach(Transform rockTransform in gameObject.GetComponentInChildren<Transform>())
        {           
          rockTransform.GetComponent<Rigidbody>().AddForce(new Vector3(pushForceX, pushForceY, pushForceZ),  ForceMode.VelocityChange);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
