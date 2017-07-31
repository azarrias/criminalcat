using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePlayerState : MonoBehaviour {

    private int _healthDefault;
    private int _staminaDefault;
    private int _beerDefault;
    private int _coinsDefault;
    private int _health;
    private int _stamina;
    private int _beer;
    private int _coins;

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
        _healthDefault = playerStatus.healthAtStart;
        _staminaDefault = playerStatus.staminaAtStart;
        _beerDefault = playerStatus.beerAtStart;
        _coinsDefault = playerStatus.coinsAtStart;
        GetPlayerStatusValues(playerStatus);
    }

    private void GetPlayerStatusValues(PlayerStatus playerStatus)
    {
        _health = playerStatus.healthAtStart;
        _stamina = playerStatus.staminaAtStart;
        _beer = playerStatus.beerAtStart;
        _coins = playerStatus.coinsAtStart;
    }

    public void ResetPlayerStatusValues()
    {
        _health = _healthDefault;
        _stamina = _staminaDefault;
        _beer = _beerDefault;
        _coins = _coinsDefault;
    }
}
