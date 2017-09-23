using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownTime : MonoBehaviour {

    public float slowedTimeScale = 0.2f;
    public string newSceneName = "Credits";
    public float changeToSceneIn = 1.8f;

    private float elapsedTime = 0.0f;
    private bool startCountdown = false;
    private SceneLoader loader;

    private float previousTimeScale;
    private float previousFixedDeltaTime;

    void Start()
    {
        GetComponent<Renderer>().enabled = false;
        loader = GameObject.FindWithTag("SceneLoader").GetComponent<SceneLoader>();
    }

    private void FixedUpdate()
    {
        if (startCountdown )
        {
            elapsedTime += Time.fixedDeltaTime;
            if (elapsedTime >= changeToSceneIn)
            {
                loader.GoToNextScene(newSceneName);
                Time.timeScale = previousTimeScale;
                Time.fixedDeltaTime = previousFixedDeltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            previousTimeScale = Time.timeScale;
            previousFixedDeltaTime = Time.fixedDeltaTime;

            Time.timeScale = slowedTimeScale;
            Time.fixedDeltaTime *= slowedTimeScale;
            elapsedTime = 0.0f;
            startCountdown = true;
        }
    }
}
