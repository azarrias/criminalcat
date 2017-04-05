using UnityEngine;
using System.Collections;

public class PlayerInteractor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void HitFromBelow (RaycastHit2D hit){
			hit.transform.gameObject.SendMessage ("HitFromBelow", SendMessageOptions.DontRequireReceiver);
	}
}
