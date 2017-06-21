using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneTrigger : MonoBehaviour {

    public string newSceneName = "Dungeon Boss";
    private SceneLoader loader; 


	void Start () {
        loader = GameObject.FindWithTag("SceneLoader").GetComponent<SceneLoader>();
        GetComponent<Renderer>().enabled = false; 
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            loader.GoToNextScene( newSceneName ); 
    }
}
