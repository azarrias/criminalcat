using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoBehaviour : MonoBehaviour {

    public float speed = 1.0f;
    public int damage = 20;
    private bool facingRight = true;
    public float lifeTime = 10.0f;
    private float rotationDuration = 3.0f;
    private bool enemyInside = false;
    private List<GameObject> contains;
    private float angularSpeed = 10.0f;
    private float angle = 0.0f;
    private Transform tornadoEyeTr = null;
    private FSMBoss fsmBoss = null;

    public GameObject tornadoCircleGO;
    private ParticleSystem tornadoCirclesPS;
    private ParticleSystem.Particle[] tornadoCircles;

    public GameObject foggyBaseGO;
    private ParticleSystem foggyBasePS;

    public GameObject smallFragmentsGO;
    private ParticleSystem smallFragmentsPS;
    
    private bool disipating = false;
    private int counter = 0;
    
    public float fadeSpeed = 0.1f;
    public float fadeFrames = 120;
    float acum = 0.0f;

    void Awake()
    {
        fsmBoss = FindObjectOfType<FSMBoss>();
        if (fsmBoss == null)
            Debug.Log("Error: fsmBoss not found.");
        contains = new List<GameObject>();

        tornadoCirclesPS = tornadoCircleGO.GetComponent<ParticleSystem>();
        foggyBasePS = foggyBaseGO.GetComponent<ParticleSystem>();
        smallFragmentsPS = smallFragmentsGO.GetComponent<ParticleSystem>();
    }

	void Start ()
    {                
        tornadoEyeTr = transform.FindChild("TornadoEye");
        tornadoCircles = new ParticleSystem.Particle[tornadoCirclesPS.main.maxParticles];
    }

    void OnEnable()
    {
        StartCoroutine(ManageLifeTime(lifeTime));
    }
	
	// Update is called once per frame
	void Update () {
        if (!enemyInside && !disipating)
        {
            if (facingRight)
            {
                gameObject.transform.Translate(Vector3.right * speed * Time.deltaTime);
            }
            else
            {
                gameObject.transform.Translate(Vector3.left * speed * Time.deltaTime);
            }
        }

        if(enemyInside)
        {
            foreach(GameObject go in contains)
            {
                //if (!AbsorbEnemy(go))
                //{              
                    AbsorbEnemy(go);
                    RotateEnemy(go);           
                //}
            }
        }        
    }

    void LateUpdate()
    {
        if (disipating)
        {
            int numParticlesAlive = tornadoCirclesPS.GetParticles(tornadoCircles);
            for (int i = 0; i < numParticlesAlive; i++)
            {
                Color newColor = tornadoCircles[i].GetCurrentColor(tornadoCirclesPS);
                newColor.a -= fadeSpeed * Time.deltaTime;
                if (newColor.a < 0.0f)
                    newColor.a = 0.0f;

                tornadoCircles[i].startColor = newColor;
                tornadoCirclesPS.SetParticles(tornadoCircles, numParticlesAlive);
            }

            ParticleSystem.EmissionModule foggyEmission = foggyBasePS.emission;
            foggyEmission.rateOverTime = 0.0f;

            ParticleSystem.EmissionModule dustEmission = smallFragmentsPS.emission;
            dustEmission.rateOverTime = 0.0f;

            counter++;
            if (counter == fadeFrames)
            {
                gameObject.SetActive(false);
                counter = 0;
                disipating = false;
                foggyEmission.rateOverTime = 50.0f;
                dustEmission.rateOverTime = 50.0f;
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        string colliderLayer = LayerMask.LayerToName(collider.gameObject.layer);
        if (colliderLayer == "ground")
        {
            disipating = true;
        }
        else if (colliderLayer == "destroyable" || colliderLayer == "destroyableEagle")
        {
            ApplyDamage(damage, collider.gameObject);
        }

        if (collider.gameObject.CompareTag("Viking")   && 
            collider.gameObject.GetComponent<FSMEnemy>().currentState != FSMEnemy.State.DEAD &&
            collider.gameObject.GetComponent<FSMEnemy>().currentState != FSMEnemy.State.STUNNED)
        {
            PrepareRotation(collider.gameObject);           
        }

        if (collider.gameObject.CompareTag("Dark Elf") &&
           collider.gameObject.GetComponent<FSMDarkElf>().currentState != FSMDarkElf.State.DEAD &&
           collider.gameObject.GetComponent<FSMDarkElf>().currentState != FSMDarkElf.State.STUNNED)
        {
            PrepareRotation(collider.gameObject);
        }

        if (collider.gameObject.CompareTag("Boss"))
        {           
            FSMBoss.State state = fsmBoss.GetCurrentState();
            if (state != FSMBoss.State.DEAD)
            {
                if (state == FSMBoss.State.CHASE || state == FSMBoss.State.BALL_ATTACK || state == FSMBoss.State.MELEE_ATTACK)
                {
                    contains.Add(collider.gameObject);
                    fsmBoss.IsInsideTornado(true);                    
                    fsmBoss.NotifyRotationDuration(rotationDuration);
                    if (enemyInside == false)
                    {
                        enemyInside = true;
                        StartCoroutine(ManageRotationDuration(rotationDuration));
                    }                   
                }
            }
        }
    }

    private IEnumerator ManageLifeTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if(enemyInside == false)
            disipating = true;
    }


    private IEnumerator ManageRotationDuration(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        enemyInside = false;
       
        foreach(GameObject go in contains)
        {                                 
            if(go.CompareTag("Boss"))
            {
                go.transform.position = go.GetComponent<BossStats>().initialPosition;
                go.transform.localRotation = go.GetComponent<BossStats>().initialRotation;
                ApplyDamageBoss(damage);
            }

            if(go.CompareTag("Viking") || go.CompareTag("Dark Elf"))
            {
                go.transform.position = go.GetComponent<EnemyStats>().initialPosition;
                go.transform.localRotation = go.GetComponent<EnemyStats>().initialRotation;                
                go.SendMessage("WakeUp", SendMessageOptions.DontRequireReceiver);
                ApplyDamage(damage, go);
            }
        }

        disipating = true;
        fsmBoss.IsInsideTornado(false);
        contains.Clear();
        angle = 0.0f;
    }

    //For Dark Elves and Vikings
    private void PrepareRotation(GameObject enemy)
    {
        enemy.GetComponent<EnemyStats>().initialRotation = enemy.transform.localRotation;
        enemy.GetComponent<EnemyStats>().initialPosition = enemy.transform.position;

        enemy.SendMessage("Stun", SendMessageOptions.DontRequireReceiver);
        contains.Add(enemy);
        
        if (enemyInside == false)
        {
            enemyInside = true;

            StartCoroutine(ManageRotationDuration(rotationDuration));
        }
    }

    private void RotateEnemy(GameObject enemy)
    {
        Transform trf = enemy.transform;

        angle += angularSpeed * Time.deltaTime;

        if (Time.timeScale != 0.0f)
            trf.localRotation *= Quaternion.Euler(0.0f, angle, 0.0f);
        else
            trf.localRotation = trf.localRotation;

    }

    private void AbsorbEnemy(GameObject enemy)
    {
        enemy.transform.position = new Vector3(tornadoEyeTr.position.x, tornadoEyeTr.position.y, 0.0f);
        
    }

    public void SetFacingRight (bool facingRight)
    {
        this.facingRight = facingRight;
    }

    private void ApplyDamage(int damage, GameObject go)
    {
        go.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
    }

    private void ApplyDamageBoss(int damage)
    {
        fsmBoss.ApplyDamage(damage);
    }
}
