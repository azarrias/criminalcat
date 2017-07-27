﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullsAttackBehaviour : MonoBehaviour {

    public float rotationSpeed;
    private ParticleSystem dust;
    public int damage;

    private bool disipate = false;
    public float disipationScaleSpeed = 1.0f;
    public float maxScale = 1.0f;

    private GameObject skullMesh;

    [HideInInspector]
    public GameObject parentGO; //boss

    void Awake()
    {
        skullMesh = transform.Find("SkullMesh").gameObject;      
        dust = GetComponent<ParticleSystem>();
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (parentGO.activeSelf == false) //boss is dead
        {           
            skullMesh.GetComponent<SphereCollider>().enabled = false;               
        }

        if (!disipate)
            gameObject.transform.localRotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
        else
        {           
           skullMesh.GetComponent<SphereCollider>().enabled = false;              
           Disipate();
        }
	}

    void OnTriggerEnter(Collider collider)
    {      
        if (collider.CompareTag("Player") || collider.gameObject.layer == LayerMask.NameToLayer("ground") ||
                collider.gameObject.layer == LayerMask.NameToLayer("wall") || collider.CompareTag("MovingDoor"))
        {
            dust.Play();
            collider.gameObject.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            disipate = true;
                
        }      
    }

    private void Disipate()
    {
        Vector3 scale = skullMesh.transform.localScale;
        scale.x -= disipationScaleSpeed * Time.deltaTime;
        if (scale.x <= 0.0f)
            scale.x = 0.0f;

        scale.y -= disipationScaleSpeed * Time.deltaTime;
        if (scale.y <= 0.0f)
            scale.y = 0.0f;

        scale.z -= disipationScaleSpeed * Time.deltaTime;
        if (scale.z <= 0.0f)
            scale.z = 0.0f;

        skullMesh.transform.localScale = scale;
        if (scale.x == 0.0f)
        {           
            disipate = false;
            gameObject.SetActive(false);
            //Set boss as parent
            gameObject.transform.parent = parentGO.transform;
        }
    }

    public void ShrinkSkulls()
    {
        disipate = true;
    }
}
