using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireBall : MonoBehaviour {

    public float lifeTime = 3.0f;
    public float speed = 1.0f;
    private Vector3 direction;
    public int ballDamage = 10;
    
	// Use this for initialization
	void Start() { 
             
        Destroy(gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void Update () {

        transform.Translate(direction * speed * Time.deltaTime);
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") || LayerMask.LayerToName(collider.gameObject.layer) == "ground")
        {
            collider.gameObject.SendMessage("ApplyDamage", ballDamage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

}
