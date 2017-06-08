using UnityEngine;

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

    public void ApplyDamage(int dmg = 0)
    {
        GameObject broken = Instantiate(remains, transform.position, transform.rotation);
        broken.GetComponent<AdjustDirection>().fragmentScale = transform.localScale;
        broken.GetComponent<AdjustDirection>().pushForceX = pushForceX;
        broken.GetComponent<AdjustDirection>().pushForceY = pushForceY;

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().StartShake(magnitude, duration);
        Destroy(gameObject);

    }
}