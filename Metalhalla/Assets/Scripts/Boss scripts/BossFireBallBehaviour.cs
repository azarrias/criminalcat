using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireBallBehaviour : MonoBehaviour {

    public float lifeTime = 3.0f;
    public float speed = 1.0f;
    private Vector3 direction;
    public int ballDamage = 10;
    
	// Use this for initialization
	void Start() {

        //Destroy(gameObject, lifeTime);
        Invoke("Deactivate", lifeTime);
	}
	
	// Update is called once per frame
	void Update () {

        transform.Translate(direction * speed * Time.deltaTime);
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") || LayerMask.LayerToName(collider.gameObject.layer) == "ground" || LayerMask.LayerToName(collider.gameObject.layer) == "player")
        {
            collider.gameObject.SendMessage("ApplyDamage", ballDamage, SendMessageOptions.DontRequireReceiver);
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void SetFacingRight(bool facingRight)
    {
        Transform smokeTr = gameObject.transform.Find("Ball").transform;

        if (facingRight)
        {
            direction = Vector3.right;
            
            if (smokeTr.eulerAngles.y != -90.0f)
                smokeTr.localRotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
        }
        else
        {
            direction = Vector3.left;

            if (smokeTr.eulerAngles.y != 90.0f)
                smokeTr.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        }
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
