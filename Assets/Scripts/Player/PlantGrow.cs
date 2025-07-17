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
        _player = GetComponent<Player>();
    }

    public void OnTest()
    {
        _player.PlayerData.AddExp(10);
        CurrencyStorage.Instance.Add(CurrencyType.Water, 10);
        CurrencyStorage.Instance.Add(CurrencyType.Sunlight, 10);
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
        if (!_canWater)
        {
            Debug.LogWarning("Cannot water plants at night.");
            return;
        }

        WaterPlant();
    }

    void WaterPlant()
    {
        if (_player.PlayerData != null)
        {
            int currLevel = (int)_player.PlayerData.Get(PlayerStats.Level);
            float waterAmount = CurrencyStorage.Instance.Get(CurrencyType.Water);

            if (CurrencyStorage.Instance.Spend(CurrencyType.Water, CurrencyStorage.Instance.Get(CurrencyType.Water)))
            {
                AudioManager.Instance.PlaySound(_waterPlantSound);
                _player.PlayerData.AddExp(waterAmount);
            }
        }
    }
}
