using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenuState : MonoBehaviour {

    private GameObject musicSliderGO;
    private GameObject fxSoundSliderGO;
    private float musicSliderValue;
    private float fxSoundSliderValue;
    
	// Use this for initialization
	void Start () {
        musicSliderGO = GameObject.FindGameObjectWithTag("OptionsMenuMusicVolume");
        fxSoundSliderGO = GameObject.FindGameObjectWithTag("OptionsMenuSoundEffectsVolume");
        //Default sound values
        musicSliderValue = 0.5f;
        fxSoundSliderValue = 0.5f;
    }

    public void SaveMusicVolume()
    {
        musicSliderValue = musicSliderGO.GetComponent<Slider>().value;
    }

    public void SaveFxVolume()
    {
        fxSoundSliderValue = fxSoundSliderGO.GetComponent<Slider>().value;
    }

    //Getters  & setters
    public float GetMusicSliderValue()
    {
        return musicSliderValue;
    }

    public void SetMusicSliderValue(float value)
    {
        musicSliderValue = value;
    }

    public float GetFxSoundSliderValue()
    {
        return fxSoundSliderValue;
    }

    public void SetFxSoundSliderValue(float value)
    {
        fxSoundSliderValue = value;
    }
}
