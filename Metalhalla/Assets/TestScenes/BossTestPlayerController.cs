using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTestPlayerController : MonoBehaviour {

    private string horizontalAxis = "Horizontal";
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    private bool onLand = true;
    private FSMBoss fsm = null;
    private GameObject topDownCam = null;
    private GameObject lateralCam = null;
    private bool toggleCam = false;
    
    void Awake()
    {
        fsm = FindObjectOfType<FSMBoss>();
        if (fsm == null)
            Debug.LogError("fsm not found");
    }

    // Use this for initialization
	void Start () {
        lateralCam = GameObject.Find("Lateral Camera");
        topDownCam = GameObject.Find("Top Down Camera");
    }

	void FixedUpdate () {

        //if(Input.GetKeyDown(KeyCode.Space) && onLand)
        //{
        //    gameObject.GetComponent<Rigidbody>().velocity = Vector3.up * jumpSpeed;
        //    onLand = false;
        //}

        //float horizontal = Input.GetAxis(horizontalAxis);
        //Vector3 translation = new Vector3(horizontal * speed * Time.deltaTime, 0, 0);
        //transform.Translate(translation);

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            fsm.DamageBoss(10);            
        }

        //Change camera
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    toggleCam = !toggleCam;

        //    if (!toggleCam)
        //    {
        //        lateralCam.SetActive(true);
        //        topDownCam.SetActive(false);
        //    }
        //    else
        //    {
        //        lateralCam.SetActive(false);
        //        topDownCam.SetActive(true);
        //    }

        //}

    }

    //void OnCollisionEnter(Collision collision)
    //{
    //   if( collision.gameObject.layer == LayerMask.NameToLayer("ground"))
    //    {
    //        onLand = true;
    //    }

    //}
}
