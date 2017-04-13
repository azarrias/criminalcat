using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageCircles : MonoBehaviour {

    private  GameObject[] tornadoCircles;
    private bool wait = false;
    private int index = 0;
    private float waitingTime = 0.3f;
    public int circleAmount = 5;
    public GameObject tornadoCircle = null;

	// Use this for initialization
	void Start () {

        tornadoCircles = new GameObject[circleAmount];

        for (int i = 0; i < circleAmount; i++)
        {
            tornadoCircles[i] = Instantiate<GameObject>(tornadoCircle, transform.position, Quaternion.identity);
            tornadoCircles[i].transform.parent = transform;
        }
     
	}
	
	// Update is called once per frame
	void Update () {

        if (!wait)
        {
            tornadoCircles[index].GetComponent<TornadoCircleBehaviour>().readyToMove = true;
            index++;
            if (index >= tornadoCircles.Length)
            {
                index = 0;
            }

            wait = true;
            StartCoroutine("WaitMyTurn", waitingTime);
        }

    }

    IEnumerator WaitMyTurn(float seconds)
    {      
        Debug.Log("wait value=" + wait);
        yield return new WaitForSeconds(seconds);
        wait = false;        
    }
}
