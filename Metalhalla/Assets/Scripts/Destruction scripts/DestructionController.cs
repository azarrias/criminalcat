using UnityEngine;

public class DestructionController : MonoBehaviour {

     public GameObject remains;
    
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
            GameObject broken = Instantiate(remains, transform.position, transform.rotation);
            broken.GetComponent<SelfDestruct>().pushForceX = pushForceX;
            broken.GetComponent<SelfDestruct>().pushForceX = pushForceX;
            broken.GetComponent<SelfDestruct>().pushForceX = pushForceX;
            Destroy(gameObject);
        }
	}
}
