using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour {

    public int damage = 1;
    public float lifeTime = 3.0f;
	// Use this for initialization
	void Start () {

        StartCoroutine("DestroyRock", lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnColissionEnter(Collision collision)
    {
        
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SendMessage("ApplyDamage", damage);
        }

    }

    private IEnumerator DestroyRock(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        //Crear algún efecto de polvo o algo con sistemas de partículas
        Destroy(gameObject, 0.5f);
    }
}
