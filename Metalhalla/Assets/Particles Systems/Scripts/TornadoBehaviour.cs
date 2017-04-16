using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoBehaviour : MonoBehaviour {

    public float speed = 1.0f;
    public int damage = 1;
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
        if (LayerMask.LayerToName(collider.gameObject.layer) == "ground")
        {
            DisipateTornado();
        }

        if (collider.gameObject.CompareTag("Viking"))
        {
            contains.Add(collider.gameObject);

            if (enemyInside == false)
            {
                enemyInside = true;
                StartCoroutine(ManageRotationDuration(rotationDuration));               
            }                 
        }

        if (LayerMask.LayerToName(collider.gameObject.layer) == "destroyableEagle")
        {
            collider.gameObject.SendMessage("ApplyDamage", 1, SendMessageOptions.DontRequireReceiver);
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
            pos.y = go.GetComponent<EnemyStats>().initialHeight;            
            go.transform.position = pos;
            go.transform.localRotation = go.GetComponent<EnemyStats>().initialRotation;
        }
        DisipateTornado();
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


}
