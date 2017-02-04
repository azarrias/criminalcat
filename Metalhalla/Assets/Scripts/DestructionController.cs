using UnityEngine;

public class DestructionController : MonoBehaviour {

    [SerializeField]
    private GameObject remains;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Space))
        {
            Instantiate(remains, transform.position, transform.rotation);
            Destroy(gameObject);
        }
	}
}
