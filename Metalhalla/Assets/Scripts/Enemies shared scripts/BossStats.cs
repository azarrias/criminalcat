using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : MonoBehaviour {
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

    private FSMBoss fsmBoss;

    [Header("Sound Effects")]
    public AudioClip fxEnemyWasHit;

    // Use this for initialization
    void Start () {

        fsmBoss = gameObject.GetComponent<FSMBoss>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        //Save local rotation to return to it when the tornado vanishes
        if (collider.gameObject.CompareTag("Tornado"))
        {
            if (fsmBoss.GetCurrentState() != FSMBoss.State.INSIDE_TORNADO)
            {
                initialRotation = transform.localRotation;
                initialPosition = transform.position;
            }
        }    
    }

    public void BossApplyDamage(int value)
    {
        //AudioManager.instance.PlayFx(fxEnemyWasHit);
        hitPoints -= value;
    }
}
