using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IceSpikesBehaviour : MonoBehaviour {

    Animator spikesAnimator = null;

	// Use this for initialization
	void Start () {
        spikesAnimator = GetComponent<Animator>();       
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
    }

    public void EnableAnimator()
    {
        spikesAnimator.enabled = true;
    }

    public void DisableAnimator()
    {
        spikesAnimator.enabled = false;
    }
}
