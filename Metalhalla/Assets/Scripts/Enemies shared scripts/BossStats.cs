using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : MonoBehaviour {
    
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

    public void BossApplyDamage(int value)
    {
        //AudioManager.instance.PlayFx(fxEnemyWasHit);
        hitPoints -= value;
    }
}
