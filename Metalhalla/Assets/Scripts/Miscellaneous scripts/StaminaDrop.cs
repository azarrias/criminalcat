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

    private PlayerStatus playerStatus;

    private void Start()
    {
        framesToGUICount = 0;
        guiGameObject = GameObject.Find("Stamina"); // to be modified dinamically depending on the fill of the bar
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
}
