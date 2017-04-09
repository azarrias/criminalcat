using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidPlayerThrough : MonoBehaviour {

    private GameObject thePlayer = null;
    private PlayerStatus thePlayerStatus = null;
    private GameObject theBoss = null;
    private PlayerOverHead overHead = null;
    //private PlayerCollider playerCollider = null;
    private bool slidePlayer = false;

	// Use this for initialization
	void Start () {
        thePlayer = GameObject.FindGameObjectWithTag("Player");
        if (thePlayer == null)
            Debug.LogError("thePlayer not found.");

        thePlayerStatus = thePlayer.GetComponent<PlayerStatus>();
        if (thePlayerStatus == null)
            Debug.LogError("thePlayerStatus not found.");

        theBoss = GameObject.FindGameObjectWithTag("Boss");
        if (theBoss == null)
            Debug.LogError("theBoss not found.");
     
        overHead = FindObjectOfType<PlayerOverHead>();
        if (overHead == null)
            Debug.LogError("overHead not found.");

        //playerCollider = thePlayer.GetComponent<PlayerCollider>();
        //if (playerCollider == null)
        //    Debug.LogError("playerCollider not found.");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        if(slidePlayer)
        {
            Vector3 pos = thePlayer.transform.position;           
            if (thePlayerStatus.facingRight)
            {
                pos = new Vector3(pos.x - 0.1f, pos.y, pos.z);
            }
            else
            {                
                pos = new Vector3(pos.x + 0.1f, pos.y, pos.z);
            }
                       
            thePlayer.transform.position = pos;
        }

        if(overHead.IsOverHead())
        {
            Vector3 pos = thePlayer.transform.position;
            if (thePlayerStatus.facingRight)
            {                
                pos = new Vector3(pos.x + 0.1f, pos.y, pos.z);
            }
            else
            {              
                pos = new Vector3(pos.x - 0.1f, pos.y, pos.z);
            }

            thePlayer.transform.position = pos;
        }

    }

    void OnTriggerEnter(Collider collider)
    {     
        if (collider.CompareTag("Player") && overHead.IsOverHead() == false)
        {                
            slidePlayer = true;        
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            slidePlayer = false;
        }
    }

}
