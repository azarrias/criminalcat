using UnityEngine;

public class SelfDestruct : MonoBehaviour {
    
     public float lifeTime = 5.0f;
    // Use this for initialization

    public int pushForceX = 0;
    public int pushForceY = 0;
    public int pushForceZ = 0;

    // Use this for initialization
    void Start()
    {
        foreach (Transform trf in gameObject.GetComponentInChildren<Transform>())
        {
            trf.GetComponent<Rigidbody>().AddForce(new Vector3(pushForceX, pushForceY, pushForceZ), ForceMode.VelocityChange);
        }
    
        Destroy(gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
