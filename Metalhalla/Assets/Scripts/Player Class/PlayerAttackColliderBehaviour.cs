using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackColliderBehaviour : MonoBehaviour {

    public LayerMask hittableLayer;

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hittableLayer == (hittableLayer | (1 << other.gameObject.layer)))
        {
            //Destroy(other.gameObject);
            other.gameObject.SendMessage("ApplyDamage", 5, SendMessageOptions.DontRequireReceiver);

        }
    }
}
