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
    public float finalScale = 0.1f;
    public int framesToGUI = 30;
    
    private int framesToGUICount;
    private GameObject guiGameObject;

    Vector3 takenSpeed;
    Vector3 scaleSpeed;

    private PlayerStatus playerStatus;
    private Camera mainCamera; 

    private void Start()
    {
        transform.Rotate(Random.Range(0, 90) * Vector3.up);
        rotationSpeed = 180f;
        framesToGUICount = 0;

        guiGameObject = GameObject.Find("Beer");
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        if (taken)
        {
            if (framesToGUICount >= framesToGUI)
            {
                playerStatus.CollectCoins(1);
                Destroy(this.gameObject);
            }
            else
            {
                //transform.position = Vector3.SmoothDamp(transform.position, guiGameObject.transform.position, ref takenSpeed, 0.2f);
                Vector3 targetPosition = GetWorldPositionOnPlane(mainCamera, guiGameObject.transform.position, -9); 

                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref takenSpeed, 0.2f);

                Vector3 targetScale = new Vector3(finalScale, finalScale, finalScale); 
                transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref scaleSpeed, 0.2f);

                framesToGUICount++;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!taken && collision.tag == "Player")
        {
            taken = true;

            playerStatus = collision.gameObject.GetComponent<PlayerStatus>();

            if (sourcePrefab != null && coinFx != null)
            {
                GameObject source = GameObject.Instantiate(sourcePrefab, transform.position, Quaternion.identity);
                source.GetComponent<AudioSource>().clip = coinFx;
                source.GetComponent<AudioSource>().Play();
                Destroy(source, 2.0f);
            }
        }
    }

    private Vector3 GetWorldPositionOnPlane( Camera cam,  Vector3 screenPosition, float z)
    {
        Ray ray = cam.ScreenPointToRay(screenPosition);
        Plane playerPlane = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        playerPlane.Raycast(ray, out distance);
        return ray.GetPoint(distance); 
    }
}
