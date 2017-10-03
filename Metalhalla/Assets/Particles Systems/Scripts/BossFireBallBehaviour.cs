using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireBallBehaviour : MonoBehaviour {

    public float lifeTime = 3.0f;
    public float speed = 1.0f;
    private Vector3 direction;
    public int ballDamage = 15;
    private float lifeTimeCounter = 0.0f;
    private float deactivationTime = 2.0f;
    private float deactivationCounter = 0.0f;
    private bool deactivate = false;
    private bool generatingBall = false;
    private GameObject ballExplosion;
    private GameObject ball;
    private GameObject smoke;
    private float timeToGenerate = 1.5f;
    private FSMBoss fsmBoss;
    private GameObject particlesManager;

    [Header("Sound FXs")]
    public AudioClip fireBall;

    void Awake()
    {
        ballExplosion = gameObject.transform.Find("BallExplosion").gameObject;
        ball = gameObject.transform.Find("Ball").gameObject;
        smoke = gameObject.transform.Find("Ball/Smoke").gameObject;
        if (GameObject.FindGameObjectWithTag("Boss"))
        {
            fsmBoss = GameObject.FindGameObjectWithTag("Boss").GetComponent<FSMBoss>();
            timeToGenerate = fsmBoss.preBallAttackDuration;
        }
        particlesManager = GameObject.Find("ParticlesManager");    
    }

	// Use this for initialization
	void Start() {
        ball.transform.localScale = Vector3.zero;
        gameObject.GetComponent<SphereCollider>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if(generatingBall)
        {
            Vector3 scale = ball.transform.localScale;
            scale.x += Time.deltaTime / timeToGenerate;
            scale.y += Time.deltaTime / timeToGenerate;
            scale.z += Time.deltaTime / timeToGenerate;
            ball.transform.localScale = scale;

            if (scale.x >= 1.0f)
            {
                generatingBall = false;
                SetFacingRight(fsmBoss.facingRight);
                smoke.SetActive(true);
                gameObject.GetComponent<SphereCollider>().enabled = true;
                AudioManager.instance.PlayDiegeticFx(gameObject, fireBall, false, 1.0f, AudioManager.FX_BOSS_FIREBALL_VOL);
            }                       
        }
        else if (!generatingBall && !deactivate)
        {
            transform.Translate(direction * speed * Time.deltaTime);

            lifeTimeCounter += Time.deltaTime;
            if (lifeTimeCounter >= lifeTime)
            {
                lifeTimeCounter = 0.0f;
                deactivate = true;
                ballExplosion.SetActive(true);
                ball.SetActive(false);
            }
        }
        else if (deactivate)
            Deactivate();
	}

    void OnCollisionEnter(Collision collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "ground" || 
            LayerMask.LayerToName(collision.gameObject.layer) == "wall" ||
            LayerMask.LayerToName(collision.gameObject.layer) == "player" ||
            LayerMask.LayerToName(collision.gameObject.layer) == "shield")
        {
            collision.gameObject.SendMessage("ApplyDamage", ballDamage, SendMessageOptions.DontRequireReceiver);
            ballExplosion.SetActive(true);
            ball.SetActive(false);
            gameObject.GetComponent<SphereCollider>().enabled = false;
            deactivate = true;
            lifeTimeCounter = 0.0f;  //reset lifetime
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("MovingDoor"))
        {
            ballExplosion.SetActive(true);
            ball.SetActive(false);
            gameObject.GetComponent<SphereCollider>().enabled = false;
            deactivate = true;
            lifeTimeCounter = 0.0f;  //reset lifetime
        }
    }

    public void SetFacingRight(bool facingRight)
    {
        Transform ball = gameObject.transform.Find("Ball").transform;

        if (facingRight)
        {
            direction = Vector3.right;

            if (ball.eulerAngles.y != 90.0f)
                ball.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        }
        else
        {
            direction = Vector3.left;

            if (ball.eulerAngles.y != -90.0f)
                ball.localRotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
        }
    }

    void Deactivate()
    {
        deactivationCounter += Time.deltaTime;
        if (deactivationCounter >= deactivationTime)
        {
            deactivationCounter = 0.0f;
            deactivate = false;
            gameObject.SetActive(false);
            //gameObject.GetComponent<SphereCollider>().enabled = true;
            ball.transform.localScale = Vector3.zero;                   
        }
    }

    public void GenerateBall()
    {
        generatingBall = true;
        smoke.SetActive(false);
    }

    public void SetBossAsParent()
    {
        transform.parent = fsmBoss.gameObject.transform;
    }

    public void SetPMAsParent()
    {
        transform.parent = particlesManager.transform;
    }
}
