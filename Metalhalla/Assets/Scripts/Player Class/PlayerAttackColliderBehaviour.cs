using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackColliderBehaviour : MonoBehaviour {

    public LayerMask hittableLayer;
    private CameraFollow camFollow;

    private void Start()
    {
        camFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hittableLayer == (hittableLayer | (1 << other.gameObject.layer)))
        {
            other.gameObject.SendMessage("ApplyDamage", 10, SendMessageOptions.DontRequireReceiver);
            camFollow.StartShake();
        }
    }
}
