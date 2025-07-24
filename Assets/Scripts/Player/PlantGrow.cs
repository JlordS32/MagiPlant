using System;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrow : MonoBehaviour, IPhaseListener
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
    Player _player;
    PlayerData _playerData;

    // VARIABLES
    int _lastKnownIndex = -1;
    bool _canWater = true;

    void OnEnable()
    {
        GameEventsManager.OnLevelUpUpdate += OnLevelUp;
        PhaseService.Register(this);
        OnPhaseChanged(PhaseService.Current);
    }

    void OnDisable()
    {
        GameEventsManager.OnLevelUpUpdate -= OnLevelUp;
        PhaseService.Unregister(this);
    }

    public void OnPhaseChanged(GamePhase phase)
    {
        _canWater = phase == GamePhase.Day;
    }

    void Awake()
    {
        _currentSprite = GetComponentInChildren<SpriteRenderer>().gameObject; // WARNING: Only works if there's one child sprite renderer. We'll have to rework this approach if we want multiple sprites.

        if (_currentSprite == null)
        {
            Debugger.LogError(DebugCategory.Player, "No sprite found for PlantGrow.");
            return;
        }

        _player = GetComponent<Player>();
        _playerData = _player.PlayerData;

        if (_playerData == null)
        {
            Debugger.LogError(DebugCategory.Player, "Player data is not available.");
            return;
        }
    }

    public void OnTest()
    {
        _player.PlayerData.LevelUp();
        CurrencyStorage.Instance.Add(CurrencyType.Water, 10);
        CurrencyStorage.Instance.Add(CurrencyType.Sunlight, 10);
    }

    void OnLevelUp(int level)
    {
        Debugger.Log(DebugCategory.Player, $"Player leveled up to {level}.");

        foreach (var kv in _playerData.Snapshot)
            Debugger.Log(DebugCategory.Player, $"{kv.Key}: {kv.Value}");

        AudioManager.Instance.PlaySound(_levelUpSound);
        OnPlantGrowing?.Invoke();
        UpdateSprite();
    }

    void UpdateSprite()
    {
        int currLevel = _playerData.Level;
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
        if (!_canWater)
        {
            Debugger.LogWarning(DebugCategory.Player, "Cannot water plants during the night.");
            return;
        }

        WaterPlant();
    }

    void WaterPlant()
    {
        if (_player.PlayerData != null)
        {
            Debugger.Log(DebugCategory.Player, "Watering plant (player)...");
            float waterAmount = CurrencyStorage.Instance.Get(CurrencyType.Water);

            if (CurrencyStorage.Instance.Spend(CurrencyType.Water, CurrencyStorage.Instance.Get(CurrencyType.Water)))
            {
                AudioManager.Instance.PlaySound(_waterPlantSound);
                _player.PlayerData.AddExp(waterAmount);
            }
        }
    }
}
