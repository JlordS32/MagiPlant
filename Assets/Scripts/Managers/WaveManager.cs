using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour, IPhaseListener
{
    [SerializeField] Spawner[] _spawners;
    [SerializeField] float _interGroupDelay = 1f;

    WaveBuilder _builder;

    int _waveIndex = 0;
    Coroutine _nightLoop;

    void OnEnable()
    {
        PhaseService.Register(this);
    }

    void OnDisable()
    {
        PhaseService.Unregister(this);
    }

    void Awake()
    {
        _builder = GetComponent<WaveBuilder>();
    }

    public void OnPhaseChanged(GamePhase phase)
    {
        Debug.Log($"Phase changed: {phase}");
        if (phase == GamePhase.Night && _nightLoop == null)
            _nightLoop = StartCoroutine(RunWaves());
        else if (phase == GamePhase.Day && _nightLoop != null)
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
        if (_spawners.Length == 0)
        {
            Debug.LogWarning("No spawners available to spawn enemies.");
            yield break;
        }

        Debug.Log(w.Groups.Length + " groups in wave " + _waveIndex);

        Spawner s;
        foreach (var g in w.Groups)
        {
            s = _spawners[Random.Range(0, _spawners.Length)];
            for (int i = 0; i < g.Count; i++)
            {
                if (!s.TrySpawn(g.Prefab))
                    i--;
                else
                    yield return Wait(g.Gap);
            }

            Debug.Log($"Spawned {g.Count} of {g.Prefab.name} with gap {g.Gap}");
            yield return Wait(_interGroupDelay);
        }
    }

    static WaitForSeconds Wait(float t) => new(t);
}
