using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepThroughLevels : MonoBehaviour {

	void Awake()
    {       
        DontDestroyOnLoad(gameObject);
    }
}
