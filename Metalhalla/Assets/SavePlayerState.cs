using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePlayerState : MonoBehaviour {

    private int _health = 100;
    private int _stamina = 10;
    private int _beer = 5;
    private int _coins = 9; 

    public static SavePlayerState instance = null;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GetPlayerStatusDefaultValues(GameObject.Find("Player").GetComponent<PlayerStatus>());
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    public void RecoverPlayerStatusValues( PlayerStatus playerStatus)
    {
        playerStatus.SetCurrentHealth(_health);
        playerStatus.SetCurrentStamina(_stamina);
        playerStatus.SetCurrentBeer(_beer);
        playerStatus.SetCurrentCoins(_coins);
    }

    public void SavePlayerStatusValues(PlayerStatus playerStatus)
    {
        _health = playerStatus.GetCurrentHealth();
        _stamina = playerStatus.GetCurrentStamina();
        _beer = playerStatus.GetCurrentBeer();
        _coins = playerStatus.GetCurrentCoins();
    }

    private void GetPlayerStatusDefaultValues(PlayerStatus playerStatus)
    {
        _health = playerStatus.healthAtStart;
        _stamina = playerStatus.staminaAtStart;
        _beer = playerStatus.beerAtStart;
        _coins = playerStatus.coinsAtStart;
    }
}
