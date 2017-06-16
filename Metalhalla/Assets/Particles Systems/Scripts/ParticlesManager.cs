using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{

    public static ParticlesManager particlesManager = null;
    private Dictionary<string, List<GameObject>> particlesPool;
    public GameObject tornadoPrefab;
    public GameObject wildboarPrefab;
    public GameObject bossFireBallPrefab;
    public GameObject elfFireBallPrefab;
    public GameObject bloodPrefab;
    private GameObject particlesPrefab;
   

    void Awake()
    {
        if (particlesManager != null)
        {
            Destroy(this);
        }
        else
        {
            particlesManager = this;
            particlesPool = new Dictionary<string, List<GameObject>>();
            particlesManager.transform.position = Vector3.zero;
        }
    }


    // Use this for initialization
    void Start()
    {

        //-------------------------------- TORNADO ---------------------
        particlesPool["tornado"] = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
            GameObject tornado = Instantiate(tornadoPrefab, Vector3.zero, Quaternion.identity);
            tornado.SetActive(false);
            tornado.transform.parent = transform;
            particlesPool["tornado"].Add(tornado);
        }

        //------------------------------- WILDBOAR ----------------------------
        particlesPool["wildboar"] = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
            GameObject wildboar = Instantiate(wildboarPrefab, Vector3.zero, Quaternion.identity);
            wildboar.SetActive(false);
            wildboar.transform.parent = transform;
            particlesPool["wildboar"].Add(wildboar);
        }

        //------------------------------- BOSS FIREBALL ------------------------
        particlesPool["bossFireBall"] = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
            GameObject bossFireBall = Instantiate(bossFireBallPrefab, Vector3.zero, Quaternion.identity);
            bossFireBall.SetActive(false);
            bossFireBall.transform.parent = transform;
            particlesPool["bossFireBall"].Add(bossFireBall);
        }

        //------------------------------- ELF FIREBALL ------------------------
        particlesPool["elfFireBall"] = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
            GameObject elfFireBall = Instantiate(elfFireBallPrefab, Vector3.zero, Quaternion.identity);
            elfFireBall.SetActive(false);
            elfFireBall.transform.parent = transform;
            particlesPool["elfFireBall"].Add(elfFireBall);
        }

        //------------------------------- BLOOD PARTICLES  ------------------------
        particlesPool["blood"] = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
            GameObject blood = Instantiate(bloodPrefab, Vector3.zero, Quaternion.identity);
            blood.SetActive(false);
            blood.transform.parent = transform;
            particlesPool["blood"].Add(blood);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static GameObject SpawnParticle(string name, Vector3 spawnPosition, bool facingRight)
    {
        GameObject particleToSpawn = null;

        foreach (GameObject particle in particlesManager.particlesPool[name])
        {
            if (!particle.activeSelf)
            {
                particle.SetActive(true);
                particle.transform.position = spawnPosition;

                if (name == "tornado")
                {
                    particle.GetComponent<TornadoBehaviour>().SetFacingRight(facingRight);
                }
                else if (name == "wildboar")
                {
                    particle.GetComponent<WildBoarBehaviour>().SetFacingRight(facingRight);
                }
                else if (name == "bossFireBall")
                {
                    particle.GetComponent<BossFireBallBehaviour>().SetFacingRight(facingRight);
                }
                else if(name == "blood")
                {
                    particle.GetComponent<BloodBehaviour>().SetFacingRight(facingRight);
                }
                
                particleToSpawn = particle;
                break;
            }
        }

        //no inactive gameObject found   
        if (particleToSpawn == null)
        {
            if (name == "tornado")
            {
                particlesManager.particlesPrefab = particlesManager.tornadoPrefab;
            }
            else if (name == "wildboar")
            {
                particlesManager.particlesPrefab = particlesManager.wildboarPrefab;
            }
            else if (name == "bossFireBall")
            {
                particlesManager.particlesPrefab = particlesManager.bossFireBallPrefab;
            }
            else if (name == "blood")
            {
                particlesManager.particlesPrefab = particlesManager.bloodPrefab;
            }

            GameObject newParticle = Instantiate(particlesManager.particlesPrefab, spawnPosition, Quaternion.identity);
            newParticle.SetActive(true);
            newParticle.transform.parent = particlesManager.transform;
            particlesManager.particlesPool[name].Add(newParticle);
            particleToSpawn = newParticle;
        }

        return particleToSpawn;
    }

    //Dark Elves fireballs
    public static GameObject SpawnElfFireBall(Vector3 spawnPosition, Vector3 ballDirection)
    {
        GameObject particleToSpawn = null;

        foreach (GameObject particle in particlesManager.particlesPool["elfFireBall"])
        {
            if (!particle.activeSelf)
            {
                particle.SetActive(true);
                particle.transform.Find("Ball").gameObject.SetActive(true);
                particle.transform.Find("BallExplosion").gameObject.SetActive(false);
                particle.transform.position = spawnPosition;
                particle.GetComponent<ElfFireBallBehaviour>().SetDirection(ballDirection);

                particleToSpawn = particle;
                break;
            }
        }

        //no inactive gameObject found   
        if (particleToSpawn == null)
        {
            particlesManager.particlesPrefab = particlesManager.elfFireBallPrefab;

            GameObject newParticle = Instantiate(particlesManager.particlesPrefab, spawnPosition, Quaternion.identity);
            newParticle.GetComponent<ElfFireBallBehaviour>().SetDirection(ballDirection);
            newParticle.SetActive(true);
            newParticle.transform.Find("Ball").gameObject.SetActive(true);
            newParticle.transform.Find("BallExplosion").gameObject.SetActive(false);
            newParticle.transform.parent = particlesManager.transform;
            particlesManager.particlesPool["elfFireBall"].Add(newParticle);
            particleToSpawn = newParticle;
        }

        return particleToSpawn;
    }
}
