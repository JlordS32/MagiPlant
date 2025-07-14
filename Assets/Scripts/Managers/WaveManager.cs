using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [SerializeField] Spawner[] _spawners;
    [SerializeField] float _interGroupDelay = 1f;

    WaveBuilder _builder;

    int _waveIndex = 0;
    Coroutine _nightLoop;

    void OnEnable()
    {
        TimeManager.OnNightToggle += HandleNightToggle;
        _nightLoop = null; // Reset the night loop when enabled
    }

    void OnDisable()
    {
        TimeManager.OnNightToggle -= HandleNightToggle;
    }

    void Awake()
    {
        _builder = GetComponent<WaveBuilder>();
    }

    void HandleNightToggle(bool isNight)
    {
        Debug.Log($"Night toggle: {isNight}");
        if (isNight && _nightLoop == null)
            _nightLoop = StartCoroutine(RunWaves());
        else if (!isNight && _nightLoop != null)
        {
            StopCoroutine(_nightLoop);
            _nightLoop = null;
        }
    }

    IEnumerator RunWaves()
    {
        while (true)
        {
            Wave wave = _builder.Build(_waveIndex++);
            yield return StartCoroutine(SpawnWave(wave));
            yield return new WaitUntil(() => EnemyManager.ActiveCount == 0);
        }
    }

    IEnumerator SpawnWave(Wave w)
    {
        Debug.Log(w.Groups.Length + " groups in wave " + _waveIndex);
        Spawner s = _spawners[Random.Range(0, _spawners.Length)];
        foreach (var g in w.Groups)
        {
            for (int i = 0; i < g.Count; i++)
            {
                if (!s.TrySpawn(g.Prefab))
                    i--;
                else
                    yield return Wait(g.Gap);
            }
            yield return Wait(_interGroupDelay);
        }
    }

    static WaitForSeconds Wait(float t) => new(t);
}
