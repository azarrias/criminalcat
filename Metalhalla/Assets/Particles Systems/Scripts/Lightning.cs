using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Lightning : MonoBehaviour {

    private ParticleSystem lightningGenerator;
    private List<GameObject> enemiesAtRange;
    public int frames = 10;
    private int frameCount = 0;
    private int index = 0;
    private int dictionaryLength;
    public float force = 10.0f;

	// Use this for initialization
	void Start () {

        lightningGenerator = GetComponent<ParticleSystem>();
            
	}
	
	// Update is called once per frame
	void Update () {

        //apply force to lightning particles

    }

    void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Enemy")
        {
            enemiesAtRange.Add(other.gameObject);          
        }
    }

    void OnTriggerExit(Collider other) //Ver qué pasa si other muere y se destruye su collider. Si other.gameObject == null petará
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Enemy")
        {
            int index = enemiesAtRange.IndexOf(other.gameObject);

            if (index != -1)
            {
                enemiesAtRange.RemoveAt(index);
            }
        }      
    } 
    
    void AddForceOnParticles(Vector3 target)
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[lightningGenerator.particleCount];
        int length = lightningGenerator.GetParticles(particles);
        for (int i = 0; i < length; i++)
        {
            Vector3 vel = force * (target - particles[i].position).normalized;
            particles[i].velocity = vel;
        }

        lightningGenerator.SetParticles(particles, length);
    }  
}
