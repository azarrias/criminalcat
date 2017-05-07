using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMove : MonoBehaviour {

    [Tooltip("Speed in which the layer will move against the camera. The greater the closer to the camera")]
    public float speed = 0.05f;
    [Tooltip("The whole parallax boundaries")]
    public GameObject parallaxBoundaries; 

    [Tooltip("The camera the parallax effect is performed with. If unassigned, it will search for the MainCamera of the scene")]
    public GameObject playerCamera;

    private float lastCameraXPos;
    private Vector2 mainLimits;
  //  private Vector2 layerLimits;

    void Start () {
        // if no camera is specified, it searches the MainCamera in the scene
        if (playerCamera == null)
            playerCamera = GameObject.FindWithTag("MainCamera");

        if (playerCamera == null)
            Debug.LogError("There is no camera ready to perform the parallax effect");
        else
        {
            Bounds b = parallaxBoundaries.GetComponent<BoxCollider>().bounds;
            mainLimits.x = b.center.x - b.extents.x;
            mainLimits.y = b.center.x + b.extents.x;
      /*      layerLimits.x = mainLimits.x * speed;
            layerLimits.y = mainLimits.y * speed; 
            */
        }
	}

    private void FixedUpdate()
    {
        Vector3 calculatedPosition = transform.position;
        calculatedPosition.x = -DenormalizeXPosInParallaxBoundary(CalculateNormalizedCameraPositionInParallaxBoundary());
        transform.position = calculatedPosition;
    }

    float CalculateNormalizedCameraPositionInParallaxBoundary()
    {
        float normalizedPosition = 0.0f;
        normalizedPosition = (playerCamera.transform.position.x - mainLimits.x) / (mainLimits.y - mainLimits.x);
        //Debug.Log("Normalized camera position x = " + normalizedPosition);
        return normalizedPosition; 
    }
    
    float DenormalizeXPosInParallaxBoundary( float normalizedPosition)
    {
        float denormalizedPosition;
        //denormalizedPosition = (normalizedPosition * speed) * (mainLimits.y - mainLimits.x) + mainLimits.x;
        denormalizedPosition = (normalizedPosition * speed) * (mainLimits.y - mainLimits.x);
        //Debug.Log("Denormalized layer position.x = " + denormalizedPosition);
        return denormalizedPosition; 
    }
}
