﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TornadoBehaviour3D : MonoBehaviour {

    public enum State
    {
        MOVE,
        ROTATE,
        DISIPATE
    }
    // ---------------------- VARIABLES TO MANAGE TORNADO FSM STATE TRANSITIONS ---------------
    private bool disipate = false;
    private bool rotate = false;

    //----------------------- TORNADO PROPERTIES -----------------
    private State tornadoState = State.MOVE;

    public float lifeTime = 10.0f;
    private float lifeTimeCounter = 0.0f;

    private Transform tornadoEyeTr = null;
    public float speed = 1.0f;
    private float angle = 0.0f;
    private float angularSpeed = 10.0f;
    private float rotationTimeCounter = 0.0f;
    private float rotationTime = 3.0f;
   
    public int damage = 20;
    private bool facingRight = true;
    private List<GameObject> contains;

    [HideInInspector]
    private Quaternion initialRotation = Quaternion.identity;
    [HideInInspector]
    private Vector3 initialPosition = Vector3.zero;
    
    private static bool triggerTaken = false;


    //------------------------ MANAGING TORNADO PARTICLE SYSTEM ON DISIPATION -----------------
    public GameObject tornadoBodyGO;
    private ParticleSystem tornadoBodyPS;
    private TornadoBody tornadoBodyScript;
    private ParticleSystem.Particle[] tornadoBodyParticles;
    private ParticleSystem.Particle[] tornadoBaseParticles;
    public GameObject tornadoBaseGO;
    private ParticleSystem tornadoBasePS;
    public float fadeSpeed = 0.1f;
    public float fadeSeconds = 2.0f;   
    private float fadeCounter = 0;

    private FSMBoss fsmBoss = null;
    private EnemyStats bossStats = null;

    void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Dungeon Boss")
        {
            fsmBoss = GameObject.FindGameObjectWithTag("Boss").GetComponent<FSMBoss>();
            bossStats = GameObject.FindGameObjectWithTag("Boss").GetComponent<EnemyStats>();
        }

        contains = new List<GameObject>();
        tornadoBodyScript = tornadoBodyGO.GetComponent<TornadoBody>();        
        tornadoBodyPS = tornadoBodyGO.GetComponent<ParticleSystem>();
        tornadoBasePS = tornadoBaseGO.GetComponent<ParticleSystem>();
               
    }

	void Start ()
    {                
        tornadoEyeTr = transform.FindChild("TornadoEye");
        tornadoBodyScript.CreateHelix();
        tornadoBodyParticles = new ParticleSystem.Particle[tornadoBodyPS.main.maxParticles];
        tornadoBaseParticles = new ParticleSystem.Particle[tornadoBasePS.main.maxParticles];
    }

    
	
	// Update is called once per frame
	void Update () {
        
        switch(tornadoState)
        {
            case State.MOVE:               
                Move();
                TickLifeTime();
                if (rotate)
                {                    
                    tornadoState = State.ROTATE;
                    lifeTimeCounter = 0.0f;  //reset counter                  
                    break;                        
                }
                if(disipate)
                {
                    OnDisipateEnter();
                    tornadoState = State.DISIPATE;                   
                    break;
                }
                break;

            case State.ROTATE:               
                Rotate();
                TickRotationTime();               
                if(disipate)
                {   
                    OnDisipateEnter();
                    tornadoState = State.DISIPATE;
                    break;
                }
                break;

            case State.DISIPATE:
                Disipate();
                break;                                              
        }  
    }

    void OnTriggerEnter(Collider collider)
    {
        string colliderLayer = LayerMask.LayerToName(collider.gameObject.layer);
        if (colliderLayer == "ground" || colliderLayer == "wall" || colliderLayer == "destroyableEagle") //Tornado doesnt break vertical walls
        {
            disipate = true;
        }
        else if (colliderLayer == "destroyable" /*|| colliderLayer == "destroyableEagle"*/)
        {
            collider.gameObject.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
        }

        else if (collider.gameObject.CompareTag("Viking"))
        {
            if (!disipate)
            {
                FSMEnemy.State state = collider.gameObject.GetComponent<FSMEnemy>().currentState;
                if (state != FSMEnemy.State.DEAD && state != FSMEnemy.State.STUNNED)
                {
                    PrepareRotation(collider.gameObject);
                }
            }
        }

        else if (collider.gameObject.CompareTag("Dark Elf"))
        {
            if (!disipate)
            {
                FSMDarkElf.State state = collider.gameObject.GetComponent<FSMDarkElf>().currentState;
                if (state != FSMDarkElf.State.DEAD && state != FSMDarkElf.State.STUNNED)
                {
                    PrepareRotation(collider.gameObject);
                }
            }
        }

        else if (collider.gameObject.name != "FireAura" && collider.gameObject.CompareTag("Boss"))
        {
            if (!triggerTaken && !disipate)
            {
                triggerTaken = true;

                FSMBoss.State state = fsmBoss.GetCurrentState();

                if (state == FSMBoss.State.CHASE && fsmBoss.prepareCast == false && bossStats.hitPoints > 0 ||
                    state == FSMBoss.State.PRE_MELEE_ATTACK ||
                    state == FSMBoss.State.MELEE_ATTACK ||
                    state == FSMBoss.State.POST_MELEE_ATTACK ||
                    state == FSMBoss.State.POST_BALL_ATTACK)
                {
                    PrepareRotation(collider.gameObject);
                }              
            }           
        }     
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.name == "FireAura")
            disipate = true;
    }



    // ---------------------- FSM METHODS -------------------------------

    private void TickLifeTime()
    {
        lifeTimeCounter += Time.deltaTime;
        if(lifeTimeCounter >= lifeTime)
        {
            lifeTimeCounter = 0.0f;
            disipate = true;
        }
    }

    private void TickRotationTime()
    {
        rotationTimeCounter += Time.deltaTime;
        if(rotationTimeCounter >= rotationTime)
        {
            rotationTimeCounter = 0.0f;
            disipate = true;
            rotate = false;
            angle = 0.0f;           
        }
    }

   private void Move()
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

    private void PrepareRotation(GameObject enemy)
    {
        enemy.SendMessage("Stunt", SendMessageOptions.DontRequireReceiver);
        enemy.GetComponent<EnemyStats>().initialRotation = enemy.transform.Find("ModelContainer").localRotation;
        enemy.GetComponent<EnemyStats>().initialPosition = enemy.transform.position;
        Absorb(enemy);       
        //enemy.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

        if (enemy.GetComponent<EnemyStats>().hitPoints > 0)
            rotate = true;
        else
            disipate = true;                   
    }

    private void Rotate()
    {
        if(Time.timeScale != 0.0f)
        {
            foreach (var enemy in contains)
            {
                Transform trf = enemy.transform.Find("ModelContainer");
                trf.localRotation *= Quaternion.Euler(0.0f, angle, 0.0f);
            }
            angle += angularSpeed * Time.deltaTime;
        }       
    }

    private void OnDisipateEnter()
    {
        foreach (var enemy in contains)
        {
            enemy.transform.Find("ModelContainer").localRotation = enemy.GetComponent<EnemyStats>().initialRotation;
            enemy.transform.position = enemy.GetComponent<EnemyStats>().initialPosition;
            enemy.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
            enemy.SendMessage("WakeUp", SendMessageOptions.DontRequireReceiver);
        }

        ParticleSystem.EmissionModule baseParticles = tornadoBasePS.emission;
        baseParticles.rateOverTime = 0.0f;

        ParticleSystem.EmissionModule bodyParticles = tornadoBodyPS.emission;
        bodyParticles.rateOverTime = 0.0f;
    }
    private void Disipate()
    {
        triggerTaken = false;

        int numParticlesAlive = tornadoBodyPS.GetParticles(tornadoBodyParticles);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            Color newColor = tornadoBodyParticles[i].GetCurrentColor(tornadoBodyPS);
            newColor.a -= fadeSpeed * Time.deltaTime;
            if (newColor.a < 0.0f)
                newColor.a = 0.0f;

            tornadoBodyParticles[i].startColor = newColor;
            tornadoBodyPS.SetParticles(tornadoBodyParticles, numParticlesAlive);
        }

        numParticlesAlive = tornadoBasePS.GetParticles(tornadoBaseParticles);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            Color newColor = tornadoBaseParticles[i].GetCurrentColor(tornadoBasePS);
            newColor.a -= fadeSpeed * Time.deltaTime;
            if (newColor.a < 0.0f)
                newColor.a = 0.0f;

            tornadoBaseParticles[i].startColor = newColor;
            tornadoBasePS.SetParticles(tornadoBaseParticles, numParticlesAlive);
        }

        fadeCounter += Time.deltaTime;
        if (fadeCounter >= fadeSeconds)
        {
            //Debug.Log("BEFORE DEACTIVATING TORNADO ID = " + GetInstanceID() + " ROTATION TIME COUNTER = " + rotationTimeCounter);
            fadeCounter = 0.0f;
            disipate = false;
            ParticleSystem.EmissionModule bodyParticles = tornadoBodyPS.emission;
            bodyParticles.rateOverTime = 15;
            ParticleSystem.EmissionModule baseParticles = tornadoBasePS.emission;
            baseParticles.rateOverTime = 10;            
            contains.Clear();
            gameObject.SetActive(false);
            tornadoState = State.MOVE; //reset state           
        }
    }

    private void Absorb(GameObject enemy)
    {
        enemy.transform.position = new Vector3(tornadoEyeTr.position.x, tornadoEyeTr.position.y, enemy.transform.position.z);
        contains.Add(enemy);
    }
     
    //Method called by the tornado user
    public void SetFacingRight(bool facingRight)
    {
        this.facingRight = facingRight;
    }
}
