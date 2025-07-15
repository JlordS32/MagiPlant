using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;

public class TimeManager : MonoBehaviour
{
    // PROPERTIES
    [Header("Time Settings")]
    [SerializeField, Range(0f, 24f)] float _startHour = 7f;
    [SerializeField] float _timeRate = 1f;

    [Header("Day-night break-points")]

    [Tooltip("Hour when night begins fading into day (e.g., 6 = 6 AM)")]
    [SerializeField, Range(0f, 24f)] float _dawnStart = 6f;

    [Tooltip("Hour when full daylight is reached (e.g., 7 = 7 AM)")]
    [SerializeField, Range(0f, 24f)] float _dawnEnd = 7f;

    [Tooltip("Hour when day begins fading into night (e.g., 17 = 5 PM)")]
    [SerializeField, Range(0f, 24f)] float _duskStart = 17f;

    [Tooltip("Hour when full night is reached (e.g., 18 = 6 PM)")]
    [SerializeField, Range(0f, 24f)] float _duskEnd = 18f;

    [Header("Lighting")]
    [SerializeField] Light2D _globalLight;

    // TOTAL DAY DURATION (in seconds)
    const float DAY_DURATION = 360f;

    // VARIABLES
    float _seconds;
    int _day;
    bool _wasNight = false;

    // GETTERS & SETTERS
    public float Seconds => _seconds / DAY_DURATION;
    public int CurrentDay => _day;

    void Awake()
    {
        _seconds = Mathf.Repeat(_startHour, 24f) / 24f * DAY_DURATION;

        bool initialNight = IsNight(_seconds);
        PhaseService.Set(initialNight ? GamePhase.Night : GamePhase.Day);
        _wasNight = initialNight;
    }

    void Update()
    {
        // Advance clock
        _seconds += Time.deltaTime * _timeRate;
        if (_seconds >= DAY_DURATION) { _day++; _seconds = 0f; }

        float h = _seconds / DAY_DURATION * 24f;

        // Night detection
        bool isNight = h >= _duskEnd || h < _dawnStart;
        if (isNight != _wasNight)
            HandleNightToggle(isNight);

        _wasNight = isNight;

        // Light intensity
        float intensity = h < _dawnStart ? 0f :
                          h < _dawnEnd ? Mathf.InverseLerp(_dawnStart, _dawnEnd, h) :
                          h < _duskStart ? 1f :
                          h < _duskEnd ? Mathf.InverseLerp(_duskEnd, _duskStart, h) :
                                                0f;

        _globalLight.intensity = Mathf.Max(0.1f, intensity);
    }

    bool IsNight(float seconds)
    {
        float h = seconds / DAY_DURATION * 24f;
        return h >= _duskEnd || h < _dawnStart;
    }

    void HandleNightToggle(bool isNight)
    {
        PhaseService.Set(isNight ? GamePhase.Night : GamePhase.Day);
    }

    public string GetTimeString()
    {
        int totalMinutes = Mathf.FloorToInt(_seconds / DAY_DURATION * 24f * 60f);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        // round down to the nearest 10-minute mark
        minutes = minutes / 10 * 10;

        return $"{hours:D2}:{minutes:D2}";
    }
}
