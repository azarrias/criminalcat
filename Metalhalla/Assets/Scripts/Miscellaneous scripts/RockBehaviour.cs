﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour {

    public int damage = 10;
    public float lifeTime = 3.0f;
    private ParticleSystem rockDustParticles;
    public bool disipate = false;
    public float disipationTime = 2.0f;
    private float disipationCounter = 0.0f;    
    private MeshRenderer estalactiteMR;
    public float fadeoutSpeed = 0.01f;

    void Start()
    {
        rockDustParticles = GetComponent<ParticleSystem>();
        GameObject estalactiteMeshGO = transform.Find("estalactita/Group6554").gameObject;
        estalactiteMR = estalactiteMeshGO.GetComponent<MeshRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        if (disipate)
            Disipate();
    }

    void OnTriggerEnter(Collider collider)
    {       
        if(collider.CompareTag("Player") && !disipate)
        {
            collider.gameObject.SendMessage("ApplyDamage", damage);
        }
    }

    private void Disipate()
    {
        if (disipationCounter == 0.0f)
        {
            rockDustParticles.Play();
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }

        //fadeout
        Color color = estalactiteMR.materials[0].color;

        color.a -= fadeoutSpeed;
        if (color.a < 0.0f)
            color.a = 0.0f;

        estalactiteMR.materials[0].color = color;

        disipationCounter += Time.deltaTime;
        if(disipationCounter >= disipationTime)
        {
            disipationCounter = 0.0f;
            disipate = false;
            gameObject.SetActive(false);
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            Color startingColor = estalactiteMR.materials[0].color;
            startingColor.a = 1.0f;
            estalactiteMR.materials[0].color = startingColor;
        }
    }
}
