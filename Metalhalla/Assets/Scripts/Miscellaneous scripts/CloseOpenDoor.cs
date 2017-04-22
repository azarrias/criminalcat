using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseOpenDoor : MonoBehaviour {

    private  GameObject movingDoor = null;
    private Vector3 initialPosition = new Vector3(89.5f, 17.47f, 0.0f);
    private bool grounded = false;   
    private bool playerInside = false;

    void Awake()
    {
        movingDoor = GameObject.FindGameObjectWithTag("MovingDoor");
        if (movingDoor == null)
            Debug.Log("movingDoor not found.");   
    }

    void Start () {

       // movingDoor.transform.position = initialPosition;
    }
	
	// Update is called once per frame
	void Update () {
        if (!grounded && playerInside)
            CloseDoor();
	}

    public void CloseDoor()
    {
        transform.position += Vector3.down * 4 * Time.deltaTime;
    }

    //To be called when player respawns
    public void OpenDoor()
    {
        gameObject.transform.position = initialPosition;
        grounded = false;
        playerInside = false; 
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "BossFightAreaFloor")
            grounded = true;
    }

    public void PlayerInside()
    {
        playerInside = true;
    }

    public void PlayerOutside()
    {
        playerInside = false;
    }
}
