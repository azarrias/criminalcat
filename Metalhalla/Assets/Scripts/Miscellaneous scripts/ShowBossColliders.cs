using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBossColliders : MonoBehaviour {

    private bool show = false;
    MeshRenderer[] colliders;

    void Awake()
    {
        colliders = new MeshRenderer[3];
    }

	// Use this for initialization
	void Start () {

        colliders = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in colliders)
        {
            mr.enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.P))
            ToggleShow();

    }

    public void ToggleShow()
    {
        show = !show;

        if (show)
        {
            foreach (MeshRenderer mr in colliders)
            {
                mr.enabled = true;
            }
        }
        if (!show)
        {
            foreach (MeshRenderer mr in colliders)
            {
                mr.enabled = false;
            }
        }
    }
}
