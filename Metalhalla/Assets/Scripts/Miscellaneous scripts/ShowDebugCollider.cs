using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDebugCollider : MonoBehaviour {

    public bool showCollider = false;
    private Renderer colliderRenderer;

    // Update is called once per frame

    private void Start()
    {
        colliderRenderer = GetComponent<Renderer>();
        colliderRenderer.enabled = showCollider;
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.P) == true)
            ToggleVisibility();
        
    }

    void ToggleVisibility()
    {
        showCollider = !showCollider;
        colliderRenderer.enabled = showCollider;
    }
}
