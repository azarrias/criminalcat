using UnityEngine;

public class DestructionController : MonoBehaviour {

    public GameObject remains;
    public GameObject content;
    
    [Range(-20, 20)]
    public int pushForceX = 4;
    [Range(-20, 20)]
    public int pushForceY = 4;
    [Range(-20, 20)]
    public int pushForceZ = 0;


    // Update is called once per frame
    void Update () {

		if(Input.GetKey(KeyCode.LeftAlt))
        {
            ApplyDamage(); 
        }
	}

    public void ApplyDamage(int dmg = 0)
    {
        GameObject broken = Instantiate(remains, transform.position, transform.rotation);
        broken.GetComponent<SelfDestruct>().pushForceX = pushForceX;
        broken.GetComponent<SelfDestruct>().pushForceX = pushForceX;
        broken.GetComponent<SelfDestruct>().pushForceX = pushForceX;
        if (content != null)
            Instantiate(content, new Vector3 (transform.position.x, transform.position.y, 0), transform.rotation);

        Destroy(gameObject);
        
    }
}
