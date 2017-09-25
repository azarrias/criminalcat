using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaGenerator : MonoBehaviour {

    public float newStaminaRecoveryRate = 2.0f;
    public GameObject staminaFakeDrop;

    private PlayerStatus playerStatus;
    private float originalRecoveryRate;

	void Start () {
        playerStatus = GameObject.FindWithTag("Player").GetComponent<PlayerStatus>();
        originalRecoveryRate = playerStatus.staminaRecoveryRate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerStatus.SetStaminaRecoveryParameters(true, newStaminaRecoveryRate);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerStatus.SetStaminaRecoveryParameters(false, originalRecoveryRate);
        }
    }

}
