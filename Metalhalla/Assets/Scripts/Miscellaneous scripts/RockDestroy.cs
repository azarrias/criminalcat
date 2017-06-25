using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RockCollider"))
        {
            //other.transform.parent.gameObject.SetActive(false);
            other.transform.parent.gameObject.GetComponent<RockBehaviour>().disipate = true;
        }
    }
}
