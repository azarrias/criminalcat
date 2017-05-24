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

        Invoke("Deactivate", lifeTime);
	}
	
	// Update is called once per frame
	void Update () {

        transform.Translate(direction * speed * Time.deltaTime);
	}

    void OnCollisionEnter(Collision collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "ground" || 
            LayerMask.LayerToName(collision.gameObject.layer) == "wall" ||
            LayerMask.LayerToName(collision.gameObject.layer) == "player")
        {
            collision.gameObject.SendMessage("ApplyDamage", ballDamage, SendMessageOptions.DontRequireReceiver);
           
            gameObject.SetActive(false);
        }
    }

    public void SetFacingRight(bool facingRight)
    {
        Transform ball = gameObject.transform.Find("Ball").transform;

        if (facingRight)
        {
            direction = Vector3.right;

            if (ball.eulerAngles.y != -90.0f)
                ball.localRotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
        }
        else
        {
            direction = Vector3.left;

            if (ball.eulerAngles.y != 90.0f)
                ball.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        }
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
