using System;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrow : MonoBehaviour
{
    public static event Action OnPlantGrowing;

    // PROPERTIES
    [Header("Base")]
    [SerializeField] List<GameObject> _levelPrefabs;
    [SerializeField] Transform _spriteParent;

    [Header("Level")]
    [SerializeField] int _levelGap;

    [Header("Audio")]
    [SerializeField] AudioClip _levelUpSound;
    [SerializeField] AudioClip _growUpSound;
    [SerializeField] AudioClip _waterPlantSound;

    [Header("Level up rates")]
    [SerializeField] PlayerStatConfig _playerStatConfig;

    // REFERENCES
    GameObject _currentSprite;
    CurrencyStorage _currencyStorage;
    Player _player;

    // VARIABLES
    int _lastKnownIndex = -1;

    void OnEnable() => GameEventsManager.OnLevelUpUpdate += OnLevelUp;
    void OnDisable() => GameEventsManager.OnLevelUpUpdate -= OnLevelUp;

    void Awake()
    {
        _currencyStorage = FindFirstObjectByType<CurrencyStorage>();
        _player = GetComponent<Player>();
    }

    public void OnTest()
    {
        _player.PlayerData.AddExp(10);
        // int currLevel = (int)_player.PlayerData.Get(PlayerStats.Level);
    }

    void OnLevelUp(int level)
    {
        AudioManager.Instance.PlaySound(_levelUpSound);
        OnPlantGrowing?.Invoke();
        UpdateSprite();
    }

    void UpdateSprite()
    {
        int currLevel = (int)_player.PlayerData.Get(PlayerStats.Level);
        int index = Mathf.Clamp((currLevel - 1) / _levelGap, 0, _levelPrefabs.Count - 1);
        _lastKnownIndex = index;

        if (index != _lastKnownIndex && index > 0)
        {
            AudioManager.Instance.PlaySound(_growUpSound);
        }

        if (_currentSprite != null)
            Destroy(_currentSprite);
        _currentSprite = Instantiate(_levelPrefabs[index], _spriteParent);
    }

    void OnMouseUpAsButton()
    {
        WaterPlant();
    }

    void WaterPlant()
    {
        if (_player.PlayerData != null)
        {
            int currLevel = (int)_player.PlayerData.Get(PlayerStats.Level);
            float waterAmount = _currencyStorage.Get(CurrencyType.Water);

            if (_currencyStorage.Spend(CurrencyType.Water, _currencyStorage.Get(CurrencyType.Water)))
            {
                AudioManager.Instance.PlaySound(_waterPlantSound);
                _player.PlayerData.AddExp(waterAmount);
            }
        }
    }
}
