using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour {

    private bool loadScene = false; 

    [Tooltip("Text object where the loading message will appear")]
    public Text loadingText;

    [Tooltip("Loading Background image. For best results, use the same texture that the camera uses")]
    public Image loadingBackground;

    [Tooltip("Show % of loaded scene")]
    public bool showLoadedPercentage = true; 

    private void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.F9))
            GoToNextScene("Dungeon entrance");
        if (Input.GetKeyDown(KeyCode.F10))
            GoToNextScene("Dungeon");
        if (Input.GetKeyDown(KeyCode.F11))
            GoToNextScene( "Dungeon Boss" );

        if (loadScene == true )
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
    }

    IEnumerator LoadNewScene( string sceneName, Text progressText)
    {
        //yield return new WaitForSeconds(1); // use to see effect in fast pcs. In old pcs remove or the wait time will high and then will be increased without any reason

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        while ( !async.isDone)
        {
            if (showLoadedPercentage)
                progressText.text = "Loading " + async.progress*100.0f + "%";
            yield return null;
        }

    }

    public void GoToNextScene(string sceneName)
    {
        if (loadScene == false)
        {
            loadScene = true;
            loadingText.text = "Loading...";
            loadingBackground.color = new Color(1, 1, 1, 1);
            StartCoroutine(LoadNewScene(sceneName, loadingText));
        }
    }

}
