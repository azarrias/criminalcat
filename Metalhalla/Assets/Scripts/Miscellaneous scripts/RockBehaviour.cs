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
    private MeshRenderer rockMR;
    public float fadeoutSpeed = 0.01f;

    public bool hasHitShield = false;

    void Start()
    {
        rockDustParticles = GetComponent<ParticleSystem>();

        Transform rockTr = transform.Find("rock/Group6554");
        if(rockTr == null)
            rockTr = transform.Find("rock/rock");

        rockMR = rockTr.gameObject.GetComponent<MeshRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        if (disipate)
            Disipate();
        else if (hasHitShield)
            UpdateAfterShieldHit();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!disipate && !hasHitShield)
        {
            if (collider.CompareTag("Player") || collider.CompareTag("Viking"))
            {
                collider.gameObject.SendMessage("ApplyBloodyDamage", SendMessageOptions.DontRequireReceiver);
                collider.gameObject.SendMessage("ApplyDamage", damage);
            }
            else if (LayerMask.LayerToName(collider.gameObject.layer) == "shield")
            {
                hasHitShield = true;
            }
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
        Color color = rockMR.materials[0].color;

        color.a -= fadeoutSpeed;
        if (color.a < 0.0f)
            color.a = 0.0f;

        rockMR.materials[0].color = color;

        disipationCounter += Time.deltaTime;
        if(disipationCounter >= disipationTime)
        {
            disipationCounter = 0.0f;
            disipate = false;
            gameObject.SetActive(false);
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            Color startingColor = rockMR.materials[0].color;
            startingColor.a = 1.0f;
            rockMR.materials[0].color = startingColor;
        }
    }

    private void UpdateAfterShieldHit()
    {
        Debug.Log("Shield hit");
        disipate = true; 
    }
}
