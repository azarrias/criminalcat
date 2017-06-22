using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackColliderBehaviour : MonoBehaviour {

    public LayerMask hittableLayer;
    public AudioClip fxHit;

    private void OnTriggerEnter(Collider other)
    {
        if (hittableLayer == (hittableLayer | (1 << other.gameObject.layer)))
        {
            // todo: update with modifiable damage
            other.gameObject.SendMessage("ApplyDamage", 10, SendMessageOptions.DontRequireReceiver);
            if (fxHit && (other.CompareTag("Viking") || other.CompareTag("Dark Elf") || other.CompareTag("Boss")) )
                AudioManager.instance.PlayFx(fxHit);
        }
    }
}
