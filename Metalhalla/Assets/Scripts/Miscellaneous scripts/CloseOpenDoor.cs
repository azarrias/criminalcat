using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseOpenDoor : MonoBehaviour {

    private  GameObject movingDoor = null;
    private Vector3 localInitialPosition = new Vector3(89.5f, 17.47f, 0.0f);
    private bool closed = false;   
    private bool playerInside = false;

    void Awake()
    {
        movingDoor = GameObject.FindGameObjectWithTag("MovingDoor");
        if (movingDoor == null)
            Debug.Log("movingDoor not found.");   
    }

    void Start () {

        movingDoor.transform.localPosition = localInitialPosition;
    }
	
	// Update is called once per frame
	void Update () {
        if (!closed && playerInside)
            CloseDoor();
	}

    public void CloseDoor()
    {
        transform.position += Vector3.down * 4 * Time.deltaTime;
    }

    //To be called when player respawns
    public void OpenDoor()
    {
        gameObject.transform.localPosition = localInitialPosition;
        closed = false;
        playerInside = false; 
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "BossFightAreaFloor")
            closed = true;
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
