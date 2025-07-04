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
    Player _player;
    TileManager _tileManager;

    // VARIABLES
    int _lastKnownIndex = -1;

    void Awake()
    {
        _uiManager = FindFirstObjectByType<UIManager>();
        _currencyStorage = FindFirstObjectByType<CurrencyStorage>();
        _tileManager = FindFirstObjectByType<TileManager>();
        _player = GetComponent<Player>();
    }

    // TODO: Dynamic tile size resize should be implement
    // Suggestions: Declare a Vector2 variable that'll contain the tile region it'll take.
    // The tile must scale by n^2.
    void Start()
    {
        // Take a spot on the grid on initial load.
        Vector2Int gridIndex = _tileManager.WorldToGridIndex(transform.position);
        _tileManager.UpdateGrid(gridIndex.x, gridIndex.y, TileWeight.Blocked);
    }

    public void OnTest()
    {
        Debug.Log("hello");
        _player.PlayerData.AddExp(10);
        int currLevel = (int)_player.PlayerData.Get(PlayerStats.Level);
        _uiManager.UpdateExpText(_player.PlayerData.Get(PlayerStats.EXP), _player.PlayerData.GetRequiredEXP(currLevel));
    }

    void Update()
    {
        int currLevel = (int)_player.PlayerData.Get(PlayerStats.Level);
        if (_player.PlayerData.CheckLevelUp())
        {
            // Play audio on level up
            AudioManager.Instance.PlaySound(_levelUpSound);

            // UI Handling
            _uiManager.UpdateLevelText(currLevel);
            _uiManager.UpdateExpText(_player.PlayerData.Get(PlayerStats.EXP), _player.PlayerData.GetRequiredEXP(currLevel));

            // Sprite Handling
            UpdateSprite();
        }
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
                _uiManager.UpdateExpText(_player.PlayerData.Get(PlayerStats.EXP), _player.PlayerData.GetRequiredEXP(currLevel));
                _uiManager.UpdateLevelText(currLevel);
            }
        }
    }
}
