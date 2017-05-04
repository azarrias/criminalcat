using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour {

    public int damage = 10;
    public float lifeTime = 3.0f;

    void OnEnable()
    {
        //Invoke("DestroyRock", lifeTime);
    }

    void OnTriggerEnter(Collider collider)
    {       
        if(collider.CompareTag("Player"))
        {
            collider.gameObject.SendMessage("ApplyDamage", damage);
        }
    }

    private void DestroyRock()
    {      
       gameObject.SetActive(false);
    }
}
