using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLeaveTrigger : MonoBehaviour {

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
            //if (playerStatus.IsJump() == true || playerStatus.IsDash() || playerStatus.IsFall() || playerStatus.IsWalk())
                other.gameObject.transform.parent = null;
        }
    }
}
