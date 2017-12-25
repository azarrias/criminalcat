using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkElfObserver : MonoBehaviour {

    public GameObject darkElf;
    private EnemyStats enemyStatsScript;

	// Use this for initialization
	void Start () {
        enemyStatsScript = darkElf.GetComponent<EnemyStats>();
	}
	
	// Update is called once per frame
	void Update () {
        if (enemyStatsScript.hitPoints <= 0)
            GetComponent<BoxCollider>().enabled = false;
	}
}
