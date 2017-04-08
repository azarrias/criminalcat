using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOverHead : MonoBehaviour {

    private bool isOverHead = false;
    private PlayerCollider playerCollider;
	// Use this for initialization
	void Start () {
        playerCollider = FindObjectOfType<PlayerCollider>();
        if (playerCollider == null)
            Debug.LogError("playerCollider not found.");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        if (playerCollider.IsGrounded())
        {
            isOverHead = false;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
           isOverHead = true;
    }

    public bool IsOverHead()
    {
        return isOverHead;
    }
}
