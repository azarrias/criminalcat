using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IceSpikesBehaviour3D : MonoBehaviour {

    private Animator spikesAnimator = null;
    private GameObject spikesLeft1 = null;
    private GameObject spikesLeft2 = null;
    private GameObject spikesRight1 = null;
    private GameObject spikesRight2 = null;
    public bool leftSafe = false;
    public bool rightSafe = false;
    private GameObject thePlayer = null;
    public int spikesDamage = 25;

    private SafeAreaRight safeAreaRightScript;
    private SafeAreaLeft safeAreaLeftScript;

    //private GameObject rightSphere = null;
    //private GameObject leftSphere = null;

    private ParticleSystem leftDust1PS;
    private ParticleSystem leftDust2PS;
    private ParticleSystem middleDustPS;
    private ParticleSystem rightDust1PS;
    private ParticleSystem rightDust2PS;
    


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

        thePlayer = GameObject.FindGameObjectWithTag("Player");
        if (thePlayer == null)
            Debug.Log("Error: player not found.");

        //rightSphere = transform.parent.FindChild("RightSphere").gameObject;
        //if (rightSphere == null)
        //    Debug.Log("Error: rightSphere not found.");

        //leftSphere = transform.parent.FindChild("LeftSphere").gameObject;
        //if (leftSphere == null)
        //    Debug.Log("Error: leftSphere not found.");

        safeAreaRightScript = transform.parent.FindChild("SafeAreaRight").gameObject.GetComponent<SafeAreaRight>();
        if (safeAreaRightScript == null)
            Debug.Log("Error: safeAreaRightScript not found");

        safeAreaLeftScript = transform.parent.FindChild("SafeAreaLeft").gameObject.GetComponent<SafeAreaLeft>();
        if (safeAreaLeftScript == null)
            Debug.Log("Error: safeAreaLeftScript not found");

        leftDust1PS = transform.parent.FindChild("LeftDust1").gameObject.GetComponent<ParticleSystem>();
        leftDust2PS = transform.parent.FindChild("LeftDust2").gameObject.GetComponent<ParticleSystem>();
        middleDustPS = transform.parent.FindChild("MiddleDust").gameObject.GetComponent<ParticleSystem>();
        rightDust1PS = transform.parent.FindChild("RightDust1").gameObject.GetComponent<ParticleSystem>();
        rightDust2PS = transform.parent.FindChild("RightDust2").gameObject.GetComponent<ParticleSystem>();
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
        if (spikesAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            leftDust1PS.gameObject.GetComponent<EarthAuraDamage>().auraActive = false;
            leftDust1PS.Stop();

            leftDust2PS.gameObject.GetComponent<EarthAuraDamage>().auraActive = false;
            leftDust2PS.Stop();

            middleDustPS.gameObject.GetComponent<EarthAuraDamage>().auraActive = false;
            middleDustPS.Stop();

            rightDust1PS.gameObject.GetComponent<EarthAuraDamage>().auraActive = false;
            rightDust1PS.Stop();

            rightDust2PS.gameObject.GetComponent<EarthAuraDamage>().auraActive = false;
            rightDust2PS.Stop();
        }
    }

    public void ResetAnimator()
    {
        spikesAnimator.SetBool("Reset", true);
        spikesAnimator.SetBool("ShowIceSpikes", false);
        spikesAnimator.SetBool("HideIceSpikes", false);
        EnableLeftSpikes();
        EnableRightSpikes();       
        //leftSphere.GetComponent<Renderer>().material.color = Color.grey;
        //rightSphere.GetComponent<Renderer>().material.color = Color.grey;


    }

    public void EnableLeftSpikes()
    {
        spikesLeft2.SetActive(true);        
        leftSafe = false;
    }

    public void DisableLeftSpikes()
    {
        spikesLeft2.SetActive(false);        
        leftSafe = true;
    }

    public void EnableRightSpikes()
    {
        spikesRight2.SetActive(true);       
        rightSafe = false;
    }

    public void DisableRightSpikes()
    {
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

        //Revisar cuando sepamos qué tipo de objeto nos dirá qué lado es el seguro
        if (leftSafe)
        {
            leftDust1PS.Play();
            leftDust1PS.gameObject.GetComponent<EarthAuraDamage>().auraActive = true;

            middleDustPS.Play();
            middleDustPS.gameObject.GetComponent<EarthAuraDamage>().auraActive = true;

            rightDust1PS.Play();
            rightDust1PS.gameObject.GetComponent<EarthAuraDamage>().auraActive = true;

            rightDust2PS.Play();
            rightDust2PS.gameObject.GetComponent<EarthAuraDamage>().auraActive = true;

            //leftSphere.GetComponent<Renderer>().material.color = Color.green;
            //rightSphere.GetComponent<Renderer>().material.color = Color.red;
        }
        if (rightSafe)
        {
            rightDust1PS.Play();
            rightDust1PS.gameObject.GetComponent<EarthAuraDamage>().auraActive = true;

            middleDustPS.Play();
            middleDustPS.gameObject.GetComponent<EarthAuraDamage>().auraActive = true;

            leftDust1PS.Play();
            leftDust1PS.gameObject.GetComponent<EarthAuraDamage>().auraActive = true;

            leftDust2PS.Play();
            leftDust2PS.gameObject.GetComponent<EarthAuraDamage>().auraActive = true;

            //leftSphere.GetComponent<Renderer>().material.color = Color.red;
            //rightSphere.GetComponent<Renderer>().material.color = Color.green;
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
