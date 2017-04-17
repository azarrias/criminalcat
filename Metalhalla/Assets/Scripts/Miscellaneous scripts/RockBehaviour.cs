using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour {

    public int damage = 10;
    public float lifeTime = 3.0f;
	// Use this for initialization
	void Start () {

        StartCoroutine("DestroyRock", lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        
        if(collider.CompareTag("Player"))
        {
            collider.gameObject.SendMessage("ApplyDamage", damage);
        }

    }

    private IEnumerator DestroyRock(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        //Crear algún efecto de polvo o algo con sistemas de partículas
        Destroy(gameObject, 0.5f);
    }
}
