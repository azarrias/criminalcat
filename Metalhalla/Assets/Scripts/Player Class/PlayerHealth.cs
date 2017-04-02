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
    [Tooltip("Health recovery on beer consumption")]
    public int beerHealthRecovery = 20; 
    int beer;


    //----------------------------------------------------
    void Start () {
        health = healthAtStart;
        stamina = staminaAtStart;
        beer = beerAtStart; 		
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.F1) == true)
            ApplyDamage(3);
        if (Input.GetKeyDown(KeyCode.F2) == true)
            RestoreHealth(25);
        if (Input.GetKeyDown(KeyCode.F3) == true)
            ConsumeStamina(7);
        if (Input.GetKeyDown(KeyCode.F4) == true)
            RestoreStamina(1);
        if (Input.GetKeyDown(KeyCode.F5) == true)
            ConsumeBeer(1);
        if (Input.GetKeyDown(KeyCode.F6) == true)
            RefillBeer(5); 
	}


    // ---- HEALTH functions ----
    public void ApplyDamage( int damage )
    {
        health -= damage;
        if (health <= 0)   
            health = 0; // TODO: improve when making full player FSM
    }

    public bool RestoreHealth( int restore )
    {
        if (health == healthMaximum)    // cannot restore any more health
            return false;

        health += restore;
        if (health >= healthMaximum)
            health = healthMaximum;
        return true; 
    }

    // ---- STAMINA functions ----
    public bool ConsumeStamina( int consumption )
    {
        if (stamina < consumption) // cannot use magic
            return false;   
        stamina -= consumption; 
        return true; 
    }

    public void RestoreStamina( int restore )
    {
        stamina += restore;
        if (stamina >= staminaMaximum)
            stamina = staminaMaximum; 
    }

    // ---- BEER functions ----
    public bool ConsumeBeer (int consumption)
    {
        if (beer == 0)
            return false;

        if (RestoreHealth(beerHealthRecovery) == false)     // no health recovery, no beer drink
            return false;

        beer -= consumption; 
        return true; 
    }

    public bool RefillBeer( int refill )
    {
        if (beer == beerMaximum)
            return false;
        beer += refill;
        if (beer > beerMaximum)
            beer = beerMaximum; 

        return true; 
    }


    // ---- GUI communication functions ----
    public float GetCurrentHealthRatio() {  return (float) health / (float) healthMaximum; }
    public int GetCurrentStamina() { return stamina; }
    public int GetCurrentBeer() { return beer; }

}
