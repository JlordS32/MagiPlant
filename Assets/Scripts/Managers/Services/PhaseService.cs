using System.Collections.Generic;

public static class PhaseService
{
    private static List<IPhaseListener> _listeners = new();
    public static IReadOnlyList<IPhaseListener> Listeners => _listeners;
    public static GamePhase Current { get; private set; } = GamePhase.Day;

    public static void Register(IPhaseListener listener)
    {
        if (_listeners.Contains(listener)) return;
        _listeners.Add(listener);
    }

    public static void Unregister(IPhaseListener listener)
    {
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
        foreach (var listener in _listeners)
        {
            listener.OnPhaseChanged(phase);
        }
    }
}