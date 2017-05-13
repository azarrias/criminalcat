using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour {

    public static ParticlesManager particlesManager = null;
    private Dictionary<string, List<GameObject>> particlesPool;
    public GameObject tornadoPrefab;
    public GameObject wildboarPrefab;
    private GameObject particlesPrefab;

    void Awake()
    {
        if (particlesManager != null)
        {
            Destroy(this);
        }
        else
        {
            particlesManager = this;
            particlesPool = new Dictionary<string, List<GameObject>>();
            particlesManager.transform.position = Vector3.zero;
        }  
    }


	// Use this for initialization
	void Start () {

        //-------------------------------- TORNADO ---------------------
        particlesPool["tornado"] = new List<GameObject>();

        GameObject tornado1 = Instantiate(tornadoPrefab, Vector3.zero, Quaternion.identity);
        tornado1.SetActive(false);
        tornado1.transform.parent = transform;
        particlesPool["tornado"].Add(tornado1);

        GameObject tornado2 = Instantiate(tornadoPrefab, Vector3.zero, Quaternion.identity);
        tornado2.SetActive(false);
        tornado2.transform.parent = transform;
        particlesPool["tornado"].Add(tornado2);

        GameObject tornado3 = Instantiate(tornadoPrefab, Vector3.zero, Quaternion.identity);
        tornado3.SetActive(false);
        tornado3.transform.parent = transform;
        particlesPool["tornado"].Add(tornado3);


        //------------------------------- WILDBOAR ----------------------------
        particlesPool["wildboar"] = new List<GameObject>();

        GameObject wildboar1 = Instantiate(wildboarPrefab, Vector3.zero, Quaternion.identity);
        wildboar1.SetActive(false);
        wildboar1.transform.parent = transform;
        particlesPool["wildboar"].Add(wildboar1);

        GameObject wildboar2 = Instantiate(wildboarPrefab, Vector3.zero, Quaternion.identity);
        wildboar2.SetActive(false);
        wildboar2.transform.parent = transform;
        particlesPool["wildboar"].Add(wildboar2);

        GameObject wildboar3 = Instantiate(wildboarPrefab, Vector3.zero, Quaternion.identity);
        wildboar3.SetActive(false);
        wildboar3.transform.parent = transform;
        particlesPool["wildboar"].Add(wildboar3);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public static GameObject SpawnParticle(string name, Vector3 spawnPosition, bool facingRight)
    {
        GameObject particleToSpawn = null;
        
        foreach(GameObject particle in particlesManager.particlesPool[name])
        {
            if(!particle.activeSelf)
            {             
                particle.SetActive(true);
                particle.transform.position = spawnPosition;

                if (name == "tornado")
                {
                    particle.GetComponent<TornadoBehaviour>().SetFacingRight(facingRight);
                }
                else if (name == "wildboar")
                {
                    particle.GetComponent<WildBoarBehaviour>().SetFacingRight(facingRight);
                }

                //particlesEffect.GetComponent<ParticleSystem>().Play();
               particleToSpawn = particle;
               break;                     
            }
        }

        //no inactive gameObject found   
        if(particleToSpawn == null)
        {
            if (name == "tornado")
            {
                particlesManager.particlesPrefab = particlesManager.tornadoPrefab;               
            }
            else if (name == "wildboar")
            {
                particlesManager.particlesPrefab = particlesManager.wildboarPrefab;              
            }

            GameObject newParticle = Instantiate(particlesManager.particlesPrefab, spawnPosition, Quaternion.identity);
            newParticle.SetActive(true);
            newParticle.transform.parent = particlesManager.transform;
            //newParticles.GetComponent<ParticleSystem>().Play();
            particlesManager.particlesPool[name].Add(newParticle);
            particleToSpawn = newParticle;
        }

        return particleToSpawn;     
    }  
}
