using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    int _score;

    public int Score => _score;
    public void IncreaseScore(int value)
    {
        _score += value;
    }
}