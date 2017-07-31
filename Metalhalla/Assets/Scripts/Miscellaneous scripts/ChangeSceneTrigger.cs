using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneTrigger : MonoBehaviour {

    public string newSceneName = "Dungeon Boss";
    private SceneLoader loader;
    private SavePlayerState savePlayerStateScript;

    void Start () {
        loader = GameObject.FindWithTag("SceneLoader").GetComponent<SceneLoader>();
        GetComponent<Renderer>().enabled = false;
        savePlayerStateScript = GameObject.FindGameObjectWithTag("GameSession").GetComponent<SavePlayerState>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            savePlayerStateScript.SavePlayerStatusValues(other.GetComponent<PlayerStatus>());
            loader.GoToNextScene(newSceneName);
        }
    }
}
