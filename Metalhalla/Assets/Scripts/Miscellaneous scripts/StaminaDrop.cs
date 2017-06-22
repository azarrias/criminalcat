using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaDrop : MonoBehaviour {

    [Header("Drop restore value")]
    public int staminaRestore = 1;

    [Header("Sound effects variables")]
    public GameObject sourcePrefab;
    public AudioClip staminaFx;

    [Header("Animation to get to the GUI position")]
    public bool taken = false;
    [Range(0.1f, 1f)]
    public float finalScale = 0.3f;
    public int framesToGUI = 30;

    private int framesToGUICount;
    private GameObject guiGameObject;

    Vector3 takenSpeed;
    Vector3 scaleSpeed;

    private PlayerStatus playerStatus;
    private Camera mainCamera;

    private void Start()
    {
        framesToGUICount = 0;
        guiGameObject = GameObject.Find("Background"); // to be modified dinamically depending on the fill of the bar
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (taken)
        {
            if (framesToGUICount >= framesToGUI)
            {
                Destroy(this.gameObject);
                playerStatus.RestoreStamina(staminaRestore);
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
        //if (collision.tag == "Player")
        if (!taken && collision.tag == "Player")
        {
            taken = true;
            playerStatus = collision.gameObject.GetComponent<PlayerStatus>();

            if (sourcePrefab != null && staminaFx != null)
            {
                GameObject source = GameObject.Instantiate(sourcePrefab, transform.position, Quaternion.identity);
                source.GetComponent<AudioSource>().clip = staminaFx;
                source.GetComponent<AudioSource>().Play();
                Destroy(source, 2.0f);
            }
        }
    }

    private Vector3 GetWorldPositionOnPlane(Camera cam, Vector3 screenPosition, float z)
    {
        Ray ray = cam.ScreenPointToRay(screenPosition);
        Plane playerPlane = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        playerPlane.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
}
