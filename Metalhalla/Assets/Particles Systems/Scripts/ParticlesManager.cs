﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{

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
    void Start()
    {

        //-------------------------------- TORNADO ---------------------
        particlesPool["tornado"] = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
            GameObject tornado = Instantiate(tornadoPrefab, Vector3.zero, Quaternion.identity);
            tornado.SetActive(false);
            tornado.transform.parent = transform;
            particlesPool["tornado"].Add(tornado);
        }

        //------------------------------- WILDBOAR ----------------------------
        particlesPool["wildboar"] = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
            GameObject wildboar = Instantiate(wildboarPrefab, Vector3.zero, Quaternion.identity);
            wildboar.SetActive(false);
            wildboar.transform.parent = transform;
            particlesPool["wildboar"].Add(wildboar);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static GameObject SpawnParticle(string name, Vector3 spawnPosition, bool facingRight)
    {
        GameObject particleToSpawn = null;

        foreach (GameObject particle in particlesManager.particlesPool[name])
        {
            if (!particle.activeSelf)
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

                particleToSpawn = particle;
                break;
            }
        }

        //no inactive gameObject found   
        if (particleToSpawn == null)
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
            particlesManager.particlesPool[name].Add(newParticle);
            particleToSpawn = newParticle;
        }

        return particleToSpawn;
    }
}
