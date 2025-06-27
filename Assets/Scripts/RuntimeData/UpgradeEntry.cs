using System;
using UnityEngine.Events;

public class UpgradeEntry
{
    public string Name;
    public Func<string> GetRate;
    public Func<string> GetLevel;
    public Func<string> GetButtonLabel;
    public UnityAction UpgradeLogic;
    public UpgradeUI UIRef;
}