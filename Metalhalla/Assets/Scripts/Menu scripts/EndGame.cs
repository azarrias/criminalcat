using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour {

    public string nextScene;

    public void GoToMainMenuPressed()
    {
        SceneManager.LoadScene(nextScene);
    }
}
