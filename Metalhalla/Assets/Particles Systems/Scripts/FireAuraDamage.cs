using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAuraDamage : MonoBehaviour {

    [HideInInspector]
    public bool preBallAttack = false;
    private bool playerInsideFireAura = true;
    private float waitCounter = 0.0f;
    public float waitTime = 0.5f;
    private bool applyAuraDamage = true;
    public int auraDamage = 1;
    private GameObject player;

    // Use this for initialization
    void Start () {

        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if(preBallAttack)
        {
            if (playerInsideFireAura && applyAuraDamage)
            {
                player.SendMessage("ApplyDamage", auraDamage, SendMessageOptions.DontRequireReceiver);
                applyAuraDamage = false;
            }

            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime)
            {
                waitCounter = 0.0f;
                applyAuraDamage = true;
            }
        }
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInsideFireAura = true;
            Debug.Log("Player inside fire aura");
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInsideFireAura = false;
            Debug.Log("Player outside fire aura");
        }
    }


}
