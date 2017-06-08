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
    private ParticleSystem.Particle[] baseEffects;

    public GameObject smallFragmentsGO;
    private ParticleSystem smallFragmentsPS;
    private ParticleSystem.Particle[] smallFragments;

    private bool disipating = false;
    private int counter = 0;
    
    public float fadeSpeed = 0.1f;
    public float fadeFrames = 120;

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
        baseEffects = new ParticleSystem.Particle[foggyBasePS.main.maxParticles];
        smallFragments = new ParticleSystem.Particle[smallFragmentsPS.main.maxParticles];
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

        if(disipating)
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

            numParticlesAlive = foggyBasePS.GetParticles(baseEffects);
            for (int i = 0; i < numParticlesAlive; i++)
            {
                Color newColor = baseEffects[i].GetCurrentColor(foggyBasePS);

                Debug.Log("color.a = " + newColor.a);

                newColor.a -= fadeSpeed * Time.deltaTime;
                if (newColor.a < 0.0f)
                    newColor.a = 0.0f;

                baseEffects[i].startColor = newColor;
                foggyBasePS.SetParticles(baseEffects, numParticlesAlive);
            }

            numParticlesAlive = smallFragmentsPS.GetParticles(smallFragments);
            for (int i = 0; i < numParticlesAlive; i++)
            {
                Color newColor = smallFragments[i].GetCurrentColor(smallFragmentsPS);
                newColor.a -= fadeSpeed * Time.deltaTime;
                if (newColor.a < 0.0f)
                    newColor.a = 0.0f;

                smallFragments[i].startColor = newColor;
                smallFragmentsPS.SetParticles(smallFragments, numParticlesAlive);
            }

            counter++;
            if(counter == fadeFrames)
            {
                gameObject.SetActive(false);
                counter = 0;
                disipating = false;
            }
        }
	}

    void OnTriggerEnter(Collider collider)
    {
        string colliderLayer = LayerMask.LayerToName(collider.gameObject.layer);
        if (colliderLayer == "ground")
        {
            DisipateTornado();
        }
        else if (colliderLayer == "destroyable" || colliderLayer == "destroyableEagle")
        {
            ApplyDamage(damage, collider.gameObject);
        }

        if (collider.gameObject.CompareTag("Viking")   && 
            collider.gameObject.GetComponent<FSMEnemy>().currentState != FSMEnemy.State.DEAD &&
            collider.gameObject.GetComponent<FSMEnemy>().currentState != FSMEnemy.State.STUNNED)
        {
            collider.gameObject.SendMessage("Stun", SendMessageOptions.DontRequireReceiver);
            contains.Add(collider.gameObject);
            ApplyDamage(damage, collider.gameObject);

            if (enemyInside == false)
            {
                enemyInside = true;

                StartCoroutine(ManageRotationDuration(rotationDuration));
            }
        }

        if (collider.gameObject.CompareTag("Dark Elf") &&
           collider.gameObject.GetComponent<FSMDarkElf>().currentState != FSMDarkElf.State.DEAD &&
           collider.gameObject.GetComponent<FSMDarkElf>().currentState != FSMDarkElf.State.STUNNED)
        {
            collider.gameObject.SendMessage("Stun", SendMessageOptions.DontRequireReceiver);
            contains.Add(collider.gameObject);
            ApplyDamage(damage, collider.gameObject);

            if (enemyInside == false)
            {
                enemyInside = true;

                StartCoroutine(ManageRotationDuration(rotationDuration));
            }
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

    private void DisipateTornado()
    {
        //tornado disipation effect
        //gameObject.GetComponent<ParticleSystem>().Stop();
        //gameObject.SetActive(false);
        disipating = true;

    }

    private IEnumerator ManageLifeTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if(enemyInside == false)
            DisipateTornado();
    }


    private IEnumerator ManageRotationDuration(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        enemyInside = false;
       
        foreach(GameObject go in contains)
        {          
            go.transform.position = go.GetComponent<EnemyStats>().initialPosition;
            go.transform.localRotation = go.GetComponent<EnemyStats>().initialRotation;
            go.SendMessage("WakeUp", SendMessageOptions.DontRequireReceiver);

            if(go.CompareTag("Boss"))
            {
                ApplyDamageBoss(damage);
            }
        }
        DisipateTornado();
        fsmBoss.IsInsideTornado(false);
        contains.Clear();
        angle = 0.0f;
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
        
        //if (Vector3.Distance(tornadoEyeTr.position, enemy.transform.position) > 0.5f)
        //{
        //    Vector3 direction = (tornadoEyeTr.position - enemy.transform.position).normalized;
        //    //enemy.transform.Translate(direction * Time.deltaTime);// creo que falla aquí
        //    Vector3 newPosition = new Vector3(
        //                        enemy.transform.position.x + direction.x * 5 * Time.deltaTime,
        //                        enemy.transform.position.y + direction.y + 5 * Time.deltaTime,
        //                        enemy.transform.position.z);

        //    enemy.transform.position = newPosition;

        //    Debug.Log("distance=" + Vector3.Distance(tornadoEyeTr.position, enemy.transform.position) + " " + "eyePos=" + tornadoEyeTr.position + "  " + "bossPoss=" + enemy.transform.position + " " + "direction=" + direction);
        //}
        //else
        //{
        //    ret = false;
        //}

        //return ret;

        //if(enemy.transform.position.x != tornadoEyeTr.position.x)
        //{

        //return false;
        //}

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
