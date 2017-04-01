using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    // health variables
    [Header("Health Setup")]
    [Tooltip("Health start value")]
    public int healthAtStart= 100;
    [Tooltip("Health maximum value")]
    public int healthMaximum = 100;
    int health;

    // stamina variables
    [Header("Stamina Setup")]
    [Tooltip("Starting stamina value")]
    public int staminaAtStart = 10;
    [Tooltip("Stamina maximum value")]
    public int staminaMaximum = 10;
    int stamina;

    // beer variables
    [Header("Beer Setup")]
    [Tooltip("Starting beer value")]
    public int beerAtStart = 1;
    [Tooltip("Stamina maximum value")]
    public int beerMaximum = 5;
    int beer;

    // Use this for initialization
    void Start () {
        health = healthAtStart;
        stamina = staminaAtStart;
        beer = beerAtStart; 		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
