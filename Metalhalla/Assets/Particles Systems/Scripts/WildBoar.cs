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
    private float timeToDeactivate;
    private float attackHorizontalRadius = 0.0f;
    private RaycastHit[] hits;
    public LayerMask attackableLayers;
    public float attackVerticalRange = 1.0f;
    private Vector3 halfExtents;

    void Awake()
    {
        trailParticles = trail.GetComponent<ParticleSystem>();
        dustParticles = dust.GetComponent<ParticleSystem>();
        stoneParticles = stoneEmitter.GetComponent<ParticleSystem>();
        timeToDeactivate = trailParticles.main.startLifetime.constant;
        attackHorizontalRadius = stoneParticles.shape.radius;
        halfExtents = new Vector3(0.1f, attackVerticalRange, 1.0f);
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
            MoveParticleSystem(moveDirection.normalized);
        }

        Debug.DrawLine(transform.position - attackHorizontalRadius * moveDirection.normalized , transform.position + attackHorizontalRadius * moveDirection.normalized, Color.red);
    }

    void MoveParticleSystem(Vector3 direction)
    {
        gameObject.transform.position += direction * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision other)
    {
        //Raycast in one dimension
        //float attackStartPosX = transform.position.x - attackHorizontalRadius * moveDirection.normalized.x;
        //Vector3 attackStartPos = new Vector3(attackStartPosX, transform.position.y, transform.position.y);  
        //hits = Physics.RaycastAll(attackStartPos, moveDirection.normalized, 2 * attackHorizontalRadius, attackableLayers);

        hits = Physics.BoxCastAll(transform.position, halfExtents, moveDirection.normalized, Quaternion.identity, attackHorizontalRadius, attackableLayers);
        
        if (hits.Length != 0)
        {
            ActivateStoneExplosion();

            for (int i = 0; i < hits.Length; i++)
            {
                if (LayerMask.LayerToName(hits[i].collider.gameObject.layer) != "destroyableEagle")
                {
                    hits[i].collider.gameObject.SendMessage("ApplyDamage", 0, SendMessageOptions.DontRequireReceiver);
                }
            }

        }    
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
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

    void ActivateStoneExplosion()
    {
        trailParticles.Stop();
        dustParticles.Play();
        stoneParticles.Play();
        stop = true;
        collision = true;
    }
}
