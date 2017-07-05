using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(RawImage))]
public class TransitionTitleMenuAnimation : MonoBehaviour
{
    public MovieTexture movie;
    private AudioSource movieAudio;

    public string sceneName;
    private SceneLoader loader;

    private void Start()
    {
        loader = GameObject.FindWithTag("SceneLoader").GetComponent<SceneLoader>();
        GetComponent<RawImage>().texture = movie as MovieTexture;
        movieAudio = GetComponent<AudioSource>();
        movieAudio.clip = movie.audioClip;
        movie.Play();
        movieAudio.Play();
    }

    private void Update()
    {
        if (Input.GetButtonDown("DisplayMenu") || movie.isPlaying == false  )
        {
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        //SceneManager.LoadScene(sceneName);
        loader.GoToNextScene(sceneName);
    }
}
