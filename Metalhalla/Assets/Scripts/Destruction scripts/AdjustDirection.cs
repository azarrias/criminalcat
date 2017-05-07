using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustDirection : MonoBehaviour
{

    [HideInInspector]
    public int pushForceX = 0;
    [HideInInspector]
    public int pushForceY = 0;
    [HideInInspector]
    public Vector3 fragmentScale;

    // Use this for initialization
    void Start()
    {
        foreach (Transform fragment in gameObject.GetComponentInChildren<Transform>())
        {
            Vector3 newFragmentScale = fragment.localScale;
            newFragmentScale.x *= fragmentScale.x;
            newFragmentScale.y *= fragmentScale.y;
            newFragmentScale.z *= fragmentScale.z;
            fragment.localScale = newFragmentScale;

            float randomX = Random.Range(-1.0f, 1.0f);
            if (randomX == 0.0f)
                randomX += 1.0f;

            float randomY = Random.Range(0.1f, 1.0f);
            if (randomY == 0.0f)
                randomY += 1.0f;

            fragment.GetComponent<Rigidbody>().AddForce(new Vector3(randomX * pushForceX, randomY * pushForceY, 0), ForceMode.Force);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
