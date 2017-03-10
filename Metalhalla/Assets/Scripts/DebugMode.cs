using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMode : MonoBehaviour
{

    private bool debugMode = false;
    public GameObject debug_canvas;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            debugMode = !debugMode;
        }

        if (debugMode)
        {
            debug_canvas.SetActive(true);
        }
        else
        {
            debug_canvas.SetActive(false);
        }
    }
}
