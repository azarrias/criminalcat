using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour {

    public static ParticlesManager particlesManager = null;
    private Dictionary<string, List<GameObject>> particlesPool;
    public GameObject tornadoPrefab;


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

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    ///<summary>
    ///To spawn tornados use name = "tornado" 
    ///</summary>
    public static GameObject SpawnParticle(string name, Vector3 spawnPosition, bool facingRight)
    {
            
        foreach(GameObject tornado in particlesManager.particlesPool[name])
        {
            if(!tornado.activeSelf)
            {
                tornado.SetActive(true);
                tornado.transform.position = spawnPosition;
                tornado.GetComponent<TornadoBehaviour>().SetFacingRight(facingRight);
                tornado.GetComponent<ParticleSystem>().Play();
                return tornado;          
            }
        }

        //no inactive object found
        GameObject newTornado = Instantiate(particlesManager.tornadoPrefab, spawnPosition, Quaternion.identity);
        newTornado.SetActive(true);
        newTornado.transform.parent = particlesManager.transform;
        newTornado.GetComponent<ParticleSystem>().Play();
        particlesManager.particlesPool["tornado"].Add(newTornado);
        return newTornado;
    }
}
