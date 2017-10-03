using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TornadoBehaviour : MonoBehaviour {

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
    public GameObject tornadoCircleGO;
    private ParticleSystem tornadoCirclesPS;
    private ParticleSystem.Particle[] tornadoCircles;
    public GameObject foggyBaseGO;
    private ParticleSystem foggyBasePS;
    public float fadeSpeed = 0.1f;
    public float fadeSeconds = 2.0f;
    public GameObject smallFragmentsGO;
    private ParticleSystem smallFragmentsPS;
    private float fadeCounter = 0.0f;

    private FSMBoss fsmBoss = null;
    private BossStats bossStats = null;

    private GameObject player;
    private PlayerStatus playerStatus;
    private AudioSource tornadoAudioSource;
    private bool firstTime = true;

    void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Dungeon Boss")
        {
            fsmBoss = GameObject.FindGameObjectWithTag("Boss").GetComponent<FSMBoss>();
            bossStats = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossStats>();
        }

        contains = new List<GameObject>();
        
        tornadoCirclesPS = tornadoCircleGO.GetComponent<ParticleSystem>();
        foggyBasePS = foggyBaseGO.GetComponent<ParticleSystem>();
        smallFragmentsPS = smallFragmentsGO.GetComponent<ParticleSystem>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerStatus = player.GetComponent<PlayerStatus>();

        tornadoEyeTr = transform.FindChild("TornadoEye");
        tornadoCircles = new ParticleSystem.Particle[tornadoCirclesPS.main.maxParticles];
    }

	void Start ()
    {                
        //tornadoEyeTr = transform.FindChild("TornadoEye");
        //tornadoCircles = new ParticleSystem.Particle[tornadoCirclesPS.main.maxParticles];
    }

    private void OnEnable()
    {
        if (!firstTime)
        {
            tornadoAudioSource = AudioManager.instance.PlayDiegeticFx(gameObject, playerStatus.fxTornado, false, 1.0f, AudioManager.FX_PLAYER_TORNADO_VOL);
            if (tornadoAudioSource)
            {
                AudioManager.instance.FadeAudioSource(tornadoAudioSource, FadeAudio.FadeType.FadeIn, fadeSeconds, 1.0f);
            }
        }
        else
            firstTime = false;
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

        if(enemy.CompareTag("Boss"))
        {
            enemy.GetComponent<BossStats>().initialRotation = enemy.transform.Find("ModelContainer").localRotation;
            enemy.GetComponent<BossStats>().initialPosition = enemy.transform.position;
        }
        else
        {
            enemy.GetComponent<EnemyStats>().initialRotation = enemy.transform.Find("ModelContainer").localRotation;
            enemy.GetComponent<EnemyStats>().initialPosition = enemy.transform.position;
        }
        
        Absorb(enemy);
        //enemy.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

        if (enemy.CompareTag("Boss"))
        {
            if (enemy.GetComponent<BossStats>().hitPoints > 0)
                rotate = true;
            else
                disipate = true;
        }
        else
        {
            if (enemy.GetComponent<EnemyStats>().hitPoints > 0)
                rotate = true;
            else
                disipate = true;
        }
                           
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
        AudioManager.instance.FadeAudioSource(tornadoAudioSource, FadeAudio.FadeType.FadeOut, fadeSeconds, 0.0f);
        foreach (var enemy in contains)
        {
            if(enemy.CompareTag("Boss"))
            {
                enemy.transform.Find("ModelContainer").localRotation = enemy.GetComponent<BossStats>().initialRotation;
                enemy.transform.position = enemy.GetComponent<BossStats>().initialPosition;
            }
            else
            {
                enemy.transform.Find("ModelContainer").localRotation = enemy.GetComponent<EnemyStats>().initialRotation;
                enemy.transform.position = enemy.GetComponent<EnemyStats>().initialPosition;
            }
            enemy.SendMessage("ApplyBloodyDamage", SendMessageOptions.DontRequireReceiver);
            enemy.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
            enemy.SendMessage("WakeUp", SendMessageOptions.DontRequireReceiver);
        }
    }
    private void Disipate()
    {
        triggerTaken = false;
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

        fadeCounter += Time.deltaTime;
        if (fadeCounter >= fadeSeconds)
        {
            //Debug.Log("BEFORE DEACTIVATING TORNADO ID = " + GetInstanceID() + " ROTATION TIME COUNTER = " + rotationTimeCounter);
            fadeCounter = 0.0f;
            disipate = false;
            foggyEmission.rateOverTime = 50.0f;
            dustEmission.rateOverTime = 50.0f;
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
