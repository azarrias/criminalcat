using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfFireBallBehaviour : MonoBehaviour {

    public float lifeTime = 3.0f;
    public float speed = 1.0f;
    private Vector3 direction;
    public int ballDamage = 10;
    private float counter = 0.0f;


    
	// Use this for initialization
	void Start() {

	}
	
	// Update is called once per frame
	void Update () {

        transform.Translate(direction * speed * Time.deltaTime);

        counter += Time.deltaTime;
        if(counter >= lifeTime)
        {
            counter = 0.0f;
            Deactivate();
        }
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

    public void SetDirection(Vector3 ballDirection)
    {
        Transform ball = gameObject.transform.Find("Ball").transform;
        direction = ballDirection.normalized;
        Quaternion rotation = Quaternion.LookRotation(ballDirection);
        ball.localRotation = rotation;
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
