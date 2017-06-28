using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlayerPositioning : MonoBehaviour {

    [Tooltip("The player gameobject to be positioned in the debugpoints")]
    public GameObject player;
    [Tooltip("Debug Points variable gets filled in runtime, with all the children debug points, and in the order they are attached to the parent. Use this for testing and debugging.")]
    [SerializeField]
    private Transform[] debugPoints;

    private void Start()
    {
        debugPoints = GetComponentsInChildren<Transform>();
        // careful! index 0 corresponds to THE PARENT
    }

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1))
            player.transform.position = debugPoints[1].position;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            player.transform.position = debugPoints[2].position;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            player.transform.position = debugPoints[3].position;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            player.transform.position = debugPoints[4].position;
        if (Input.GetKeyDown(KeyCode.Alpha5))
            player.transform.position = debugPoints[5].position;
        if (Input.GetKeyDown(KeyCode.Alpha6))
            player.transform.position = debugPoints[6].position;
    }
}
