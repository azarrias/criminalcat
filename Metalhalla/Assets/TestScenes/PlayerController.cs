using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private string horizontalAxis = "Horizontal";
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    private bool onLand = true;
    private GameObject theBoss;
    
    // Use this for initialization
	void Start () {
        theBoss = GameObject.FindGameObjectWithTag("Boss");
	}

	
	void FixedUpdate () {

        if(Input.GetKeyDown(KeyCode.Space) && onLand)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.up * jumpSpeed;
            onLand = false;
        }

        float horizontal = Input.GetAxis(horizontalAxis);
        Vector3 translation = new Vector3(horizontal * speed * Time.deltaTime, 0, 0);
        transform.Translate(translation);

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            theBoss.GetComponent<BossStats>().DamageBoss(10);
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
       if( collision.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            onLand = true;
        }

    }
}
