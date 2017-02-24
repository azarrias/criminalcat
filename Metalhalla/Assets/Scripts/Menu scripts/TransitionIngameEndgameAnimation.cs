using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionIngameEndgameAnimation : MonoBehaviour {

    public string nextScene;

	private void ChangeScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
