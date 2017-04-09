using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireBall : MonoBehaviour {

    public float lifeTime = 3.0f;
    public float speed = 1.0f;
    private GameObject theBoss  = null;
    FSMBoss fsmBoss = null;
    
	// Use this for initialization
	void Start() { 
         
        theBoss = GameObject.FindGameObjectWithTag("Boss");

        if (theBoss == null)
            Debug.LogError("theBoss not found.");
        fsmBoss = theBoss.GetComponent<FSMBoss>();

        if (fsmBoss == null)
            Debug.Log("fsmBoss not found.");
        Destroy(gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void Update () {

       if (fsmBoss.facingRight)
        {
            Vector3 translation = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
            transform.Translate(translation);
        }
        else
        {
            Vector3 translation = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
            transform.Translate(translation);
        }
	}

    void TriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
            Destroy(gameObject);
    }

}
