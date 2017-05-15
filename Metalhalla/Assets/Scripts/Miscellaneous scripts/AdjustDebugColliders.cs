using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustDebugColliders : MonoBehaviour {

    public GameObject showBoxCollidersPrefab;
    
	// Use this for initialization
	void Start () {
       
        Transform bodyCubeTr = showBoxCollidersPrefab.transform.Find("BodyCube");
        Transform meleeCubeTr = showBoxCollidersPrefab.transform.Find("MeleeCube");
        Transform rangeCubeTr = showBoxCollidersPrefab.transform.Find("RangeCube");

        GameObject bossGO = transform.parent.gameObject;
        BoxCollider bossBodyCollider = bossGO.GetComponent<BoxCollider>();
        BoxCollider meleeRangeCollider = bossGO.transform.Find("MeleeRange").GetComponent<BoxCollider>();
        BoxCollider ballRangeCollider = bossGO.transform.Find("BallRange").GetComponent<BoxCollider>();

        //set body cube dimensions and position
        bodyCubeTr.localScale = bossBodyCollider.bounds.size;
        bodyCubeTr.position = bossBodyCollider.bounds.center;

        //set melee range cube dimensions and position
        meleeCubeTr.localScale = meleeRangeCollider.bounds.size;
        meleeCubeTr.position = meleeRangeCollider.bounds.center;

        //set ball range cube dimensions and position
        rangeCubeTr.localScale = ballRangeCollider.bounds.size;
        rangeCubeTr.position = ballRangeCollider.bounds.center;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
