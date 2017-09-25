using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedRoute : MonoBehaviour {
    [Tooltip("Gameobject that will follow the route")]
    public GameObject mc;

    [Header("Parameters")]
    [Tooltip("Route points that define the route the MC gameobject will follow. Should have at least 2 points")]
    public Transform[] stepPoints;

    [Tooltip("Determines if the mc will travel from the last point to the first or not")]
    public bool loop = true;

    [Tooltip("Speed to move in the route")]
    public float speed = 2.0f;

    [Header("Randomization")]
    [Tooltip("Randomize the MC movement")]
    public bool randomize = true;
    public float speedRandomVariation = 10.0f;

    private int totalSteps;
    private int currentStepIndex;
    private int nextStepIndex;
    private Vector3 currentStep;
    private Vector3 nextStep;
    private Vector3 currentDirection;
    private float currentDistance;
    private bool reverse = false;
    private float baseSpeed;

    private void Start()
    {
        if (randomize)
            baseSpeed = speed;
                
        totalSteps = stepPoints.Length;
        if (totalSteps >= 2)
        {
            currentStepIndex = 0;
            nextStepIndex = 1;
            UpdateSteps();
            UpdateMCRotation();
            mc.transform.position = transform.position;
        }
        else
        {
            Debug.LogError("Guided route step points aren't properly setup (there's not at least 2 of those).");
        }
    }

	void Update () {
        //mc.transform.Translate(currentDirection * Time.deltaTime * speed);
        mc.transform.position += currentDirection * speed * Time.deltaTime;
        if (Vector3.Distance(currentStep, mc.transform.position) >= currentDistance)
        {
            UpdateStepIndexes();
            UpdateSteps();
            UpdateMCRotation();
        }
	}

    private void UpdateStepIndexes()
    {
        if (loop == true)
        {
            currentStepIndex = nextStepIndex;
            nextStepIndex = (nextStepIndex + 1) % totalSteps;
        }
        else
        {
            
            currentStepIndex = nextStepIndex;
            nextStepIndex += reverse ? -1 : 1; 
            if (nextStepIndex < 0)
            {
                nextStepIndex = 1;
                reverse = false;
            }
            else if (nextStepIndex == totalSteps)
            {
                nextStepIndex = totalSteps - 2;
                reverse = true; 
            }
        }
    }

    private void UpdateSteps()
    {
        if (randomize)
            speed = Random.Range(baseSpeed - speedRandomVariation, baseSpeed + speedRandomVariation);
        currentStep = stepPoints[currentStepIndex].position;
        nextStep = stepPoints[nextStepIndex].position;
        currentDirection = (nextStep - currentStep).normalized;
        currentDistance = (nextStep - currentStep).magnitude;
    }

    private void UpdateMCRotation()
    {
        mc.transform.rotation = Quaternion.LookRotation(currentDirection);
    } 
}
