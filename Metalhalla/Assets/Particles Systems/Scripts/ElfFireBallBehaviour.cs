using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfFireBallBehaviour : MonoBehaviour {

    public float lifeTime = 3.0f;
    public float speed = 1.0f;
    private Vector3 direction;
    public int ballDamage = 10;
    private float lifeTimeCounter = 0.0f;
    private float deactivationTime = 2.0f;
    private float deactivationCounter = 0.0f;
    private bool deactivate = false;
     
	// Use this for initialization
	void Start() {

	}
	
	// Update is called once per frame
	void Update () {

        if (!deactivate)
        {
            transform.Translate(direction * speed * Time.deltaTime);

            lifeTimeCounter += Time.deltaTime;
            if (lifeTimeCounter >= lifeTime)
            {
                lifeTimeCounter = 0.0f;
                deactivate = true;
                gameObject.transform.Find("BallExplosion").gameObject.SetActive(true);
                gameObject.transform.Find("Ball").gameObject.SetActive(false);                
            }
        }
        if (deactivate)
            Deactivate();
	}

    void OnCollisionEnter(Collision collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "ground" || 
            LayerMask.LayerToName(collision.gameObject.layer) == "wall" ||
            LayerMask.LayerToName(collision.gameObject.layer) == "player" ||
            LayerMask.LayerToName(collision.gameObject.layer) == "shield")
        {
            collision.gameObject.SendMessage("ApplyDamage", ballDamage, SendMessageOptions.DontRequireReceiver);
            gameObject.transform.Find("BallExplosion").gameObject.SetActive(true);
            gameObject.transform.Find("Ball").gameObject.SetActive(false);
            gameObject.GetComponent<SphereCollider>().enabled = false;
            deactivate = true;                        
            lifeTimeCounter = 0.0f;  //reset lifetime
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
        deactivationCounter += Time.deltaTime;
        if (deactivationCounter >= deactivationTime)
        {
            deactivationCounter = 0.0f;
            deactivate = false;         
            gameObject.SetActive(false);
            gameObject.GetComponent<SphereCollider>().enabled = true;
        }
    }
}
