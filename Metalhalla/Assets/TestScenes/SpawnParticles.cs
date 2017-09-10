using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticles : MonoBehaviour {

    public Transform spawnPosition;
    public GameObject target;
    public GameObject particlesPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //ParticlesManager.SpawnParticle("tornado3D", spawnPosition.position, true);
            ParticlesManager.SpawnParticle("wildboar", spawnPosition.position, true);
            //GameObject go = Instantiate(particlesPrefab, spawnPosition.position, Quaternion.identity);
            //go.GetComponent<TornadoBehaviour>().SetFacingRight(true);
        }

        //if (Input.GetKeyDown(KeyCode.RightControl))
        //{
        //    ParticlesManager.SpawnParticle("wildboar", spawnPosition.position, true);
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ParticlesManager.SpawnElfFireBall(spawnPosition.position, target.transform.position - transform.position);
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ParticlesManager.SpawnParticle("blood", spawnPosition.position, true);
        //}

    }
}
