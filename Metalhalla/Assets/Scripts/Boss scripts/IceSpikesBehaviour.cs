using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IceSpikesBehaviour : MonoBehaviour {

    private Animator spikesAnimator = null;
    private GameObject spikesLeft1 = null;
    private GameObject spikesLeft2 = null;
    private GameObject spikesRight1 = null;
    private GameObject spikesRight2 = null;
    public bool leftSafe = false;
    public bool rightSafe = false;
    public bool playerOnLeft = false;
    public bool playerOnRight = false;

	// Use this for initialization
	void Start () {
        spikesAnimator = GetComponent<Animator>();

        spikesLeft1 = transform.FindChild("SpikesLeft1").gameObject;
        if (spikesLeft1 == null)
            Debug.LogError("Error : SpikesLeft1 not found");

        spikesLeft2 = transform.FindChild("SpikesLeft2").gameObject;
        if (spikesLeft2 == null)
            Debug.LogError("Error : SpikesLeft2 not found");

        spikesRight1 = transform.FindChild("SpikesRight1").gameObject;
        if (spikesRight1 == null)
            Debug.LogError("Error : SpikesRight1 not found");

        spikesRight2 = transform.FindChild("SpikesRight2").gameObject;
        if (spikesRight2 == null)
            Debug.LogError("Error : SpikesRight2 not found");
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

    public void EnableLeftSpikes()
    {
        spikesLeft1.SetActive(true);
        spikesLeft2.SetActive(true);
        leftSafe = false;
    }

    public void DisableLeftSpikes()
    {
        spikesLeft1.SetActive(false);
        spikesLeft1.SetActive(false);
        leftSafe = true;
    }

    public void EnableRightSpikes()
    {
        spikesRight1.SetActive(true);
        spikesRight2.SetActive(true);
        rightSafe = false;
    }

    public void DisableRightSpikes()
    {
        spikesRight1.SetActive(false);
        spikesRight2.SetActive(false);
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
      
    }

}
