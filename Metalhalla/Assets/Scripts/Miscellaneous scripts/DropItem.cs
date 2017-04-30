using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour {

    [Header("Drop Behaviour")]
    
    public int healthRestore = 0;
    public int staminaRestore = 1;
    public int beerRestore = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider collision)
    {   
        if (collision.tag == "Player")
        {
            if (healthRestore > 0)
                collision.GetComponent<PlayerStatus>().RestoreHealth(healthRestore);
            else if (staminaRestore > 0)
                collision.GetComponent<PlayerStatus>().RestoreStamina(staminaRestore);
            else if (beerRestore > 0)
                collision.GetComponent<PlayerStatus>().RefillBeer(beerRestore);
            Destroy(this.gameObject);
        }
    }
}
