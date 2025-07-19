using System.Collections.Generic;

public static class PhaseService
{
    private static List<IPhaseListener> _listeners = new();
    public static IReadOnlyList<IPhaseListener> Listeners => _listeners;
    public static GamePhase Current { get; private set; } = GamePhase.Day;

    public static void Register(IPhaseListener listener)
    {
        if (listener == null)
        {
            Debugger.LogError(DebugCategory.Services, "Attempted to register a null listener.");
            return;
        }

        if (_listeners.Contains(listener)) return;
        _listeners.Add(listener);
    }

    public static void Unregister(IPhaseListener listener)
    {
        if (listener == null)
        {
            Debugger.LogError(DebugCategory.Services, "Attempted to unregister a null listener.");
            return;
        }

        if (!_listeners.Contains(listener)) return;
        _listeners.Remove(listener);
    }

    public static void Set(GamePhase phase)
    {
        if (phase == Current) return;
        Current = phase;
        NotifyPhaseChanged(phase);
    }

    static void NotifyPhaseChanged(GamePhase phase)
    {
        Debugger.Log(DebugCategory.Services, $"Phase changed to: {phase}");

        var snapshot = _listeners.ToArray();
        foreach (var l in snapshot)
            l.OnPhaseChanged(phase);
    }
}