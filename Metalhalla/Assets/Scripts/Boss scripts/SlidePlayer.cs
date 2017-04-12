using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidePlayer : MonoBehaviour {

    private GameObject thePlayer = null;
    private PlayerStatus thePlayerStatus = null;
    private GameObject theBoss = null;
    private bool slidePlayer = false;
    public float slideSpeed = 10f;
    private bool slideLeft = true;
    private FSMBoss fsmBoss = null; 

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

        fsmBoss = theBoss.GetComponent<FSMBoss>();
        if (fsmBoss == null)
            Debug.Log("fsmBoss not found.");
    }
	
	// Update is called once per frame
	void Update () {
        if (slidePlayer && fsmBoss.GetCurrentState() == FSMBoss.State.BACK_TO_CENTER)
        {
            if (theBoss.transform.position.x >= thePlayer.transform.position.x)
                slideLeft = true;
            else
                slideLeft = false;

            Vector3 pos = thePlayer.transform.position;
            if (slideLeft)
            {
                pos = new Vector3(pos.x - slideSpeed * Time.deltaTime, pos.y, pos.z);
            }
            else
            {
                pos = new Vector3(pos.x + slideSpeed * Time.deltaTime, pos.y, pos.z);
            }

            thePlayer.transform.position = pos;
        }
    }

    void OnTriggerEnter(Collider collider)
    {     
        if (collider.CompareTag("Player"))
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
