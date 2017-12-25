using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildBoarBehaviour : MonoBehaviour
{

    public float speed = 1.0f;

    public GameObject trail;
    public GameObject dust;
    public GameObject stoneEmitter;
    public GameObject smallFragments;

    private ParticleSystem trailParticles;
    private ParticleSystem dustParticles;
    private ParticleSystem stoneParticles;
    private ParticleSystem smallFragmentsParticles;
    private Vector3 moveDirection = Vector3.right; //debug test
    private bool stop = false;
    private bool explosion = false;
    public float lifeTime = 2.0f;
    private int frames_counter = 0;
    private float timeToDeactivateNoExplosion = 5.0f;
    private float timeToDeactivateWithExplosion = 5.0f;
    private float attackHorizontalRadius = 0.0f;
    private RaycastHit[] hits;
    public LayerMask attackableLayers;
    public LayerMask blockingLayers;
    public float attackVerticalRange = 1.0f;
    private Vector3 halfExtents;
    public int damage = 20;
    private bool allowApplyDamage = true;
    private GameObject player;
    private PlayerStatus playerStatus;
    private AudioSource wildboardAudioSource;
    private bool firstTime = true;
    float fadeOutTime = 1.0f;

    void Awake()
    {
        trailParticles = trail.GetComponent<ParticleSystem>();
        dustParticles = dust.GetComponent<ParticleSystem>();
        stoneParticles = stoneEmitter.GetComponent<ParticleSystem>();
        smallFragmentsParticles = smallFragments.GetComponent<ParticleSystem>();
        timeToDeactivateNoExplosion = trailParticles.main.startLifetime.constant;
        timeToDeactivateWithExplosion = timeToDeactivateNoExplosion + stoneParticles.main.startLifetime.constant;
        attackHorizontalRadius = stoneParticles.shape.radius;
        halfExtents = new Vector3(0.1f, attackVerticalRange, 1.0f);
        player = GameObject.FindGameObjectWithTag("Player");
        playerStatus = player.GetComponent<PlayerStatus>();
    }

    // Use this for initialization
    void Start()
    {


    }

    void OnEnable()
    {
        trailParticles.Play();
        dustParticles.Stop();
        stoneParticles.Stop();
        smallFragmentsParticles.Stop();
        allowApplyDamage = true;

        if (!firstTime)
        {
            wildboardAudioSource = AudioManager.instance.PlayDiegeticFx(gameObject, playerStatus.fxWildboar, false, 1.0f, AudioManager.FX_PLAYER_WILDBOAR_VOL);    
        }
        else
            firstTime = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stop)
        {
            MoveParticleSystem(moveDirection);
        }

        Debug.DrawLine(transform.position - attackHorizontalRadius * moveDirection, transform.position + attackHorizontalRadius * moveDirection, Color.red);
        Debug.DrawLine(transform.position, transform.position + Vector3.up * attackVerticalRange, Color.red);
        Debug.DrawLine(transform.position, transform.position - Vector3.up * attackVerticalRange, Color.red);
    }

    void LateUpdate()
    {
        if (!stop)
        {
            frames_counter++;
            if (frames_counter * Time.deltaTime >= lifeTime)
            {
                stop = true;
                StopAttack();
                frames_counter = 0;
            }
        }
    }

    void MoveParticleSystem(Vector3 direction)
    {
        gameObject.transform.position += direction * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision other)
    {
        //Check if the "other" layer belongs to the attackable layers
        if (attackableLayers == (attackableLayers | (1 << other.gameObject.layer)) &&  allowApplyDamage)
        {
            allowApplyDamage = false;

            hits = Physics.BoxCastAll(transform.position, halfExtents, moveDirection.normalized, Quaternion.identity, attackHorizontalRadius, attackableLayers);

            if (hits.Length != 0)
            {
                ActivateStoneExplosion();
                AudioManager.instance.PlayDiegeticFx(gameObject, playerStatus.fxWildboarDestruction, false, 1.0f, AudioManager.FX_PLAYER_WILDBOAR_DESTRUCTION_VOL);

                for (int i = 0; i < hits.Length; i++)
                {
                    hits[i].collider.gameObject.SendMessage("ApplyBloodyDamage", SendMessageOptions.DontRequireReceiver);
                    hits[i].collider.gameObject.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);                   
                }

            }
        }

        if (blockingLayers == (blockingLayers | (1 << other.gameObject.layer)))
        {
            //Debug.Log("collision with blocking object");
            stop = true;
            StopAttack();
        }
    }

    public void SetFacingRight(bool facingRight)
    {
        if (facingRight)
            moveDirection = Vector3.right;
        else
            moveDirection = Vector3.left;
    }

    void ActivateStoneExplosion()
    {
        trailParticles.Stop();
        dustParticles.Play();
        stoneParticles.Play();
        smallFragmentsParticles.Play();
        stop = true;
        explosion = true;
        StopAttack();
    }

    void StopAttack()
    {
        if (wildboardAudioSource)
        {
            AudioManager.instance.FadeAudioSource(wildboardAudioSource, FadeAudio.FadeType.FadeOut, fadeOutTime, 0.0f);
        }

        if (!explosion)
        {
            trailParticles.Stop();
            stop = true;
            Invoke("DisableWildboar", timeToDeactivateNoExplosion);
        }
        else
        {
            Invoke("DisableWildboar", timeToDeactivateWithExplosion);
        }
    }

    void DisableWildboar()
    {
    //    Debug.Log("Disable wildboar");
        gameObject.SetActive(false);
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        frames_counter = 0;
        explosion = false;
        stop = false;
    }
}
