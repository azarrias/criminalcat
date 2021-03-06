﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IceSpikesBehaviour : MonoBehaviour {

    private Animator spikesAnimator = null;
    private GameObject spikesLeft1 = null;
    private GameObject spikesRight1 = null;
    public bool leftSafe = false;
    public bool rightSafe = false;
    private GameObject thePlayer = null;
    public bool isPlayerSafe = false;
    public int spikesDamage = 25;

    private SafeAreaRight safeAreaRightScript;
    private SafeAreaLeft safeAreaLeftScript;

    private GameObject rightSphere = null;
    private GameObject leftSphere = null;

    // Use this for initialization
    void Start () {
        spikesAnimator = GetComponent<Animator>();

        spikesLeft1 = transform.FindChild("SpikesLeft1").gameObject;
        if (spikesLeft1 == null)
            Debug.LogError("Error : SpikesLeft1 not found");

        spikesRight1 = transform.FindChild("SpikesRight1").gameObject;
        if (spikesRight1 == null)
            Debug.LogError("Error : SpikesRight1 not found");
        
        thePlayer = GameObject.FindGameObjectWithTag("Player");
        if (thePlayer == null)
            Debug.Log("Error: player not found.");

        rightSphere = transform.parent.FindChild("RightSphere").gameObject;
        if (rightSphere == null)
            Debug.Log("Error: rightSphere not found.");

        leftSphere = transform.parent.FindChild("LeftSphere").gameObject;
        if (leftSphere == null)
            Debug.Log("Error: leftSphere not found.");

        safeAreaRightScript = transform.parent.FindChild("SafeAreaRight").gameObject.GetComponent<SafeAreaRight>();
        if (safeAreaRightScript == null)
            Debug.Log("Error: safeAreaRightScript not found");

        safeAreaLeftScript = transform.parent.FindChild("SafeAreaLeft").gameObject.GetComponent<SafeAreaLeft>();
        if (safeAreaLeftScript == null)
            Debug.Log("Error: safeAreaLeftScript not found");

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowIceSpikes()
    {
        spikesAnimator.SetBool("ShowIceSpikes", true);       
    }

    public void HideIceSpikes()
    {
        spikesAnimator.SetBool("HideIceSpikes", true);
    }

    public void ResetAnimator()
    {
        spikesAnimator.SetBool("Reset", true);
        spikesAnimator.SetBool("ShowIceSpikes", false);
        spikesAnimator.SetBool("HideIceSpikes", false);
        EnableLeftSpikes();
        EnableRightSpikes();
        isPlayerSafe = false;
        leftSphere.GetComponent<Renderer>().material.color = Color.grey;
        rightSphere.GetComponent<Renderer>().material.color = Color.grey;
    }

    public void EnableLeftSpikes()
    {
        spikesLeft1.SetActive(true);        
        leftSafe = false;
    }

    public void DisableLeftSpikes()
    {
        spikesLeft1.SetActive(false);        
        leftSafe = true;
    }

    public void EnableRightSpikes()
    {
        spikesRight1.SetActive(true);       
        rightSafe = false;
    }

    public void DisableRightSpikes()
    {
        spikesRight1.SetActive(false);
        rightSafe = true;
    }

    //Select the safe side to go while spikes are cast
    public void SelectIceSafe()
    {
        System.Random rand = new System.Random();
        int num = rand.Next(0, 2);

        if (num == 0)
           DisableLeftSpikes();
        if (num == 1)
           DisableRightSpikes();

        //Revisar cuando sepamos qué tipo de objeto nos dirá qué lado es el seguro
        if (leftSafe)
        {
            leftSphere.GetComponent<Renderer>().material.color = Color.green;
            rightSphere.GetComponent<Renderer>().material.color = Color.red;
        }
        if (rightSafe)
        {
            leftSphere.GetComponent<Renderer>().material.color = Color.red;
            rightSphere.GetComponent<Renderer>().material.color = Color.green;
        }

    }

    public void ApplySpikesDamage()
    {
        if(leftSafe && !safeAreaLeftScript.isOnLeftSafeArea)
            thePlayer.SendMessage("ApplyDamage", spikesDamage, SendMessageOptions.DontRequireReceiver);

        if (rightSafe && !safeAreaRightScript.isOnRightSafeArea)
            thePlayer.SendMessage("ApplyDamage", spikesDamage, SendMessageOptions.DontRequireReceiver);
    }

}
