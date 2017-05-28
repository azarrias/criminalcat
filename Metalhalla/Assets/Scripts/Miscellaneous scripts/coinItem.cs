using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinItem : MonoBehaviour {

    public GameObject sourcePrefab;
    public AudioClip coinFx;

    private float rotationSpeed;
    
    private void Start()
    {
        transform.Rotate(Random.Range(0, 90) * Vector3.up);
        rotationSpeed = Random.Range(100, 150);
        
    }
    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            // add score / items to player
            GameObject source = GameObject.Instantiate(sourcePrefab, transform.position, Quaternion.identity);
            source.GetComponent<AudioSource>().clip  = coinFx;
            source.GetComponent<AudioSource>().Play();
            Destroy(source, 2.0f);
            Destroy(this.gameObject);
        }
    }
}
