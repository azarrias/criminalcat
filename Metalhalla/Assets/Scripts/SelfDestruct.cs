using UnityEngine;

public class SelfDestruct : MonoBehaviour {
    [SerializeField]
    float lifeTime = 5.0f;
	// Use this for initialization
	void Start () {
        Destroy(gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
