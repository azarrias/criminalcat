using UnityEngine;

public class DestructionController : MonoBehaviour
{

    public GameObject remains;

    [Range(-1000, 1000)]
    public int pushForceX = 0;
    [Range(-1000, 1000)]
    public int pushForceY = 0;


    // Update is called once per frame
    void Update()
    {

    }

    public void ApplyDamage(int dmg = 0)
    {
        GameObject broken = Instantiate(remains, transform.position, transform.rotation);
        broken.GetComponent<AdjustDirection>().fragmentScale = transform.localScale;
        broken.GetComponent<AdjustDirection>().pushForceX = pushForceX;
        broken.GetComponent<AdjustDirection>().pushForceY = pushForceY;

        Destroy(gameObject);

    }
}