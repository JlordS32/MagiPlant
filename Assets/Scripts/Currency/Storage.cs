[System.Serializable]
public class Storage
{
    public float Value;

    public Storage() => Value = 0;
    public Storage(float initialValue) => Value = initialValue;

    public void Add(float amount) => Value += amount;

    public bool Spend(float amount)
    {
        if (Value >= amount)
        {
            Value -= amount;
            return true;
        }
        return false;
    }
}
