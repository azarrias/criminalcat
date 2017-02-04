using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCube : MonoBehaviour {

    public GameObject remains;

   public void Destroy()
    {
        Instantiate(remains, transform.position, transform.rotation);
        Destroy(gameObject);   
    }
}
