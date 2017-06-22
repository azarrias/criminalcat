using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthAttackRockBehaviour : MonoBehaviour {

    public float rotationSpeed;
    private ParticleSystem dust;
    public int damage;

    private bool disipate = false;
    private float disipationCounter = 0.0f;
    public float disipationTime;


    void Awake()
    {
        Color color = transform.Find("rock").GetComponent<MeshRenderer>().sharedMaterial.color;
        color.a = 0.0f;
        transform.Find("rock").GetComponent<MeshRenderer>().sharedMaterial.color = color;
        dust = GetComponent<ParticleSystem>();
        dust.Stop();
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (!disipate)
            gameObject.transform.localRotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
        else
            Disipate();
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") || collider.gameObject.layer == LayerMask.NameToLayer("ground") || 
                collider.gameObject.layer == LayerMask.NameToLayer("wall") || collider.CompareTag("MovingDoor"))
        {
            dust.Play();
            collider.gameObject.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
            disipate = true;
        }
    }

    private void Disipate()
    {     
        Color color = gameObject.transform.Find("rock").GetComponent<MeshRenderer>().sharedMaterial.color;
        color.a -= color.a * 1 / disipationTime;
        if (color.a < 0.0f)
            color.a = 0.0f;
        gameObject.transform.Find("rock").GetComponent<MeshRenderer>().sharedMaterial.color = color;


        disipationCounter += Time.deltaTime;
        if (disipationCounter >= disipationTime)
        {
            disipationTime = 0.0f;
            disipate = false;
            gameObject.SetActive(false);                        
        }
    }
}
