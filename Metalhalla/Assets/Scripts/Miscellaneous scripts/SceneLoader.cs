using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour {

    private bool loadScene = false; 

    [Tooltip("Scene Number that's shown in File > Build Settings... menu. If the desired scene doesn't have any number, add it to the build order first")]
    public int sceneNumber = -1;

    [Tooltip("Text object where the loading message will appear")]
    public Text loadingText;

    [Tooltip("Loading Background image. For best results, use the same texture that the camera uses")]
    public Image loadingBackground;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && loadScene == false)
        {
            loadScene = true;
            loadingText.text = "Loading...";
            loadingBackground.color = new Color(1, 1, 1, 1);
            StartCoroutine(LoadNewScene()); 

        }

        if (loadScene == true )
        {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
        }
    }

    IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(1);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneNumber);

        while ( !async.isDone)
        {
            yield return null;
        }

    }

}
