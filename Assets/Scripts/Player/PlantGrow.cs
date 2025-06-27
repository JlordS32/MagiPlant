using System.Collections.Generic;
using UnityEngine;

public class PlantGrow : MonoBehaviour
{
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
    UIManager _uiManager;
    CurrencyStorage _currencyStorage;
    PlayerData _playerData;

    // VARIABLES
    int _lastKnownLevel = -1;
    int _lastKnownIndex = -1;

    void Awake()
    {
        _uiManager = FindFirstObjectByType<UIManager>();
        _currencyStorage = FindAnyObjectByType<CurrencyStorage>();
        _playerData = new PlayerData(_playerStatConfig);
    }

    public void OnTest()
    {
        _playerData.AddExp(10);
    }

    void Update()
    {
        int currLevel = (int)_playerData.Get(PlayerStats.Level);
        if (_playerData.Get(PlayerStats.Level) != _lastKnownLevel)
        {
            // Play audio on level up
            AudioManager.Instance.PlaySound(_levelUpSound);
            _lastKnownLevel = currLevel;

            // UI Handling
            _uiManager.UpdateLevelText(currLevel);
            _uiManager.UpdateExpText(_playerData.Get(PlayerStats.EXP), _playerData.GetRequiredEXP(currLevel));

            // Sprite Handling
            UpdateSprite();
        }
    }

    void UpdateSprite()
    {
        int currLevel = (int)_playerData.Get(PlayerStats.Level);
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

    void OnMouseDown()
    {
        AudioManager.Instance.PlaySound(_waterPlantSound);
        WaterPlant();
    }

    void WaterPlant()
    {
        if (_playerData != null)
        {
            int currLevel = (int)_playerData.Get(PlayerStats.Level);
            float waterAmount = _currencyStorage.Get(CurrencyType.Water);

            if (_currencyStorage.Spend(CurrencyType.Water, _currencyStorage.Get(CurrencyType.Water)))
            {
                _playerData.AddExp(waterAmount);
                _uiManager.UpdateExpText(_playerData.Get(PlayerStats.EXP), _playerData.GetRequiredEXP(currLevel));
                _uiManager.UpdateLevelText(currLevel);
            }
        }
    }
}
