using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildBoar : MonoBehaviour
{

    public float speed = 1.0f;

    public GameObject trail;
    public GameObject dust;
    public GameObject stoneEmitter;

    private ParticleSystem trailParticles;
    private ParticleSystem dustParticles;
    private ParticleSystem stoneParticles;
    private Vector3 moveDirection = Vector3.right; //debug test
    private bool stop = false;
    private bool collision = false;
    public float lifeTime = 2.0f;
    public float timeToDeactivate;


    void Awake()
    {
        trailParticles = trail.GetComponent<ParticleSystem>();
        dustParticles = dust.GetComponent<ParticleSystem>();
        stoneParticles = stoneEmitter.GetComponent<ParticleSystem>();

        timeToDeactivate = trailParticles.main.startLifetime.constant;
    }

    // Use this for initialization
    void Start()
    {

        trailParticles.Play();
        dustParticles.Stop();
        stoneParticles.Stop();
        Invoke("StopAttack", lifeTime);
    }

    // Update is called once per frame
    void Update()
    {

        if (!stop)
        {
            MoveParticleSystem(moveDirection);
        }
    }

    void MoveParticleSystem(Vector3 direction)
    {
        gameObject.transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "destroyableWildboar")
        {
            trailParticles.Stop();
            dustParticles.Play();
            stoneParticles.Play();
            stop = true;
            collision = true;
            other.gameObject.SendMessage("ApplyDamage", 0, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }

    void StopAttack()
    {
        if (!collision)
        {
            trailParticles.Stop();
            stop = true;
            Invoke("DisableWildboar", timeToDeactivate);
        }
    }

    void DisableWildboar()
    {
        gameObject.SetActive(false);
    }
}
