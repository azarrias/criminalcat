using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthAuraDamage : MonoBehaviour {
    [HideInInspector]
    public bool auraActive = false;
    private bool playerInsideEarthAura = true;
    private float waitCounter = 0.0f;
    public float waitTime = 0.5f;
    private bool applyAuraDamage = true;
    public int auraDamage = 1;
    private GameObject player;

    public float allowedTimeNoDamage;
    private float timeNoDamageCounter = 0.0f;

    // Use this for initialization
    void Start()
    {

        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (auraActive)
        {
            timeNoDamageCounter += Time.deltaTime;
            if(timeNoDamageCounter >= allowedTimeNoDamage)
            {

                if (playerInsideEarthAura && applyAuraDamage)
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
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInsideEarthAura = true;
            Debug.Log("Player inside earth aura");

            timeNoDamageCounter = 0.0f;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInsideEarthAura = false;
            Debug.Log("Player outside earth aura");
        }
    }
}
