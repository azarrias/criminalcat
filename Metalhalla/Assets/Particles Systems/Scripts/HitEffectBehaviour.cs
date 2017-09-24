using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectBehaviour : MonoBehaviour {

    private Vector3 scaleFacingRight = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 scaleFacingLeft = new Vector3(-1.0f, 1.0f, 1.0f);
   
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetFacingRight(bool facingRight)
    {
        if(facingRight)
        {
            transform.localScale = scaleFacingRight;
        }
        else
            transform.localScale = scaleFacingLeft;
    }
}
