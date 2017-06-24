using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightRange : MonoBehaviour {

    FSMBoss fsmBoss = null;
    GameObject[] movingDoors;
    BoxCollider sightRange = null;
    
    void Awake()
    {
        fsmBoss = FindObjectOfType<FSMBoss>();
        if (fsmBoss == null)
            Debug.LogError("Error: fsmBoss not found");

        movingDoors = GameObject.FindGameObjectsWithTag("MovingDoor");
        if (movingDoors.Length == 0)
            Debug.Log("Error : movingDoor not found.");

        sightRange = GetComponent<BoxCollider>();

        //Collider starting size
        sightRange.size = new Vector3(24.0f, 7.0f, 5.0f);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {

            sightRange.size = new Vector3(35.5f, 7.0f, 5.0f);
            foreach (var movingDoor in movingDoors)
            {
                movingDoor.SendMessage("PlayerInside");
            }
            
            fsmBoss.playerInSight = true;

            Vector3 playerPos = collider.gameObject.transform.position;
            Vector3 bossPos = fsmBoss.transform.position;

            float diff = playerPos.x - bossPos.x;
            if (diff > 0)
            {
                if (fsmBoss.facingRight == false)
                {
                    fsmBoss.Flip();
                }
            }
            if (diff < 0)
            {
                if (fsmBoss.facingRight == true)
                {
                    fsmBoss.Flip();
                }
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            //Collider starting size
            sightRange.size = new Vector3(24.0f, 7.0f, 5.0f);
            fsmBoss.playerInSight = false;
        }

        if (collider.CompareTag("Boss"))
        {
            fsmBoss.Flip();
        }
    }  
}
