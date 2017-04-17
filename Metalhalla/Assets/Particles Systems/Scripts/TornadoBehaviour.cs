using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoBehaviour : MonoBehaviour {

    public float speed = 1.0f;
    public int damage = 20;
    private bool facingRight = true;
    private float lifeTime = 10.0f;
    private bool platformLimitReached = false;
    private float rotationDuration = 3.0f;
    private bool enemyInside = false;
    private List<GameObject> contains;
    private float angularSpeed = 10.0f;
    private float angle = 0.0f;
    private Vector3 translation;
    private Transform tornadoEyeTr = null;
    private FSMBoss fsmBoss = null;

    void Awake()
    {
        fsmBoss = FindObjectOfType<FSMBoss>();
        if (fsmBoss == null)
            Debug.Log("Error: fsmBoss not found.");
    }

	void Start () {

        contains = new List<GameObject>();
        StartCoroutine(ManageLifeTime(lifeTime));
        tornadoEyeTr = transform.FindChild("TornadoEye");   
	}
	
	// Update is called once per frame
	void Update () {
        if (!enemyInside)
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
                if (!AbsorbEnemy(go))
                {
                    RotateEnemy(go);
                }
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

        if (collider.gameObject.CompareTag("Viking"))
        {
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
            ApplyDamageBoss(damage);
            FSMBoss.State state = fsmBoss.GetCurrentState();
            if(state == FSMBoss.State.CHASE || state == FSMBoss.State.BALL_ATTACK || state == FSMBoss.State.MELEE_ATTACK)
            {
                contains.Add(collider.gameObject);
                fsmBoss.IsInsideTornado(true);
                if (enemyInside == false)
                {
                    enemyInside = true;

                    StartCoroutine(ManageRotationDuration(rotationDuration));
                }
            }
        }
    }

    private void DisipateTornado()
    {
        //tornado disipation effect
      
        Destroy(gameObject);
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
        Vector3 pos;
        foreach(GameObject go in contains)
        {
            pos = go.transform.position;            
            pos = go.GetComponent<EnemyStats>().initialPosition;            
            go.transform.position = pos;
            go.transform.localRotation = go.GetComponent<EnemyStats>().initialRotation;
        }
        DisipateTornado();
        fsmBoss.IsInsideTornado(false);
        contains.Clear();
    }

    private void RotateEnemy(GameObject enemy)
    {
        Transform trf = enemy.transform;

        angle += angularSpeed * Time.deltaTime;
        trf.localRotation *= Quaternion.AngleAxis(angle, Vector3.up);   
    }

    private bool AbsorbEnemy(GameObject enemy)
    {
        bool ret = true;
        if (Vector3.Distance(tornadoEyeTr.position, enemy.transform.position) > 0.5f)
        {
            Vector3 direction = (tornadoEyeTr.position - enemy.transform.position).normalized;
            translation += direction * 5 * Time.deltaTime;
            enemy.transform.Translate(translation);
        }
        else
        {
            ret = false;
        }

        return ret;
    }

    public void SetFacingRight (bool newValue)
    {
        facingRight = newValue;
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
