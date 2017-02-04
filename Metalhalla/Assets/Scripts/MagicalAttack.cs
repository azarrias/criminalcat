using UnityEngine;

public class MagicalAttack : MonoBehaviour {

    public GameObject dragonHead;
    public float dragonCooldown = 10.0f;

    private bool isDragonInCooldown = false;
    private float dragonTimeCounter = 0.0f;
    
	// Use this for initialization
	void Start () {

    }
    
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!isDragonInCooldown)
            {
                isDragonInCooldown = true;
                Vector3 dragonPos = transform.position;
                dragonPos.y += 2;
                GameObject dragon = Instantiate(dragonHead, dragonPos, transform.rotation);
                dragon.transform.parent = transform;
            }
        }

        if(isDragonInCooldown)
        {
            dragonTimeCounter += Time.deltaTime;
            Debug.Log("dragonTimeCounter = " + dragonTimeCounter);

            if (dragonTimeCounter >= dragonCooldown)
            {
                isDragonInCooldown = false;
                dragonTimeCounter = 0.0f;
               
            }
        }

	}  
}
