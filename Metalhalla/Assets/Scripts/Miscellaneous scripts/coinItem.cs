using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinItem : MonoBehaviour {

    public GameObject sourcePrefab;
    public AudioClip coinFx;

    private float rotationSpeed;

    [Header("Animation to get to the GUI position")]
    public bool taken = false;
    [Range(0.1f, 1f)]
    public float finalScale = 0.3f;
    public int framesToGUI = 30;
    
    private int framesToGUICount;
    private GameObject guiGameObject;
    Vector3 takenSpeed;

    private void Start()
    {
        transform.Rotate(Random.Range(0, 90) * Vector3.up);
        //rotationSpeed = Random.Range(100, 150);
        rotationSpeed = 180f;
        framesToGUICount = 0;

        guiGameObject = GameObject.Find("Beer"); // to be modified to another position
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        if (taken)
        {
            if (framesToGUICount >= framesToGUI)
                Destroy(this.gameObject);
            else
            {

                transform.position = Vector3.SmoothDamp(transform.position, guiGameObject.transform.position, ref takenSpeed, 0.2f);
                framesToGUICount++;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //if (collision.tag == "Player")
        if (!taken && collision.tag == "Player")
        {
            taken = true;
            // add score / items to player
            GameObject source = GameObject.Instantiate(sourcePrefab, transform.position, Quaternion.identity);
            source.GetComponent<AudioSource>().clip  = coinFx;
            source.GetComponent<AudioSource>().Play();
            Destroy(source, 2.0f);
           // Destroy(this.gameObject);
        }
    }
}
