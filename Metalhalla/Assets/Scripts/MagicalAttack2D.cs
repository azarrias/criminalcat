using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalAttack2D : MonoBehaviour {


    public GameObject dragonHead;
    public float dragonCooldown = 10.0f;

    private bool isDragonInCooldown = false;
    private float dragonTimeCounter = 0.0f;

    // Update is called once per frame
    void Update()
    {
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

        if (isDragonInCooldown)
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

