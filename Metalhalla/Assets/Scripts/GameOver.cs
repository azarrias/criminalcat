using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    public string nextScene;
    
    public void GoToMainMenuPressed()
    {
        SceneManager.LoadScene(nextScene);
    }

}
