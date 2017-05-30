using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour {

    public int damage = 10;
    public float lifeTime = 3.0f;
    //ParticleSystem rockDustParticles;

    
    void OnEnable()
    {
        //rockDustParticles.Play();
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
        //rockDustParticles.Stop();
        gameObject.SetActive(false);
    }
}
