using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Lightning : MonoBehaviour {

    private ParticleSystem lightningGenerator;

	// Use this for initialization
	void Start () {

        lightningGenerator = GetComponent<ParticleSystem>();
            
	}
	
	// Update is called once per frame
	void Update () {

        //Test particles activation/deactivation 
        //if (Input.GetKeyDown(KeyCode.RightControl))
        //{
        //    if (lightningGenerator.isStopped)
        //        ActivateLightning();
        //    else
        //        DeactivateLightning();
        //}
    }

    public void ActivateLightning()
    {
        lightningGenerator.GetComponent<ParticleSystemRenderer>().enabled = true;
        lightningGenerator.Play();
    }

    public void DeactivateLightning()
    {
        lightningGenerator.GetComponent<ParticleSystemRenderer>().enabled = false;
        lightningGenerator.Stop();
    }

    
}
