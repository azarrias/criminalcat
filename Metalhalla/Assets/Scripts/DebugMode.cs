using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    private bool debugMode = false;
    public GameObject debug_canvas;
    private Dictionary<string, Material[]> materials = new Dictionary<string, Material[]>();
    private bool isApplicationQuitting = false;

    void Start()
    {
        Object[] tempList = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        List<GameObject> realList = new List<GameObject>();
        GameObject go;

        foreach (Object obj in tempList)
        {
            if (obj is GameObject)
            {
                go = (GameObject)obj;
                if (go.GetComponent<MeshRenderer>() != null)
                {
                    MeshRenderer renderer = go.GetComponent<MeshRenderer>() as MeshRenderer;
                    materials[go.name] = renderer.sharedMaterials;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            debugMode = !debugMode;
            debug_canvas.SetActive(debugMode);
            ToggleWireframe(debugMode);
        }
    }

    void ToggleWireframe(bool active)
    {
        Object[] tempList = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        List<GameObject> realList = new List<GameObject>();
        GameObject go;

        foreach (Object obj in tempList)
        {
            if (obj is GameObject)
            {
                go = (GameObject)obj;
                if (go.GetComponent<MeshRenderer>() != null)
                {
                    if (active)
                    {
                        MeshRenderer renderer = go.GetComponent<MeshRenderer>() as MeshRenderer;
                        renderer.sharedMaterials = new Material[] { Resources.Load("Wireframe") as Material };
                    }
                    else
                    {
                        MeshRenderer renderer = go.GetComponent<MeshRenderer>() as MeshRenderer;
                        renderer.sharedMaterials = materials[go.name];
                    }
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        isApplicationQuitting = true;
        RestoreMaterials();
    }

    private void OnDisable()
    {
        RestoreMaterials();
    }

    private void RestoreMaterials()
    {
        Object[] tempList = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        List<GameObject> realList = new List<GameObject>();
        GameObject go;

        foreach (Object obj in tempList)
        {
            if (obj is GameObject)
            {
                go = (GameObject)obj;
                if (go.GetComponent<MeshRenderer>() != null)
                {
                    MeshRenderer renderer = go.GetComponent<MeshRenderer>() as MeshRenderer;
                    renderer.sharedMaterials = materials[go.name];
                }
            }
        }
    }
}
