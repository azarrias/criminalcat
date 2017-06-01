using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour {
    [HideInInspector]
    public Quaternion initialRotation = Quaternion.identity;
    [HideInInspector]
    public Vector3 initialPosition = Vector3.zero;
    public float normalSpeed = 1;
    public float chasingSpeed = 5;
    public int maxHitPoints = 200;
    public int hitPoints = 200;
    public int meleeDamage = 5;
    public int specialAttackDamage = 25;

    [Header("Sound Effects")]
    public AudioClip fxEnemyWasHit;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        //Save local rotation to return to it when the tornado vanishes
        if(collider.gameObject.CompareTag("Tornado"))
        {
            initialRotation = transform.localRotation;
            initialPosition = transform.position;
        }
    }

    public void ApplyDamage(int value)
    {
        AudioManager.instance.PlayFx(fxEnemyWasHit);
        hitPoints -= value;
    }
}
