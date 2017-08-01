using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePlayerState : MonoBehaviour {

    [Header("Defalt values - in case player is not in scene")]
    [SerializeField]
    private int _healthDefault = 100;
    [SerializeField]
    private int _staminaDefault = 4;
    [SerializeField]
    private int _beerDefault = 1;
    [SerializeField]
    private int _coinsDefault = 0;
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
            GameObject player = GameObject.Find("Player");
            if (player)
                GetPlayerStatusDefaultValues(GetComponent<PlayerStatus>());
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
