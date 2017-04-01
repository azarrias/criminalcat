using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovementPauseTest : MonoBehaviour {

    public Vector3 dir;
    public float speed;
	
	// Update is called once per frame
	void Update () {

        transform.Translate(dir * speed * Time.deltaTime);
	}

     void OnBecameInvisible()
    {
        dir *= -1;
    }
}
