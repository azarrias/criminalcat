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
    public int maxHitPoints = 50    ;
    public int hitPoints = 50;
    public int meleeDamage = 10;
    //public int specialAttackDamage = 25;

    [Header("Enemy recoils when taking damage")]
    public float hitRecoil = 3.0f;
    public float deadRecoil = 1.0f;

    [Header("Sound Effects")]
    public AudioClip fxEnemyWasHit;

    // Use this for initialization
    void Start () {

       
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ApplyDamage(int value)
    {
        //AudioManager.instance.PlayFx(fxEnemyWasHit);
        hitPoints -= value;
    }
}
