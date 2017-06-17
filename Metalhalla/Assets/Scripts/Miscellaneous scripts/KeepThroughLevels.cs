using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepThroughLevels : MonoBehaviour {

    // azarrias (17/06/2017): made this a singleton
    // to prevent additional GameManager objects from being created
    // when transitioning from one scene to another or when clicking "New Game"
    // please review if this has any side effects
    public static KeepThroughLevels instance = null;

	void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
