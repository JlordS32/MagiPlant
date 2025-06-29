using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] float _timeRate = 1f;

    // VARIABLES
    float _seconds = 0f;
    int _day = 0;

    const float DAY_DURATION = 480f;

    public float Seconds => _seconds;
    public int GetCurrentDay => _day;
    
    void Update()
    {
        _seconds += Time.deltaTime * _timeRate;

        if (_seconds >= DAY_DURATION)
        {
            _day++;
            _seconds = 0f;
        }
    }

    public string GetTimeString()
    {
        int totalMinutes = Mathf.FloorToInt(_seconds / DAY_DURATION * 24 * 60);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        return $"{hours:D2}:{minutes:D2}";
    }
}
