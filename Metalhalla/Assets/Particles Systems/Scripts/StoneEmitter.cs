using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneEmitter : MonoBehaviour {

    public ParticleSystem stoneEmitter;
    private ParticleSystem.Particle[] particles;
    public float heightThreshold = 1.0f;

	// Use this for initialization
	void Start () {

        stoneEmitter = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[stoneEmitter.main.maxParticles];

	}
	
	// Update is called once per frame
	void Update () {

        int numParticles = stoneEmitter.GetParticles(particles);
        for (int i = 0; i < numParticles; i++)
        {  
          //Debug.Log("particle pos z = " + particles[i].position.z + "    particles velocity z = " + particles[i].velocity.z);
          if(particles[i].position.z < 1.0f && particles[i].velocity.z < -5.0f)
            {
                Debug.Log("ang vel=" + particles[i].angularVelocity);
                particles[i].angularVelocity = 0.0f;
            }

        }

	}

    //void OnParticleCollision(GameObject other)
    //{

    //}
}
