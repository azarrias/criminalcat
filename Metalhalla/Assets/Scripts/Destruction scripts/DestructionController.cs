﻿using UnityEngine;

public class DestructionController : MonoBehaviour
{

    public GameObject remains;

    [Range(-1000, 1000)]
    public int pushForceX = 0;
    [Range(-1000, 1000)]
    public int pushForceY = 0;

    [Header("Camera Shake on destruction")]
    public float magnitude = 0.15f;
    public float duration = 0.2f;

    [Header("Sound FXs")]
    public AudioClip destructionSound;

    public void ApplyDamage(int dmg = 0)
    {
        ParticlesManager.SpawnParticle("hitEffect", transform.position, true);
        GameObject broken = Instantiate(remains, transform.position, transform.rotation);
        broken.GetComponent<AdjustDirection>().fragmentScale = transform.localScale;
        broken.GetComponent<AdjustDirection>().pushForceX = pushForceX;
        broken.GetComponent<AdjustDirection>().pushForceY = pushForceY;

        AudioManager.instance.PlayDiegeticFx(gameObject, destructionSound, false, 1.0f, AudioManager.FX_DESTRUCTION_VOL);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().StartShake(magnitude, duration);
        Destroy(gameObject);

    }
}