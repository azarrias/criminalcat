using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackColliderBehaviour : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("enemy"))
            Destroy(other.gameObject);
    }
}
