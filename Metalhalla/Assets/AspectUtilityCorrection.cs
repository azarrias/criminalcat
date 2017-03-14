using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectUtilityCorrection : MonoBehaviour {

    RectTransform rect; 
	// Use this for initialization
	void Start () {
        rect = GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        rect.anchoredPosition = new Vector2(AspectUtility.xOffset, AspectUtility.yOffset);
    }
 
}
