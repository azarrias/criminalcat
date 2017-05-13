using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticles : MonoBehaviour {

    public Transform spawnPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
        {
            ParticlesManager.SpawnParticle("tornado", spawnPosition.position, true) ;
        }

        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            ParticlesManager.SpawnParticle("wildboar", spawnPosition.position, true);
        }
    }
}
