using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaftormStayTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
          if (other.transform.position.y >= transform.position.y)
                other.gameObject.transform.parent = transform.parent;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
            if (playerStatus.IsJump() == true || playerStatus.IsDash() || playerStatus.IsFall()  || playerStatus.IsWalk())
                other.gameObject.transform.parent = null; 
        }
    }
}
