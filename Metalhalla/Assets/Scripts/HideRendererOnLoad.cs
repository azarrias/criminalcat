﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideRendererOnLoad : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
